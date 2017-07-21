namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Models;

    /// <summary>
    /// Configパネル モニター
    /// </summary>
    public partial class ConfigPanel
    {
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

                    foreach (var ph in placeholders)
                    {
                        var values = new string[]
                        {
                        ph.Placeholder,
                        ph.ReplaceString,
                        Enum.GetName(typeof(PlaceholderTypes), ph.Type)
                        };

                        this.PlaceholderListView.Items.Add(new ListViewItem(values));
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
