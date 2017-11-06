namespace ACT.SpecialSpellTimer.Models
{
    public interface ITrigger
    {
        void MatchTrigger(string logLine);
    }
}
