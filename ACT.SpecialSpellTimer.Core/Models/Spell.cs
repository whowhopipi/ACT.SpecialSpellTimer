using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Media;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.Image;
using ACT.SpecialSpellTimer.Sound;
using FFXIV.Framework.Bridge;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// スペルタイマ
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "SpellTimer")]
    public class Spell :
        TreeItemBase,
        IDisposable,
        ITrigger
    {
        [XmlIgnore]
        public override ItemTypes ItemType => ItemTypes.Spell;

        #region ITrigger

        public void MatchTrigger(string logLine)
            => SpellsController.Instance.MatchCore(this, logLine);

        #endregion ITrigger

        #region ITreeItem

        private bool enabled = false;

        [XmlIgnore]
        public override string DisplayText => this.SpellTitle;

        [XmlIgnore]
        public override int SortPriority { get; set; }

        [XmlIgnore]
        public override bool IsExpanded
        {
            get => false;
            set { }
        }

        public override bool Enabled
        {
            get => this.enabled;
            set => this.SetProperty(ref this.enabled, value);
        }

        [XmlIgnore]
        public override ICollectionView Children => null;

        #endregion ITreeItem

        [XmlIgnore]
        public volatile bool UpdateDone;

        [XmlIgnore]
        private Timer beforeSoundTimer = new Timer();

        [XmlIgnore]
        private Timer garbageInstanceTimer = new Timer();

        [XmlIgnore]
        private Timer overSoundTimer = new Timer();

        [XmlIgnore]
        private Timer timeupSoundTimer = new Timer();

        public Spell()
        {
            this.SpellTitle = string.Empty;
            this.SpellIcon = string.Empty;
            this.Keyword = string.Empty;
            this.KeywordForExtend1 = string.Empty;
            this.KeywordForExtend2 = string.Empty;
            this.MatchSound = string.Empty;
            this.MatchTextToSpeak = string.Empty;
            this.OverSound = string.Empty;
            this.OverTextToSpeak = string.Empty;
            this.TimeupSound = string.Empty;
            this.TimeupTextToSpeak = string.Empty;
            this.FontColor = string.Empty;
            this.FontOutlineColor = string.Empty;
            this.WarningFontColor = string.Empty;
            this.WarningFontOutlineColor = string.Empty;
            this.BarColor = string.Empty;
            this.BarOutlineColor = string.Empty;
            this.BackgroundColor = string.Empty;
            this.SpellTitleReplaced = string.Empty;
            this.MatchedLog = string.Empty;
            this.RegexPattern = string.Empty;
            this.JobFilter = string.Empty;
            this.ZoneFilter = string.Empty;
            this.TimersMustRunningForStart = new Guid[0];
            this.TimersMustStoppingForStart = new Guid[0];
            this.Font = new FontInfo();
            this.KeywordReplaced = string.Empty;
            this.KeywordForExtendReplaced1 = string.Empty;
            this.KeywordForExtendReplaced2 = string.Empty;

            // マッチ後ｎ秒後のサウンドタイマをセットする
            this.overSoundTimer = new Timer
            {
                AutoReset = false,
                Enabled = false
            };

            this.overSoundTimer.Elapsed += this.OverSoundTimer_Elapsed;

            // リキャストｎ秒前のサウンドタイマをセットする
            this.beforeSoundTimer = new Timer
            {
                AutoReset = false,
                Enabled = false
            };

            this.beforeSoundTimer.Elapsed += this.BeforeSoundTimer_Elapsed;

            // リキャスト完了のサウンドタイマをセットする
            this.timeupSoundTimer = new Timer
            {
                AutoReset = false,
                Enabled = false
            };

            this.timeupSoundTimer.Elapsed += this.TimeupSoundTimer_Elapsed;

            // インスタンススペルのガーベージタイマをセットする
            this.garbageInstanceTimer = new Timer
            {
                AutoReset = true,
                Enabled = false,

                // 10秒毎
                Interval = 10 * 1000,
            };

            this.garbageInstanceTimer.Elapsed += this.GarbageInstanceTimer_Elapsed;
        }

        public Guid Guid { get; set; } = Guid.NewGuid();

        private Guid panelID = Guid.Empty;

        public Guid PanelID
        {
            get => this.panelID;
            set => this.SetProperty(ref this.panelID, value);
        }

        [XmlIgnore]
        public SpellPanel Panel => SpellPanelTable.Instance.Table.FirstOrDefault(x => x.ID == this.PanelID);

        private string panelName = string.Empty;

        [XmlElement(ElementName = "Panel")]
        public string PanelName
        {
            get
            {
                if (this.PanelID == Guid.Empty)
                {
                    return this.panelName;
                }

                return this.Panel?.PanelName;
            }

            set => this.panelName = value;
        }

        private bool isDesignMode = false;

        [XmlIgnore]
        public bool IsDesignMode
        {
            get => this.isDesignMode;
            set => this.SetProperty(ref this.isDesignMode, value);
        }

        private string jobFilter;

        public string JobFilter
        {
            get => this.jobFilter;
            set => this.SetProperty(ref this.jobFilter, value);
        }

        private string zoneFilter;

        public string ZoneFilter
        {
            get => this.zoneFilter;
            set => this.SetProperty(ref this.zoneFilter, value);
        }

        public string SpellTitle { get; set; }

        [XmlIgnore]
        public string SpellTitleReplaced { get; set; }

        /// <summary>
        /// ※注意が必要な項目※
        /// 昔の名残で項目名と異なる動作になっている。
        /// プログレスバーの表示／非表示ではなく、スペル全体の表示／非表示を司る重要な項目として動作している
        /// </summary>
        public bool ProgressBarVisible { get; set; } = true;

        public int BackgroundAlpha { get; set; }

        public string BackgroundColor { get; set; }

        public string BarColor { get; set; }

        public int BarHeight { get; set; }

        public string BarOutlineColor { get; set; }

        public int BarWidth { get; set; }

        [XmlIgnore]
        public bool BeforeDone { get; set; }

        public string BeforeTextToSpeak { get; set; }

        public double BeforeTime { get; set; } = 0;

        public bool ChangeFontColorsWhenWarning { get; set; }

        [XmlIgnore]
        public DateTime CompleteScheduledTime { get; set; }

        public long DisplayNo { get; set; }

        public bool DontHide { get; set; }

        public bool ExtendBeyondOriginalRecastTime { get; set; }

        public FontInfo Font { get; set; } = FontInfo.DefaultFont;

        public string FontColor { get; set; }

        public string FontOutlineColor { get; set; }

        public bool HideSpellName { get; set; }
        public long ID { get; set; }

        /// <summary>インスタンス化されたスペルか？</summary>
        [XmlIgnore]
        public bool IsInstance { get; set; }

        public bool IsReverse { get; set; }
        public string Keyword { get; set; }
        public string KeywordForExtend1 { get; set; }
        public string KeywordForExtend2 { get; set; }

        [XmlIgnore]
        public string KeywordForExtendReplaced1 { get; set; }

        [XmlIgnore]
        public string KeywordForExtendReplaced2 { get; set; }

        [XmlIgnore]
        public string KeywordReplaced { get; set; }

        public DateTime MatchDateTime { get; set; }

        [XmlIgnore]
        public string MatchedLog { get; set; }

        public string MatchTextToSpeak { get; set; }

        [XmlIgnore]
        public bool OverDone { get; set; }

        public bool OverlapRecastTime { get; set; }
        public string OverTextToSpeak { get; set; }
        public double OverTime { get; set; } = 0;
        public double RecastTime { get; set; } = 0;
        public double RecastTimeExtending1 { get; set; } = 0;
        public double RecastTimeExtending2 { get; set; } = 0;
        public bool ReduceIconBrightness { get; set; }

        [XmlIgnore]
        public Regex Regex { get; set; }

        public bool RegexEnabled { get; set; }

        [XmlIgnore]
        public Regex RegexForExtend1 { get; set; }

        [XmlIgnore]
        public Regex RegexForExtend2 { get; set; }

        [XmlIgnore]
        public string RegexForExtendPattern1 { get; set; }

        [XmlIgnore]
        public string RegexForExtendPattern2 { get; set; }

        [XmlIgnore]
        public string RegexPattern { get; set; }

        public bool RepeatEnabled { get; set; }

        private string spellIcon;

        public string SpellIcon
        {
            get => this.spellIcon;
            set
            {
                if (this.SetProperty(ref this.spellIcon, value))
                {
                    this.RaisePropertyChanged(nameof(this.SpellIconFullPath));
                }
            }
        }

        [XmlIgnore]
        public string SpellIconFullPath =>
            string.IsNullOrEmpty(this.SpellIcon) ?
            string.Empty :
            IconController.Instance.GetIconFile(this.SpellIcon)?.FullPath;

        public int SpellIconSize { get; set; } = 24;

        /// <summary>スペルが作用した対象</summary>
        [XmlIgnore]
        public string TargetName { get; set; }

        public Guid[] TimersMustRunningForStart { get; set; }
        public Guid[] TimersMustStoppingForStart { get; set; }

        [XmlIgnore]
        public bool TimeupDone { get; set; }

        public bool TimeupHide { get; set; }
        public string TimeupTextToSpeak { get; set; }

        /// <summary>インスタンス化する</summary>
        /// <remarks>表示テキストが異なる条件でマッチングした場合に当該スペルの新しいインスタンスを生成する</remarks>
        public bool ToInstance { get; set; }

        public double UpperLimitOfExtension { get; set; } = 0;
        public string WarningFontColor { get; set; }

        public string WarningFontOutlineColor { get; set; }
        public double WarningTime { get; set; } = 0;
        public double BlinkTime { get; set; } = 0;
        public bool BlinkIcon { get; set; } = false;
        public bool BlinkBar { get; set; } = false;
        public bool NotifyToDiscord { get; set; } = false;
        public bool NotifyToDiscordAtComplete { get; set; } = false;

        #region Sequential TTS

        /// <summary>
        /// 同時再生を抑制してシーケンシャルにTTSを再生する
        /// </summary>
        public bool IsSequentialTTS { get; set; } = false;

        public delegate void DoPlay(string source);

        /// <summary>
        /// 再生処理のデリゲート
        /// </summary>
        [XmlIgnore]
        public DoPlay PlayDelegate { get; set; } = null;

        private volatile string tts;
        private Timer speakTimer;

        /// <summary>
        /// TTSを発声する
        /// </summary>
        /// <param name="tts">
        /// TTS</param>
        public void Play(
            string tts)
        {
            if (this.PlayDelegate != null)
            {
                this.PlayDelegate(tts);
                return;
            }

            if (tts.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                tts.EndsWith(".wave", StringComparison.OrdinalIgnoreCase))
            {
                SoundController.Instance.Play(tts);
                return;
            }

            if (!this.IsSequentialTTS)
            {
                SoundController.Instance.Play(tts);
                return;
            }

            this.speakTimer?.Stop();

            lock (this)
            {
                if (this.speakTimer == null)
                {
                    this.speakTimer = new Timer(50)
                    {
                        AutoReset = false
                    };

                    this.speakTimer.Elapsed += (x, y) =>
                    {
                        SoundController.Instance.Play(this.tts);
                        this.tts = string.Empty;
                    };
                }

                if (!tts.EndsWith("。") &&
                    !tts.EndsWith(".") &&
                    !tts.EndsWith("、") &&
                    !tts.EndsWith(","))
                {
                    tts += ".";
                }

                if (string.IsNullOrEmpty(this.tts))
                {
                    this.tts = tts;
                }
                else
                {
                    this.tts += Environment.NewLine + tts;
                }
            }

            this.speakTimer?.Start();
        }

        #endregion Sequential TTS

        #region Sound files

        [XmlIgnore]
        private string beforeSound = string.Empty;

        [XmlIgnore]
        private string matchSound = string.Empty;

        [XmlIgnore]
        private string overSound = string.Empty;

        [XmlIgnore]
        private string timeupSound = string.Empty;

        [XmlIgnore]
        public string BeforeSound { get => this.beforeSound; set => this.beforeSound = value; }

        [XmlElement(ElementName = "BeforeSound")]
        public string BeforeSoundToFile
        {
            get => Path.GetFileName(this.beforeSound);
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.beforeSound = Path.Combine(SoundController.Instance.WaveDirectory, value);
                }
            }
        }

        [XmlIgnore]
        public string MatchSound { get => this.matchSound; set => this.matchSound = value; }

        [XmlElement(ElementName = "MatchSound")]
        public string MatchSoundToFile
        {
            get => Path.GetFileName(this.matchSound);
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.matchSound = Path.Combine(SoundController.Instance.WaveDirectory, value);
                }
            }
        }

        [XmlIgnore]
        public string OverSound { get => this.overSound; set => this.overSound = value; }

        [XmlElement(ElementName = "OverSound")]
        public string OverSoundToFile
        {
            get => Path.GetFileName(this.overSound);
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.overSound = Path.Combine(SoundController.Instance.WaveDirectory, value);
                }
            }
        }

        [XmlIgnore]
        public string TimeupSound { get => this.timeupSound; set => this.timeupSound = value; }

        [XmlElement(ElementName = "TimeupSound")]
        public string TimeupSoundToFile
        {
            get => Path.GetFileName(this.timeupSound);
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.timeupSound = Path.Combine(SoundController.Instance.WaveDirectory, value);
                }
            }
        }

        #endregion Sound files

        #region Performance Monitor

        [XmlIgnore]
        public double MatchingDuration { get; set; } = 0.0;

        [XmlIgnore]
        private DateTime matchingStartDateTime;

        public void StartMatching()
        {
            this.matchingStartDateTime = DateTime.Now;
        }

        public void EndMatching()
        {
            var ticks = (DateTime.Now - this.matchingStartDateTime).Ticks;
            if (ticks == 0)
            {
                return;
            }

            var cost = ticks / 1000;

            if (this.MatchingDuration != 0)
            {
                this.MatchingDuration += cost;
                this.MatchingDuration /= 2;
            }
            else
            {
                this.MatchingDuration += cost;
            }
        }

        #endregion Performance Monitor

        public void Dispose()
        {
            if (this.overSoundTimer != null)
            {
                this.overSoundTimer.Stop();
                this.overSoundTimer.Dispose();
                this.overSoundTimer = null;
            }

            if (this.beforeSoundTimer != null)
            {
                this.beforeSoundTimer.Stop();
                this.beforeSoundTimer.Dispose();
                this.beforeSoundTimer = null;
            }

            if (this.timeupSoundTimer != null)
            {
                this.timeupSoundTimer.Stop();
                this.timeupSoundTimer.Dispose();
                this.timeupSoundTimer = null;
            }

            if (this.garbageInstanceTimer != null)
            {
                this.garbageInstanceTimer.Stop();
                this.garbageInstanceTimer.Dispose();
                this.garbageInstanceTimer = null;
            }
        }

        /// <summary>
        /// リキャストｎ秒前のサウンドタイマを開始する
        /// </summary>
        public void StartBeforeSoundTimer()
        {
            var timer = this.beforeSoundTimer;

            if (timer == null)
            {
                return;
            }

            if (timer.Enabled)
            {
                timer.Stop();
            }

            if (this.BeforeTime <= 0 ||
                this.MatchDateTime <= DateTime.MinValue)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.BeforeSound) &&
                string.IsNullOrWhiteSpace(this.BeforeTextToSpeak))
            {
                return;
            }

            if (this.CompleteScheduledTime <= DateTime.MinValue)
            {
                return;
            }

            // タイマをセットする
            var timeToPlay = this.CompleteScheduledTime.AddSeconds(this.BeforeTime * -1);
            var duration = (timeToPlay - DateTime.Now).TotalMilliseconds;

            if (duration > 0d)
            {
                // タイマスタート
                timer.Interval = duration;
                timer.Start();
            }
        }

        /// <summary>
        /// マッチ後ｎ秒後のサウンドタイマを開始する
        /// </summary>
        public void StartOverSoundTimer()
        {
            var timer = this.overSoundTimer;

            if (timer == null)
            {
                return;
            }

            if (timer.Enabled)
            {
                timer.Stop();
            }

            if (this.OverTime <= 0 ||
                this.MatchDateTime <= DateTime.MinValue)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.OverSound) &&
                string.IsNullOrWhiteSpace(this.OverTextToSpeak))
            {
                return;
            }

            // タイマをセットする
            var timeToPlay = this.MatchDateTime.AddSeconds(this.OverTime);
            var duration = (timeToPlay - DateTime.Now).TotalMilliseconds;

            if (duration > 0d)
            {
                // タイマスタート
                timer.Interval = duration;
                timer.Start();
            }
        }

        /// <summary>
        /// 遅延処理のタイマを開始する
        /// </summary>
        public void StartTimer()
        {
            this.StartOverSoundTimer();
            this.StartBeforeSoundTimer();
            this.StartTimeupSoundTimer();
            this.StartGarbageInstanceTimer();
        }

        /// <summary>
        /// リキャスト完了のサウンドタイマを開始する
        /// </summary>
        public void StartTimeupSoundTimer()
        {
            var timer = this.timeupSoundTimer;

            if (timer == null)
            {
                return;
            }

            if (timer.Enabled)
            {
                timer.Stop();
            }

            if (this.CompleteScheduledTime <= DateTime.MinValue ||
                this.MatchDateTime <= DateTime.MinValue)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.TimeupSound) &&
                string.IsNullOrWhiteSpace(this.TimeupTextToSpeak) &&
                !this.NotifyToDiscord &&
                !this.NotifyToDiscordAtComplete)
            {
                return;
            }

            // タイマをセットする
            var timeToPlay = this.CompleteScheduledTime;
            var duration = (timeToPlay - DateTime.Now).TotalMilliseconds;

            if (duration > 0d)
            {
                // タイマスタート
                timer.Interval = duration;
                timer.Start();
            }
        }

        private void BeforeSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.BeforeDone = true;

            var regex = this.Regex;
            var wave = this.BeforeSound;
            var speak = this.BeforeTextToSpeak;

            this.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
            {
                if (regex == null ||
                    !speak.Contains("$"))
                {
                    this.Play(speak);
                    return;
                }

                var match = regex.Match(this.MatchedLog);
                speak = match.Result(speak);

                this.Play(speak);
            }
        }

        private void OverSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.OverDone = true;

            var regex = this.Regex;
            var wave = this.OverSound;
            var speak = this.OverTextToSpeak;

            this.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
            {
                if (regex == null ||
                    !speak.Contains("$"))
                {
                    this.Play(speak);
                    return;
                }

                var match = regex.Match(this.MatchedLog);
                speak = match.Result(speak);

                this.Play(speak);
            }
        }

        private void TimeupSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.TimeupDone = true;

            // DISCORDに通知する？
            if (this.NotifyToDiscord)
            {
                var compText = !this.IsReverse ?
                    Settings.Default.ReadyText :
                    Settings.Default.OverText;
                var title = string.IsNullOrEmpty(this.SpellTitleReplaced) ?
                    this.SpellTitle :
                    this.SpellTitleReplaced;
                DiscordBridge.Instance.SendMessageDelegate?.Invoke(
                    $"{title} {compText}");
            }

            var regex = this.Regex;
            var wave = this.TimeupSound;
            var speak = this.TimeupTextToSpeak;

            this.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
            {
                if (regex == null ||
                    !speak.Contains("$"))
                {
                    this.Play(speak);
                    return;
                }

                var match = regex.Match(this.MatchedLog);
                speak = match.Result(speak);

                this.Play(speak);
            }
        }

        #region To Instance Spells

        /// <summary>
        /// インスタンススペルのガーベージタイマを開始する
        /// </summary>
        public void StartGarbageInstanceTimer()
        {
            lock (this)
            {
                var timer = this.garbageInstanceTimer;

                if (timer == null)
                {
                    return;
                }

                if (timer.Enabled)
                {
                    timer.Stop();
                }

                if (!this.IsInstance)
                {
                    return;
                }

                timer.AutoReset = true;
                timer.Start();
            }
        }

        /// <summary>
        /// インスタンススペルのガーベージタイマを開始する
        /// </summary>
        public void StopGarbageInstanceTimer()
        {
            var timer = this.garbageInstanceTimer;

            if (timer != null)
            {
                timer.AutoReset = false;
                timer.Stop();
            }
        }

        private void GarbageInstanceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SpellTable.Instance.TryRemoveInstance(this);
        }

        #endregion To Instance Spells

        #region Clone

        public Spell Clone() => (Spell)this.MemberwiseClone();

        #endregion Clone

        #region NewSpell

        public static Spell CreateNew()
        {
            var n = new Spell();

            lock (SpellTable.Instance.Table)
            {
                n.ID = SpellTable.Instance.Table.Any() ?
                    SpellTable.Instance.Table.Max(x => x.ID) + 1 :
                    1;

                n.DisplayNo = SpellTable.Instance.Table.Any() ?
                    SpellTable.Instance.Table.Max(x => x.DisplayNo) + 1 :
                    50;
            }

            n.PanelID = SpellPanel.GeneralPanel.ID;

            n.SpellTitle = "New Spell";
            n.SpellIconSize = 24;
            n.FontColor = Colors.White.ToLegacy().ToHTML();
            n.FontOutlineColor = Colors.MidnightBlue.ToLegacy().ToHTML();
            n.WarningFontColor = Colors.White.ToLegacy().ToHTML();
            n.WarningFontOutlineColor = Colors.OrangeRed.ToLegacy().ToHTML();
            n.BarColor = Colors.White.ToLegacy().ToHTML();
            n.BarOutlineColor = Colors.MidnightBlue.ToLegacy().ToHTML();
            n.BackgroundColor = Colors.Transparent.ToLegacy().ToHTML();
            n.BarWidth = 190;
            n.BarHeight = 8;

            n.Enabled = true;

            return n;
        }

        /// <summary>
        /// 同様のインスタンスを作る（新規スペルの登録用）
        /// </summary>
        /// <returns>
        /// 同様のインスタンス</returns>
        public Spell CreateSimilarNew()
        {
            var n = Spell.CreateNew();

            n.PanelID = this.PanelID;
            n.SpellTitle = this.SpellTitle + " New";
            n.SpellIcon = this.SpellIcon;
            n.SpellIconSize = this.SpellIconSize;
            n.Keyword = this.Keyword;
            n.RegexEnabled = this.RegexEnabled;
            n.RecastTime = this.RecastTime;
            n.KeywordForExtend1 = this.KeywordForExtend1;
            n.RecastTimeExtending1 = this.RecastTimeExtending1;
            n.KeywordForExtend2 = this.KeywordForExtend2;
            n.RecastTimeExtending2 = this.RecastTimeExtending2;
            n.ExtendBeyondOriginalRecastTime = this.ExtendBeyondOriginalRecastTime;
            n.UpperLimitOfExtension = this.UpperLimitOfExtension;
            n.RepeatEnabled = this.RepeatEnabled;
            n.ProgressBarVisible = this.ProgressBarVisible;
            n.IsReverse = this.IsReverse;
            n.FontColor = this.FontColor;
            n.FontOutlineColor = this.FontOutlineColor;
            n.WarningFontColor = this.WarningFontColor;
            n.WarningFontOutlineColor = this.WarningFontOutlineColor;
            n.BarColor = this.BarColor;
            n.BarOutlineColor = this.BarOutlineColor;
            n.DontHide = this.DontHide;
            n.HideSpellName = this.HideSpellName;
            n.WarningTime = this.WarningTime;
            n.BlinkTime = this.BlinkTime;
            n.BlinkIcon = this.BlinkIcon;
            n.BlinkBar = this.BlinkBar;
            n.ChangeFontColorsWhenWarning = this.ChangeFontColorsWhenWarning;
            n.OverlapRecastTime = this.OverlapRecastTime;
            n.ReduceIconBrightness = this.ReduceIconBrightness;
            n.Font = this.Font;
            n.BarWidth = this.BarWidth;
            n.BarHeight = this.BarHeight;
            n.BackgroundColor = this.BackgroundColor;
            n.BackgroundAlpha = this.BackgroundAlpha;
            n.JobFilter = this.JobFilter;
            n.ZoneFilter = this.ZoneFilter;
            n.TimersMustRunningForStart = this.TimersMustRunningForStart;
            n.TimersMustStoppingForStart = this.TimersMustStoppingForStart;
            n.ToInstance = this.ToInstance;
            n.NotifyToDiscord = this.NotifyToDiscord;
            n.NotifyToDiscordAtComplete = this.NotifyToDiscordAtComplete;

            return n;
        }

        #endregion NewSpell
    }
}
