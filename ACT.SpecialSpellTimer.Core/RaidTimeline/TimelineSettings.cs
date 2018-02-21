using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Utility;
using FFXIV.Framework.Common;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [Serializable]
    [XmlRoot(ElementName = "TimelineConfig")]
    [XmlType(TypeName = "TimelineConfig")]
    public class TimelineSettings :
        BindableBase
    {
        private static readonly object Locker = new object();

        #region Singleton

        private static TimelineSettings instance;

        public static TimelineSettings Instance =>
            instance ?? (instance = Load(FileName));

        public TimelineSettings()
        {
            if (!WPFHelper.IsDesignMode)
            {
                this.PropertyChanged += this.TimelineSettings_PropertyChanged;
            }
        }

        #endregion Singleton

        #region Data

        private bool designMode = false;

        [XmlIgnore]
        public bool DesignMode
        {
            get => this.designMode;
            set => this.SetProperty(ref this.designMode, value);
        }

        private bool enabled = false;

        public bool Enabled
        {
            get => this.enabled;
            set => this.SetProperty(ref this.enabled, value);
        }

        private bool overlayVisible = true;

        public bool OverlayVisible
        {
            get => this.overlayVisible;
            set => this.SetProperty(ref this.overlayVisible, value);
        }

        private double left = 10;

        public double Left
        {
            get => this.left;
            set => this.SetProperty(ref this.left, Math.Round(value));
        }

        private double top = 10;

        public double Top
        {
            get => this.top;
            set => this.SetProperty(ref this.top, Math.Round(value));
        }

        private bool clickthrough = false;

        public bool Clickthrough
        {
            get => this.clickthrough;
            set => this.SetProperty(ref this.clickthrough, value);
        }

        private double overlayOpacity = 0.95;

        public double OverlayOpacity
        {
            get => this.overlayOpacity;
            set => this.SetProperty(ref this.overlayOpacity, value);
        }

        private double overlayScale = 1.0;

        public double OverlayScale
        {
            get => this.overlayScale;
            set => this.SetProperty(ref this.overlayScale, value);
        }

        private double nearestActivityScale = 1.2;

        public double NearestActivityScale
        {
            get => this.nearestActivityScale;
            set => this.SetProperty(ref this.nearestActivityScale, value);
        }

        private double nextActivityBrightness = 0.7;

        public double NextActivityBrightness
        {
            get => this.nextActivityBrightness;
            set => this.SetProperty(ref this.nextActivityBrightness, value);
        }

        private int showActivitiesCount = 8;

        public int ShowActivitiesCount
        {
            get => this.showActivitiesCount;
            set => this.SetProperty(ref this.showActivitiesCount, value);
        }

        private double showActivitiesTime = 60;

        public double ShowActivitiesTime
        {
            get => this.showActivitiesTime;
            set => this.SetProperty(ref this.showActivitiesTime, value);
        }

        private Color backgroundColor = Colors.Transparent;

        public Color BackgroundColor
        {
            get => this.backgroundColor;
            set => this.SetProperty(ref this.backgroundColor, value);
        }

        private bool indicatorVisible = true;

        public bool IndicatorVisible
        {
            get => this.indicatorVisible;
            set => this.SetProperty(ref this.indicatorVisible, value);
        }

        private string indicatorStyle = "Default";

        public string IndicatorStyle
        {
            get => this.indicatorStyle;
            set
            {
                if (this.SetProperty(ref this.indicatorStyle, value))
                {
                    this.RaisePropertyChanged(nameof(this.IndicatorStyleModel));
                }
            }
        }

        [XmlIgnore]
        public TimelineStyle IndicatorStyleModel
        {
            get
            {
                var style = this.DefaultStyle;

                if (!string.IsNullOrEmpty(this.IndicatorStyle))
                {
                    var s = this.Styles.FirstOrDefault(x =>
                        string.Equals(x.Name, this.IndicatorStyle, StringComparison.OrdinalIgnoreCase));
                    if (s != null)
                    {
                        style = s;
                    }
                }

                return style;
            }
        }

        private ObservableCollection<TimelineStyle> styles = new ObservableCollection<TimelineStyle>();

        public ObservableCollection<TimelineStyle> Styles
        {
            get => this.styles;
            set
            {
                this.styles.Clear();
                this.styles.AddRange(value);
                this.RaisePropertyChanged(nameof(this.DefaultStyle));
            }
        }

        [XmlIgnore]
        public TimelineStyle DefaultStyle =>
            this.Styles.FirstOrDefault(x => x.IsDefault) ?? TimelineStyle.SuperDefaultStyle;

        #endregion Data

        #region Methods

        public static readonly string FileName = Path.Combine(
            DirectoryHelper.FindSubDirectory(@"resources\timeline"),
            @"Timeline.config");

        public static void Load() => instance = Load(FileName);

        public static void Save() => instance.Save(FileName);

        public static TimelineSettings Load(
            string file)
        {
            var data = default(TimelineSettings);

            lock (Locker)
            {
                try
                {
                    autoSave = false;

                    if (!File.Exists(file))
                    {
                        data = new TimelineSettings();
                        return data;
                    }

                    using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                    {
                        if (sr.BaseStream.Length > 0)
                        {
                            var xs = new XmlSerializer(typeof(TimelineSettings));
                            data = xs.Deserialize(sr) as TimelineSettings;
                        }
                    }
                }
                finally
                {
                    if (data != null &&
                        !data.Styles.Any())
                    {
                        var style = TimelineStyle.SuperDefaultStyle;
                        style.IsDefault = true;
                        data.Styles.Add(style);
                    }

                    autoSave = true;
                }
            }

            return data;
        }

        public void Save(
            string file)
        {
            lock (Locker)
            {
                FileHelper.CreateDirectory(file);

                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    var xs = new XmlSerializer(this.GetType());
                    xs.Serialize(sw, this, ns);
                }

                sb.Replace("utf-16", "utf-8");

                File.WriteAllText(
                    file,
                    sb.ToString() + Environment.NewLine,
                    new UTF8Encoding(false));
            }
        }

        private static bool autoSave = false;

        private void TimelineSettings_PropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (autoSave)
            {
                TimelineSettings.Save();
            }
        }

        #endregion Methods
    }
}
