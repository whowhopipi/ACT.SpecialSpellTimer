using System.ComponentModel;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Models;
using Prism.Commands;

namespace ACT.SpecialSpellTimer.Config.Models
{
    public enum ItemTypes
    {
        Root,
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

    public static class TreeItemCommands
    {
        private static ICommand createNewCommand;

        public static ICommand CreateNewCommand =>
            createNewCommand ?? (createNewCommand = new DelegateCommand<ITreeItem>(item =>
            {
                switch (item)
                {
                    case SpellPanel spellPanel:
                        break;

                    case Spell spell:
                        break;

                    case Ticker ticker:
                        break;

                    case Tag tag:
                        TagTable.Instance.AddNew(tag).Name = "New Tag";
                        break;

                    default:
                        switch (item.DisplayText)
                        {
                            case "All Spells":
                                break;

                            case "All Tickers":
                                break;

                            case "Tags":
                                TagTable.Instance.AddNew().Name = "New Tag";
                                break;
                        }

                        break;
                }
            }));
    }
}
