using ACT.SpecialSpellTimer.Sound;
using Prism.Commands;
using System.Windows.Input;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public partial class SpellConfigViewModel
    {
        public static SoundController.WaveFile[] WaveList => SoundController.Instance.EnumlateWave();

        private ICommand testSequencialTTSCommand;

        public ICommand TestSequencialTTSCommand =>
            this.testSequencialTTSCommand ?? (this.testSequencialTTSCommand = new DelegateCommand(() =>
            {
                this.Model.Play("おしらせ1");
                this.Model.Play("おしらせ2");
                this.Model.Play("おしらせ3");
                this.Model.Play("おしらせ4");
            }));
    }
}
