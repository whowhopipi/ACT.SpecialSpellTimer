using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Sound;
using Prism.Commands;
using static ACT.SpecialSpellTimer.Sound.TTSDictionary;

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

        public ObservableCollection<PCPhonetic> PartyList => TTSDictionary.Instance.Phonetics;

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
            }));
    }
}
