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

        private void SetupTreeRoot()
        {
            var spells = new TriggersTreeRoot()
            {
                DisplayText = "All Spells",
                Children = new CollectionViewSource()
                {
                    Source = SpellPanelTable.Instance.Table,
                }
            };

            var tickers = new TriggersTreeRoot()
            {
                DisplayText = "All Tickers",
                Children = new CollectionViewSource()
                {
                    Source = TickerTable.Instance.Table,
                }
            };

            var tags = new TriggersTreeRoot()
            {
                DisplayText = "Tags",
                Children = new CollectionViewSource()
                {
                    Source = TagTable.Instance.Tags,
                }
            };

            spells.Children.SortDescriptions.Add(new SortDescription()
            {
                PropertyName = nameof(SpellPanel.PanelName),
                Direction = ListSortDirection.Ascending,
            });

            tickers.Children.SortDescriptions.Add(new SortDescription()
            {
                PropertyName = nameof(Ticker.Title),
                Direction = ListSortDirection.Ascending,
            });

            tags.Children.Filter += (x, y) =>
            {
                var item = y.Item as Tag;
                y.Accepted = item.ParentTagID == Guid.Empty;
            };

            tags.Children.SortDescriptions.Add(new SortDescription()
            {
                PropertyName = nameof(Tag.Name),
                Direction = ListSortDirection.Ascending,
            });

            this.TreeRoot.Add(spells);
            this.TreeRoot.Add(tickers);
            this.TreeRoot.Add(tags);
        }
    }
}
