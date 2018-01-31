using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;
using ACT.SpecialSpellTimer.Views;
using FFXIV.Framework.Bridge;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.WPF.Views;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// ワンポイントテレロップ Controller
    /// </summary>
    public class TickersController
    {
        #region Singleton

        private static TickersController instance = new TickersController();

        public static TickersController Instance => instance;

        #endregion Singleton

        /// <summary>
        /// テロップWindowのリスト
        /// </summary>
        private Dictionary<long, TickerWindow> telopWindowList =
            new Dictionary<long, TickerWindow>();

        /// <summary>
        /// ログとマッチングする
        /// </summary>
        /// <param name="telops">Telops</param>
        /// <param name="logLines">ログ行</param>
        public void Match(
            IReadOnlyList<Ticker> telops,
            IReadOnlyList<string> logLines)
        {
            if (telops.Count < 1 ||
                logLines.Count < 1)
            {
                return;
            }

#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            try
            {
                logLines.AsParallel().ForAll(logLine =>
                {
                    foreach (var telop in telops)
                    {
                        this.MatchCore(telop, logLine);
                    }
                });
            }
            finally
            {
#if DEBUG
                sw.Stop();
                Debug.WriteLine($"●TickersController.Match() {sw.Elapsed.TotalMilliseconds:N1}ms spells={telops.Count:N0} lines={logLines.Count:N0}");
#endif
            }
        }

        /// <summary>
        /// ログ1行1テロップに対して判定する
        /// </summary>
        /// <param name="telop">テロップ</param>
        /// <param name="log">ログ</param>
        public void MatchCore(
            Ticker telop,
            string log)
        {
            var matched = false;

            var regex = telop.Regex;
            var regexToHide = telop.RegexToHide;

            // マッチング計測開始
            telop.StartMatching();

            // 開始条件を確認する
            if (ConditionUtility.CheckConditionsForTelop(telop))
            {
                // 通常マッチ
                if (regex == null)
                {
                    var keyword = telop.KeywordReplaced;
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        if (log.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            var messageReplaced = ConditionUtility.GetReplacedMessage(telop);

                            // PC名を置換する
                            messageReplaced = FFXIVPlugin.Instance.ReplacePartyMemberName(messageReplaced);

                            if (!telop.AddMessageEnabled)
                            {
                                telop.MessageReplaced = messageReplaced;
                            }
                            else
                            {
                                telop.MessageReplaced += string.IsNullOrWhiteSpace(telop.MessageReplaced) ?
                                    messageReplaced :
                                    Environment.NewLine + messageReplaced;
                            }

                            telop.ForceHide = false;
                            telop.Delayed = false;
                            telop.MatchedLog = log;
                            telop.MatchDateTime = DateTime.Now;

                            // マッチング計測終了
                            telop.EndMatching();

                            telop.Play(telop.MatchSound);
                            telop.Play(telop.MatchTextToSpeak);

                            // DISCORDに通知する？
                            if (telop.NotifyToDiscord)
                            {
                                DiscordBridge.Instance.SendMessageDelegate?.Invoke(messageReplaced);
                            }

                            matched = true;
                        }
                    }
                }

                // 正規表現マッチ
                else
                {
                    var match = regex.Match(log);
                    if (match.Success)
                    {
                        var messageReplaced = ConditionUtility.GetReplacedMessage(telop);
                        messageReplaced = match.Result(messageReplaced);

                        // PC名を置換する
                        messageReplaced = FFXIVPlugin.Instance.ReplacePartyMemberName(messageReplaced);

                        if (!telop.AddMessageEnabled)
                        {
                            telop.MessageReplaced = messageReplaced;
                        }
                        else
                        {
                            telop.MessageReplaced += string.IsNullOrWhiteSpace(telop.MessageReplaced) ?
                                messageReplaced :
                                Environment.NewLine + messageReplaced;
                        }

                        telop.ForceHide = false;
                        telop.Delayed = false;
                        telop.MatchedLog = log;
                        telop.MatchDateTime = DateTime.Now;

                        // マッチング計測終了
                        telop.EndMatching();

                        telop.Play(telop.MatchSound);
                        if (!string.IsNullOrWhiteSpace(telop.MatchTextToSpeak))
                        {
                            var tts = match.Result(telop.MatchTextToSpeak);
                            telop.Play(tts);
                        }

                        // DISCORDに通知する？
                        if (telop.NotifyToDiscord)
                        {
                            DiscordBridge.Instance.SendMessageDelegate?.Invoke(messageReplaced);
                        }

                        matched = true;
                    }
                }
            }

            if (matched)
            {
                // ディレイサウンドをスタートさせる
                telop.StartDelayedSoundTimer();

                SpellsController.Instance.UpdateNormalSpellTimerForTelop(telop, telop.ForceHide);
                SpellsController.Instance.NotifyNormalSpellTimerForTelop(telop.Title);

                return;
            }

            // 通常マッチ(強制非表示)
            if (regexToHide == null)
            {
                var keyword = telop.KeywordToHideReplaced;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (log.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        telop.ForceHide = true;
                        matched = true;
                    }
                }
            }

            // 正規表現マッチ(強制非表示)
            else
            {
                if (regexToHide.IsMatch(log))
                {
                    telop.ForceHide = true;
                    matched = true;
                }
            }

            if (matched)
            {
                SpellsController.Instance.UpdateNormalSpellTimerForTelop(telop, telop.ForceHide);
                SpellsController.Instance.NotifyNormalSpellTimerForTelop(telop.Title);
            }
        }

        private bool beforeChickThrough = false;

        /// <summary>
        /// Windowをリフレッシュする
        /// </summary>
        /// <param name="telop">テロップ</param>
        public void RefreshTelopOverlays(
            IReadOnlyList<Ticker> telops)
        {
            void refreshTelop(
                Ticker telop)
            {
                var w = default(TickerWindow);
                lock (this.telopWindowList)
                {
                    var exists = this.telopWindowList.ContainsKey(telop.ID);
                    w = exists ?
                        this.telopWindowList[telop.ID] :
                        new TickerWindow()
                        {
                            Title = "Ticker - " + telop.Title,
                            DataSource = telop
                        };

                    if (!exists)
                    {
                        this.telopWindowList.Add(telop.ID, w);
                        w.Show();
                    }
                }

                // クリックスルーを適用する
                if (this.beforeChickThrough != Settings.Default.ClickThroughEnabled)
                {
                    this.beforeChickThrough = Settings.Default.ClickThroughEnabled;
                    if (Settings.Default.ClickThroughEnabled)
                    {
                        w.ToTransparent();
                    }
                    else
                    {
                        w.ToNotTransparent();
                    }
                }

                if (telop.IsDesignMode ||
                    (Settings.Default.OverlayVisible && Settings.Default.TelopAlwaysVisible))
                {
                    w.Refresh();
                    if (w.ShowOverlay())
                    {
                        w.StartProgressBar(true);
                    }

                    return;
                }

                // 実際のテロップの位置を取得しておく
                telop.Left = w.Left;
                telop.Top = w.Top;

                if (telop.MatchDateTime > DateTime.MinValue)
                {
                    var start = telop.MatchDateTime.AddSeconds(telop.Delay);
                    var end = telop.MatchDateTime.AddSeconds(telop.Delay + telop.DisplayTime);

                    if (start <= DateTime.Now && DateTime.Now <= end)
                    {
                        w.Refresh();
                        w.ShowOverlay();
                        w.StartProgressBar();
                    }
                    else
                    {
                        w.HideOverlay();

                        if (DateTime.Now > end)
                        {
                            telop.MatchDateTime = DateTime.MinValue;
                            telop.MessageReplaced = string.Empty;
                        }
                    }

                    if (telop.ForceHide)
                    {
                        w.HideOverlay();
                        telop.MatchDateTime = DateTime.MinValue;
                        telop.MessageReplaced = string.Empty;
                    }
                }
                else
                {
                    w.HideOverlay();
                    telop.MessageReplaced = string.Empty;
                }
            }

            foreach (var telop in telops)
            {
#if DEBUG
                var sw = Stopwatch.StartNew();
#endif
                refreshTelop(telop);
#if DEBUG
                sw.Stop();
                if (telop.IsDesignMode &&
                    sw.Elapsed.TotalMilliseconds >= 1.0)
                {
                    Debug.WriteLine($"●refreshTelop {telop.Title} {sw.Elapsed.TotalMilliseconds:N0}ms");
                }
#endif
            }
        }

        #region Overlays Controller

        #region Hide & Close

        /// <summary>
        /// テロップを閉じる
        /// </summary>
        public void CloseTelops()
        {
            lock (this.telopWindowList)
            {
                foreach (var window in this.telopWindowList.Values)
                {
                    window.DataSource.ToClose = true;
                }
            }
        }

        public void ExecuteCloseTelops()
        {
            var closed = false;

            lock (this.telopWindowList)
            {
                var targets = this.telopWindowList
                    .Where(x => x.Value.DataSource.ToClose).ToList();

                foreach (var entry in targets)
                {
                    var window = entry.Value;
                    if (window == null)
                    {
                        continue;
                    }

                    if (window.DataSource.ToClose)
                    {
                        window.DataSource.ToClose = false;

                        window.DataSource.Left = window.Left;
                        window.DataSource.Top = window.Top;

                        window.Close();

                        telopWindowList.Remove(entry.Key);

                        closed = true;
                    }
                }
            }

            if (closed)
            {
                TickerTable.Instance.Save();
            }
        }

        /// <summary>
        /// 不要になったWindowを閉じる
        /// </summary>
        /// <param name="telops">Telops</param>
        public void GarbageWindows(
            IReadOnlyList<Ticker> telops)
        {
            lock (this.telopWindowList)
            {
                foreach (var window in this.telopWindowList.Values)
                {
                    if (!telops.Any(x => x.ID == window.DataSource.ID))
                    {
                        window.DataSource.ToClose = true;
                    }
                }
            }
        }

        /// <summary>
        /// テロップを隠す
        /// </summary>
        public void HideTelops()
        {
            lock (this.telopWindowList)
            {
                foreach (var telop in this.telopWindowList.Values)
                {
                    telop.HideOverlay();
                }
            }
        }

        #endregion Hide & Close

        /// <summary>
        /// 位置を取得する
        /// </summary>
        /// <param name="telopID">設定するテロップのID</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public void GettLocation(
            long telopID,
            out double left,
            out double top)
        {
            left = 0;
            top = 0;

            var telop = default(TickerWindow);
            lock (this.telopWindowList)
            {
                telop = this.telopWindowList.ContainsKey(telopID) ?
                    this.telopWindowList[telopID] :
                    null;
            }

            if (telop != null)
            {
                left = telop.Left;
                top = telop.Top;

                return;
            }

            var telopSettings = TickerTable.Instance.Table
                .FirstOrDefault(x => x.ID == telopID);

            if (telopSettings != null)
            {
                left = telopSettings.Left;
                top = telopSettings.Top;
            }
        }

        /// <summary>
        /// 位置を設定する
        /// </summary>
        /// <param name="telopID">設定するテロップのID</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public void SetLocation(
            long telopID,
            double left,
            double top)
        {
            var telop = default(TickerWindow);
            lock (this.telopWindowList)
            {
                telop = this.telopWindowList.ContainsKey(telopID) ?
                    this.telopWindowList[telopID] :
                    null;
            }

            if (telop != null)
            {
                telop.Left = left;
                telop.Top = top;

                return;
            }

            var telopSettings = TickerTable.Instance.Table
                .FirstOrDefault(x => x.ID == telopID);

            if (telopSettings != null)
            {
                telopSettings.Left = left;
                telopSettings.Top = top;
            }
        }

        #endregion Overlays Controller
    }
}
