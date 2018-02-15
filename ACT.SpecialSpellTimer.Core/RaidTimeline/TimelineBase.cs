using System;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [Serializable]
    public abstract class TimelineBase :
        BindableBase
    {
        private string name = string.Empty;

        [XmlAttribute(AttributeName = "name")]
        public string Name
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
