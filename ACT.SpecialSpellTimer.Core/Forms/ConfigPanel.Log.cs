using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ACT.SpecialSpellTimer.Models;

namespace ACT.SpecialSpellTimer.Forms
{
    public partial class ConfigPanelLog :
        UserControl
    {
        #region Singleton

        private static ConfigPanelLog instance = new ConfigPanelLog();

        public static ConfigPanelLog Instance =>
            !instance.IsDisposed ? instance : (instance = new ConfigPanelLog());

        #endregion Singleton

        private volatile StringBuilder logBuffer = new StringBuilder();
        private Timer updatePlaceholderTimer = new Timer();

        public ConfigPanelLog()
        {
            this.InitializeComponent();

            typeof(ListView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(this.PlaceholderListView, true, null);
            typeof(ListView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(this.SpellsListView, true, null);

            this.Load += (s, e) => this.LoadLogTab();
        }

        public bool IsLogTabActive => this.IsLogTabActiveDelegate?.Invoke() ?? false;
        public Func<bool> IsLogTabActiveDelegate { get; set; }

        /// <summary>
        /// ログを追加する
        /// </summary>
        /// <param name="text">追加するテキスト</param>
        public void AppendLog(
            string text)
        {
            void appendLog()
            {
                this.LogTextBox.AppendText(text);
            }

            this.logBuffer.Append(text);

            if (this.LogTextBox.IsDisposed ||
                !this.LogTextBox.IsHandleCreated)
            {
                return;
            }

            try
            {
                if (this.LogTextBox.InvokeRequired)
                {
                    this.LogTextBox.Invoke((Action)appendLog);
                }
                else
                {
                    appendLog();
                }
            }
            catch (Exception)
            {
            }
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
                if (!this.IsLogTabActive)
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

            this.UpdateSpells();
            TableCompiler.Instance.OnTableChanged -= this.TableCompilerOnTableChanged;
            TableCompiler.Instance.OnTableChanged += this.TableCompilerOnTableChanged;
        }

        public async void UpdatePlaceholder()
        {
            if (this.IsDisposed ||
                !this.IsHandleCreated)
            {
                return;
            }

            var listItems = new List<ListViewItem>();

            await Task.Run(() =>
            {
                var placeholders = TableCompiler.Instance.PlaceholderList;
                foreach (var ph in placeholders
                    .OrderBy(x => x.Type))
                {
                    var values = new string[]
                    {
                        ph.Placeholder,
                        ph.ReplaceString,
                        Enum.GetName(typeof(PlaceholderTypes), ph.Type)
                    };

                    listItems.Add(new ListViewItem(values));
                }
            });

            void refresh()
            {
                try
                {
                    this.PlaceholderListView.SuspendLayout();

                    this.PlaceholderListView.Items.Clear();
                    this.PlaceholderListView.Items.AddRange(listItems.ToArray());

                    if (this.PlaceholderListView.Items.Count > 0)
                    {
                        for (int i = 0; i < this.PlaceholderListView.Columns.Count; i++)
                        {
                            this.PlaceholderListView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                    }
                }
                finally
                {
                    this.PlaceholderListView.ResumeLayout();
                }
            }

            if (this.InvokeRequired)
            {
                this.Invoke((Action)refresh);
            }
            else
            {
                refresh();
            }
        }

        private void TableCompilerOnTableChanged(
            object sender,
            EventArgs e) => this.UpdateSpells();

        private async void UpdateSpells()
        {
            if (this.IsDisposed ||
                !this.IsHandleCreated)
            {
                return;
            }

            var listItems = new List<ListViewItem>();

            await Task.Run(() =>
            {
                var spells =
                    from s in TableCompiler.Instance.SpellList
                    where
                    !s.IsInstance
                    orderby
                    s.Panel,
                    s.DisplayNo,
                    s.ID
                    select
                    s;

                foreach (var x in spells)
                {
                    var values = new string[]
                    {
                        "Spell",
                        x.SpellTitle,
                        !x.RegexEnabled ? x.KeywordReplaced : x.RegexPattern,
                        x.RegexEnabled.ToString()
                    };

                    listItems.Add(new ListViewItem(values));
                }

                var tickers = TableCompiler.Instance.TickerList;

                foreach (var x in tickers)
                {
                    var values = new string[]
                    {
                        "Ticker",
                        x.Title,
                        !x.RegexEnabled ? x.KeywordReplaced : x.RegexPattern,
                        x.RegexEnabled.ToString()
                    };

                    listItems.Add(new ListViewItem(values));
                }
            });

            void refresh()
            {
                try
                {
                    this.SpellsListView.SuspendLayout();

                    this.SpellsListView.Items.Clear();
                    this.SpellsListView.Items.AddRange(listItems.ToArray());

                    if (this.SpellsListView.Items.Count > 0)
                    {
                        for (int i = 0; i < this.SpellsListView.Columns.Count; i++)
                        {
                            this.SpellsListView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                    }
                }
                finally
                {
                    this.SpellsListView.ResumeLayout();
                }
            }

            if (this.InvokeRequired)
            {
                this.Invoke((Action)refresh);
            }
            else
            {
                refresh();
            }
        }
    }
}
