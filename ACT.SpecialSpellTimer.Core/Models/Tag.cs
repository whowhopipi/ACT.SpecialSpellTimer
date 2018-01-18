using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Models
{
    [Serializable]
    public class Tag :
        BindableBase
    {
        private Guid id = Guid.NewGuid();
        private Guid parentID = Guid.Empty;
        private string name = string.Empty;

        public Guid ID
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public Guid ParentID
        {
            get => this.parentID;
            set
            {
                if (this.SetProperty(ref this.parentID, value))
                {
                    this.RaisePropertyChanged(nameof(this.FullName));
                }
            }
        }

        public string Name
        {
            get => this.name;
            set
            {
                if (this.SetProperty(ref this.name, value))
                {
                    this.RaisePropertyChanged(nameof(this.FullName));
                }
            }
        }

        [XmlIgnore]
        public string FullName
        {
            get
            {
                if (this.ParentID == Guid.Empty)
                {
                    return this.Name;
                }

                var names = new List<string>();
                var current = this;
                var parent = default(Tag);
                while ((parent = current.Parent) != null)
                {
                    names.Add(parent.Name);
                    current = parent;
                }

                names.Reverse();

                return $"{string.Join("/", names)}/{this.Name}";
            }
        }

        [XmlIgnore]
        public Tag Parent =>
            this.ParentID == Guid.Empty ?
            null :
            TagTable.Instance.Table
                .FirstOrDefault(x => x.ID == this.ParentID);

        [XmlIgnore]
        public IReadOnlyList<Tag> Children =>
            TagTable.Instance.Table
                .Where(x => x.ParentID == this.ID)
                .ToList();
    }
}
