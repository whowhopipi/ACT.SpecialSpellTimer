using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Views;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.Globalization;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config
{
    [Serializable]
    public class Settings :
        BindableBase
    {
        #region Singleton

        private static Settings instance;
        private static object singletonLocker = new object();

        public static Settings Default
        {
            get
            {
#if DEBUG
                if (WPFHelper.IsDesignMode)
                {
                    return new Settings();
                }
#endif
                lock (singletonLocker)
                {
                    if (instance == null)
                    {
                        instance = new Settings();
                    }
                }

                return instance;
            }
        }

        #endregion Singleton

        public Settings()
        {
            this.Reset();
        }

        public readonly string FileName = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.config");

        #region Constants

        [XmlIgnore]
        public const double UpdateCheckInterval = 12;

        #endregion Constants

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

        #region Data - ProgressBar Background

        [XmlIgnore] private readonly System.Windows.Media.Color DefaultBackgroundColor = System.Windows.Media.Colors.Black;
        [XmlIgnore] private bool barBackgroundFixed;
        [XmlIgnore] private double barBackgroundBrightness;
        [XmlIgnore] private System.Windows.Media.Color barDefaultBackgroundColor;

        /// <summary>
        /// プログレスバーの背景が固定色か？
        /// </summary>
        public bool BarBackgroundFixed
        {
            get => this.barBackgroundFixed;
            set => this.barBackgroundFixed = value;
        }

        /// <summary>
        /// プログレスバーの背景の輝度
        /// </summary>
        public double BarBackgroundBrightness
        {
            get => this.barBackgroundBrightness;
            set => this.barBackgroundBrightness = value;
        }

        /// <summary>
        /// プログレスバーの標準の背景色
        /// </summary>
        [XmlIgnore]
        public System.Windows.Media.Color BarDefaultBackgroundColor
        {
            get => this.barDefaultBackgroundColor;
            set => this.barDefaultBackgroundColor = value;
        }

        /// <summary>
        /// プログレスバーの標準の背景色
        /// </summary>
        [XmlElement(ElementName = "BarDefaultBackgroundColor")]
        public string BarDefaultBackgroundColorText
        {
            get => this.BarDefaultBackgroundColor.ToString();
            set => this.BarDefaultBackgroundColor = this.DefaultBackgroundColor.FromString(value);
        }

        #endregion Data - ProgressBar Background

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

        public string Language { get; set; }

        [XmlIgnore]
        public Locales UILocale
        {
            get
            {
                switch (this.Language)
                {
                    case "EN":
                        return Locales.EN;

                    case "JP":
                        return Locales.JA;

                    case "KR":
                        return Locales.KO;

                    default:
                        return Locales.EN;
                }
            }
        }

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
        public long LogPollSleepInterval { get; set; }
        public string NotifyNormalSpellTimerPrefix { get; set; }
        public int Opacity { get; set; }
        public bool OverlayForceVisible { get; set; }
        public bool OverlayVisible { get; set; }
        public string OverText { get; set; }
        public NameStyles PCNameInitialOnDisplayStyle { get; set; } = NameStyles.FullName;
        public NameStyles PCNameInitialOnLogStyle { get; set; } = NameStyles.FullName;
        public double PlayerInfoRefreshInterval { get; set; }
        public string ReadyText { get; set; }
        public int ReduceIconBrightness { get; set; }
        public long RefreshInterval { get; set; }
        public bool RemoveTooltipSymbols { get; set; }
        public bool RenderCPUOnly { get; set; } = true;
        public bool ResetOnWipeOut { get; set; }
        public string SaveLogDirectory { get; set; }
        public bool SaveLogEnabled { get; set; }
        public string SaveLogFile { get; set; }
        public bool SimpleRegex { get; set; }
        public bool TelopAlwaysVisible { get; set; }
        public double TextBlurRate { get; set; }
        public double TextOutlineThicknessRate { get; set; }
        public double TimeOfHideSpell { get; set; }
        public bool ToComplementUnknownSkill { get; set; } = true;
        public bool UseOtherThanFFXIV { get; set; }
        public bool WipeoutNotifyToACT { get; set; }

        public bool SingleTaskLogMatching { get; set; }

        public bool DisableStartCondition { get; set; }

        public bool EnableMultiLineMaching { get; set; }

        public AutoScaleMode UIAutoScaleMode { get; set; }

        private bool lpsViewVisible = false;
        private double lpsViewX;
        private double lpsViewY;
        private double lpsViewScale = 1.0;

        public bool LPSViewVisible
        {
            get => this.lpsViewVisible;
            set
            {
                if (this.SetProperty(ref this.lpsViewVisible, value))
                {
                    if (LPSView.Instance != null)
                    {
                        LPSView.Instance.OverlayVisible = value;
                    }
                }
            }
        }

        public double LPSViewX
        {
            get => this.lpsViewX;
            set => this.SetProperty(ref this.lpsViewX, value);
        }

        public double LPSViewY
        {
            get => this.lpsViewY;
            set => this.SetProperty(ref this.lpsViewY, value);
        }

        public double LPSViewScale
        {
            get => this.lpsViewScale;
            set => this.SetProperty(ref this.lpsViewScale, value);
        }

        #endregion Data

        #region Data - Hidden

        private DateTime lastUpdateDateTime;

        [XmlIgnore]
        public DateTime LastUpdateDateTime
        {
            get => this.lastUpdateDateTime;
            set => this.lastUpdateDateTime = value;
        }

        [XmlElement(ElementName = "LastUpdateDateTime")]
        public string LastUpdateDateTimeCrypted
        {
            get => Crypter.EncryptString(this.lastUpdateDateTime.ToString("o"));
            set
            {
                DateTime d;
                if (DateTime.TryParse(value, out d))
                {
                    if (d > DateTime.Now)
                    {
                        d = DateTime.Now;
                    }

                    this.lastUpdateDateTime = d;
                    return;
                }

                try
                {
                    var decrypt = Crypter.DecryptString(value);
                    if (DateTime.TryParse(decrypt, out d))
                    {
                        if (d > DateTime.Now)
                        {
                            d = DateTime.Now;
                        }

                        this.lastUpdateDateTime = d;
                        return;
                    }
                }
                catch (Exception)
                {
                }

                this.lastUpdateDateTime = DateTime.MinValue;
            }
        }

        public int MaxFPS { get; set; }

        /// <summary>点滅の輝度倍率 暗</summary>
        public double BlinkBrightnessDark { get; set; }

        /// <summary>点滅の輝度倍率 明</summary>
        public double BlinkBrightnessLight { get; set; }

        /// <summary>点滅のピッチ(秒)</summary>
        public double BlinkPitch { get; set; }

        /// <summary>点滅のピーク状態でのホールド時間(秒)</summary>
        public double BlinkPeekHold { get; set; }

        #endregion Data - Hidden

        #region Load & Save

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
                var directoryName = Path.GetDirectoryName(this.FileName);

                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                using (var xw = XmlWriter.Create(
                    this.FileName,
                    this.XmlWriterSettings))
                {
                    this.Serializer.Serialize(xw, this);
                }
            }
        }

        /// <summary>
        /// レンダリングモードを適用する
        /// </summary>
        public void ApplyRenderMode()
        {
            var renderMode =
                this.RenderCPUOnly ? RenderMode.SoftwareOnly : RenderMode.Default;

            if (System.Windows.Media.RenderOptions.ProcessRenderMode != renderMode)
            {
                System.Windows.Media.RenderOptions.ProcessRenderMode = renderMode;
            }
        }

        #endregion Load & Save

        #region Default Values & Reset

        public static readonly Dictionary<string, object> DefaultValues = new Dictionary<string, object>()
        {
            { nameof(Settings.Language), "EN" },
            { nameof(Settings.ProgressBarSize), new Size(190, 8) },
            { nameof(Settings.ProgressBarColor), Color.White },
            { nameof(Settings.ProgressBarOutlineColor), Color.FromArgb(22, 120, 157) },
            { nameof(Settings.FontColor), Color.AliceBlue },
            { nameof(Settings.FontOutlineColor), Color.FromArgb(22, 120, 157) },
            { nameof(Settings.WarningFontColor), Color.OrangeRed },
            { nameof(Settings.WarningFontOutlineColor), Color.DarkRed },
            { nameof(Settings.BackgroundColor), Color.Transparent },
            { nameof(Settings.NotifyNormalSpellTimerPrefix), "spespe_" },
            { nameof(Settings.ReadyText), "Ready" },
            { nameof(Settings.OverText), "Over" },
            { nameof(Settings.TimeOfHideSpell), 1.0d },
            { nameof(Settings.PlayerInfoRefreshInterval), 3.0d },
            { nameof(Settings.LogPollSleepInterval), 10 },
            { nameof(Settings.RefreshInterval), 60 },
            { nameof(Settings.CombatLogBufferSize), 30000 },
            { nameof(Settings.ReduceIconBrightness), 55 },
            { nameof(Settings.Opacity), 10 },
            { nameof(Settings.Font), FontInfo.DefaultFont.ToFontForWindowsForm() },
            { nameof(Settings.OverlayVisible), true },
            { nameof(Settings.AutoSortEnabled), true },
            { nameof(Settings.ClickThroughEnabled), false },
            { nameof(Settings.AutoSortReverse), false },
            { nameof(Settings.TelopAlwaysVisible), false },
            { nameof(Settings.EnabledPartyMemberPlaceholder), true },
            { nameof(Settings.CombatLogEnabled), false },
            { nameof(Settings.OverlayForceVisible), false },
            { nameof(Settings.EnabledSpellTimerNoDecimal), true },
            { nameof(Settings.EnabledNotifyNormalSpellTimer), false },
            { nameof(Settings.SaveLogEnabled), false },
            { nameof(Settings.SaveLogFile), string.Empty },
            { nameof(Settings.SaveLogDirectory), string.Empty },
            { nameof(Settings.HideWhenNotActive), false },
            { nameof(Settings.UseOtherThanFFXIV), false },
            { nameof(Settings.DQXUtilityEnabled), false },
            { nameof(Settings.DQXPlayerName), "トンヌラ" },
            { nameof(Settings.ResetOnWipeOut), true },
            { nameof(Settings.WipeoutNotifyToACT), true },
            { nameof(Settings.RemoveTooltipSymbols), true },
            { nameof(Settings.SimpleRegex), true },
            { nameof(Settings.DetectPacketDump), false },
            { nameof(Settings.TextBlurRate), 1.2d },
            { nameof(Settings.TextOutlineThicknessRate), 1.0d },
            { nameof(Settings.PCNameInitialOnLogStyle), NameStyles.FullName },
            { nameof(Settings.PCNameInitialOnDisplayStyle), NameStyles.FullName },
            { nameof(Settings.RenderCPUOnly), true },
            { nameof(Settings.ToComplementUnknownSkill), true },
            { nameof(Settings.SingleTaskLogMatching), false },
            { nameof(Settings.DisableStartCondition), false },
            { nameof(Settings.EnableMultiLineMaching), false },
            { nameof(Settings.UIAutoScaleMode), AutoScaleMode.Inherit },
            { nameof(Settings.MaxFPS), 30 },

            { nameof(Settings.LPSViewVisible), false },
            { nameof(Settings.LPSViewX), 0 },
            { nameof(Settings.LPSViewY), 0 },
            { nameof(Settings.LPSViewScale), 1.0 },

            { nameof(Settings.BarBackgroundFixed), false },
            { nameof(Settings.BarBackgroundBrightness), 0.3 },
            { nameof(Settings.BarDefaultBackgroundColor), System.Windows.Media.Color.FromArgb(240, 0, 0, 0) },

            // 設定画面のない設定項目
            { nameof(Settings.LastUpdateDateTime), DateTime.Parse("2000-1-1") },
            { nameof(Settings.BlinkBrightnessDark), 0.3d },
            { nameof(Settings.BlinkBrightnessLight), 2.5d },
            { nameof(Settings.BlinkPitch), 0.5d },
            { nameof(Settings.BlinkPeekHold), 0.08d },
        };

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>
        /// このオブジェクトのクローン</returns>
        public Settings Clone() => (Settings)this.MemberwiseClone();

        public void Reset()
        {
            lock (this.locker)
            {
                var pis = this.GetType().GetProperties();
                foreach (var pi in pis)
                {
                    try
                    {
                        var defaultValue =
                            DefaultValues.ContainsKey(pi.Name) ?
                            DefaultValues[pi.Name] :
                            null;

                        if (defaultValue != null)
                        {
                            pi.SetValue(this, defaultValue);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine($"Settings Reset Error: {pi.Name}");
                    }
                }
            }
        }

        #endregion Default Values & Reset
    }
}
