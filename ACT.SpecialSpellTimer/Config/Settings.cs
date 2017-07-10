using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using ACT.SpecialSpellTimer.Utility;
using ACT.SpecialSpellTimer.Views;

namespace ACT.SpecialSpellTimer.Config
{
    [Serializable]
    public class Settings
    {
        #region Singleton

        private static Settings instance;

        public static Settings Default
        {
            get
            {
                if (WPFHelper.IsDesignMode)
                {
                    return DefaultSettings;
                }

                if (instance == null)
                {
                    instance = new Settings();
                }

                return instance;
            }
        }

        #endregion Singleton

        public readonly string FileName = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.config");

        #region Data - Colors

        [XmlIgnore]
        public Color BackgroundColor { get; set; }

        [XmlElement(ElementName = "BackgroundColor")]
        public string BackgroundColorText
        {
            get => this.BackgroundColor.ToHTML();
            set => this.BackgroundColor = ColorTranslator.FromHtml(value);
        }

        [XmlIgnore]
        public Color FontColor { get; set; }

        [XmlElement(ElementName = "FontColor")]
        public string FontColorText
        {
            get => this.FontColor.ToHTML();
            set => this.FontColor = ColorTranslator.FromHtml(value);
        }

        [XmlIgnore]
        public Color FontOutlineColor { get; set; }

        [XmlElement(ElementName = "FontOutlineColor")]
        public string FontOutlineColorText
        {
            get => this.FontOutlineColor.ToHTML();
            set => this.FontOutlineColor = ColorTranslator.FromHtml(value);
        }

        [XmlIgnore]
        public Color ProgressBarColor { get; set; }

        [XmlElement(ElementName = "ProgressBarColor")]
        public string ProgressBarColorText
        {
            get => this.ProgressBarColor.ToHTML();
            set => this.ProgressBarColor = ColorTranslator.FromHtml(value);
        }

        [XmlIgnore]
        public Color ProgressBarOutlineColor { get; set; }

        [XmlElement(ElementName = "ProgressBarOutlineColor")]
        public string ProgressBarOutlineColorText
        {
            get => this.ProgressBarOutlineColor.ToHTML();
            set => this.ProgressBarOutlineColor = ColorTranslator.FromHtml(value);
        }

        [XmlIgnore]
        public Color WarningFontColor { get; set; }

        [XmlElement(ElementName = "WarningFontColor")]
        public string WarningFontColorText
        {
            get => this.WarningFontColor.ToHTML();
            set => this.WarningFontColor = ColorTranslator.FromHtml(value);
        }

        [XmlIgnore]
        public Color WarningFontOutlineColor { get; set; }

        [XmlElement(ElementName = "WarningFontOutlineColor")]
        public string WarningFontOutlineColorText
        {
            get => this.WarningFontOutlineColor.ToHTML();
            set => this.WarningFontOutlineColor = ColorTranslator.FromHtml(value);
        }

        #endregion Data - Colors

        #region Data - Sizes

        [XmlIgnore]
        public Size ProgressBarSize { get; set; }

        [XmlElement(ElementName = "ProgressBarSize")]
        public SerializableSize ProgressBarSizeText
        {
            get => new SerializableSize() { Height = this.ProgressBarSize.Height, Width = this.ProgressBarSize.Width };
            set => this.ProgressBarSize = new Size(value.Width, value.Height);
        }

        #endregion Data - Sizes

        #region Data - Fonts

        [XmlIgnore]
        public Font Font { get; set; }

        [XmlElement(ElementName = "Font")]
        public string FontText

        {
            get => FontSerializationHelper.ToString(this.Font);
            set => this.Font = FontSerializationHelper.FromString(value);
        }

        #endregion Data - Fonts

        #region Data

        public bool AutoSortEnabled { get; set; }
        public bool AutoSortReverse { get; set; }
        public bool ClickThroughEnabled { get; set; }
        public long CombatLogBufferSize { get; set; }
        public bool CombatLogEnabled { get; set; }
        public bool DetectPacketDump { get; set; }
        public string DQXPlayerName { get; set; }
        public bool DQXUtilityEnabled { get; set; }
        public bool EnabledNotifyNormalSpellTimer { get; set; }
        public bool EnabledPartyMemberPlaceholder { get; set; }
        public bool EnabledSpellTimerNoDecimal { get; set; }
        public bool HideWhenNotActive { get; set; }
        public string Language { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public long LogPollSleepInterval { get; set; }
        public int MaxFPS { get; set; }
        public string NotifyNormalSpellTimerPrefix { get; set; }
        public int Opacity { get; set; }
        public bool OverlayForceVisible { get; set; }
        public bool OverlayVisible { get; set; }
        public string OverText { get; set; }
        public double PlayerInfoRefreshInterval { get; set; }
        public string ReadyText { get; set; }
        public int ReduceIconBrightness { get; set; }
        public long RefreshInterval { get; set; }
        public bool RemoveTooltipSymbols { get; set; }
        public bool ResetOnWipeOut { get; set; }
        public bool SaveLogEnabled { get; set; }
        public string SaveLogFile { get; set; }
        public bool SimpleRegex { get; set; }
        public bool TelopAlwaysVisible { get; set; }
        public double TextBlurRate { get; set; }
        public double TextOutlineThicknessRate { get; set; }
        public double TimeOfHideSpell { get; set; }
        public double UpdateCheckInterval { get; set; }
        public bool UseOtherThanFFXIV { get; set; }

        #endregion Data

        #region Load Save

        private readonly object locker = new object();

        /// <summary>
        /// シリアライザ
        /// </summary>
        private readonly XmlSerializer Serializer = new XmlSerializer(typeof(Settings));

        /// <summary>
        /// XMLライターSettings
        /// </summary>
        private readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings()
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
        };

        public void Load()
        {
            lock (this.locker)
            {
                this.Reset();

                if (!File.Exists(this.FileName))
                {
                    this.Save();
                    return;
                }

                var fi = new FileInfo(this.FileName);
                if (fi.Length <= 0)
                {
                    this.Save();
                    return;
                }

                using (var xr = XmlReader.Create(this.FileName))
                {
                    var data = this.Serializer.Deserialize(xr) as Settings;
                    if (data != null)
                    {
                        instance = data;
                    }
                }
            }
        }

        public void Save()
        {
            lock (this.locker)
            {
                using (var xw = XmlWriter.Create(
                    this.FileName,
                    this.XmlWriterSettings))
                {
                    this.Serializer.Serialize(xw, this);
                }
            }
        }

        #endregion Load Save

        #region Default Values & Reset

        public static readonly Settings DefaultSettings = new Settings()
        {
            Language = "EN",
            LastUpdateDateTime = DateTime.Parse("2000-1-1"),
            UpdateCheckInterval = 12.0d,
            ProgressBarSize = new Size()
            {
                Width = 190,
                Height = 8,
            },
            ProgressBarColor = Color.White,
            ProgressBarOutlineColor = Color.FromArgb(22, 120, 157),
            FontColor = Color.AliceBlue,
            FontOutlineColor = Color.FromArgb(22, 120, 157),
            WarningFontColor = Color.OrangeRed,
            WarningFontOutlineColor = Color.DarkRed,
            BackgroundColor = Color.Transparent,
            NotifyNormalSpellTimerPrefix = "spespe_",
            ReadyText = "Ready",
            OverText = "Over",
            SaveLogFile = string.Empty,
            TimeOfHideSpell = 0.0d,
            PlayerInfoRefreshInterval = 3.0d,
            LogPollSleepInterval = 10,
            RefreshInterval = 20,
            CombatLogBufferSize = 30000,
            ReduceIconBrightness = 55,
            Opacity = 10,
            MaxFPS = 60,
            Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold),
            OverlayVisible = true,
            AutoSortEnabled = true,
            ClickThroughEnabled = false,
            AutoSortReverse = false,
            TelopAlwaysVisible = false,
            EnabledPartyMemberPlaceholder = true,
            CombatLogEnabled = false,
            OverlayForceVisible = false,
            EnabledSpellTimerNoDecimal = false,
            EnabledNotifyNormalSpellTimer = false,
            SaveLogEnabled = false,
            HideWhenNotActive = false,
            UseOtherThanFFXIV = false,
            DQXUtilityEnabled = false,
            DQXPlayerName = string.Empty,
            ResetOnWipeOut = true,
            SimpleRegex = false,
            DetectPacketDump = false,
            TextBlurRate = 1.2d,
            TextOutlineThicknessRate = 1.0d,
        };

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>
        /// このオブジェクトのクローン</returns>
        public Settings Clone()
        {
            return (Settings)this.MemberwiseClone();
        }

        public void Reset()
        {
            lock (this.locker)
            {
                instance = DefaultSettings.Clone();
            }
        }

        #endregion Default Values & Reset
    }
}