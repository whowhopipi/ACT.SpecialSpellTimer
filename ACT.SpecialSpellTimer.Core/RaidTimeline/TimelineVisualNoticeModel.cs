using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using FFXIV.Framework.Common;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "v-notice")]
    [Serializable]
    public class TimelineVisualNoticeModel :
        TimelineBase,
        IStylable
    {
        #region TimelineBase

        public override TimelineElementTypes TimelineType => TimelineElementTypes.VisualNotice;

        public override IList<TimelineBase> Children => null;

        #endregion TimelineBase

        public const string ParentTextPlaceholder = "{text}";
        public const string ParentNoticePlaceholder = "{notice}";

        private string text = null;

        [XmlAttribute(AttributeName = "text")]
        [DefaultValue(ParentTextPlaceholder)]
        public string Text
        {
            get => this.text;
            set => this.SetProperty(ref this.text, value);
        }

        private string textToDisplay = ParentTextPlaceholder;

        [XmlIgnore]
        public string TextToDisplay
        {
            get => this.textToDisplay;
            set => this.SetProperty(ref this.textToDisplay, value);
        }

        private double? duration = null;

        [XmlIgnore]
        public double? Duration
        {
            get => this.duration;
            set => this.SetProperty(ref this.duration, value);
        }

        private double durationToDisplay = 3;

        [XmlIgnore]
        public double DurationToDisplay
        {
            get => this.durationToDisplay;
            set => this.SetProperty(ref this.durationToDisplay, value);
        }

        [XmlAttribute(AttributeName = "duration")]
        public string DurationXML
        {
            get => this.Duration?.ToString();
            set => this.Duration = double.TryParse(value, out var v) ? v : (double?)null;
        }

        private bool? durationVisible = null;

        [XmlIgnore]
        public bool? DurationVisible
        {
            get => this.durationVisible;
            set => this.SetProperty(ref this.durationVisible, value);
        }

        [XmlAttribute(AttributeName = "duration-visible")]
        public string DurationVisibleXML
        {
            get => this.DurationVisible?.ToString();
            set => this.DurationVisible = bool.TryParse(value, out var v) ? v : (bool?)null;
        }

        private int? order = null;

        [XmlIgnore]
        public int? Order
        {
            get => this.order;
            set => this.SetProperty(ref this.order, value);
        }

        [XmlAttribute(AttributeName = "order")]
        public string OrderXML
        {
            get => this.Order?.ToString();
            set => this.Duration = int.TryParse(value, out var v) ? v : (int?)null;
        }

        private bool isVisible = false;

        [XmlIgnore]
        public bool IsVisible
        {
            get => this.isVisible;
            set => this.SetProperty(ref this.isVisible, value);
        }

        private long logSeq = 0L;

        [XmlIgnore]
        public long LogSeq
        {
            get => this.logSeq;
            set => this.SetProperty(ref this.logSeq, value);
        }

        private DateTime timestamp = DateTime.MinValue;

        [XmlIgnore]
        public DateTime Timestamp
        {
            get => this.timestamp;
            set => this.SetProperty(ref this.timestamp, value);
        }

        private DispatcherTimer timer;

        public void StartNotice(
            Action<TimelineVisualNoticeModel> removeAction,
            bool dummyMode = false)
        {
            this.DurationToDisplay = this.Duration.GetValueOrDefault();
            this.IsVisible = true;

            if (this.timer == null)
            {
                this.timer = new DispatcherTimer(DispatcherPriority.Background)
                {
                    Interval = TimeSpan.FromSeconds(1)
                };

                this.timer.Tick += (x, y) =>
                {
                    lock (this)
                    {
                        if (this.DurationToDisplay > 0)
                        {
                            this.DurationToDisplay--;
                        }
                        else
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

        #region IStylable

        private string style = null;

        [XmlAttribute(AttributeName = "style")]
        public string Style
        {
            get => this.style;
            set => this.SetProperty(ref this.style, value);
        }

        private TimelineStyle styleModel = null;

        [XmlIgnore]
        public TimelineStyle StyleModel
        {
            get => this.styleModel;
            set => this.SetProperty(ref this.styleModel, value);
        }

        private string icon = null;

        [XmlAttribute(AttributeName = "icon")]
        public string Icon
        {
            get => this.icon;
            set
            {
                if (this.SetProperty(ref this.icon, value))
                {
                    this.RaisePropertyChanged(nameof(this.IconImage));
                    this.RaisePropertyChanged(nameof(this.ThisIconImage));
                    this.RaisePropertyChanged(nameof(this.ExistsIcon));
                }
            }
        }

        [XmlIgnore]
        public bool ExistsIcon => this.GetExistsIcon();

        [XmlIgnore]
        public BitmapImage IconImage => this.GetIconImage();

        [XmlIgnore]
        public BitmapImage ThisIconImage => this.GetThisIconImage();

        #endregion IStylable

        #region IClonable

        public TimelineVisualNoticeModel Clone()
        {
            var clone = this.MemberwiseClone() as TimelineVisualNoticeModel;

            if (clone.timer != null)
            {
                clone.timer.Stop();
                clone.timer = null;
            }

            return clone;
        }

        #endregion IClonable

        #region Dummy Notice

        private static List<TimelineVisualNoticeModel> dummyNotices;

        public static List<TimelineVisualNoticeModel> DummyNotices =>
            dummyNotices ?? (dummyNotices = CreateDummyNotices());

        public static List<TimelineVisualNoticeModel> CreateDummyNotices(
            TimelineStyle testStyle = null)
        {
            var notices = new List<TimelineVisualNoticeModel>();

            if (testStyle == null)
            {
                testStyle = TimelineStyle.SuperDefaultStyle;
                if (!WPFHelper.IsDesignMode)
                {
                    testStyle = TimelineSettings.Instance.DefaultNoticeStyle;
                }
            }

            var notice1 = new TimelineVisualNoticeModel()
            {
                Enabled = true,
                TextToDisplay = "デスセンテンス\n→ タンク",
                Duration = 3,
                DurationVisible = true,
                StyleModel = testStyle,
                Icon = "1マーカー.png",
                IsVisible = true,
            };

            var notice2 = new TimelineVisualNoticeModel()
            {
                Enabled = true,
                TextToDisplay = "ツイスター",
                Duration = 10,
                DurationVisible = true,
                StyleModel = testStyle,
                Icon = "2マーカー.png",
                IsVisible = true,
            };

            notices.Add(notice1);
            notices.Add(notice2);

            return notices;
        }

        #endregion Dummy Notice
    }
}
