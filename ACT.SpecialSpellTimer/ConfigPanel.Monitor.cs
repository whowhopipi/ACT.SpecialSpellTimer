using System;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using ACT.SpecialSpellTimer.Models;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// Configパネル モニター
    /// </summary>
    public partial class ConfigPanel
    {
        private StringBuilder logBuffer = new StringBuilder();
        private Timer updatePlaceholderTimer = new Timer();
        private bool LogTabSelected => this.TabControl.SelectedTab == this.LogTabPage;

        /// <summary>
        /// ログを追加する
        /// </summary>
        /// <param name="text">追加するテキスト</param>
        public void AppendLog(
            string text)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.LogTextBox.AppendText(text + Environment.NewLine);
            });

            this.logBuffer.AppendLine(text);
        }

        /// <summary>
        /// ログタブ用のロード
        /// </summary>
        public void LoadLogTab()
        {
            this.UpdatePlaceholder();

            this.updatePlaceholderTimer.Interval = 8000;
            this.updatePlaceholderTimer.Tick += (s, e) =>
            {
                if (!LogTabSelected)
                {
                    return;
                }

                this.UpdatePlaceholder();
            };

            this.updatePlaceholderTimer.Start();

            if (string.IsNullOrEmpty(this.LogTextBox.Text) &&
                this.logBuffer.Length > 0)
            {
                this.LogTextBox.AppendText(this.logBuffer.ToString());
            }
        }

        public void UpdatePlaceholder()
        {
            var placeholders = TableCompiler.Instance.PlaceholderList;

            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    this.PlaceholderListView.Visible = false;
                    this.PlaceholderListView.SuspendLayout();

                    this.PlaceholderListView.Items.Clear();

                    foreach (var ph in placeholders
                        .OrderBy(x =>x.Type))
                    {
                        var values = new string[]
                        {
                            ph.Placeholder,
                            ph.ReplaceString,
                            Enum.GetName(typeof(PlaceholderTypes), ph.Type)
                        };

                        this.PlaceholderListView.Items.Add(new ListViewItem(values));
                    }

                    if (placeholders.Count > 0)
                    {
                        for (int i = 0; i < this.PlaceholderListView.Columns.Count; i++)
                        {
                            this.CombatLogListView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                    }
                }
                finally
                {
                    this.PlaceholderListView.ResumeLayout();
                    this.PlaceholderListView.Visible = true;
                }
            });
        }
    }
}
