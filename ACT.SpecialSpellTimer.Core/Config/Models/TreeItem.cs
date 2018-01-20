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

        ICollectionView Children { get; }
    }

    [Serializable]
    public abstract class TreeItemBase :
        BindableBase,
        ITreeItem
    {
        public abstract ItemTypes ItemType { get; }

        public abstract int SortPriority { get; set; }

        public abstract string DisplayText { get; }

        public abstract bool IsExpanded { get; set; }

        [XmlElement(ElementName = "Enabled")]
        public abstract bool IsEnabled { get; set; }

        public abstract ICollectionView Children { get; }

        private ICommand createNewSpellPanelCommand;

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

        public ICommand CreateNewTagCommand =>
            this.createNewTagCommand ?? (this.createNewTagCommand = new DelegateCommand<ITreeItem>(item =>
            {
                switch (item.ItemType)
                {
                    case ItemTypes.TagsRoot:
                        TagTable.Instance.AddNew().Name = "New Tag";
                        break;

                    case ItemTypes.Tag:
                        TagTable.Instance.AddNew(item as Tag).Name = "New Tag";
                        break;
                }
            }));
    }
}
