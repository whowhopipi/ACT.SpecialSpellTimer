using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Data;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Sound;
using FFXIV.Framework.Common;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// ワンポイントテロップ
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "OnePointTelop")]
    public class Ticker :
        BindableBase,
        IDisposable,
        ITrigger,
        ITreeItem
    {
        #region ITrigger

        [XmlIgnore]
        public ItemTypes TriggerType => ItemTypes.Ticker;

        public void MatchTrigger(string logLine)
            => TickersController.Instance.MatchCore(this, logLine);

        #endregion ITrigger

        #region ITreeItem

        private bool isEnabled = false;

        [XmlIgnore]
        public string DisplayText => this.Title;

        [XmlIgnore]
        public bool IsExpanded
        {
            get => false;
            set { }
        }

        [XmlElement(ElementName = "Enabled")]
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetProperty(ref this.isEnabled, value);
        }

        [XmlIgnore]
        public CollectionViewSource Children => null;

        #endregion ITreeItem

        [XmlIgnore]
        private Timer delayedSoundTimer;

        [XmlIgnore]
        public bool ToClose { get; set; } = false;

        public Ticker()
        {
            this.Title = string.Empty;
            this.Keyword = string.Empty;
            this.KeywordToHide = string.Empty;
            this.Message = string.Empty;
            this.MatchSound = string.Empty;
            this.MatchTextToSpeak = string.Empty;
            this.DelaySound = string.Empty;
            this.DelayTextToSpeak = string.Empty;
            this.BackgroundColor = string.Empty;
            this.FontColor = string.Empty;
            this.FontOutlineColor = string.Empty;
            this.MatchedLog = string.Empty;
            this.MessageReplaced = string.Empty;
            this.RegexPattern = string.Empty;
            this.RegexPatternToHide = string.Empty;
            this.JobFilter = string.Empty;
            this.ZoneFilter = string.Empty;
            this.TimersMustRunningForStart = new Guid[0];
            this.TimersMustStoppingForStart = new Guid[0];
            this.Font = new FontInfo();
            this.KeywordReplaced = string.Empty;
            this.KeywordToHideReplaced = string.Empty;

            // ディレイサウンドタイマをセットする
            this.delayedSoundTimer = new Timer
            {
                AutoReset = false,
                Enabled = false
            };

            this.delayedSoundTimer.Elapsed += this.DelayedSoundTimer_Elapsed;
        }

        public Guid Guid { get; set; } = Guid.NewGuid();

        private double left = 0;
        private double top = 0;

        public double Left
        {
            get => this.left;
            set => this.SetProperty(ref this.left, value);
        }

        public double Top
        {
            get => this.top;
            set => this.SetProperty(ref this.top, value);
        }

        [XmlIgnore]
        public bool IsTemporaryDisplay { get; set; } = false;

        public bool AddMessageEnabled { get; set; }

        public int BackgroundAlpha { get; set; }

        public string BackgroundColor { get; set; }

        public double Delay { get; set; } = 0;

        [XmlIgnore]
        public bool Delayed { get; set; }

        public string DelayTextToSpeak { get; set; }

        public double DisplayTime { get; set; } = 0;

        public FontInfo Font { get; set; } = FontInfo.DefaultFont;

        public string FontColor { get; set; }

        public string FontOutlineColor { get; set; }

        [XmlIgnore]
        public bool ForceHide { get; set; }

        public long ID { get; set; }

        public string JobFilter { get; set; }

        public string Keyword { get; set; }

        [XmlIgnore]
        public string KeywordReplaced { get; set; }

        public string KeywordToHide { get; set; }

        [XmlIgnore]
        public string KeywordToHideReplaced { get; set; }

        [XmlIgnore]
        public DateTime MatchDateTime { get; set; }

        [XmlIgnore]
        public string MatchedLog { get; set; }

        public string MatchTextToSpeak { get; set; }

        public string Message { get; set; }

        [XmlIgnore]
        public string MessageReplaced { get; set; }

        public bool ProgressBarEnabled { get; set; }

        [XmlIgnore]
        public Regex Regex { get; set; }

        public bool RegexEnabled { get; set; }

        [XmlIgnore]
        public string RegexPattern { get; set; }

        [XmlIgnore]
        public string RegexPatternToHide { get; set; }

        [XmlIgnore]
        public Regex RegexToHide { get; set; }

        public Guid[] TimersMustRunningForStart { get; set; }

        public Guid[] TimersMustStoppingForStart { get; set; }

        public string Title { get; set; }

        public string ZoneFilter { get; set; }

        public bool NotifyToDiscord { get; set; } = false;

        #region Soundfiles

        [XmlIgnore]
        private string delaySound = string.Empty;

        [XmlIgnore]
        private string matchSound = string.Empty;

        [XmlIgnore]
        public string DelaySound { get => this.delaySound; set => this.delaySound = value; }

        [XmlElement(ElementName = "DelaySound")]
        public string DelaySoundToFile
        {
            get => Path.GetFileName(this.delaySound);
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.delaySound = Path.Combine(SoundController.Instance.WaveDirectory, value);
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

        #endregion Soundfiles

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
            if (this.delayedSoundTimer != null)
            {
                this.delayedSoundTimer.Stop();
                this.delayedSoundTimer.Dispose();
                this.delayedSoundTimer = null;
            }
        }

        /// <summary>
        /// ディレイサウンドのタイマを開始する
        /// </summary>
        public void StartDelayedSoundTimer()
        {
            var timer = this.delayedSoundTimer;

            if (timer == null)
            {
                return;
            }

            if (timer.Enabled)
            {
                timer.Stop();
            }

            if (this.Delay <= 0 ||
                this.MatchDateTime <= DateTime.MinValue)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.DelaySound) &&
                string.IsNullOrWhiteSpace(this.DelayTextToSpeak))
            {
                return;
            }

            // タイマをセットする
            var timeToPlay = this.MatchDateTime.AddSeconds(this.Delay);
            var duration = (timeToPlay - DateTime.Now).TotalMilliseconds;

            if (duration > 0d)
            {
                // タイマスタート
                timer.Interval = duration;
                timer.Start();
            }
        }

        private void DelayedSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Delayed = true;

            var regex = this.Regex;
            var wave = this.DelaySound;
            var speak = this.DelayTextToSpeak;

            SoundController.Instance.Play(this.DelaySound);

            if (!string.IsNullOrWhiteSpace(this.DelayTextToSpeak))
            {
                if (regex == null ||
                    !speak.Contains("$"))
                {
                    SoundController.Instance.Play(speak);
                    return;
                }

                var match = regex.Match(this.MatchedLog);
                speak = match.Result(speak);

                SoundController.Instance.Play(speak);
            }
        }

        #region Clone

        public Ticker Clone() => (Ticker)this.MemberwiseClone();

        #endregion Clone
    }
}
