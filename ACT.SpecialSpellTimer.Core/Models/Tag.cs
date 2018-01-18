using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
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
        private Guid parentTagID = Guid.Empty;
        private string name = string.Empty;

        public Guid ID
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public Guid ParentTagID
        {
            get => this.parentTagID;
            set
            {
                if (this.SetProperty(ref this.parentTagID, value))
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
                if (this.ParentTagID == Guid.Empty)
                {
                    return this.Name;
                }

                var names = new List<string>();
                var current = this;
                var parent = default(Tag);
                while ((parent = current.ParentTag) != null)
                {
                    names.Add(parent.Name);
                    current = parent;
                }

                names.Reverse();

                return $"{string.Join("/", names)}/{this.Name}";
            }
        }

        [XmlIgnore]
        public Tag ParentTag =>
            this.ParentTagID == Guid.Empty ?
            null :
            TagTable.Instance.Tags
                .FirstOrDefault(x => x.ID == this.ParentTagID);

        [XmlIgnore]
        public IReadOnlyList<Tag> ChildTags =>
            TagTable.Instance.Tags
                .Where(x => x.ParentTagID == this.ID)
                .ToList();

        [XmlIgnore]
        public IReadOnlyList<ITrigger> Triggers
        {
            get
            {
                var triggers = new List<ITrigger>();
                triggers.AddRange(SpellPanelTable.Instance.Table.Where(x => x.Tags.Contains(this.ID)));
                triggers.AddRange(TickerTable.Instance.Table.Where(x => x.Tags.Contains(this.ID)));
                return triggers;
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
            get => false;
            set
            {
                foreach (var item in this.ChildrenSource)
                {
                    item.IsEnabled = value;
                }
            }
        }

        [XmlIgnore]
        public CollectionViewSource Children
        {
            get;
            private set;
        } = new CollectionViewSource()
        {
            Source = TagTable.Instance.ItemTags
        };

        private void SetupChildrenSource()
        {
            this.Children.Filter += (x, y) =>
            {
            };
        }

        [XmlIgnore]
        public IReadOnlyList<ITreeItem> ChildrenSource
        {
            get
            {
                var items = new List<ITreeItem>();

                items.AddRange(this.ChildTags);
                if (items.Any())
                {
                    return items;
                }

                foreach (var trigger in this.Triggers)
                {
                    items.Add(trigger as ITreeItem);
                }

                return items;
            }
        }

        #endregion ITreeItem
    }
}
