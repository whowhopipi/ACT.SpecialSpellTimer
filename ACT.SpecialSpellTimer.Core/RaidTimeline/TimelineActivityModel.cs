using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [Serializable]
    [XmlType(TypeName = "a")]
    public class TimelineActivityModel :
        TimelineBase
    {
        private string syncKeyword = string.Empty;

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

        private double syncOffsetStart = -12;
        private double syncOffsetEnd = 12;

        [XmlAttribute(AttributeName = "sync-s")]
        public double SyncOffsetStart
        {
            get => this.syncOffsetStart;
            set => this.SetProperty(ref this.syncOffsetStart, value);
        }

        [XmlAttribute(AttributeName = "sync-e")]
        public double SyncOffsetEnd
        {
            get => this.syncOffsetEnd;
            set => this.SetProperty(ref this.syncOffsetEnd, value);
        }

        public string gotoDestination = string.Empty;

        [XmlAttribute(AttributeName = "goto")]
        public string GoToDestination
        {
            get => this.gotoDestination;
            set => this.SetProperty(ref this.gotoDestination, value);
        }

        public string callTarget = string.Empty;

        [XmlAttribute(AttributeName = "call")]
        public string CallTarget
        {
            get => this.callTarget;
            set => this.SetProperty(ref this.callTarget, value);
        }
    }
}
