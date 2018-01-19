using System.Linq;
using System.Windows.Data;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Models;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.Models
{
    public class TriggersTreeRoot :
        BindableBase,
        ITreeItem
    {
        private string displayText = string.Empty;
        private CollectionViewSource children;

        [XmlIgnore]
        public ItemTypes ItemType => ItemTypes.Root;

        public int SortPriority { get; set; }

        public string DisplayText
        {
            get => this.displayText;
            set => this.SetProperty(ref this.displayText, value);
        }

        public CollectionViewSource Children
        {
            get => this.children;
            set => this.SetProperty(ref this.children, value);
        }

        public bool IsExpanded
        {
            get
            {
                var entry = Settings.Default.ExpandedList.FirstOrDefault(x => x.Key == this.DisplayText);
                if (entry == null)
                {
                    entry = new ExpandedContainer()
                    {
                        Key = this.DisplayText,
                        IsExpanded = true,
                    };

                    Settings.Default.ExpandedList.Add(entry);
                }

                return entry.IsExpanded;
            }
            set
            {
                var entry = Settings.Default.ExpandedList.FirstOrDefault(x => x.Key == this.DisplayText);
                if (entry == null)
                {
                    entry = new ExpandedContainer()
                    {
                        Key = this.DisplayText,
                        IsExpanded = value,
                    };

                    Settings.Default.ExpandedList.Add(entry);
                }

                if (entry.IsExpanded != value)
                {
                    entry.IsExpanded = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsEnabled
        {
            get => false;
            set { }
        }
    }
}
