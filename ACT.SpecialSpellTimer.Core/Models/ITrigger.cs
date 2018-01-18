using System;
using System.Collections.ObjectModel;

namespace ACT.SpecialSpellTimer.Models
{
    public enum TriggerTypes
    {
        Spell,
        Ticker,
        SpellPanel
    }

    public interface ITrigger
    {
        TriggerTypes TriggerType { get; }

        void MatchTrigger(string logLine);

        ObservableCollection<Guid> Tags { get; }
    }
}
