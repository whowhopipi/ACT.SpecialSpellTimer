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

        private bool isVisible = false;

        [XmlIgnore]
        public bool IsVisible
        {
            get => this.isVisible;
            set => this.SetProperty(ref this.isVisible, value);
        }

        private DispatcherTimer timer;

        public void StartNotice(
            bool dummyMode = false)
        {
            this.IsVisible = true;

            if (this.timer == null)
            {
                this.timer = new DispatcherTimer(DispatcherPriority.Background)
                {
                    Interval = TimeSpan.FromSeconds(1)
                };

                this.timer.Tick += (x, y) =>
                {
                    if (this.Duration > 0)
                    {
                        this.Duration--;
                    }
                    else
                    {
                        if (!dummyMode)
                        {
                            this.IsVisible = false;
                        }

                        (x as DispatcherTimer)?.Stop();
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
                TextToDisplay = "デスセンテンス",
                Duration = 3,
                StyleModel = testStyle,
                Icon = "1マーカー128px.png",
                IsVisible = true,
            };

            var notice2 = new TimelineVisualNoticeModel()
            {
                Enabled = true,
                TextToDisplay = "ツイスター",
                Duration = 10,
                StyleModel = testStyle,
                Icon = "2マーカー128px.png",
                IsVisible = true,
            };

            notices.Add(notice1);
            notices.Add(notice2);

            return notices;
        }

        #endregion Dummy Notice
    }
}
