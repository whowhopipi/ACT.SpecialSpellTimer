using System.Windows.Controls;
using ACT.SpecialSpellTimer.Forms;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config
{
    /// <summary>
    /// BaseView.xaml の相互作用ロジック
    /// </summary>
    public partial class BaseView : UserControl
    {
        public ConfigPanel ConfigPanel { get; private set; } = new ConfigPanel()
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            AutoScaleMode = Settings.Default.UIAutoScaleMode,
        };

        public BaseView(
            System.Drawing.Font font = null)
        {
            this.InitializeComponent();

            // Windows FormのConfigパネルをセットする
            if (font != null)
            {
                this.ConfigPanel.Font = font;
            }

            this.WindowsFormsHost.Child = this.ConfigPanel;

            // HelpViewを設定する
            this.HelpView.SetLocale(
                Settings.Default.Language == "JP" ?
                    Locales.JA :
                    Locales.EN);

            this.HelpView.ViewModel.ConfigFile = Settings.Default.FileName;
            this.HelpView.ViewModel.ReloadConfigAction = null;
        }
    }
}
