using System.Windows.Forms;

namespace ACT.SpecialSpellTimer
{
    public partial class ConfigPanelNameStyle :
        UserControl
    {
        #region Singleton

        private static ConfigPanelNameStyle instance = new ConfigPanelNameStyle();

        public static ConfigPanelNameStyle Instance => instance;

        #endregion Singleton

        public ConfigPanelNameStyle()
        {
            this.InitializeComponent();
        }
    }
}
