using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Sound;
using ACT.SpecialSpellTimer.Utility;
using ACT.SpecialSpellTimer.Views;
using Advanced_Combat_Tracker;

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
        private volatile List<SpellTimerListWindow> spellTimerPanels =
            new List<SpellTimerListWindow>();

        /// <summary>
        /// Spellをマッチングする
        /// </summary>
        /// <param name="spells">Spell</param>
        /// <param name="logLines">ログ</param>
        public void Match(
            IReadOnlyList<Models.SpellTimer> spells,
            IReadOnlyList<string> logLines)
        {
            foreach (var logLine in logLines)
            {
                // マッチする？
                spells.AsParallel().ForAll(spell =>
                {
                    var regex = spell.Regex;
                    var notifyNeeded = false;

                    if (!spell.IsInstance)
                    {
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
                                if (logLine.ToUpper().Contains(
                                    keyword.ToUpper()))
                                {
                                    var targetSpell = spell;

                                    // ヒットしたログを格納する
                                    targetSpell.MatchedLog = logLine;

                                    // スペル名（表示テキスト）を置換する
                                    var replacedTitle = ConditionUtility.GetReplacedTitle(targetSpell);

                                    // PC名を置換する
                                    replacedTitle = FFXIV.Instance.ReplacePartyMemberName(replacedTitle);

                                    targetSpell.SpellTitleReplaced = replacedTitle;
                                    targetSpell.MatchDateTime = DateTime.Now;
                                    targetSpell.UpdateDone = false;
                                    targetSpell.OverDone = false;
                                    targetSpell.BeforeDone = false;
                                    targetSpell.TimeupDone = false;
                                    targetSpell.CompleteScheduledTime = targetSpell.MatchDateTime.AddSeconds(targetSpell.RecastTime);

                                    // マッチ時点のサウンドを再生する
                                    this.Play(targetSpell.MatchSound);
                                    this.Play(targetSpell.MatchTextToSpeak);

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
                                    replacedTitle = FFXIV.Instance.ReplacePartyMemberName(replacedTitle);

                                    // インスタンス化する？
                                    if (spell.ToInstance)
                                    {
                                        // 同じタイトルのインスタンススペルを探す
                                        // 存在すればそれを使用して、なければ新しいインスタンスを生成する
                                        targetSpell = SpellTimerTable.Instance.GetOrAddInstance(
                                            replacedTitle,
                                            spell);

                                        // インスタンスのガーベージタイマをスタートする
                                        targetSpell.StartGarbageInstanceTimer();
                                    }

                                    targetSpell.SpellTitleReplaced = replacedTitle;
                                    targetSpell.MatchDateTime = DateTime.Now;
                                    targetSpell.UpdateDone = false;
                                    targetSpell.OverDone = false;
                                    targetSpell.BeforeDone = false;
                                    targetSpell.TimeupDone = false;

                                    // 効果時間を決定する
                                    // グループ "duration" をキャプチャーしていた場合は効果時間を置換する
                                    var durationAsText = match.Groups["duration"].Value;
                                    double duration;
                                    if (!double.TryParse(durationAsText, out duration))
                                    {
                                        duration = targetSpell.RecastTime;
                                    }

                                    targetSpell.CompleteScheduledTime = targetSpell.MatchDateTime.AddSeconds(duration);

                                    // スペル対象を保存する
                                    // グループ "target" をキャプチャーしていた場合はその文字列を保存する
                                    var targetName = match.Groups["target"].Value;
                                    if (!string.IsNullOrWhiteSpace(targetName))
                                    {
                                        targetSpell.TargetName = targetName;
                                    }

                                    // マッチ時点のサウンドを再生する
                                    this.Play(targetSpell.MatchSound);

                                    if (!string.IsNullOrWhiteSpace(targetSpell.MatchTextToSpeak))
                                    {
                                        var tts = match.Result(targetSpell.MatchTextToSpeak);
                                        this.Play(tts);
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
                                    matched = logLine.ToUpper().Contains(keywordToExtend.ToUpper());
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

                            spell.MatchDateTime = now;
                            spell.CompleteScheduledTime = newSchedule;

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
                });
                // end loop of Spells
            }
            // end loop of LogLines
        }

        /// <summary>
        /// スペルパネルWindowを更新する
        /// </summary>
        /// <param name="spells">
        /// 対象のスペル</param>
        public void RefreshSpellOverlays(
            IReadOnlyList<Models.SpellTimer> spells)
        {
            var spellsGroupByPanel =
                from s in spells
                where !s.ToInstance
                group s by s.Panel.Trim();

            foreach (var panel in spellsGroupByPanel)
            {
                var f = panel.First();

                var w = this.FindPanelByName(f.Panel);
                if (w == null)
                {
                    w = new SpellTimerListWindow()
                    {
                        Title = "SpecialSpellTimer - " + f.Panel,
                        PanelName = f.Panel,
                    };

                    lock (this.spellTimerPanels)
                    {
                        this.spellTimerPanels.Add(w);
                    }

                    // クリックスルー？
                    if (Settings.Default.ClickThroughEnabled)
                    {
                        w.ToTransparentWindow();
                    }

                    /// このパネルに属するスペルを再描画させる
                    foreach (var spell in panel)
                    {
                        spell.UpdateDone = false;
                    }

                    w.Show();
                }

                w.SpellTimers = panel.ToArray();

                // ドラッグ中じゃない？
                if (!w.IsDragging)
                {
                    w.RefreshSpellTimer();
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
            lock (this.spellTimerPanels)
            {
                foreach (var setting in PanelTable.Instance.SettingsTable)
                {
                    setting.ToClose = true;
                }
            }
        }

        public void ExecuteClosePanels()
        {
            var closed = false;

            lock (this.spellTimerPanels)
            {
                var targets = PanelTable.Instance.SettingsTable
                    .Where(x => x.ToClose).ToList();

                foreach (var setting in targets)
                {
                    var panel = setting.PanelWindow;
                    if (panel == null)
                    {
                        continue;
                    }

                    if (setting.ToClose)
                    {
                        setting.ToClose = false;

                        setting.PanelName = panel.PanelName;
                        setting.Left = panel.Left;
                        setting.Top = panel.Top;

                        panel.Close();
                        this.spellTimerPanels.Remove(panel);

                        closed = true;
                    }
                }
            }

            if (closed)
            {
                PanelTable.Instance.Save();
            }
        }

        /// <summary>
        /// 不要なスペルタイマWindowを閉じる
        /// </summary>
        /// <param name="spells">Spell</param>
        public void GarbageSpellPanelWindows(
            IReadOnlyList<Models.SpellTimer> spells)
        {
            lock (this.spellTimerPanels)
            {
                foreach (var setting in PanelTable.Instance.SettingsTable)
                {
                    // スペルリストに存在しないパネルを閉じる
                    if (!spells.Any(x =>
                        x.Panel == setting.PanelName))
                    {
                        setting.ToClose = true;
                    }
                }
            }
        }

        /// <summary>
        /// Panelを隠す
        /// </summary>
        public void HidePanels()
        {
            lock (this.spellTimerPanels)
            {
                foreach (var setting in PanelTable.Instance.SettingsTable)
                {
                    setting.PanelWindow?.HideOverlay();
                }
            }
        }

        #endregion Close & Hide

        public PanelSettings GetPanelSettings(
            string panelName)
        {
            double normalize(double value)
            {
                var result = value;

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

            var settings = new PanelSettings()
            {
                PanelName = panelName,
                Top = 10,
                Left = 10,
                Margin = 5,
                Horizontal = false,
                FixedPositionSpell = false,
            };

            lock (this.spellTimerPanels)
            {
                var panel = this.FindPanelByName(panelName);
                if (panel != null)
                {
                    settings.Top = panel.Top;
                    settings.Left = panel.Left;
                    settings.Margin = panel.SpellMargin;
                    settings.Horizontal = panel.IsHorizontal;
                    settings.FixedPositionSpell = panel.SpellPositionFixed;
                }
                else
                {
                    var s = this.FindPanelSettingByName(panelName);
                    if (s != null)
                    {
                        settings = s;
                    }
                }
            }

            // 変な値が入っていたら補正する
            settings.Top = normalize(settings.Top);
            settings.Left = normalize(settings.Left);
            settings.Margin = normalize(settings.Margin);

            return settings;
        }

        /// <summary>
        /// Panelの位置を設定する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public void SetPanelSettings(
            string panelName,
            double left,
            double top,
            double margin,
            bool horizontal,
            bool fixedPositionSpell)
        {
            lock (this.spellTimerPanels)
            {
                var panel = this.spellTimerPanels
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();

                if (panel != null)
                {
                    panel.Left = left;
                    panel.Top = top;
                    panel.SpellMargin = (int)margin;
                    panel.IsHorizontal = horizontal;
                    panel.SpellPositionFixed = fixedPositionSpell;
                }

                var panelSettings = PanelTable.Instance.SettingsTable
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();

                if (panelSettings != null)
                {
                    panelSettings.Left = left;
                    panelSettings.Top = top;
                    panelSettings.Margin = margin;
                    panelSettings.Horizontal = horizontal;
                    panelSettings.FixedPositionSpell = fixedPositionSpell;
                }
            }
        }

        /// <summary>
        /// SpellTimerListWindowを取得する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        private SpellTimerListWindow FindPanelByName(string panelName)
        {
            lock (this.spellTimerPanels)
            {
                return this.spellTimerPanels
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// PanelSettingsRowを取得する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        public PanelSettings FindPanelSettingByName(string panelName)
        {
            var settings = new PanelSettings()
            {
                PanelName = panelName,
                Top = 10,
                Left = 10,
                Margin = 5,
                Horizontal = false,
                FixedPositionSpell = false,
            };

            lock (this.spellTimerPanels)
            {
                var s = PanelTable.Instance.SettingsTable
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();

                if (s != null)
                {
                    settings = s;
                }
                else
                {
                    PanelTable.Instance.SettingsTable.Add(settings);
                }
            }

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

            var spells = SpellTimerTable.Instance.Table.Where(x => x.Enabled);
            foreach (var spell in spells)
            {
                UpdateNormalSpellTimer(spell, true);
            }

            var telops = OnePointTelopTable.Instance.Table.Where(x => x.Enabled);
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
        public void NotifyNormalSpellTimer(Models.SpellTimer spellTimer)
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
        public void UpdateNormalSpellTimer(Models.SpellTimer spellTimer, bool useRecastTime)
        {
            if (!Settings.Default.EnabledNotifyNormalSpellTimer)
            {
                return;
            }

            var prefix = Settings.Default.NotifyNormalSpellTimerPrefix;
            var spellName = prefix + "spell_" + spellTimer.SpellTitle;
            var categoryName = prefix + spellTimer.Panel;
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
        public void UpdateNormalSpellTimerForTelop(OnePointTelop telop, bool forceHide)
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

        /// <summary>
        /// 再生する
        /// </summary>
        /// <param name="source">再生するSource</param>
        private void Play(
            string source)
        {
            SoundController.Instance.Play(source);
        }
    }
}
