using System.Windows.Data;

namespace ACT.SpecialSpellTimer.Models
{
    public interface ITreeItem
    {
        string DisplayText { get; }

        bool IsExpanded { get; set; }

        bool IsEnabled { get; set; }

        CollectionViewSource Children { get; }
    }
}
