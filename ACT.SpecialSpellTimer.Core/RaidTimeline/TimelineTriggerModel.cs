using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "t")]
    public class TimelineTriggerModel :
        TimelineBase
    {
        [XmlIgnore]
        public override TimelineElementTypes TimelineType => TimelineElementTypes.Trigger;

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

        private int? syncCount = null;

        [XmlIgnore]
        public int? SyncCount
        {
            get => this.syncCount;
            set => this.SetProperty(ref this.syncCount, value);
        }

        [XmlAttribute(AttributeName = "sync-count")]
        public string SyncCountXML
        {
            get => this.SyncCount?.ToString();
            set => this.SyncCount = int.TryParse(value, out var v) ? v : (int?)null;
        }

        public string gotoDestination = null;

        [XmlAttribute(AttributeName = "goto")]
        public string GoToDestination
        {
            get => this.gotoDestination;
            set => this.SetProperty(ref this.gotoDestination, value);
        }

        public string callTarget = null;

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

        private string style = null;

        [XmlAttribute(AttributeName = "style")]
        public string Style
        {
            get => this.style;
            set => this.SetProperty(ref this.style, value);
        }

        private int matchedCounter = 0;

        [XmlIgnore]
        public int MatchedCounter
        {
            get => this.matchedCounter;
            set => this.SetProperty(ref this.matchedCounter, value);
        }
    }
}
