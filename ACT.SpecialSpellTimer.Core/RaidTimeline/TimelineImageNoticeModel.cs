using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.RaidTimeline.Views;
using FFXIV.Framework.Common;
using Prism.Commands;

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

        public TimelineImageNoticeModel()
        {
            this.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(this.Left):
                    case nameof(this.Top):
                    case nameof(this.Scale):
                    case nameof(this.Duration):
                        this.RaisePropertyChanged(nameof(this.Tag));
                        break;
                }

                switch (e.PropertyName)
                {
                    case nameof(this.Left):
                    case nameof(this.Top):
                        this.RaisePropertyChanged(nameof(this.StartupLocation));
                        break;
                }
            };
        }

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

        [XmlIgnore]
        public string Tag =>
            $@"<i-notice" + Environment.NewLine +
            $@"  image=""{this.Image}""" + Environment.NewLine +
            $@"  scale=""{this.Scale}""" + Environment.NewLine +
            $@"  left=""{this.Left}""" + Environment.NewLine +
            $@"  top=""{this.Top}""" + Environment.NewLine +
            $@"  duration=""{this.Duration}""" + " />";

        [XmlIgnore]
        public WindowStartupLocation StartupLocation
            => this.Left == -1 && this.Top == -1 ?
                WindowStartupLocation.CenterScreen :
                WindowStartupLocation.Manual;

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

        private TimelineImageNoticeOverlay overlay;

        private DispatcherTimer timer;

        public void StanbyNotice()
        {
            lock (this)
            {
                if (this.overlay != null)
                {
                    this.overlay.Close();
                    this.overlay = null;
                }

                this.overlay = new TimelineImageNoticeOverlay()
                {
                    Model = this,
                    DummyMode = false,
                };

                this.overlay.WindowStartupLocation = this.StartupLocation;
                this.overlay.ChangeClickthrough(TimelineSettings.Instance.Clickthrough);
                this.overlay.Show();
            }
        }

        public void StartNotice()
        {
            lock (this)
            {
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
                                if (this.overlay != null)
                                {
                                    this.overlay.OverlayVisible = false;
                                }

                                (x as DispatcherTimer)?.Stop();
                            }
                        }
                    };
                }

                this.overlay?.ShowNotice();
                this.timer.Stop();
                this.timer.Start();
            }
        }

        public void CloseNotice()
        {
            lock (this)
            {
                if (this.overlay != null)
                {
                    this.overlay.Close();
                    this.overlay = null;
                }
            }
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

        #region Commmands

        private static readonly System.Windows.Forms.OpenFileDialog OpenFileDialog = new System.Windows.Forms.OpenFileDialog()
        {
            InitialDirectory = DirectoryHelper.FindSubDirectory(@"resources\images"),
            RestoreDirectory = true,
            Filter = "PNG Images|*.png|All Files|*.*",
            FilterIndex = 0,
            DefaultExt = ".png",
            SupportMultiDottedExtensions = true,
        };

        private ICommand browseImageCommand;

        public ICommand BrowseImageCommand =>
            this.browseImageCommand ?? (this.browseImageCommand = new DelegateCommand(() =>
            {
                var result = OpenFileDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.Image = Path.GetFileName(OpenFileDialog.FileName);
                }
            }));

        private ICommand copyTagCommand;

        public ICommand CcopyTagCommand =>
            this.copyTagCommand ?? (this.copyTagCommand = new DelegateCommand(() =>
            {
                Clipboard.SetText(this.Tag + Environment.NewLine);
            }));

        #endregion Commmands

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

        #region Dummy Notice

        private static TimelineImageNoticeModel dummyNotice;

        public static TimelineImageNoticeModel DummyNotice =>
            dummyNotice ?? (dummyNotice = CreateDummyNotice());

        private static TimelineImageNoticeModel CreateDummyNotice()
        {
            var dummy = new TimelineImageNoticeModel();

            dummy.Image = "Sample.png";
            dummy.Scale = 1.0;
            dummy.Duration = 5;
            dummy.Left = -1;
            dummy.Top = -1;

            return dummy;
        }

        #endregion Dummy Notice
    }
}
