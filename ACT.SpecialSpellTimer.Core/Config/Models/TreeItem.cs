using System;
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

        bool IsEnabled { get; set; }

        bool IsSelected { get; set; }

        ICollectionView Children { get; }
    }

    [Serializable]
    public abstract class TreeItemBase :
        BindableBase,
        ITreeItem
    {
        private bool isSelected;

        public abstract ItemTypes ItemType { get; }

        public abstract int SortPriority { get; set; }

        public abstract string DisplayText { get; }

        public abstract bool IsExpanded { get; set; }

        [XmlElement(ElementName = "Enabled")]
        public abstract bool IsEnabled { get; set; }

        [XmlIgnore]
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetProperty(ref this.isSelected, value);
        }

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
                        newItem = TagTable.Instance.AddNew();
                        newItem.Name = "New Tag";
                        newItem.isSelected = true;
                        break;

                    case ItemTypes.Tag:
                        newItem = TagTable.Instance.AddNew(item as Tag);
                        newItem.Name = "New Tag";
                        newItem.isSelected = true;
                        break;
                }
            }));

        #endregion Commands
    }
}
