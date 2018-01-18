using System.Windows.Controls;
using ACT.SpecialSpellTimer.Config.ViewModels;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TriggersView.xaml の相互作用ロジック
    /// </summary>
    public partial class TriggersView : UserControl, ILocalizable
    {
        public TriggersView()
        {
            this.InitializeComponent();
            this.DataContext = new TriggersViewModel();
            this.SetLocale(Settings.Default.UILocale);
        }

        public TriggersViewModel ViewModel => this.DataContext as TriggersViewModel;

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);
    }
}
