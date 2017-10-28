namespace ACT.SpecialSpellTimer.Models
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Timers;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Sound;
    using FFXIV.Framework.Common;

    /// <summary>
    /// スペルタイマ
    /// </summary>
    [Serializable]
    public class SpellTimer :
        IDisposable
    {
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

        public SpellTimer()
        {
            this.Guid = Guid.Empty;
            this.Panel = string.Empty;
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
            this.JobFilter = string.Empty;
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

        [XmlIgnore]
        public bool IsTemporaryDisplay { get; set; } = false;

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

        public bool Enabled { get; set; }

        public bool ExtendBeyondOriginalRecastTime { get; set; }

        public FontInfo Font { get; set; } = FontInfo.DefaultFont;

        public string FontColor { get; set; }

        public string FontOutlineColor { get; set; }

        public Guid Guid { get; set; }
        public bool HideSpellName { get; set; }
        public long ID { get; set; }

        /// <summary>インスタンス化されたスペルか？</summary>
        [XmlIgnore]
        public bool IsInstance { get; set; }

        public bool IsReverse { get; set; }
        public string JobFilter { get; set; }
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
        public string Panel { get; set; }
        public bool ProgressBarVisible { get; set; }
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
        public string SpellIcon { get; set; }
        public int SpellIconSize { get; set; }
        public string SpellTitle { get; set; }

        [XmlIgnore]
        public string SpellTitleReplaced { get; set; }

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
        public string ZoneFilter { get; set; }

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

            GC.SuppressFinalize(true);
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
                string.IsNullOrWhiteSpace(this.TimeupTextToSpeak))
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

            SoundController.Instance.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
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

        private void OverSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.OverDone = true;

            var regex = this.Regex;
            var wave = this.OverSound;
            var speak = this.OverTextToSpeak;

            SoundController.Instance.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
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

        private void TimeupSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.TimeupDone = true;

            var regex = this.Regex;
            var wave = this.TimeupSound;
            var speak = this.TimeupTextToSpeak;

            SoundController.Instance.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
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
            SpellTimerTable.Instance.TryRemoveInstance(this);
        }

        #endregion To Instance Spells

        #region Clone

        public SpellTimer Clone() => (SpellTimer)this.MemberwiseClone();

        #endregion Clone
    }
}
