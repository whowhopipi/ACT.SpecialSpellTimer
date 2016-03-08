namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Configパネル モニター
    /// </summary>
    public partial class ConfigPanel
    {
        private string playerName;
        private List<string> partyMemberNames;
        private List<KeyValuePair<string, string>> jobPlaceholders;

        /// <summary>
        /// モニタタブ用のロード
        /// </summary>
        public void LoadMonitorTab()
        {
            this.RefreshPlaceholders(
                this.playerName,
                this.partyMemberNames,
                this.jobPlaceholders);
        }

        /// <summary>
        /// ログを追加する
        /// </summary>
        /// <param name="text">追加するテキスト</param>
        public void AppendLog(
            string text)
        {
            this.LogTextBox.AppendText(text + Environment.NewLine);
        }

        /// <summary>
        /// プレースホルダの表示を更新する
        /// </summary>
        /// <param name="playerName">プレイヤー名</param>
        /// <param name="partyMemberNames">パーティメンバーの名前リスト</param>
        /// <param name="jobPlaceholders">ジョブ名プレースホルダのリスト</param>
        public void RefreshPlaceholders(
            string playerName,
            List<string> partyMemberNames,
            List< KeyValuePair<string, string>> jobPlaceholders)
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
                    jobPlaceholders.Where(x => x.Key.StartsWith("PLD")).ToArray());
                this.WARTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("WAR")).ToArray());
                this.DRKTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("DRK")).ToArray());

                this.WHMTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("WHM")).ToArray());
                this.SCHTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("SCH")).ToArray());
                this.ASTTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("AST")).ToArray());

                this.MNKTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("MNK")).ToArray());
                this.DRGTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("DRG")).ToArray());
                this.NINTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("NIN")).ToArray());

                this.BRDTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("BRD")).ToArray());
                this.MCHTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("MCH")).ToArray());

                this.BLMTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("BLM")).ToArray());
                this.SMNTextBox.Text = string.Join(
                    Environment.NewLine,
                    jobPlaceholders.Where(x => x.Key.StartsWith("SMN")).ToArray());
            }

            this.playerName = playerName;
            this.partyMemberNames = partyMemberNames;
            this.jobPlaceholders = jobPlaceholders;
        }
    }
}
