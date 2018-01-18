using System.Collections.Generic;

namespace ACT.SpecialSpellTimer.Models
{
    public interface ITreeItem
    {
        string DisplayText { get; }

        bool IsExpanded { get; set; }

        bool IsEnabled { get; set; }

        IReadOnlyList<ITreeItem> Children { get; }
    }
}
