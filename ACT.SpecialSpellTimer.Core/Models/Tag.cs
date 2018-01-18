using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Models
{
    [Serializable]
    public class Tag :
        BindableBase,
        ITreeItem
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
        public IReadOnlyList<Tag> Tags =>
            TagTable.Instance.Table
                .Where(x => x.ParentID == this.ID)
                .ToList();

        [XmlIgnore]
        public IReadOnlyList<Tag> AllTags
        {
            get
            {
                var tags = new List<Tag>();
                foreach (var tag in this.Tags)
                {
                    tags.AddRange(tag.AllTags);
                }

                return tags;
            }
        }

        #region ITreeItem

        private bool isExpanded = false;

        [XmlIgnore]
        public string DisplayText => this.Name;

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        [XmlIgnore]
        public bool IsEnabled
        {
            get
            {
                var tags = this.AllTags;
                return tags.Count == tags.Count(x => x.IsEnabled);
            }
            set
            {
                foreach (var tag in this.AllTags)
                {
                    tag.IsEnabled = value;
                }

                this.RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public IReadOnlyList<ITreeItem> Children => this.Tags;

        #endregion ITreeItem
    }
}
