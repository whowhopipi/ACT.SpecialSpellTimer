namespace ACT.SpecialSpellTimer.Models
{
    public interface ITrigger
    {
        ItemTypes TriggerType { get; }

        void MatchTrigger(string logLine);
    }
}
