using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.Models;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public class TriggersViewModel :
        BindableBase
    {
        public TriggersViewModel()
        {
            this.SetupTreeRoot();
        }

        private ObservableCollection<TriggersTreeRoot> treeRoot = new ObservableCollection<TriggersTreeRoot>();

        public ObservableCollection<TriggersTreeRoot> TreeRoot => treeRoot;

        private CollectionViewSource spellsSource = new CollectionViewSource()
        {
            Source = SpellPanelTable.Instance.Table,
            IsLiveFilteringRequested = true,
            IsLiveSortingRequested = true,
        };

        private CollectionViewSource tickersSource = new CollectionViewSource()
        {
            Source = TickerTable.Instance.Table,
            IsLiveFilteringRequested = true,
            IsLiveSortingRequested = true,
        };

        private CollectionViewSource tagsSource = new CollectionViewSource()
        {
            Source = TagTable.Instance.Tags,
            IsLiveFilteringRequested = true,
            IsLiveSortingRequested = true,
        };

        private void SetupTreeRoot()
        {
            var spells = new TriggersTreeRoot(
                ItemTypes.SpellsRoot,
                "All Spells",
                this.spellsSource.View);

            var tickers = new TriggersTreeRoot(
                ItemTypes.TickersRoot,
                "All Tickers",
                this.tickersSource.View);

            var tags = new TriggersTreeRoot(
                ItemTypes.TagsRoot,
                "Tags",
                this.tagsSource.View);

            this.spellsSource.SortDescriptions.AddRange(new[]
            {
                new SortDescription()
                {
                    PropertyName = nameof(SpellPanel.SortPriority),
                    Direction = ListSortDirection.Descending,
                },
                new SortDescription()
                {
                    PropertyName = nameof(SpellPanel.PanelName),
                    Direction = ListSortDirection.Ascending,
                },
                new SortDescription()
                {
                    PropertyName = nameof(SpellPanel.ID),
                    Direction = ListSortDirection.Ascending,
                },
            });

            this.tickersSource.SortDescriptions.AddRange(new[]
            {
                new SortDescription()
                {
                    PropertyName = nameof(Ticker.Title),
                    Direction = ListSortDirection.Ascending,
                },
                new SortDescription()
                {
                    PropertyName = nameof(Ticker.ID),
                    Direction = ListSortDirection.Ascending,
                },
            });

            this.tagsSource.Filter += (x, y) =>
            {
                var item = y.Item as Tag;
                y.Accepted = item.ParentTagID == Guid.Empty;
            };

            this.tagsSource.SortDescriptions.AddRange(new[]
            {
                new SortDescription()
                {
                    PropertyName = nameof(Ticker.SortPriority),
                    Direction = ListSortDirection.Descending,
                },
                new SortDescription()
                {
                    PropertyName = nameof(Tag.Name),
                    Direction = ListSortDirection.Ascending,
                },
                new SortDescription()
                {
                    PropertyName = nameof(Tag.ID),
                    Direction = ListSortDirection.Ascending,
                },
            });

            this.TreeRoot.Add(spells);
            this.TreeRoot.Add(tickers);
            this.TreeRoot.Add(tags);
        }
    }
}
