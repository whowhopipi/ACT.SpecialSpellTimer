namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Configパネル モニター
    /// </summary>
    public partial class ConfigPanel
    {
        private string playerName;
        private IReadOnlyList<string> partyMemberNames;
        private IReadOnlyDictionary<string, string> jobPlaceholders;
        private StringBuilder logBuffer = new StringBuilder();

        private const int VALID = 0;
        private const int INVALID = 1;
        private int placeholderIsValid = VALID;

        /// <summary>
        /// モニタタブ用のロード
        /// </summary>
        public void LoadMonitorTab()
        {
            this.RefreshPlaceholders(
                this.playerName,
                this.partyMemberNames,
                this.jobPlaceholders);

            if (string.IsNullOrWhiteSpace(this.LogTextBox.Text) &&
                this.logBuffer.Length > 0)
            {
                this.LogTextBox.AppendText(this.logBuffer.ToString());
            }
        }

        private bool MonitorTabSelected => this.TabControl.SelectedTab == this.tabPage3;

        /// <summary>
        /// ログを追加する
        /// </summary>
        /// <param name="text">追加するテキスト</param>
        public void AppendLog(
            string text)
        {
            this.logBuffer.AppendLine(text);
            this.LogTextBox.AppendText(text + Environment.NewLine);
        }

        public void InvalidatePlaceholders()
        {
            placeholderIsValid = INVALID;
        }

        public void UpdateMonitor()
        {
            if (!MonitorTabSelected)
            {
                InvalidatePlaceholders();
                return;
            }

            if (Interlocked.CompareExchange(ref placeholderIsValid, VALID, INVALID) != INVALID)
            {
                return;
            }

            var player = FF14PluginHelper.GetPlayer();
            RefreshPlaceholders(
                player != null ? player.Name : "",
                LogBuffer.PartyList,
                LogBuffer.PlaceholderToJobNameDictionaly);
        }

        /// <summary>
        /// プレースホルダの表示を更新する
        /// </summary>
        /// <param name="playerName">プレイヤー名</param>
        /// <param name="partyMemberNames">パーティメンバーの名前リスト</param>
        /// <param name="jobPlaceholders">ジョブ名プレースホルダのリスト</param>
        private void RefreshPlaceholders(
            string playerName,
            IReadOnlyList<string> partyMemberNames,
            IReadOnlyDictionary<string, string> jobPlaceholders)
        {
            // 一旦クリアする
            this.MeTextBox.Clear();
            this.Member2TextBox.Clear();
            this.Member3TextBox.Clear();
            this.Member4TextBox.Clear();
            this.Member5TextBox.Clear();
            this.Member6TextBox.Clear();
            this.Member7TextBox.Clear();
            this.Member8TextBox.Clear();
            this.PLDTextBox.Clear();
            this.WARTextBox.Clear();
            this.DRKTextBox.Clear();
            this.WHMTextBox.Clear();
            this.SCHTextBox.Clear();
            this.ASTTextBox.Clear();
            this.MNKTextBox.Clear();
            this.DRGTextBox.Clear();
            this.NINTextBox.Clear();
            this.DRGTextBox.Clear();
            this.BRDTextBox.Clear();
            this.MCHTextBox.Clear();
            this.BLMTextBox.Clear();
            this.SMNTextBox.Clear();

            this.MeTextBox.Text = playerName;

            if (partyMemberNames != null)
            {
                this.Member2TextBox.Text = partyMemberNames.Count > 0 ? partyMemberNames[0] : string.Empty;
                this.Member3TextBox.Text = partyMemberNames.Count > 1 ? partyMemberNames[1] : string.Empty;
                this.Member4TextBox.Text = partyMemberNames.Count > 2 ? partyMemberNames[2] : string.Empty;
                this.Member5TextBox.Text = partyMemberNames.Count > 3 ? partyMemberNames[3] : string.Empty;
                this.Member6TextBox.Text = partyMemberNames.Count > 4 ? partyMemberNames[4] : string.Empty;
                this.Member7TextBox.Text = partyMemberNames.Count > 5 ? partyMemberNames[5] : string.Empty;
                this.Member8TextBox.Text = partyMemberNames.Count > 6 ? partyMemberNames[6] : string.Empty;
            }

            if (jobPlaceholders != null)
            {
                this.PLDTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("PLD")).ToArray());
                this.WARTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("WAR")).ToArray());
                this.DRKTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("DRK")).ToArray());

                this.WHMTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("WHM")).ToArray());
                this.SCHTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("SCH")).ToArray());
                this.ASTTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("AST")).ToArray());

                this.MNKTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("MNK")).ToArray());
                this.DRGTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("DRG")).ToArray());
                this.NINTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("NIN")).ToArray());

                this.BRDTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("BRD")).ToArray());
                this.MCHTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("MCH")).ToArray());

                this.BLMTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("BLM")).ToArray());
                this.SMNTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.Contains("SMN")).ToArray());
            }

            this.playerName = playerName;
            this.partyMemberNames = partyMemberNames;
            this.jobPlaceholders = jobPlaceholders;
        }
    }
}
