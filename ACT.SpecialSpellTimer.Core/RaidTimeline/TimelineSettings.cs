using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private TimelineSettings()
        {
        }

        #endregion Singleton

        #region Data

        private bool overlayVisible = true;

        public bool OverlayVisible
        {
            get => this.overlayVisible;
            set => this.SetProperty(ref this.overlayVisible, value);
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

        private List<TimelineStyle> styles = new List<TimelineStyle>();
        private Dictionary<string, TimelineStyle> styleTable;

        public List<TimelineStyle> Styles
        {
            get => this.styles;
            set
            {
                if (this.SetProperty(ref this.styles, value))
                {
                    this.styleTable = this.styles.ToDictionary(x => x.Name);
                }
            }
        }

        [XmlIgnore]
        public IReadOnlyDictionary<string, TimelineStyle> StyleTable =>
            this.styleTable ?? (this.styleTable = this.styles.ToDictionary(x => x.Name));

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
                if (!File.Exists(file))
                {
                    return new TimelineSettings();
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

        #endregion Methods
    }
}
