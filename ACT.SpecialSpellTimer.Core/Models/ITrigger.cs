namespace ACT.SpecialSpellTimer.Models
{
    public enum TriggerTypes
    {
        Spell,
        Ticker
    }

    public interface ITrigger
    {
        TriggerTypes TriggerType { get; }

        void MatchTrigger(string logLine);
    }
}
