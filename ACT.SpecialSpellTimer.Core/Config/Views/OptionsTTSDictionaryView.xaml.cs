using System.Windows.Controls;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// OptionsTTSDictionaryView.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionsTTSDictionaryView :
        UserControl,
        ILocalizable
    {
        public OptionsTTSDictionaryView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);
    }
}
