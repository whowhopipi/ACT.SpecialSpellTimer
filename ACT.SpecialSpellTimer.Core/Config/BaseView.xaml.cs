using System.Windows.Controls;

namespace ACT.SpecialSpellTimer.Config
{
    /// <summary>
    /// BaseView.xaml の相互作用ロジック
    /// </summary>
    public partial class BaseView : UserControl
    {
        /* ■OLD_UI
        public ConfigPanel ConfigPanel { get; private set; } = new ConfigPanel()
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            AutoScaleMode = Settings.Default.UIAutoScaleMode,
        };
        */

        public BaseView(
            System.Drawing.Font font = null)
        {
            this.InitializeComponent();

            /* ■OLD_UI
            // Windows FormのConfigパネルをセットする
            if (font != null)
            {
                this.ConfigPanel.Font = font;
            }

            this.WindowsFormsHost.Child = this.ConfigPanel;
            */

            // HelpViewを設定する
            this.HelpView.SetLocale(Settings.Default.UILocale);

            this.HelpView.ViewModel.ConfigFile = Settings.Default.FileName;
            this.HelpView.ViewModel.ReloadConfigAction = null;
        }
    }
}
