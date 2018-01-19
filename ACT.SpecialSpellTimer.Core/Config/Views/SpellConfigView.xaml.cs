using System.Windows.Controls;
using ACT.SpecialSpellTimer.Config.ViewModels;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// SpellConfigView.xaml の相互作用ロジック
    /// </summary>
    public partial class SpellConfigView : UserControl, ILocalizable
    {
        public SpellConfigView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public SpellConfigViewModel ViewModel => this.DataContext as SpellConfigViewModel;

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);
    }
}
