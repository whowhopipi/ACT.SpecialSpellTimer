using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;
using ACT.SpecialSpellTimer.Views;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// SpellTimerの中核
    /// </summary>
    public class SpellTimerCore
    {
        private const int INVALID = 1;

        private const int VALID = 0;

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static SpellTimerCore instance = new SpellTimerCore();

        /// <summary>
        /// 最後にFF14プロセスをチェックした時間
        /// </summary>
        private DateTime LastFFXIVProcessDateTime;

        /// <summary>
        /// 最後のアクティブWindowの状態
        /// </summary>
        private volatile bool LastStateOfActiveWindow;

        /// <summary>
        /// 最後に全滅した日時
        /// </summary>
        private DateTime LastWipeOutDateTime = DateTime.MinValue;

        /// <summary>
        /// ログバッファ
        /// </summary>
        private LogBuffer LogBuffer;

        /// <summary>
        /// ログポーリングスレッド
        /// </summary>
        private Thread logPoller;

        private volatile bool refreshWindowExcuting = false;

        /// <summary>
        /// RefreshWindowタイマ
        /// </summary>
        private System.Windows.Threading.DispatcherTimer refreshWindowTimer;

        private volatile bool running = false;

        private int settingsIsValid = VALID;

        /// <summary>
        /// 最後にアクティブWindowをチェックした時間
        /// </summary>
        private DateTime TimeOfLastWatchedActieWindow;

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static SpellTimerCore Default => instance;

        /// <summary>
        /// SpellTimerのPanelリスト
        /// </summary>
        private List<SpellTimerListWindow> SpellTimerPanels { get; set; }

        /// <summary>
        /// Panelをアクティブ化する
        /// </summary>
        public void ActivatePanels()
        {
            if (this.SpellTimerPanels != null)
            {
                ActInvoker.Invoke(() =>
                {
                    foreach (var panel in this.SpellTimerPanels)
                    {
                        panel.Activate();
                    }
                });
            }
        }

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
        /// 開始する
        /// </summary>
        public void Begin()
        {
            // FFXIVのスキャンを開始する
            FFXIV.Instance.Start();

            // テーブルコンパイラを開始する
            TableCompiler.Instance.Begin();

            // 戦闘分析を初期化する
            CombatAnalyzer.Default.Initialize();

            // Panelリストを生成する
            this.SpellTimerPanels = new List<SpellTimerListWindow>();

            // ログバッファを生成する
            this.LogBuffer = new LogBuffer();

            // RefreshWindowタイマを開始する
            this.refreshWindowTimer =
                new DispatcherTimer(DispatcherPriority.Background);
            this.refreshWindowTimer.Tick += this.RefreshWindowTimerOnTick;
            this.refreshWindowTimer.Interval = TimeSpan.FromMilliseconds(Settings.Default.RefreshInterval);
            this.refreshWindowTimer.Start();

            // ログ監視タイマを開始する
            this.running = true;
            this.logPoller = new Thread(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Logger.Write("start log poll");
                while (this.running)
                {
                    try
                    {
                        WatchLog();
                    }
                    catch (ThreadAbortException)
                    {
                        this.running = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("logPoller error:", ex);
                    }
                }

                Logger.Write("end log poll");
            });

            this.logPoller.Start();
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
        /// Panelを閉じる
        /// </summary>
        public void ClosePanels()
        {
            if (this.SpellTimerPanels == null)
            {
                return;
            }

            void colosePanelsCore()
            {
                // Panelの位置を保存する
                foreach (var panel in this.SpellTimerPanels)
                {
                    var setting = (
                        from x in PanelSettings.Instance.SettingsTable
                        where
                        x.PanelName == panel.PanelName
                        select
                        x).FirstOrDefault();

                    if (setting == null)
                    {
                        setting = PanelSettings.Instance.SettingsTable.NewPanelSettingsRow();
                        PanelSettings.Instance.SettingsTable.AddPanelSettingsRow(setting);
                    }

                    setting.PanelName = panel.PanelName;
                    setting.Left = panel.Left;
                    setting.Top = panel.Top;
                }

                if (this.SpellTimerPanels.Count > 0)
                {
                    PanelSettings.Instance.Save();
                }

                foreach (var panel in this.SpellTimerPanels)
                {
                    panel.Close();
                }

                this.SpellTimerPanels.Clear();
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (Action)colosePanelsCore);
        }

        /// <summary>
        /// 終了する
        /// </summary>
        public void End()
        {
            this.running = false;

            // 戦闘分析を開放する
            CombatAnalyzer.Default.Denitialize();

            // ログバッファを開放する
            if (this.LogBuffer != null)
            {
                this.LogBuffer.Dispose();
                this.LogBuffer = null;
            }

            // Viewの描画を開放する
            if (this.refreshWindowTimer != null)
            {
                this.refreshWindowTimer.Stop();
                this.refreshWindowTimer = null;
            }

            // 監視を開放する
            if (this.logPoller != null)
            {
                this.logPoller.Join(TimeSpan.FromSeconds(5));
                if (this.logPoller.IsAlive)
                {
                    this.logPoller.Abort();
                }

                this.logPoller = null;
            }

            // 全てのPanelを閉じる
            this.ClosePanels();
            OnePointTelopController.CloseTelops();

            // 設定を保存する
            Settings.Default.Save();
            SpellTimerTable.Instance.Save();
            OnePointTelopTable.Instance.Save();

            // テーブルコンパイラを停止する
            TableCompiler.Instance.End();

            // FFXIVのスキャンを停止する
            FFXIV.Instance.End();

            // instanceを初期化する
            instance = null;
        }

        /// <summary>
        /// Panelのレイアウトを取得する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        /// <param name="horizontal">水平レイアウトか？</param>
        /// <param name="fixedPositionSpell">スペル位置を固定するか？</param>
        public void GetPanelLayout(
            string panelName,
            out bool horizontal,
            out bool fixedPositionSpell)
        {
            horizontal = false;
            fixedPositionSpell = false;

            if (this.SpellTimerPanels != null)
            {
                var panel = this.FindPanelByName(panelName);
                if (panel != null)
                {
                    horizontal = panel.IsHorizontal;
                    fixedPositionSpell = panel.SpellPositionFixed;
                }
                else
                {
                    var setting = this.FindPanelSettingByName(panelName);
                    if (setting != null)
                    {
                        horizontal = setting.Horizontal;
                        fixedPositionSpell = setting.FixedPositionSpell;
                    }
                }
            }
        }

        /// <summary>
        /// Panelの位置を取得する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public void GetPanelLocation(
            string panelName,
            out double left,
            out double top)
        {
            left = 10.0d;
            top = 10.0d;

            if (this.SpellTimerPanels != null)
            {
                var panel = this.SpellTimerPanels
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();

                if (panel != null)
                {
                    left = panel.Left;
                    top = panel.Top;
                }
                else
                {
                    var panelSettings = PanelSettings.Instance.SettingsTable
                        .Where(x => x.PanelName == panelName)
                        .FirstOrDefault();

                    if (panelSettings != null)
                    {
                        left = panelSettings.Left;
                        top = panelSettings.Top;
                    }
                }
            }
        }

        /// <summary>
        /// SpellTimer間のマージンを取得する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        /// <param name="margin">マージン</param>
        public void GetSpellMargin(
            string panelName,
            out int margin)
        {
            margin = 0;

            if (this.SpellTimerPanels != null)
            {
                var panel = this.FindPanelByName(panelName);
                if (panel != null)
                {
                    margin = panel.SpellMargin;
                }
                else
                {
                    var setting = this.FindPanelSettingByName(panelName);
                    if (setting != null)
                    {
                        margin = setting.Margin;
                    }
                }
            }
        }

        /// <summary>
        /// Panelを隠す
        /// </summary>
        public void HidePanels()
        {
            if (this.SpellTimerPanels != null)
            {
                foreach (var panel in this.SpellTimerPanels)
                {
                    panel.HideOverlay();
                }
            }
        }

        public void InvalidateSettings()
        {
            settingsIsValid = INVALID;
        }

        /// <summary>
        /// Panelの位置を設定する
        /// </summary>
        public void LayoutPanels()
        {
            if (this.SpellTimerPanels != null)
            {
                ActInvoker.Invoke(() =>
                {
                    foreach (var panel in this.SpellTimerPanels)
                    {
                        var setting = PanelSettings.Instance.SettingsTable
                            .Where(x => x.PanelName == panel.PanelName)
                            .FirstOrDefault();

                        if (setting != null)
                        {
                            panel.Left = setting.Left;
                            panel.Top = setting.Top;
                        }
                        else
                        {
                            panel.Left = 10.0d;
                            panel.Top = 10.0d;
                        }
                    }
                });
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
        /// Panelのレイアウトを設定する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        /// <param name="horizontal">水平レイアウトか？</param>
        /// <param name="fixedPositionSpell">スペル位置を固定するか？</param>
        public void SetPanelLayout(
            string panelName,
            bool horizontal,
            bool fixedPositionSpell)
        {
            if (this.SpellTimerPanels != null)
            {
                var panel = this.FindPanelByName(panelName);
                if (panel != null)
                {
                    panel.IsHorizontal = horizontal;
                    panel.SpellPositionFixed = fixedPositionSpell;
                }

                var setting = this.FindPanelSettingByName(panelName);
                if (setting != null)
                {
                    setting.Horizontal = horizontal;
                    setting.FixedPositionSpell = fixedPositionSpell;
                }
            }
        }

        /// <summary>
        /// Panelの位置を設定する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public void SetPanelLocation(
            string panelName,
            double left,
            double top)
        {
            if (this.SpellTimerPanels != null)
            {
                var panel = this.SpellTimerPanels
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();

                if (panel != null)
                {
                    panel.Left = left;
                    panel.Top = top;
                }

                var panelSettings = PanelSettings.Instance.SettingsTable
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();

                if (panelSettings != null)
                {
                    panelSettings.Left = left;
                    panelSettings.Top = top;
                }
            }
        }

        /// <summary>
        /// SpellTimer間のマージンを設定する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        /// <param name="marign">マージン</param>
        public void SetSpellMargin(
            string panelName,
            int margin)
        {
            if (this.SpellTimerPanels != null)
            {
                var panel = this.FindPanelByName(panelName);
                if (panel != null)
                {
                    panel.SpellMargin = margin;
                }

                var setting = this.FindPanelSettingByName(panelName);
                if (setting != null)
                {
                    setting.Margin = margin;
                }
            }
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

        /// <summary>
        /// SpellTimerListWindowを取得する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        private SpellTimerListWindow FindPanelByName(string panelName)
        {
            if (this.SpellTimerPanels != null)
            {
                return this.SpellTimerPanels
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// PanelSettingsRowを取得する
        /// </summary>
        /// <param name="panelName">パネルの名前</param>
        private SpellTimerDataSet.PanelSettingsRow FindPanelSettingByName(string panelName)
        {
            if (this.SpellTimerPanels != null)
            {
                return PanelSettings.Instance.SettingsTable
                    .Where(x => x.PanelName == panelName)
                    .FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// 不要なスペルタイマWindowを閉じる
        /// </summary>
        /// <param name="spells">Spell</param>
        public void GarbageSpellPanelWindows(
            IReadOnlyList<Models.SpellTimer> spells)
        {
            void removePanels()
            {
                if (this.SpellTimerPanels == null)
                {
                    return;
                }

                var removeList = new List<SpellTimerListWindow>();
                foreach (var panel in this.SpellTimerPanels)
                {
                    // パネルの位置を保存する
                    var setting = (
                        from x in PanelSettings.Instance.SettingsTable
                        where
                        x.PanelName == panel.PanelName
                        select
                        x).FirstOrDefault();

                    if (setting == null)
                    {
                        setting = PanelSettings.Instance.SettingsTable.NewPanelSettingsRow();
                        PanelSettings.Instance.SettingsTable.AddPanelSettingsRow(setting);
                    }

                    setting.PanelName = panel.PanelName;
                    setting.Left = panel.Left;
                    setting.Top = panel.Top;

                    PanelSettings.Instance.Save();

                    // スペルリストに存在しないパネルを閉じる
                    if (!spells.Any(x => x.Panel == panel.PanelName))
                    {
                        ActInvoker.Invoke(() => panel.Close());
                        removeList.Add(panel);
                    }
                }

                foreach (var item in removeList)
                {
                    this.SpellTimerPanels.Remove(item);
                }
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (Action)removePanels);
        }

        /// <summary>
        /// FFXIVまたはACTがアクティブか？
        /// </summary>
        /// <returns>
        /// FFXIVまたはACTがアクティブか？</returns>
        private bool IsActive()
        {
            var r = true;

            try
            {
                // 最後にチェックしてから1秒以内ならば最後の状態を返す
                if ((DateTime.Now - this.TimeOfLastWatchedActieWindow).TotalSeconds <= 1.0d)
                {
                    return this.LastStateOfActiveWindow;
                }

                // フォアグラウンドWindowのハンドルを取得する
                var hWnd = GetForegroundWindow();

                // プロセスIDに変換する
                int pid;
                GetWindowThreadProcessId(hWnd, out pid);

                // メインモジュールのファイル名を取得する
                var p = Process.GetProcessById(pid);
                if (p != null)
                {
                    var fileName = Path.GetFileName(
                        p.MainModule.FileName);

                    var actFileName = Path.GetFileName(
                        Process.GetCurrentProcess().MainModule.FileName);

                    Debug.WriteLine(
                        DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]") + " " +
                        "●WatchActive " + fileName);

                    if (fileName.ToLower() == "ffxiv.exe" ||
                        fileName.ToLower() == "ffxiv_dx11.exe" ||
                        fileName.ToLower() == "dqx.exe" ||
                        fileName.ToLower() == actFileName.ToLower())
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;
                    }
                }
            }
            catch (Win32Exception)
            {
                // ignore
            }
            catch (Exception ex)
            {
                Logger.Write(Translate.Get("WatchActiveError"), ex);
            }

            // 最後の状態を保存する
            this.LastStateOfActiveWindow = r;
            this.TimeOfLastWatchedActieWindow = DateTime.Now;

            return r;
        }

        /// <summary>
        /// Spellをマッチングする
        /// </summary>
        /// <param name="spells">Spell</param>
        /// <param name="logLines">ログ</param>
        private void MatchSpells(
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
        /// 再生する
        /// </summary>
        /// <param name="source">再生するSource</param>
        private void Play(
            string source)
        {
            ACT.SpecialSpellTimer.Sound.SoundController.Default.Play(source);
        }

        /// <summary>
        /// スペルパネルWindowを更新する
        /// </summary>
        /// <param name="spells">
        /// 対象のスペル</param>
        private void RefreshSpellPanelWindows(
            IReadOnlyList<Models.SpellTimer> spells)
        {
            var spellsGroupByPanel =
                from s in spells
                where !s.ToInstance
                group s by s.Panel.Trim();

            foreach (var panel in spellsGroupByPanel)
            {
                var f = panel.First();

                var w = this.SpellTimerPanels
                    .Where(x => x.PanelName == f.Panel)
                    .FirstOrDefault();

                if (w == null)
                {
                    w = new SpellTimerListWindow()
                    {
                        Title = "SpecialSpellTimer - " + f.Panel,
                        PanelName = f.Panel,
                    };

                    this.SpellTimerPanels.Add(w);

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

        private void RefreshWindowCore()
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            try
            {
                if (Interlocked.CompareExchange(ref settingsIsValid, VALID, INVALID) == INVALID)
                {
                    this.refreshWindowTimer.Interval =
                        TimeSpan.FromMilliseconds(Settings.Default.RefreshInterval);
                }

                if (this.refreshWindowTimer != null &&
                    this.refreshWindowTimer.IsEnabled)
                {
                    Logger.Update();

                    // 有効なスペルとテロップのリストを取得する
                    var spells = TableCompiler.Instance.SpellList;
                    var telops = TableCompiler.Instance.TickerList;

                    if (ActGlobals.oFormActMain == null)
                    {
                        this.HidePanels();
                        OnePointTelopController.HideTelops();

                        Thread.Sleep(1000);
                        return;
                    }

                    // FFXIVでの使用？
                    if (!Settings.Default.UseOtherThanFFXIV)
                    {
                        if ((DateTime.Now - this.LastFFXIVProcessDateTime).TotalSeconds >= 5.0d)
                        {
                            // FF14が起動していない？
                            if (FFXIV.Instance.Process == null)
                            {
                                if (!Settings.Default.OverlayForceVisible)
                                {
                                    this.ClosePanels();
                                    OnePointTelopController.CloseTelops();

                                    return;
                                }
                            }

                            this.LastFFXIVProcessDateTime = DateTime.Now;
                        }
                    }

                    // オーバーレイが非表示？
                    if (!Settings.Default.OverlayVisible)
                    {
                        this.HidePanels();
                        OnePointTelopController.HideTelops();
                        return;
                    }

                    // 非アクティブのとき非表示にする？
                    if (Settings.Default.HideWhenNotActive)
                    {
                        if (!this.IsActive())
                        {
                            this.HidePanels();
                            OnePointTelopController.HideTelops();
                            return;
                        }
                    }

                    // テロップWindowを表示する
                    OnePointTelopController.RefreshTelopWindows(telops);

                    // スペルWindowを表示する
                    this.RefreshSpellPanelWindows(spells);
                }
            }
            finally
            {
#if DEBUG
                sw.Stop();
                Debug.WriteLine(
                    DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]") + " " +
                    "◎RefreshWindow " + sw.Elapsed.TotalMilliseconds.ToString("N4") + "ms");
#endif
            }
        }

        /// <summary>
        /// RefreshWindowTimerOnTick
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void RefreshWindowTimerOnTick(
            object sender,
            EventArgs e)
        {
            try
            {
                if (this.refreshWindowExcuting)
                {
                    return;
                }

                this.refreshWindowExcuting = true;
                this.RefreshWindowCore();
            }
            finally
            {
                this.refreshWindowExcuting = false;
            }
        }

        /// <summary>
        /// リスタートのときスペルのカウントをリセットする
        /// </summary>
        private void ResetCountAtRestart()
        {
            // FFXIV以外での使用ならば何もしない
            if (Settings.Default.UseOtherThanFFXIV)
            {
                return;
            }

            // 無効？
            if (!Settings.Default.ResetOnWipeOut)
            {
                return;
            }

            var combatants = FFXIV.Instance.GetPartyList();

            if (combatants == null ||
                combatants.Count < 1)
            {
                return;
            }

            // 関係者が全員死んでる？
            if (combatants.Count ==
                combatants.Count(x => x.CurrentHP <= 0))
            {
                // リセットするのは15秒に1回にする
                // 暗転中もずっとリセットし続けてしまうので
                if ((DateTime.Now - this.LastWipeOutDateTime).TotalSeconds >= 15.0)
                {
                    Logger.Write("Party was wiped out. Reset spells and tickers.");

                    SpellTimerTable.ResetCount();
                    OnePointTelopTable.Instance.ResetCount();

                    // ACT本体に戦闘終了を通知する
                    if (Settings.Default.WipeoutNotifyToACT)
                    {
                        ActInvoker.Invoke(() =>
                        {
                            ActGlobals.oFormActMain.ActCommands("end");
                        });
                    }

                    this.LastWipeOutDateTime = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// ログを監視する
        /// </summary>
        private void WatchLog()
        {
            var existsLog = false;

            // ACTが起動していない？
            if (ActGlobals.oFormActMain == null)
            {
                Logger.Write("act not started.");
                Thread.Sleep(TimeSpan.FromSeconds(3));
                return;
            }

            // 全滅によるリセットを判定する
            var resetTask = Task.Run(() => this.ResetCountAtRestart());

            if (!this.LogBuffer.IsEmpty)
            {
                // ログを取り出す
                var logsTask = Task.Run(() => this.LogBuffer.GetLogLines());

                // 有効なスペルとテロップのリストを取得する
                var spellTask = Task.Run(() => TableCompiler.Instance.SpellList);
                var telopTask = Task.Run(() => TableCompiler.Instance.TickerList);

                if (logsTask.Result.Count > 0)
                {
                    // テロップとマッチングする
                    var t1 = Task.Run(() => OnePointTelopController.Match(telopTask.Result, logsTask.Result));

                    // スペルリストとマッチングする
                    var t2 = Task.Run(() => this.MatchSpells(spellTask.Result, logsTask.Result));

                    // コマンドとマッチングする
                    var t3 = Task.Run(() => TextCommandController.MatchCommand(logsTask.Result));

                    Task.WaitAll(t1, t2, t3);

                    existsLog = true;
                }
            }

            resetTask.Wait();

            if (!existsLog)
            {
                Thread.Sleep((int)Settings.Default.LogPollSleepInterval);
            }
        }

        #region NativeMethods

        /// <summary>
        /// フォアグラウンドWindowのハンドルを取得する
        /// </summary>
        /// <returns>
        /// フォアグラウンドWindowのハンドル</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// WindowハンドルからそのプロセスIDを取得する
        /// </summary>
        /// <param name="hWnd">
        /// プロセスIDを取得するWindowハンドル</param>
        /// <param name="lpdwProcessId">
        /// プロセスID</param>
        /// <returns>
        /// Windowを作成したスレッドのID</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        #endregion NativeMethods
    }
}
