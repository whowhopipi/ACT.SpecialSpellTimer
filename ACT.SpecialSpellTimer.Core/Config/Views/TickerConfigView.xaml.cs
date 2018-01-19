using System.Windows.Controls;
using ACT.SpecialSpellTimer.Config.ViewModels;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TickerConfigView.xaml の相互作用ロジック
    /// </summary>
    public partial class TickerConfigView : UserControl, ILocalizable
    {
        public TickerConfigView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public TickerConfigViewModel ViewModel => this.DataContext as TickerConfigViewModel;

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);
    }
}
