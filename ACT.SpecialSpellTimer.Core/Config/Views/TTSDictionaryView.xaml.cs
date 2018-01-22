using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Sound;
using Prism.Commands;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TTSDictionaryView.xaml の相互作用ロジック
    /// </summary>
    public partial class TTSDictionaryView : UserControl
    {
        public TTSDictionaryView()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<TTSDictionary.PCPhonetic> PartyList => TTSDictionary.Instance.Phonetics;

        private ICommand testPhoneticsCommand;

        public ICommand TestPhoneticsCommand =>
            this.testPhoneticsCommand ?? (this.testPhoneticsCommand = new DelegateCommand(() =>
            {
                var tts = string.Empty;
                foreach (var pc in this.PartyList.Where(x => !string.IsNullOrEmpty(x.Phonetic)))
                {
                    tts += $"{pc.JobID.ToString()}、頭文字{pc.Name.Substring(0, 1)}の読み仮名は、{pc.Name}です。" + Environment.NewLine;
                }

                SoundController.Instance.Play(tts);
            }));

        private ICommand openTTSDictionaryCommand;

        public ICommand OpenTTSDictionaryCommand =>
            this.openTTSDictionaryCommand ?? (this.openTTSDictionaryCommand = new DelegateCommand(() =>
            {
                var file = TTSDictionary.Instance.SourceFile;
                if (File.Exists(file))
                {
                    Process.Start(file);
                }
            }));

        private ICommand reloadTTSDictinaryCommand;

        public ICommand ReloadTTSDictinaryCommand =>
            this.reloadTTSDictinaryCommand ?? (this.reloadTTSDictinaryCommand = new DelegateCommand(() =>
            {
                TTSDictionary.Instance.Load();

                MessageBox.Show(
                    "TTSDictionary.txt is reloaded.",
                    "ACT.Hojoring",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }));
    }
}
