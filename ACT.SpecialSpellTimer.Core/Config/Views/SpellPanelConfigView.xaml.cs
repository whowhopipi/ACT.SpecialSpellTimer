using System.Windows.Controls;
using ACT.SpecialSpellTimer.Config.ViewModels;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// SpellPanelConfigView.xaml の相互作用ロジック
    /// </summary>
    public partial class SpellPanelConfigView : UserControl, ILocalizable
    {
        public SpellPanelConfigView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public SpellPanelConfigViewModel ViewModel => this.DataContext as SpellPanelConfigViewModel;

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);
    }
}
