using System.Windows.Controls;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// OptionsMiscView.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionsMiscView :
        UserControl,
        ILocalizable
    {
        public OptionsMiscView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);
    }
}
