using System;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    public enum NoticeDevices
    {
        Both = 0,
        Main,
        Sub,
    }

    public enum TimelineElementTypes
    {
        Timeline = 0,
        Default,
        Activity,
        Trigger,
        Subroutine
    }

    public static class TimelineElementTypesEx
    {
        public static string ToText(
            this TimelineElementTypes t)
            => new[]
            {
                "timeline",
                "default",
                "activity",
                "trigger",
                "subroutine",
            }[(int)t];

        public static TimelineElementTypes FromText(
            string text)
        {
            if (Enum.TryParse<TimelineElementTypes>(text, out TimelineElementTypes e))
            {
                return e;
            }

            return TimelineElementTypes.Timeline;
        }
    }

    [Serializable]
    public abstract class TimelineBase :
        BindableBase
    {
        [XmlIgnore]
        public abstract TimelineElementTypes TimelineType { get; }

        protected Guid id = Guid.NewGuid();

        [XmlIgnore]
        public Guid ID => this.id;

        private string name = null;

        [XmlAttribute(AttributeName = "name")]
        public virtual string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        private bool? enabled = null;

        [XmlIgnore]
        public virtual bool? Enabled
        {
            get => this.enabled;
            set => this.SetProperty(ref this.enabled, value);
        }

        [XmlAttribute(AttributeName = "enabled")]
        public string EnabledXML
        {
            get => this.Enabled?.ToString();
            set => this.Enabled = bool.TryParse(value, out var v) ? v : (bool?)null;
        }

        private BindableBase parent = null;

        [XmlIgnore]
        public BindableBase Parent
        {
            get => this.parent;
            set => this.SetProperty(ref this.parent, value);
        }

        public T GetParent<T>() where T : BindableBase
            => this.parent as T;
    }
}
