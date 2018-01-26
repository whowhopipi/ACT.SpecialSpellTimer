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
                    case ItemTypes.TickersRoot:
                    case ItemTypes.TagsRoot:
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

                        foreach (var tagID in
                            TagTable.Instance.ItemTags
                                .Where(x => x.ItemID == currentPanel.ID).ToArray()
                                .Select(x => x.TagID)
                                .Distinct())
                        {
                            TagTable.Instance.ItemTags.Add(new ItemTags(newPanel.ID, tagID));
                        }

                        break;

                    case ItemTypes.Tag:
                        var currentTag = item as Tag;

                        newPanel = new SpellPanel()
                        {
                            PanelName = "New Panel"
                        };

                        SpellPanelTable.Instance.Table.Add(newPanel);

                        TagTable.Instance.ItemTags.Add(new ItemTags(newPanel.ID, currentTag.ID));
                        currentTag.IsExpanded = true;
                        break;
                }

                if (newPanel != null)
                {
                    newPanel.IsSelected = true;
                }
            }));

        private ICommand createNewSpellCommand;

        [XmlIgnore]
        public ICommand CreateNewSpellCommand =>
            this.createNewSpellCommand ?? (this.createNewSpellCommand = new DelegateCommand<ITreeItem>(item =>
            {
                var newSpell = default(Spell);
                var currentSpell = default(Spell);
                var currentPanel = default(SpellPanel);

                switch (item.ItemType)
                {
                    case ItemTypes.SpellsRoot:
                    case ItemTypes.TickersRoot:
                    case ItemTypes.TagsRoot:
                        newSpell = Spell.CreateNew();
                        newSpell.PanelID = SpellPanel.GeneralPanel.ID;
                        break;

                    case ItemTypes.SpellPanel:
                        currentPanel = item as SpellPanel;
                        currentSpell = (
                            from x in SpellTable.Instance.Table
                            where
                            x.PanelID == currentPanel.ID
                            orderby
                            x.SortPriority descending,
                            x.ID descending
                            select
                            x).FirstOrDefault();
                        if (currentSpell != null)
                        {
                            newSpell = currentSpell.CreateSimilarNew();
                        }
                        else
                        {
                            newSpell = Spell.CreateNew();
                            newSpell.PanelID = currentPanel.ID;
                        }

                        currentPanel.IsExpanded = true;
                        break;

                    case ItemTypes.Spell:
                        currentSpell = item as Spell;
                        newSpell = currentSpell.CreateSimilarNew();
                        break;

                    case ItemTypes.Tag:
                        var currentTag = item as Tag;
                        currentPanel = (
                            from x in SpellPanelTable.Instance.Table
                            join y in TagTable.Instance.ItemTags on
                            x.ID equals y.ItemID
                            where
                            y.TagID == currentTag.ID
                            orderby
                            x.PanelName
                            select
                            x).FirstOrDefault();

                        if (currentPanel != null)
                        {
                            currentSpell = (
                                from x in SpellTable.Instance.Table
                                where
                                x.PanelID == currentPanel.ID
                                orderby
                                x.SortPriority descending,
                                x.ID descending
                                select
                                x).FirstOrDefault();
                        }

                        if (currentSpell != null)
                        {
                            newSpell = currentSpell.CreateSimilarNew();
                        }
                        else
                        {
                            newSpell = Spell.CreateNew();
                            newSpell.PanelID = currentPanel != null ?
                                currentPanel.ID :
                                SpellPanel.GeneralPanel.ID;
                        }

                        currentTag.IsExpanded = true;
                        break;
                }

                if (newSpell != null)
                {
                    SpellTable.Instance.Table.Add(newSpell);
                    newSpell.IsSelected = true;
                }
            }));

        private ICommand createNewTickerCommand;

        [XmlIgnore]
        public ICommand CreateNewTickerCommand =>
            this.createNewTickerCommand ?? (this.createNewTickerCommand = new DelegateCommand<ITreeItem>(item =>
            {
                var newTicker = default(Ticker);
                var currentTicker = default(Ticker);

                switch (item.ItemType)
                {
                    case ItemTypes.SpellsRoot:
                    case ItemTypes.TickersRoot:
                    case ItemTypes.TagsRoot:
                        newTicker = Ticker.CreateNew();
                        TickerTable.Instance.Table.Add(newTicker);
                        break;

                    case ItemTypes.Ticker:
                        currentTicker = item as Ticker;
                        newTicker = currentTicker.CreateSimilarNew();
                        TickerTable.Instance.Table.Add(newTicker);

                        foreach (var tagID in
                            TagTable.Instance.ItemTags
                                .Where(x => x.ItemID == currentTicker.Guid).ToArray()
                                .Select(x => x.TagID)
                                .Distinct())
                        {
                            TagTable.Instance.ItemTags.Add(new ItemTags(newTicker.Guid, tagID));
                        }

                        break;

                    case ItemTypes.Tag:
                        var currentTag = item as Tag;
                        currentTicker = (
                            from x in TickerTable.Instance.Table
                            join y in TagTable.Instance.ItemTags on
                            x.Guid equals y.ItemID
                            where
                            y.TagID == currentTag.ID
                            orderby
                            x.Title
                            select
                            x).FirstOrDefault();

                        if (currentTicker != null)
                        {
                            newTicker = currentTicker.CreateSimilarNew();
                        }
                        else
                        {
                            newTicker = Ticker.CreateNew();
                        }

                        TickerTable.Instance.Table.Add(newTicker);
                        TagTable.Instance.ItemTags.Add(new ItemTags(newTicker.Guid, currentTag.ID));
                        currentTag.IsExpanded = true;
                        break;
                }

                if (newTicker != null)
                {
                    newTicker.IsSelected = true;
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
                    case ItemTypes.SpellsRoot:
                    case ItemTypes.TickersRoot:
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
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);
                        if (result != MessageBoxResult.OK)
                        {
                            return;
                        }

                        foreach (var toRemove in
                            TagTable.Instance.ItemTags.Where(x => x.TagID == tag.ID).ToArray())
                        {
                            TagTable.Instance.ItemTags.Remove(toRemove);
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
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);
                        if (result != MessageBoxResult.OK)
                        {
                            return;
                        }

                        var targets = SpellTable.Instance.Table.Where(x => x.PanelID == panel.ID).ToArray();
                        foreach (var target in targets)
                        {
                            SpellTable.Instance.Table.Remove(target);
                        }

                        foreach (var toRemove in
                            TagTable.Instance.ItemTags.Where(x => x.ItemID == panel.ID).ToArray())
                        {
                            TagTable.Instance.ItemTags.Remove(toRemove);
                        }

                        SpellPanelTable.Instance.Table.Remove(panel);
                        break;

                    case ItemTypes.Spell:
                        result = MessageBox.Show(
                            $@"Delete ""{ item.DisplayText }"" ?",
                            "Confirm",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);
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
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);
                        if (result != MessageBoxResult.OK)
                        {
                            return;
                        }

                        var ticker = item as Ticker;

                        foreach (var toRemove in
                            TagTable.Instance.ItemTags.Where(x => x.ItemID == ticker.Guid).ToArray())
                        {
                            TagTable.Instance.ItemTags.Remove(toRemove);
                        }

                        TickerTable.Instance.Table.Remove(ticker);
                        break;
                }
            }));

        #endregion Commands
    }
}
