using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using FFXIV.Framework.Common;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "i-notice")]
    [Serializable]
    public class TimelineImageNoticeModel :
        TimelineBase
    {
        #region TimelineBase

        public override TimelineElementTypes TimelineType => TimelineElementTypes.ImageNotice;

        public override IList<TimelineBase> Children => null;

        #endregion TimelineBase

        #region Left

        private double? left = null;

        [XmlIgnore]
        public double? Left
        {
            get => this.left;
            set => this.SetProperty(ref this.left, value);
        }

        [XmlAttribute(AttributeName = "left")]
        public string LeftXML
        {
            get => this.Left?.ToString();
            set => this.Left = double.TryParse(value, out var v) ? v : (double?)null;
        }

        #endregion Left

        #region Top

        private double? top = null;

        [XmlIgnore]
        public double? Top
        {
            get => this.top;
            set => this.SetProperty(ref this.top, value);
        }

        [XmlAttribute(AttributeName = "top")]
        public string TopXML
        {
            get => this.Top?.ToString();
            set => this.Top = double.TryParse(value, out var v) ? v : (double?)null;
        }

        #endregion Top

        #region Scale

        private double? scale = null;

        [XmlIgnore]
        public double? Scale
        {
            get => this.scale;
            set => this.SetProperty(ref this.scale, value);
        }

        [XmlAttribute(AttributeName = "scale")]
        public string ScaleXML
        {
            get => this.Scale?.ToString();
            set => this.Scale = double.TryParse(value, out var v) ? v : (double?)null;
        }

        #endregion Scale

        #region Duration

        private double? duration = null;

        [XmlIgnore]
        public double? Duration
        {
            get => this.duration;
            set => this.SetProperty(ref this.duration, value);
        }

        [XmlAttribute(AttributeName = "duration")]
        public string DurationXML
        {
            get => this.Duration?.ToString();
            set => this.Duration = double.TryParse(value, out var v) ? v : (double?)null;
        }

        #endregion Duration

        private bool isVisible = false;

        [XmlIgnore]
        public bool IsVisible
        {
            get => this.isVisible;
            set => this.SetProperty(ref this.isVisible, value);
        }

        private DateTime timestamp = DateTime.MinValue;

        [XmlIgnore]
        public DateTime Timestamp
        {
            get => this.timestamp;
            set => this.SetProperty(ref this.timestamp, value);
        }

        [XmlIgnore]
        public DateTime TimeToHide
            => this.Timestamp.AddSeconds(this.Duration.GetValueOrDefault());

        private DispatcherTimer timer;

        public void StartNotice(
            Action<TimelineImageNoticeModel> removeAction,
            bool dummyMode = false)
        {
            this.IsVisible = true;

            if (this.timer == null)
            {
                this.timer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = TimeSpan.FromSeconds(0.25d)
                };

                this.timer.Tick += (x, y) =>
                {
                    lock (this)
                    {
                        if (DateTime.Now >= this.TimeToHide.AddSeconds(0.1d))
                        {
                            if (!dummyMode)
                            {
                                this.IsVisible = false;
                                removeAction?.Invoke(this);
                            }

                            (x as DispatcherTimer)?.Stop();
                        }
                    }
                };
            }

            this.timer.Start();
        }

        private string image = null;

        [XmlAttribute(AttributeName = "image")]
        public string Image
        {
            get => this.image;
            set
            {
                if (this.SetProperty(ref this.image, value))
                {
                    this.RaisePropertyChanged(nameof(this.BitmapImage));
                    this.RaisePropertyChanged(nameof(this.ExistsImage));
                }
            }
        }

        [XmlIgnore]
        public BitmapImage BitmapImage => GetBitmapImage(this.Image);

        [XmlIgnore]
        public bool ExistsImage => this.BitmapImage != null;

        private static readonly Dictionary<string, BitmapImage> ImageDictionary = new Dictionary<string, BitmapImage>();

        private static BitmapImage GetBitmapImage(
            string image)
        {
            lock (ImageDictionary)
            {
                if (ImageDictionary.ContainsKey(image))
                {
                    return ImageDictionary[image];
                }

                var file = string.Empty;

                if (File.Exists(image))
                {
                    file = image;
                }
                else
                {
                    var root = DirectoryHelper.FindSubDirectory(@"resources\images");
                    if (Directory.Exists(root))
                    {
                        file = FileHelper.FindFiles(root, image).FirstOrDefault();
                    }
                }

                if (string.IsNullOrEmpty(file) ||
                    !File.Exists(file))
                {
                    return null;
                }

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.CreateOptions = BitmapCreateOptions.None;
                bmp.UriSource = new Uri(file);
                bmp.EndInit();
                bmp.Freeze();

                ImageDictionary[image] = bmp;

                return bmp;
            }
        }

        #region IClonable

        public TimelineImageNoticeModel Clone()
        {
            var clone = this.MemberwiseClone() as TimelineImageNoticeModel;

            if (clone.timer != null)
            {
                clone.timer.Stop();
                clone.timer = null;
            }

            return clone;
        }

        #endregion IClonable
    }
}
