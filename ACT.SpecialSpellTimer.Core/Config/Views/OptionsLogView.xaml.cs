using System.Windows.Controls;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// OptionsLogView.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionsLogView :
        UserControl,
        ILocalizable
    {
        public OptionsLogView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);
    }
}
