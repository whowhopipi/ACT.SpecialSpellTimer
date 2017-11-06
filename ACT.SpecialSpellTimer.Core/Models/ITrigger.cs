namespace ACT.SpecialSpellTimer.Models
{
    public interface ITrigger
    {
        void MatchTrigger(string logLine);
    }

    public static class ITriggerExtensions
    {
        public static void MatchTriggerCore(
            this ITrigger trigger,
            string logLine)
        {
            switch (trigger)
            {
                case OnePointTelop telop:
                    TickersController.Instance.MatchCore(telop, logLine);
                    break;

                case SpellTimer spell:
                    SpellsController.Instance.MatchCore(spell, logLine);
                    break;

                default:
                    break;
            }
        }
    }
}
