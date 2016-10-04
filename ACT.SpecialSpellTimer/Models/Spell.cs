namespace ACT.SpecialSpellTimer.Models
{
    using System;
    using System.Text.RegularExpressions;
    using System.Timers;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// スペルタイマ
    /// </summary>
    [Serializable]
    public class Spell
    {
        private Timer overSoundTimer;
        private Timer beforeSoundTimer;
        private Timer timeupSoundTimer;
        private Timer garbageInstanceTimer;

        public Spell()
        {
            this.guid = Guid.Empty;
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
        /// マッチ後ｎ秒後のサウンドタイマを開始する
        /// </summary>
        public void StartOverSoundTimer()
        {
            var timer = this.overSoundTimer;
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
        /// リキャストｎ秒前のサウンドタイマを開始する
        /// </summary>
        public void StartBeforeSoundTimer()
        {
            var timer = this.beforeSoundTimer;
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
        /// リキャスト完了のサウンドタイマを開始する
        /// </summary>
        public void StartTimeupSoundTimer()
        {
            var timer = this.timeupSoundTimer;
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

        /// <summary>
        /// インスタンススペルのガーベージタイマを開始する
        /// </summary>
        public void StartGarbageInstanceTimer()
        {
            var timer = this.garbageInstanceTimer;
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

        /// <summary>
        /// インスタンススペルのガーベージタイマを開始する
        /// </summary>
        public void StopGarbageInstanceTimer()
        {
            var timer = this.garbageInstanceTimer;

            timer.AutoReset = false;
            timer.Stop();
        }

        private void OverSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.OverDone = true;

            var regex = this.Regex;
            var wave = this.OverSound;
            var speak = this.OverTextToSpeak;

            SoundController.Default.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
            {
                var tts = regex != null && !string.IsNullOrWhiteSpace(speak) ?
                    this.Regex.Replace(this.MatchedLog, speak) :
                    speak;

                SoundController.Default.Play(tts);
            }
        }

        private void BeforeSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.BeforeDone = true;

            var regex = this.Regex;
            var wave = this.BeforeSound;
            var speak = this.BeforeTextToSpeak;

            SoundController.Default.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
            {
                var tts = regex != null && !string.IsNullOrWhiteSpace(speak) ?
                    this.Regex.Replace(this.MatchedLog, speak) :
                    speak;

                SoundController.Default.Play(tts);
            }
        }

        private void TimeupSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.TimeupDone = true;

            var regex = this.Regex;
            var wave = this.TimeupSound;
            var speak = this.TimeupTextToSpeak;

            SoundController.Default.Play(wave);

            if (!string.IsNullOrWhiteSpace(speak))
            {
                var tts = regex != null && !string.IsNullOrWhiteSpace(speak) ?
                    this.Regex.Replace(this.MatchedLog, speak) :
                    speak;

                SoundController.Default.Play(tts);
            }
        }

        private void GarbageInstanceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SpellTimerTable.TryRemoveInstance(this);
        }

        public long ID { get; set; }
        public Guid guid { get; set; }
        public long DisplayNo { get; set; }
        public string Panel { get; set; }
        public string SpellTitle { get; set; }
        public string SpellIcon { get; set; }
        public int SpellIconSize { get; set; }
        public string Keyword { get; set; }
        public string KeywordForExtend1 { get; set; }
        public string KeywordForExtend2 { get; set; }
        public long RecastTime { get; set; }
        public long RecastTimeExtending1 { get; set; }
        public long RecastTimeExtending2 { get; set; }
        public bool ExtendBeyondOriginalRecastTime { get; set; }
        public long UpperLimitOfExtension { get; set; }
        public bool RepeatEnabled { get; set; }
        public bool ProgressBarVisible { get; set; }
        public string MatchSound { get; set; }
        public string MatchTextToSpeak { get; set; }
        public string OverSound { get; set; }
        public string OverTextToSpeak { get; set; }
        public long OverTime { get; set; }
        public string BeforeSound { get; set; }
        public string BeforeTextToSpeak { get; set; }
        public long BeforeTime { get; set; }
        public string TimeupSound { get; set; }
        public string TimeupTextToSpeak { get; set; }
        public DateTime MatchDateTime { get; set; }
        public bool TimeupHide { get; set; }
        public bool IsReverse { get; set; }
        public FontInfo Font { get; set; }
        public string FontFamily { get; set; }
        public float FontSize { get; set; }
        public int FontStyle { get; set; }
        public string FontColor { get; set; }
        public string FontOutlineColor { get; set; }
        public string BarColor { get; set; }
        public string BarOutlineColor { get; set; }
        public int BarWidth { get; set; }
        public int BarHeight { get; set; }
        public string BackgroundColor { get; set; }
        public int BackgroundAlpha { get; set; }
        public bool DontHide { get; set; }
        public bool HideSpellName { get; set; }
        public bool OverlapRecastTime { get; set; }
        public bool ReduceIconBrightness { get; set; }
        public bool RegexEnabled { get; set; }
        public string JobFilter { get; set; }
        public string ZoneFilter { get; set; }
        public Guid[] TimersMustRunningForStart { get; set; }
        public Guid[] TimersMustStoppingForStart { get; set; }

        /// <summary>インスタンス化する</summary>
        /// <remarks>表示テキストが異なる条件でマッチングした場合に当該スペルの新しいインスタンスを生成する</remarks>
        public bool ToInstance { get; set; }

        public bool Enabled { get; set; }

        [XmlIgnore]
        public DateTime CompleteScheduledTime { get; set; }
        [XmlIgnore]
        public volatile bool UpdateDone;
        [XmlIgnore]
        public bool OverDone { get; set; }
        [XmlIgnore]
        public bool BeforeDone { get; set; }
        [XmlIgnore]
        public bool TimeupDone { get; set; }
        [XmlIgnore]
        public string SpellTitleReplaced { get; set; }
        [XmlIgnore]
        public string MatchedLog { get; set; }
        [XmlIgnore]
        public Regex Regex { get; set; }
        [XmlIgnore]
        public string RegexPattern { get; set; }
        [XmlIgnore]
        public string KeywordReplaced { get; set; }
        [XmlIgnore]
        public Regex RegexForExtend1 { get; set; }
        [XmlIgnore]
        public string RegexForExtendPattern1 { get; set; }
        [XmlIgnore]
        public string KeywordForExtendReplaced1 { get; set; }
        [XmlIgnore]
        public Regex RegexForExtend2 { get; set; }
        [XmlIgnore]
        public string RegexForExtendPattern2 { get; set; }
        [XmlIgnore]
        public string KeywordForExtendReplaced2 { get; set; }

        /// <summary>インスタンス化されたスペルか？</summary>
        [XmlIgnore]
        public bool IsInstance { get; set; }

        /// <summary>スペルが作用した対象</summary>
        [XmlIgnore]
        public string TargetName { get; set; }
    }
}
