namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;

    /// <summary>
    /// DQX専用オプション
    /// </summary>
    public partial class ConfigPanel
    {
        private bool DQXOptionTabSelected => this.TabControl.SelectedTab == this.DQXOptionTabPage;
        private Timer DQXOptionTabRefreshTimer = new Timer();

        /// <summary>
        /// オプションのLoad
        /// </summary>
        private void LoadDQXOption()
        {
            this.LoadSettingsDQXOption();

            this.DQXOptionEnabledCheckBox.CheckedChanged += (s, e) =>
            {
                this.DQXPlayerNameTextBox.Enabled = this.DQXOptionEnabledCheckBox.Checked;
                if (!this.DQXPlayerNameTextBox.Enabled)
                {
                    this.DQXPlayerNameTextBox.Clear();
                }
            };

            this.DQXAppleyButton.Click += this.DQXAppleyButton_Click;

            this.DQXOptionTabRefreshTimer.Tick += this.DQXOtionTabRefreshTimer_Tick;
            this.DQXOptionTabRefreshTimer.Interval = 1000;
            this.DQXOptionTabRefreshTimer.Start();
        }

        private void DQXOtionTabRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!this.DQXOptionTabSelected)
            {
                return;
            }

            this.DQXPTMember2TextBox.Text = string.Empty;
            this.DQXPTMember3TextBox.Text = string.Empty;
            this.DQXPTMember4TextBox.Text = string.Empty;
            this.DQXPTMember5TextBox.Text = string.Empty;
            this.DQXPTMember6TextBox.Text = string.Empty;
            this.DQXPTMember7TextBox.Text = string.Empty;
            this.DQXPTMember8TextBox.Text = string.Empty;

            this.DQXPTMember2TextBox.Text = DQXUtility.PartyMemberList.Count > 0 ? DQXUtility.PartyMemberList[0] : string.Empty;
            this.DQXPTMember3TextBox.Text = DQXUtility.PartyMemberList.Count > 1 ? DQXUtility.PartyMemberList[1] : string.Empty;
            this.DQXPTMember4TextBox.Text = DQXUtility.PartyMemberList.Count > 2 ? DQXUtility.PartyMemberList[2] : string.Empty;
            this.DQXPTMember5TextBox.Text = DQXUtility.PartyMemberList.Count > 3 ? DQXUtility.PartyMemberList[3] : string.Empty;
            this.DQXPTMember6TextBox.Text = DQXUtility.PartyMemberList.Count > 4 ? DQXUtility.PartyMemberList[4] : string.Empty;
            this.DQXPTMember7TextBox.Text = DQXUtility.PartyMemberList.Count > 5 ? DQXUtility.PartyMemberList[5] : string.Empty;
            this.DQXPTMember8TextBox.Text = DQXUtility.PartyMemberList.Count > 6 ? DQXUtility.PartyMemberList[6] : string.Empty;
        }

        private void DQXAppleyButton_Click(object sender, EventArgs e)
        {
            this.SaveSettingsDQXOption();

            // 現在の設定を無効にする
            SpellTimerCore.Default.InvalidateSettings();

            // Windowを一旦すべて閉じる
            SpellTimerCore.Default.ClosePanels();
            OnePointTelopController.CloseTelops();
        }

        private void SaveSettingsDQXOption()
        {
            Settings.Default.DQXPlayerName = this.DQXPlayerNameTextBox.Text;
            Settings.Default.DQXUtilityEnabled = this.DQXOptionEnabledCheckBox.Checked;

            // プレイヤー名をユーティリティに反映させる
            DQXUtility.PlayerName = Settings.Default.DQXPlayerName;
            DQXUtility.RefeshKeywords();

            // 設定を保存する
            Settings.Default.Save();
        }

        private void LoadSettingsDQXOption()
        {
            this.DQXOptionEnabledCheckBox.Checked = Settings.Default.DQXUtilityEnabled;
            this.DQXPlayerNameTextBox.Text = Settings.Default.DQXPlayerName;

            this.DQXPlayerNameTextBox.Enabled = this.DQXOptionEnabledCheckBox.Checked;
            if (!this.DQXPlayerNameTextBox.Enabled)
            {
                this.DQXPlayerNameTextBox.Clear();
            }
        }
    }
}
