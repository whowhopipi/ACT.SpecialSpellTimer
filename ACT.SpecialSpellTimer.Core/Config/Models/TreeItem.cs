using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
                var newPanel = default(SpellPanel);

                switch (item.ItemType)
                {
                    case ItemTypes.SpellsRoot:
                        newPanel = new SpellPanel()
                        {
                            PanelName = "New Panel"
                        };

                        SpellPanelTable.Instance.Table.Add(newPanel);
                        break;

                    case ItemTypes.SpellPanel:
                        var currentPanel = item as SpellPanel;
                        newPanel = currentPanel.MemberwiseClone() as SpellPanel;
                        newPanel.ID = Guid.NewGuid();
                        newPanel.PanelName += " New";
                        newPanel.SetupChildrenSource();
                        SpellPanelTable.Instance.Table.Add(newPanel);
                        break;

                    case ItemTypes.Tag:
                        newPanel = new SpellPanel()
                        {
                            PanelName = "New Panel"
                        };

                        var tag = item as Tag;
                        TagTable.Instance.ItemTags.Add(new ItemTags(newPanel.ID, tag.ID));
                        SpellPanelTable.Instance.Table.Add(newPanel);
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
                    case ItemTypes.Tag:
                        newItem = TagTable.Instance.AddNew("New Tag");
                        newItem.IsSelected = true;
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

        private ICommand deleteCommand;

        [XmlIgnore]
        public ICommand DeleteCommand =>
            this.deleteCommand ?? (this.deleteCommand = new DelegateCommand<ITreeItem>(item =>
            {
                var result = default(MessageBoxResult);

                switch (item.ItemType)
                {
                    case ItemTypes.Tag:
                        var tag = item as Tag;
                        if (tag.ID == Tag.ImportsTag.ID)
                        {
                            return;
                        }

                        result = MessageBox.Show(
                            $@"Delete ""{ item.DisplayText }"" tag ?",
                            "Confirm",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Question);
                        if (result != MessageBoxResult.OK)
                        {
                            return;
                        }

                        TagTable.Instance.Remove(tag);
                        break;

                    case ItemTypes.SpellPanel:
                        var panel = item as SpellPanel;
                        if (panel.ID == SpellPanel.GeneralPanel.ID)
                        {
                            return;
                        }

                        result = MessageBox.Show(
                            $@"Delete ""{ item.DisplayText }"" panel and spells ?",
                            "Confirm",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Question);
                        if (result != MessageBoxResult.OK)
                        {
                            return;
                        }

                        var targets = SpellTable.Instance.Table.Where(x => x.PanelID == panel.ID).ToArray();
                        foreach (var target in targets)
                        {
                            SpellTable.Instance.Table.Remove(target);
                        }

                        SpellPanelTable.Instance.Table.Remove(panel);
                        break;

                    case ItemTypes.Spell:
                        result = MessageBox.Show(
                            $@"Delete ""{ item.DisplayText }"" ?",
                            "Confirm",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Question);
                        if (result != MessageBoxResult.OK)
                        {
                            return;
                        }

                        var spell = item as Spell;
                        SpellTable.Instance.Table.Remove(spell);
                        break;

                    case ItemTypes.Ticker:
                        result = MessageBox.Show(
                            $@"Delete ""{ item.DisplayText }"" ?",
                            "Confirm",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Question);
                        if (result != MessageBoxResult.OK)
                        {
                            return;
                        }

                        var ticker = item as Ticker;
                        TickerTable.Instance.Table.Remove(ticker);
                        break;
                }
            }));

        #endregion Commands
    }
}
