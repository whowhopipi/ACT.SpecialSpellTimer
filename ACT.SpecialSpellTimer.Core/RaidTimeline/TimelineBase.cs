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

    [Serializable]
    public abstract class TimelineBase :
        BindableBase
    {
        [XmlIgnore]
        public abstract TimelineElementTypes TimelineType { get; }

        private Guid id = Guid.NewGuid();

        [XmlIgnore]
        public Guid ID => this.id;

        private string name = null;

        [XmlAttribute(AttributeName = "name")]
        public virtual string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
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
