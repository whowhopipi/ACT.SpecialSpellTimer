using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;

namespace ACT.SpecialSpellTimer.Forms
{
    public partial class ConfigPanelNameStyle :
        UserControl
    {
        #region Singleton

        private static ConfigPanelNameStyle instance = new ConfigPanelNameStyle();

        public static ConfigPanelNameStyle Instance =>
            !instance.IsDisposed ? instance : (instance = new ConfigPanelNameStyle());

        #endregion Singleton

        private Dictionary<NameStyles, RadioButton> FFXIVLogStyleRadioButtons;
        private Dictionary<NameStyles, RadioButton> OverlayDisplayStyleRadioButtons;

        public ConfigPanelNameStyle()
        {
            this.InitializeComponent();

            this.FFXIVLogStyleRadioButtons = new Dictionary<NameStyles, RadioButton>()
            {
                { NameStyles.FullName, this.FFXIVLogStyle1RadioButton },
                { NameStyles.FullInitial, this.FFXIVLogStyle2RadioButton },
                { NameStyles.InitialFull, this.FFXIVLogStyle3RadioButton },
                { NameStyles.InitialInitial, this.FFXIVLogStyle4RadioButton },
            };

            this.OverlayDisplayStyleRadioButtons = new Dictionary<NameStyles, RadioButton>()
            {
                { NameStyles.FullName, this.OverlayDisplayStyle1RadioButton },
                { NameStyles.FullInitial, this.OverlayDisplayStyle2RadioButton },
                { NameStyles.InitialFull, this.OverlayDisplayStyle3RadioButton },
                { NameStyles.InitialInitial, this.OverlayDisplayStyle4RadioButton },
            };

            this.Load += this.ConfigPanelNameStyle_Load;
        }

        private async void ApplyButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.ApplyButton.Enabled = false;
                Application.DoEvents();

                await Task.Run(() =>
                {
                    this.SaveConfig();

                    TableCompiler.Instance.RefreshPlayerPlacceholder();
                    TableCompiler.Instance.RefreshPartyPlaceholders();
                    TableCompiler.Instance.RecompileSpells();
                    TableCompiler.Instance.RecompileTickers();
                });
            }
            finally
            {
                this.ApplyButton.Enabled = true;
            }
        }

        private void ConfigPanelNameStyle_Load(object sender, EventArgs e)
        {
            this.LoadConfig();
        }

        private void LoadConfig()
        {
            this.FFXIVLogStyleRadioButtons[Settings.Default.PCNameInitialOnLogStyle].Checked = true;
            this.OverlayDisplayStyleRadioButtons[Settings.Default.PCNameInitialOnDisplayStyle].Checked = true;
        }

        private void SaveConfig()
        {
            var logStyle = this.FFXIVLogStyleRadioButtons
                .Where(x => x.Value.Checked)
                .Select(x => x.Key)
                .FirstOrDefault();

            var displayStyle = this.OverlayDisplayStyleRadioButtons
                .Where(x => x.Value.Checked)
                .Select(x => x.Key)
                .FirstOrDefault();

            Settings.Default.PCNameInitialOnLogStyle = logStyle;
            Settings.Default.PCNameInitialOnDisplayStyle = displayStyle;

            Settings.Default.Save();
        }
    }
}
