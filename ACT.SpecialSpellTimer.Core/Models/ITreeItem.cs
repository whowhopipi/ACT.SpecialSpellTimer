using System.ComponentModel;

namespace ACT.SpecialSpellTimer.Models
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
}
