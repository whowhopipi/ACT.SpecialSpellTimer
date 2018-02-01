using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;
using ACT.SpecialSpellTimer.Views;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.WPF.Views;

namespace ACT.SpecialSpellTimer
{
    public class SpellsController
    {
        #region Singleton

        private static SpellsController instance = new SpellsController();

        public static SpellsController Instance => instance;

        #endregion Singleton

        /// <summary>
        /// SpellTimerのPanelリスト
        /// </summary>
        private volatile List<SpellTimerListWindow> spellPanelWindows =
            new List<SpellTimerListWindow>();

        /// <summary>
        /// Spellをマッチングする
        /// </summary>
        /// <param name="spells">Spell</param>
        /// <param name="logLines">ログ</param>
        public void Match(
            IReadOnlyList<Models.Spell> spells,
            IReadOnlyList<string> logLines)
        {
            if (spells.Count < 1 ||
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
                    foreach (var spell in spells)
                    {
                        try
                        {
                            spell.StartMatching();
                            this.MatchCore(spell, logLine);
                        }
                        finally
                        {
                            spell.EndMatching();
                        }
                    }
                });
            }
            finally
            {
#if DEBUG
                sw.Stop();
                Debug.WriteLine($"●SpellsController.Match() {sw.Elapsed.TotalMilliseconds:N1}ms spells={spells.Count:N0} lines={logLines.Count:N0}");
#endif
            }
        }

        /// <summary>
        /// 1ログ1スペルに対して判定する
        /// </summary>
        /// <param name="spell">スペル</param>
        /// <param name="logLine">ログ</param>
        public void MatchCore(
            Models.Spell spell,
            string logLine)
        {
            var regex = spell.Regex;
            var notifyNeeded = false;

            if (!spell.IsInstance)
            {
                // マッチング計測開始
                spell.StartMatching();

                // 開始条件を確認する
                if (ConditionUtility.CheckConditionsForSpell(spell))
                {
                    // 正規表現が無効？
                    if (!spell.RegexEnabled ||
                        regex == null)
                    {
                        var keyword = spell.KeywordReplaced;
                        if (string.IsNullOrWhiteSpace(keyword))
                        {
                            return;
                        }

                        // キーワードが含まれるか？
                        if (logLine.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            var targetSpell = spell;

                            // ヒットしたログを格納する
                            targetSpell.MatchedLog = logLine;

                            // スペル名（表示テキスト）を置換する
                            var replacedTitle = ConditionUtility.GetReplacedTitle(targetSpell);

                            // PC名を置換する
                            replacedTitle = FFXIVPlugin.Instance.ReplacePartyMemberName(replacedTitle);

                            targetSpell.SpellTitleReplaced = replacedTitle;
                            targetSpell.UpdateDone = false;
                            targetSpell.OverDone = false;
                            targetSpell.BeforeDone = false;
                            targetSpell.TimeupDone = false;

                            var now = DateTime.Now;
                            targetSpell.CompleteScheduledTime = now.AddSeconds(targetSpell.RecastTime);
                            targetSpell.MatchDateTime = now;

                            // マッチング計測終了
                            spell.EndMatching();

                            // マッチ時点のサウンドを再生する
                            targetSpell.Play(targetSpell.MatchSound, targetSpell.MatchAdvancedConfig);
                            targetSpell.Play(targetSpell.MatchTextToSpeak, targetSpell.MatchAdvancedConfig);

                            notifyNeeded = true;

                            // 遅延サウンドタイマを開始する
                            targetSpell.StartOverSoundTimer();
                            targetSpell.StartBeforeSoundTimer();
                            targetSpell.StartTimeupSoundTimer();
                        }
                    }
                    else
                    {
                        // 正規表現でマッチングする
                        var match = regex.Match(logLine);
                        if (match.Success)
                        {
                            var targetSpell = spell;

                            // ヒットしたログを格納する
                            targetSpell.MatchedLog = logLine;

                            // スペル名（表示テキスト）を置換する
                            var replacedTitle = match.Result(ConditionUtility.GetReplacedTitle(targetSpell));

                            // PC名を置換する
                            replacedTitle = FFXIVPlugin.Instance.ReplacePartyMemberName(replacedTitle);

                            // インスタンス化する？
                            if (spell.ToInstance)
                            {
                                // 同じタイトルのインスタンススペルを探す
                                // 存在すればそれを使用して、なければ新しいインスタンスを生成する
                                targetSpell = SpellTable.Instance.GetOrAddInstance(
                                    replacedTitle,
                                    spell);

                                // インスタンスのガーベージタイマをスタートする
                                targetSpell.StartGarbageInstanceTimer();
                            }

                            targetSpell.SpellTitleReplaced = replacedTitle;
                            targetSpell.UpdateDone = false;
                            targetSpell.OverDone = false;
                            targetSpell.BeforeDone = false;
                            targetSpell.TimeupDone = false;

                            var now = DateTime.Now;

                            // 効果時間を決定する
                            // グループ "duration" をキャプチャーしていた場合は効果時間を置換する
                            var durationAsText = match.Groups["duration"].Value;
                            double duration;
                            if (!double.TryParse(durationAsText, out duration))
                            {
                                duration = targetSpell.RecastTime;
                            }

                            targetSpell.CompleteScheduledTime = now.AddSeconds(duration);

                            // スペル対象を保存する
                            // グループ "target" をキャプチャーしていた場合はその文字列を保存する
                            var targetName = match.Groups["target"].Value;
                            if (!string.IsNullOrWhiteSpace(targetName))
                            {
                                targetSpell.TargetName = targetName;
                            }

                            // マッチ日時を格納する
                            targetSpell.MatchDateTime = now;

                            // マッチング計測終了
                            spell.EndMatching();

                            // マッチ時点のサウンドを再生する
                            targetSpell.Play(targetSpell.MatchSound, targetSpell.MatchAdvancedConfig);

                            if (!string.IsNullOrWhiteSpace(targetSpell.MatchTextToSpeak))
                            {
                                var tts = match.Result(targetSpell.MatchTextToSpeak);
                                targetSpell.Play(tts, targetSpell.MatchAdvancedConfig);
                            }

                            notifyNeeded = true;

                            // 遅延サウンドタイマを開始する
                            targetSpell.StartOverSoundTimer();
                            targetSpell.StartBeforeSoundTimer();
                            targetSpell.StartTimeupSoundTimer();
                        }
                    }
                }
            }

            // 延長をマッチングする
            if (spell.MatchDateTime > DateTime.MinValue)
            {
                var keywords = new string[] { spell.KeywordForExtendReplaced1, spell.KeywordForExtendReplaced2 };
                var regexes = new Regex[] { spell.RegexForExtend1, spell.RegexForExtend2 };
                var timeToExtends = new double[] { spell.RecastTimeExtending1, spell.RecastTimeExtending2 };

                for (int i = 0; i < 2; i++)
                {
                    var keywordToExtend = keywords[i];
                    var regexToExtend = regexes[i];
                    var timeToExtend = timeToExtends[i];

                    // マッチングする
                    var matched = false;

                    if (!spell.RegexEnabled ||
                        regexToExtend == null)
                    {
                        if (!string.IsNullOrWhiteSpace(keywordToExtend))
                        {
                            matched = logLine.Contains(keywordToExtend, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                    else
                    {
                        var match = regexToExtend.Match(logLine);
                        matched = match.Success;

                        if (matched)
                        {
                            // targetをキャプチャーしている？
                            if (!string.IsNullOrWhiteSpace(spell.TargetName))
                            {
                                var targetName = match.Groups["target"].Value;
                                if (!string.IsNullOrWhiteSpace(targetName))
                                {
                                    // targetが当初のマッチングと一致するか確認する
                                    if (spell.TargetName != targetName)
                                    {
                                        matched = false;
                                    }
                                }
                            }
                        }
                    }

                    if (!matched)
                    {
                        continue;
                    }

                    var now = DateTime.Now;

                    // リキャストタイムを延長する
                    var newSchedule = spell.CompleteScheduledTime.AddSeconds(timeToExtend);
                    spell.BeforeDone = false;
                    spell.UpdateDone = false;

                    if (spell.ExtendBeyondOriginalRecastTime)
                    {
                        if (spell.UpperLimitOfExtension > 0)
                        {
                            var newDuration = (newSchedule - now).TotalSeconds;
                            if (newDuration > (double)spell.UpperLimitOfExtension)
                            {
                                newSchedule = newSchedule.AddSeconds(
                                    (newDuration - (double)spell.UpperLimitOfExtension) * -1);
                            }
                        }
                    }
                    else
                    {
                        var newDuration = (newSchedule - now).TotalSeconds;
                        if (newDuration > (double)spell.RecastTime)
                        {
                            newSchedule = newSchedule.AddSeconds(
                                (newDuration - (double)spell.RecastTime) * -1);
                        }
                    }

                    spell.CompleteScheduledTime = newSchedule;
                    spell.MatchDateTime = now;

                    notifyNeeded = true;

                    // 遅延サウンドタイマを開始(更新)する
                    spell.StartOverSoundTimer();
                    spell.StartBeforeSoundTimer();
                    spell.StartTimeupSoundTimer();
                }
            }
            // end if 延長マッチング

            // ACT標準のSpellTimerに変更を通知する
            if (notifyNeeded)
            {
                this.UpdateNormalSpellTimer(spell, false);
                this.NotifyNormalSpellTimer(spell);
            }
        }

        private bool beforeClickThrough = false;

        /// <summary>
        /// スペルパネルWindowを更新する
        /// </summary>
        /// <param name="spells">
        /// 対象のスペル</param>
        public void RefreshSpellOverlays(
            IReadOnlyList<Models.Spell> spells)
        {
            var query =
                from s in spells
                where
                !s.ToInstance ||
                s.IsDesignMode
                group s by s.Panel.PanelName.Trim();

            foreach (var spellsByPanel in query)
            {
                var panel = spellsByPanel.First().Panel;
                var panelWindow = panel.PanelWindow;
                if (panelWindow == null)
                {
                    panelWindow = new SpellTimerListWindow(panel)
                    {
                        Title = "Spell Panel - " + panel.PanelName,
                    };

                    lock (this.spellPanelWindows)
                    {
                        this.spellPanelWindows.Add(panelWindow);
                    }

                    /// このパネルに属するスペルを再描画させる
                    foreach (var spell in spellsByPanel)
                    {
                        spell.UpdateDone = false;
                    }

                    panelWindow.Show();
                }

                // クリックスルーを反映する
                if (this.beforeClickThrough != Settings.Default.ClickThroughEnabled)
                {
                    this.beforeClickThrough = Settings.Default.ClickThroughEnabled;
                    if (Settings.Default.ClickThroughEnabled)
                    {
                        panelWindow.ToTransparent();
                    }
                    else
                    {
                        panelWindow.ToNotTransparent();
                    }
                }

                panelWindow.Spells = spellsByPanel.ToArray();
                panelWindow.RefreshSpellTimer();
            }

            lock (this.spellPanelWindows)
            {
                var toHide = this.spellPanelWindows.Where(x => !query.Any(y => y.Key == x.Config.PanelName));
                foreach (var window in toHide)
                {
                    window.HideOverlay();
                }
            }
        }

        #region Panel controller

        #region Close & Hide

        /// <summary>
        /// Panelを閉じる
        /// </summary>
        public void ClosePanels()
        {
            lock (this.spellPanelWindows)
            {
                foreach (var panel in SpellPanelTable.Instance.Table)
                {
                    panel.ToClose = true;
                }
            }
        }

        public void ExecuteClosePanels()
        {
            var closed = false;

            lock (this.spellPanelWindows)
            {
                var targets = SpellPanelTable.Instance.Table
                    .Where(x => x.ToClose).ToList();

                foreach (var panel in targets)
                {
                    var window = panel.PanelWindow;
                    if (window == null)
                    {
                        continue;
                    }

                    if (panel.ToClose)
                    {
                        panel.ToClose = false;

                        window.Close();
                        this.spellPanelWindows.Remove(window);

                        closed = true;
                    }
                }
            }

            if (closed)
            {
                SpellPanelTable.Instance.Save();
            }
        }

        /// <summary>
        /// 不要なスペルタイマWindowを閉じる
        /// </summary>
        /// <param name="spells">Spell</param>
        public void GarbageSpellPanelWindows(
            IReadOnlyList<Models.Spell> spells)
        {
            lock (this.spellPanelWindows)
            {
                foreach (var panel in SpellPanelTable.Instance.Table)
                {
                    // スペルリストに存在しないパネルを閉じる
                    if (!spells.Any(x => x.PanelID == panel.ID))
                    {
                        panel.ToClose = true;
                    }
                }
            }
        }

        /// <summary>
        /// Panelを隠す
        /// </summary>
        public void HidePanels()
        {
            lock (this.spellPanelWindows)
            {
                foreach (var panel in SpellPanelTable.Instance.Table)
                {
                    panel.PanelWindow?.HideOverlay();
                }
            }
        }

        #endregion Close & Hide

        public SpellPanel GetPanelSettings(
            string panelName)
        {
            double normalize(double value)
            {
                var result = value;

                if (double.IsNaN(result))
                {
                    result = 0;
                }

                if (value > 65535)
                {
                    result = 65535;
                }

                if (value < -65535)
                {
                    result = -65535;
                }

                return result;
            }

            var settings = new SpellPanel()
            {
                PanelName = panelName,
                Top = 10,
                Left = 10,
                Margin = 5,
                Horizontal = false,
                FixedPositionSpell = false,
                SortOrder = SpellOrders.SortRecastTimeASC,
            };

            lock (this.spellPanelWindows)
            {
                var panel = this.spellPanelWindows
                    .Where(x => x.Config.PanelName == panelName)
                    .FirstOrDefault();
                if (panel != null)
                {
                    settings.SortOrder = panel.Config?.SortOrder ?? SpellOrders.SortRecastTimeASC;
                }
                else
                {
                    var s = SpellPanelTable.Instance.Table
                        .Where(x => x.PanelName == panelName)
                        .FirstOrDefault();

                    if (s != null)
                    {
                        settings = s;
                    }
                    else
                    {
                        SpellPanelTable.Instance.Table.Add(settings);
                    }
                }
            }

            // 変な値が入っていたら補正する
            settings.Top = normalize(settings.Top);
            settings.Left = normalize(settings.Left);
            settings.Margin = normalize(settings.Margin);

            return settings;
        }

        #endregion Panel controller

        #region To Normal SpellTimer

        /// <summary>
        /// 有効なSpellTimerをACT標準のSpellTimerに設定を反映させる
        /// </summary>
        public void ApplyToNormalSpellTimer()
        {
            // 標準スペルタイマーへの通知が無効であれば何もしない
            if (!Settings.Default.EnabledNotifyNormalSpellTimer)
            {
                return;
            }

            // 設定を一旦すべて削除する
            ClearNormalSpellTimer();

            var spells = SpellTable.Instance.Table.Where(x => x.Enabled);
            foreach (var spell in spells)
            {
                UpdateNormalSpellTimer(spell, true);
            }

            var telops = TickerTable.Instance.Table.Where(x => x.Enabled);
            foreach (var telop in telops)
            {
                UpdateNormalSpellTimerForTelop(telop, false);
            }

            // ACTのスペルタイマーに変更を反映する
            ActGlobals.oFormSpellTimers.RebuildSpellTreeView();
        }

        /// <summary>
        /// ACT標準のスペルタイマーから設定を削除する
        /// </summary>
        /// <param name="immediate">変更を即時に反映させるか？</param>
        public void ClearNormalSpellTimer(bool immediate = false)
        {
            var prefix = Settings.Default.NotifyNormalSpellTimerPrefix;
            var timerDefs = ActGlobals.oFormSpellTimers.TimerDefs
                .Where(p => p.Key.StartsWith(prefix))
                .Select(x => x.Value)
                .ToList();
            foreach (var timerDef in timerDefs)
            {
                ActGlobals.oFormSpellTimers.RemoveTimerDef(timerDef);
            }

            // ACTのスペルタイマーに変更を反映する
            if (immediate)
            {
                ActGlobals.oFormSpellTimers.RebuildSpellTreeView();
            }
        }

        /// <summary>
        /// ACT標準のスペルタイマーに通知する
        /// </summary>
        /// <param name="spellTimer">通知先に対応するスペルタイマー</param>
        public void NotifyNormalSpellTimer(Models.Spell spellTimer)
        {
            if (!Settings.Default.EnabledNotifyNormalSpellTimer)
            {
                return;
            }

            var prefix = Settings.Default.NotifyNormalSpellTimerPrefix;
            var spellName = prefix + "spell_" + spellTimer.SpellTitle;
            ActGlobals.oFormSpellTimers.NotifySpell("attacker", spellName, false, "victim", false);
        }

        /// <summary>
        /// ACT標準のスペルタイマーに通知する（テロップ用）
        /// </summary>
        /// <param name="telopTitle">通知先に対応するテロップ名</param>
        public void NotifyNormalSpellTimerForTelop(string telopTitle)
        {
            if (!Settings.Default.EnabledNotifyNormalSpellTimer)
            {
                return;
            }

            var prefix = Settings.Default.NotifyNormalSpellTimerPrefix;
            var spellName = prefix + "telop_" + telopTitle;
            ActGlobals.oFormSpellTimers.NotifySpell("attacker", spellName, false, "victim", false);
        }

        /// <summary>
        /// ACT標準のスペルタイマーの設定を追加・更新する
        /// </summary>
        /// <param name="spellTimer">元になるスペルタイマー</param>
        /// <param name="useRecastTime">リキャスト時間にRecastの値を使うか。falseの場合はCompleteScheduledTimeから計算される</param>
        public void UpdateNormalSpellTimer(Models.Spell spellTimer, bool useRecastTime)
        {
            if (!Settings.Default.EnabledNotifyNormalSpellTimer)
            {
                return;
            }

            var prefix = Settings.Default.NotifyNormalSpellTimerPrefix;
            var spellName = prefix + "spell_" + spellTimer.SpellTitle;
            var categoryName = prefix + spellTimer.Panel.PanelName;
            var recastTime = useRecastTime ? spellTimer.RecastTime : (spellTimer.CompleteScheduledTime - DateTime.Now).TotalSeconds;

            var timerData = new TimerData(spellName, categoryName);
            timerData.TimerValue = (int)recastTime;
            timerData.RemoveValue = (int)-Settings.Default.TimeOfHideSpell;
            timerData.WarningValue = 0;
            timerData.OnlyMasterTicks = true;
            timerData.Tooltip = spellTimer.SpellTitleReplaced;

            timerData.Panel1Display = false;
            timerData.Panel2Display = false;

            // disable warning sound
            timerData.WarningSoundData = "none";

            // initialize other parameters
            timerData.RestrictToMe = false;
            timerData.AbsoluteTiming = false;
            timerData.RestrictToCategory = false;

            ActGlobals.oFormSpellTimers.AddEditTimerDef(timerData);
        }

        /// <summary>
        /// ACT標準のスペルタイマーの設定を追加・更新する（テロップ用）
        /// </summary>
        /// <param name="spellTimer">元になるテロップ</param>
        /// <param name="forceHide">強制非表示か？</param>
        public void UpdateNormalSpellTimerForTelop(Ticker telop, bool forceHide)
        {
            if (!Settings.Default.EnabledNotifyNormalSpellTimer)
            {
                return;
            }

            var prefix = Settings.Default.NotifyNormalSpellTimerPrefix;
            var spellName = prefix + "telop_" + telop.Title;
            var categoryName = prefix + "telops";

            var timerData = new TimerData(spellName, categoryName);
            timerData.TimerValue = forceHide ? 1 : (int)(telop.DisplayTime + telop.Delay);
            timerData.RemoveValue = forceHide ? -timerData.TimerValue : 0;
            timerData.WarningValue = (int)telop.DisplayTime;
            timerData.OnlyMasterTicks = telop.AddMessageEnabled ? false : true;
            timerData.Tooltip = telop.MessageReplaced;

            timerData.Panel1Display = false;
            timerData.Panel2Display = false;

            // disable warning sound
            timerData.WarningSoundData = "none";

            // initialize other parameters
            timerData.RestrictToMe = false;
            timerData.AbsoluteTiming = false;
            timerData.RestrictToCategory = false;

            ActGlobals.oFormSpellTimers.AddEditTimerDef(timerData);
        }

        #endregion To Normal SpellTimer
    }
}
