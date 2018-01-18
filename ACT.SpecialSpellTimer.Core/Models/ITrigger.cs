namespace ACT.SpecialSpellTimer.Models
{
    public interface ITrigger
    {
        ItemTypes ItemType { get; }

        void MatchTrigger(string logLine);
    }
}
