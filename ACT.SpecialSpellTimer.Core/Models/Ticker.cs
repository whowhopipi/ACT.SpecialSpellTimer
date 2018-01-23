using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Sound;
using FFXIV.Framework.Common;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// ワンポイントテロップ
    /// </summary>
    [Serializable]
    public class OnePointTelop :
        IDisposable,
        ITrigger
    {
        [XmlIgnore]
        public TriggerTypes TriggerType => TriggerTypes.Ticker;

        public void MatchTrigger(string logLine)
            => TickersController.Instance.MatchCore(this, logLine);

        [XmlIgnore]
        private Timer delayedSoundTimer;

        [XmlIgnore]
        public bool ToClose { get; set; } = false;

        public OnePointTelop()
        {
            this.Guid = Guid.Empty;
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

        public bool Enabled { get; set; }

        public FontInfo Font { get; set; } = FontInfo.DefaultFont;

        public string FontColor { get; set; }

        public string FontOutlineColor { get; set; }

        [XmlIgnore]
        public bool ForceHide { get; set; }

        public Guid Guid { get; set; }

        public long ID { get; set; }

        public string JobFilter { get; set; }

        public string Keyword { get; set; }

        [XmlIgnore]
        public string KeywordReplaced { get; set; }

        public string KeywordToHide { get; set; }

        [XmlIgnore]
        public string KeywordToHideReplaced { get; set; }

        public double Left { get; set; } = 0;

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

        public double Top { get; set; } = 0;

        public string ZoneFilter { get; set; }

        public bool NotifyToDiscord { get; set; } = false;

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

        private string tts;
        private Task speakTask;

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

            lock (this)
            {
                if (!tts.EndsWith("。") &&
                    !tts.EndsWith(".") &&
                    !tts.EndsWith("、") &&
                    !tts.EndsWith(","))
                {
                    tts += ".";
                }

                this.tts += tts + Environment.NewLine;

                if (this.speakTask == null)
                {
                    this.speakTask = Task.Run(async () =>
                    {
                        await Task.Delay(50);
                        SoundController.Instance.Play(tts);
                    }).ContinueWith((task) =>
                    {
                        lock (this)
                        {
                            this.speakTask.Dispose();
                            this.speakTask = null;
                            this.tts = string.Empty;
                        }
                    });
                }
            }
        }

        #endregion Sequential TTS

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

            this.Play(this.DelaySound);

            if (!string.IsNullOrWhiteSpace(this.DelayTextToSpeak))
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

        #region Clone

        public OnePointTelop Clone() => (OnePointTelop)this.MemberwiseClone();

        #endregion Clone
    }
}
