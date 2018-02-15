using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [Serializable]
    [XmlType(TypeName = "a")]
    public class TimelineActivityModel :
        TimelineBase
    {
        [XmlIgnore]
        public override TimelineElementTypes TimelineType => TimelineElementTypes.Activity;

        private TimeSpan time = TimeSpan.Zero;

        [XmlIgnore]
        public TimeSpan Time
        {
            get => this.time;
            set => this.SetProperty(ref this.time, value);
        }

        [XmlAttribute(AttributeName = "time")]
        public string TimeText
        {
            get => this.time.ToTLString();
            set => this.SetProperty(ref this.time, TimeSpanExtensions.FromTLString(value));
        }

        private string text = null;

        [XmlAttribute(AttributeName = "text")]
        public string Text
        {
            get => this.text;
            set => this.SetProperty(ref this.text, value);
        }

        private string syncKeyword = null;

        [XmlAttribute(AttributeName = "sync")]
        public string SyncKeyword
        {
            get => this.syncKeyword;
            set
            {
                if (this.SetProperty(ref this.syncKeyword, value))
                {
                    if (string.IsNullOrEmpty(this.syncKeyword))
                    {
                        this.SyncKeyword = null;
                    }
                    else
                    {
                        this.SynqRegex = new Regex(
                            this.syncKeyword,
                            RegexOptions.Compiled |
                            RegexOptions.ExplicitCapture |
                            RegexOptions.IgnoreCase);
                    }
                }
            }
        }

        private Regex syncRegex = null;

        [XmlIgnore]
        public Regex SynqRegex
        {
            get => this.syncRegex;
            private set => this.SetProperty(ref this.syncRegex, value);
        }

        private double? syncOffsetStart = null;
        private double? syncOffsetEnd = null;

        [XmlIgnore]
        public double? SyncOffsetStart
        {
            get => this.syncOffsetStart;
            set => this.SetProperty(ref this.syncOffsetStart, value);
        }

        [XmlAttribute(AttributeName = "sync-s")]
        public string SyncOffsetStartXML
        {
            get => this.SyncOffsetStart?.ToString();
            set => this.SyncOffsetStart = double.TryParse(value, out var v) ? v : (double?)null;
        }

        [XmlIgnore]
        public double? SyncOffsetEnd
        {
            get => this.syncOffsetEnd;
            set => this.SetProperty(ref this.syncOffsetEnd, value);
        }

        [XmlAttribute(AttributeName = "sync-e")]
        public string SyncOffsetEndXML
        {
            get => this.syncOffsetEnd?.ToString();
            set => this.syncOffsetEnd = double.TryParse(value, out var v) ? v : (double?)null;
        }

        private string gotoDestination = null;

        [XmlAttribute(AttributeName = "goto")]
        public string GoToDestination
        {
            get => this.gotoDestination;
            set => this.SetProperty(ref this.gotoDestination, value);
        }

        private string callTarget = null;

        [XmlAttribute(AttributeName = "call")]
        public string CallTarget
        {
            get => this.callTarget;
            set => this.SetProperty(ref this.callTarget, value);
        }

        private string notice = null;

        [XmlAttribute(AttributeName = "notice")]
        public string Notice
        {
            get => this.notice;
            set => this.SetProperty(ref this.notice, value);
        }

        private NoticeDevices? noticeDevice = null;

        [XmlIgnore]
        public NoticeDevices? NoticeDevice
        {
            get => this.noticeDevice;
            set => this.SetProperty(ref this.noticeDevice, value);
        }

        [XmlAttribute(AttributeName = "notice-d")]
        public string NoticeDeviceXML
        {
            get => this.NoticeDevice?.ToString();
            set => this.NoticeDevice = Enum.TryParse<NoticeDevices>(value, out var v) ? v : (NoticeDevices?)null;
        }

        private double? noticeOffset = null;

        [XmlIgnore]
        public double? NoticeOffset
        {
            get => this.noticeOffset;
            set => this.SetProperty(ref this.noticeOffset, value);
        }

        [XmlAttribute(AttributeName = "notice-o")]
        public string NoticeOffsetXML
        {
            get => this.NoticeOffset?.ToString();
            set => this.NoticeOffset = double.TryParse(value, out var v) ? v : (double?)null;
        }

        private string style = null;

        [XmlAttribute(AttributeName = "style")]
        public string Style
        {
            get => this.style;
            set => this.SetProperty(ref this.style, value);
        }
    }
}
