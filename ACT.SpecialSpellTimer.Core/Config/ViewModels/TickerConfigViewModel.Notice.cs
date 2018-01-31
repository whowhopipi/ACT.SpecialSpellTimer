using System;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.Sound;
using Prism.Commands;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public partial class TickerConfigViewModel
    {
        public static SoundController.WaveFile[] WaveList => SoundController.Instance.EnumlateWave();

        private ICommand CreateTestWaveCommand(
            Func<string> getWave,
            AdvancedNoticeConfig noticeConfig)
            => new DelegateCommand(()
                => noticeConfig.PlayWave(getWave()));

        private ICommand CreateTestTTSCommand(
            Func<string> getTTS,
            AdvancedNoticeConfig noticeConfig)
            => new DelegateCommand(()
                => noticeConfig.Speak(getTTS()));

        private ICommand testWave1Command;
        private ICommand testWave2Command;

        public ICommand TestWave1Command =>
            this.testWave1Command ?? (this.testWave1Command = this.CreateTestWaveCommand(
                () => this.Model.MatchSound,
                this.Model.MatchAdvancedConfig));

        public ICommand TestWave2Command =>
            this.testWave1Command ?? (this.testWave2Command = this.CreateTestWaveCommand(
                () => this.Model.DelaySound,
                this.Model.DelayAdvancedConfig));

        private ICommand testTTS1Command;
        private ICommand testTTS2Command;

        public ICommand TestTTS1Command =>
            this.testTTS1Command ?? (this.testTTS1Command = this.CreateTestTTSCommand(
                () => this.Model.MatchTextToSpeak,
                this.Model.MatchAdvancedConfig));

        public ICommand TestTTS2Command =>
            this.testTTS1Command ?? (this.testTTS2Command = this.CreateTestTTSCommand(
                () => this.Model.DelayTextToSpeak,
                this.Model.DelayAdvancedConfig));

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
