using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                foreach (ItemTags itemTags in this.Children.View)
                {
                    if (itemTags.Item != null)
                    {
                        itemTags.Item.IsEnabled = value;
                    }
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
            Source = TagTable.Instance.ItemTags,
        };

        private void SetupChildrenSource()
        {
            this.Children.Filter += (x, y) =>
            {
                var item = y.Item as ItemTags;
                y.Accepted = item.TagID == this.ID;
            };

            this.Children.SortDescriptions.Add(new SortDescription()
            {
                PropertyName = nameof(ItemTags.ItemType),
                Direction = ListSortDirection.Ascending
            });
        }

        #endregion ITreeItem
    }
}
