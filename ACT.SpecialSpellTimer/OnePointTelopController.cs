using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Sound;
using ACT.SpecialSpellTimer.Utility;
using ACT.SpecialSpellTimer.Views;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// ワンポイントテレロップ Controller
    /// </summary>
    public class OnePointTelopController
    {
        /// <summary>
        /// テロップWindowのリスト
        /// </summary>
        private volatile static Dictionary<long, OnePointTelopWindow> telopWindowList =
            new Dictionary<long, OnePointTelopWindow>();

        /// <summary>
        /// ログとマッチングする
        /// </summary>
        /// <param name="telops">Telops</param>
        /// <param name="logLines">ログ行</param>
        public static void Match(
            IReadOnlyList<OnePointTelop> telops,
            IReadOnlyList<string> logLines)
        {
            foreach (var log in logLines)
            {
                telops.AsParallel().ForAll(telop =>
                {
                    var matched = false;

                    var regex = telop.Regex;
                    var regexToHide = telop.RegexToHide;

                    // 開始条件を確認する
                    if (ConditionUtility.CheckConditionsForTelop(telop))
                    {
                        // 通常マッチ
                        if (regex == null)
                        {
                            var keyword = telop.KeywordReplaced;
                            if (!string.IsNullOrWhiteSpace(keyword))
                            {
                                if (log.ToUpper().Contains(
                                    keyword.ToUpper()))
                                {
                                    var messageReplaced = ConditionUtility.GetReplacedMessage(telop);

                                    // PC名を置換する
                                    messageReplaced = FFXIV.Instance.ReplacePartyMemberName(messageReplaced);

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

                                    telop.MatchDateTime = DateTime.Now;
                                    telop.Delayed = false;
                                    telop.MatchedLog = log;
                                    telop.ForceHide = false;

                                    SoundController.Instance.Play(telop.MatchSound);
                                    SoundController.Instance.Play(telop.MatchTextToSpeak);

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
                                messageReplaced = FFXIV.Instance.ReplacePartyMemberName(messageReplaced);

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

                                telop.MatchDateTime = DateTime.Now;
                                telop.Delayed = false;
                                telop.MatchedLog = log;
                                telop.ForceHide = false;

                                SoundController.Instance.Play(telop.MatchSound);
                                if (!string.IsNullOrWhiteSpace(telop.MatchTextToSpeak))
                                {
                                    var tts = match.Result(telop.MatchTextToSpeak);
                                    SoundController.Instance.Play(tts);
                                }

                                matched = true;
                            }
                        }
                    }

                    if (matched)
                    {
                        // ディレイサウンドをスタートさせる
                        telop.StartDelayedSoundTimer();

                        SpellTimerCore.Instance.UpdateNormalSpellTimerForTelop(telop, telop.ForceHide);
                        SpellTimerCore.Instance.NotifyNormalSpellTimerForTelop(telop.Title);

                        return;
                    }

                    // 通常マッチ(強制非表示)
                    if (regexToHide == null)
                    {
                        var keyword = telop.KeywordToHideReplaced;
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            if (log.ToUpper().Contains(
                                keyword.ToUpper()))
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
                        SpellTimerCore.Instance.UpdateNormalSpellTimerForTelop(telop, telop.ForceHide);
                        SpellTimerCore.Instance.NotifyNormalSpellTimerForTelop(telop.Title);
                    }
                });   // end loop telops
            }
        }

        /// <summary>
        /// Windowをリフレッシュする
        /// </summary>
        /// <param name="telop">テロップ</param>
        public static void RefreshTelopOverlays(
            IReadOnlyList<OnePointTelop> telops)
        {
            void refreshTelop(
                OnePointTelop telop)
            {
                var w = telopWindowList.ContainsKey(telop.ID) ? telopWindowList[telop.ID] : null;
                if (w == null)
                {
                    w = new OnePointTelopWindow()
                    {
                        Title = "OnePointTelop - " + telop.Title,
                        DataSource = telop
                    };

                    if (Settings.Default.ClickThroughEnabled)
                    {
                        w.ToTransparentWindow();
                    }

                    w.Opacity = 0;
                    w.Topmost = false;
                    w.Show();

                    telopWindowList.Add(telop.ID, w);
                }

                if (Settings.Default.OverlayVisible &&
                    Settings.Default.TelopAlwaysVisible)
                {
                    // ドラッグ中じゃない？
                    if (!w.IsDragging)
                    {
                        w.Refresh();
                        if (w.ShowOverlay())
                        {
                            w.StartProgressBar();
                        }
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
                        if (w.ShowOverlay())
                        {
                            w.StartProgressBar();
                        }
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
                refreshTelop(telop);
            }
        }

        #region Overlays Controller

        /// <summary>
        /// テロップをActive化する
        /// </summary>
        public static void ActivateTelops()
        {
            if (telopWindowList == null)
            {
                return;
            }

            void activateTelopsCore()
            {
                foreach (var telop in telopWindowList.Values)
                {
                    telop.Activate();
                }
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)activateTelopsCore);
        }

        /// <summary>
        /// テロップを閉じる
        /// </summary>
        public static void CloseTelops()
        {
            if (telopWindowList == null)
            {
                return;
            }

            void closeTelopsCore()
            {
                foreach (var telop in telopWindowList.Values)
                {
                    telop.DataSource.Left = telop.Left;
                    telop.DataSource.Top = telop.Top;
                    telop.Close();
                }
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (Action)closeTelopsCore);

            if (telopWindowList.Count > 0)
            {
                OnePointTelopTable.Instance.Save();
            }

            telopWindowList.Clear();
        }

        /// <summary>
        /// 不要になったWindowを閉じる
        /// </summary>
        /// <param name="telops">Telops</param>
        public static void GarbageWindows(
            IReadOnlyList<OnePointTelop> telops)
        {
            void removeWindows()
            {
                if (telopWindowList == null)
                {
                    return;
                }

                // 不要になったWindowを閉じる
                var removeWindowList = new List<OnePointTelopWindow>();
                foreach (var window in telopWindowList.Values)
                {
                    if (!telops.Any(x => x.ID == window.DataSource.ID))
                    {
                        removeWindowList.Add(window);
                    }
                }

                foreach (var window in removeWindowList)
                {
                    window.DataSource.Left = window.Left;
                    window.DataSource.Top = window.Top;
                    window.Close();

                    telopWindowList.Remove(window.DataSource.ID);
                }
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (Action)removeWindows);
        }

        /// <summary>
        /// 位置を取得する
        /// </summary>
        /// <param name="telopID">設定するテロップのID</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public static void GettLocation(
            long telopID,
            out double left,
            out double top)
        {
            left = 0;
            top = 0;

            if (telopWindowList != null)
            {
                var telop = telopWindowList.ContainsKey(telopID) ?
                    telopWindowList[telopID] :
                    null;

                if (telop != null)
                {
                    left = telop.Left;
                    top = telop.Top;

                    return;
                }

                var telopSettings = OnePointTelopTable.Instance.Table
                    .Where(x => x.ID == telopID)
                    .FirstOrDefault();

                if (telopSettings != null)
                {
                    left = telopSettings.Left;
                    top = telopSettings.Top;
                }
            }
        }

        /// <summary>
        /// テロップを隠す
        /// </summary>
        public static void HideTelops()
        {
            if (telopWindowList != null)
            {
                foreach (var telop in telopWindowList.Values)
                {
                    telop.HideOverlay();
                }
            }
        }

        /// <summary>
        /// 位置を設定する
        /// </summary>
        /// <param name="telopID">設定するテロップのID</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public static void SetLocation(
            long telopID,
            double left,
            double top)
        {
            if (telopWindowList != null)
            {
                var telop = telopWindowList.ContainsKey(telopID) ?
                    telopWindowList[telopID] :
                    null;

                if (telop != null)
                {
                    telop.Left = left;
                    telop.Top = top;
                }
                else
                {
                    var telopSettings = OnePointTelopTable.Instance.Table
                        .Where(x => x.ID == telopID)
                        .FirstOrDefault();

                    if (telopSettings != null)
                    {
                        telopSettings.Left = left;
                        telopSettings.Top = top;
                    }
                }
            }
        }

        #endregion Overlays Controller
    }
}
