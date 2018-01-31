using ACT.SpecialSpellTimer.Sound;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public partial class SpellConfigViewModel
    {
        public static SoundController.WaveFile[] WaveList => SoundController.Instance.EnumlateWave();
    }
}
