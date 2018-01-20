using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.Models
{
    public enum ItemTypes
    {
        Unknown,
        SpellsRoot,
        TickersRoot,
        TagsRoot,
        Tag,
        SpellPanel,
        Spell,
        Ticker,
    }

    public interface ITreeItem
    {
        ItemTypes ItemType { get; }

        int SortPriority { get; set; }

        string DisplayText { get; }

        bool IsExpanded { get; set; }

        bool Enabled { get; set; }

        bool IsSelected { get; set; }

        bool IsInEditMode { get; set; }

        bool IsInViewMode { get; }

        ICollectionView Children { get; }
    }

    public abstract class TreeItemBase :
        BindableBase,
        ITreeItem
    {
        private bool isSelected;
        private bool isInEditMode;

        public abstract ItemTypes ItemType { get; }

        public abstract int SortPriority { get; set; }

        public abstract string DisplayText { get; }

        public abstract bool IsExpanded { get; set; }

        public abstract bool Enabled { get; set; }

        [XmlIgnore]
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetProperty(ref this.isSelected, value);
        }

        [XmlIgnore]
        public bool IsInEditMode
        {
            get => this.isInEditMode;
            set
            {
                if (this.SetProperty(ref this.isInEditMode, value))
                {
                    this.RaisePropertyChanged(nameof(this.IsInViewMode));
                }
            }
        }

        [XmlIgnore]
        public bool IsInViewMode => !this.IsInEditMode;

        public abstract ICollectionView Children { get; }

        #region Commands

        private ICommand createNewSpellPanelCommand;

        [XmlIgnore]
        public ICommand CreateNewSpellPanelCommand =>
            this.createNewSpellPanelCommand ?? (this.createNewSpellPanelCommand = new DelegateCommand<ITreeItem>(item =>
            {
                switch (item.ItemType)
                {
                    case ItemTypes.SpellsRoot:
                        break;

                    case ItemTypes.SpellPanel:
                        break;

                    case ItemTypes.Tag:
                        break;
                }
            }));

        private ICommand createNewSpellCommand;

        [XmlIgnore]
        public ICommand CreateNewSpellCommand =>
            this.createNewSpellCommand ?? (this.createNewSpellCommand = new DelegateCommand<ITreeItem>(item =>
            {
                switch (item.ItemType)
                {
                    case ItemTypes.SpellsRoot:
                        break;

                    case ItemTypes.SpellPanel:
                        break;

                    case ItemTypes.Spell:
                        break;

                    case ItemTypes.Tag:
                        break;
                }
            }));

        private ICommand createNewTickerCommand;

        [XmlIgnore]
        public ICommand CreateNewTickerCommand =>
            this.createNewTickerCommand ?? (this.createNewTickerCommand = new DelegateCommand<ITreeItem>(item =>
            {
                switch (item.ItemType)
                {
                    case ItemTypes.TickersRoot:
                        break;

                    case ItemTypes.Ticker:
                        break;

                    case ItemTypes.Tag:
                        break;
                }
            }));

        private ICommand createNewTagCommand;

        [XmlIgnore]
        public ICommand CreateNewTagCommand =>
            this.createNewTagCommand ?? (this.createNewTagCommand = new DelegateCommand<ITreeItem>(item =>
            {
                var newItem = default(Tag);

                switch (item.ItemType)
                {
                    case ItemTypes.TagsRoot:
                        newItem = TagTable.Instance.AddNew("New Tag");
                        newItem.isSelected = true;
                        break;

                    case ItemTypes.Tag:
                        newItem = TagTable.Instance.AddNew(item as Tag, "New Tag");
                        newItem.isSelected = true;
                        break;
                }
            }));

        private ICommand renameCommand;

        [XmlIgnore]
        public ICommand RenameCommand =>
            this.renameCommand ?? (this.renameCommand = new DelegateCommand<ITreeItem>(item =>
            {
                switch (item.ItemType)
                {
                    case ItemTypes.Tag:
                        if ((item as Tag).ID != Tag.ImportsTag.ID)
                        {
                            item.IsInEditMode = true;
                        }

                        break;
                }
            }));

        #endregion Commands
    }
}
