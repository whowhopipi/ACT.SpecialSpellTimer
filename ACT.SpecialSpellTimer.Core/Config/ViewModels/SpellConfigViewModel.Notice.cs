using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.Sound;
using Prism.Commands;
using System;
using System.Windows.Input;

namespace ACT.SpecialSpellTimer.Config.ViewModels
{
    public partial class SpellConfigViewModel
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
        private ICommand testWave3Command;
        private ICommand testWave4Command;

        public ICommand TestWave1Command =>
            this.testWave1Command ?? (this.testWave1Command = this.CreateTestWaveCommand(
                () => this.Model.MatchSound,
                this.Model.MatchAdvancedConfig));

        public ICommand TestWave2Command =>
            this.testWave1Command ?? (this.testWave2Command = this.CreateTestWaveCommand(
                () => this.Model.OverSound,
                this.Model.OverAdvancedConfig));

        public ICommand TestWave3Command =>
            this.testWave1Command ?? (this.testWave3Command = this.CreateTestWaveCommand(
                () => this.Model.BeforeSound,
                this.Model.BeforeAdvancedConfig));

        public ICommand TestWave4Command =>
            this.testWave1Command ?? (this.testWave4Command = this.CreateTestWaveCommand(
                () => this.Model.TimeupSound,
                this.Model.TimeupAdvancedConfig));

        private ICommand testTTS1Command;
        private ICommand testTTS2Command;
        private ICommand testTTS3Command;
        private ICommand testTTS4Command;

        public ICommand TestTTS1Command =>
            this.testTTS1Command ?? (this.testTTS1Command = this.CreateTestTTSCommand(
                () => this.Model.MatchTextToSpeak,
                this.Model.MatchAdvancedConfig));

        public ICommand TestTTS2Command =>
            this.testTTS1Command ?? (this.testTTS2Command = this.CreateTestTTSCommand(
                () => this.Model.OverTextToSpeak,
                this.Model.OverAdvancedConfig));

        public ICommand TestTTS3Command =>
            this.testTTS1Command ?? (this.testTTS3Command = this.CreateTestTTSCommand(
                () => this.Model.BeforeTextToSpeak,
                this.Model.BeforeAdvancedConfig));

        public ICommand TestTTS4Command =>
            this.testTTS1Command ?? (this.testTTS4Command = this.CreateTestTTSCommand(
                () => this.Model.TimeupTextToSpeak,
                this.Model.TimeupAdvancedConfig));

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
