namespace ACT.SpecialSpellTimer.Forms
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Config;
    using ACT.SpecialSpellTimer.Models;
    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;
    using FFXIV.Framework.Extensions;

    /// <summary>
    /// Configパネル ワンポイントテロップ
    /// </summary>
    public partial class ConfigPanel
    {
        /// <summary>
        /// テロップテーブルをツリーにロードする
        /// </summary>
        public void LoadTelopTable()
        {
            try
            {
                this.TelopTreeView.SuspendLayout();

                this.TelopTreeView.Nodes.Clear();

                var telops = OnePointTelopTable.Instance.Table.OrderBy(x => x.Title);
                foreach (var telop in telops)
                {
                    var n = new TreeNode();

                    n.Tag = telop;
                    n.Text = telop.Title;
                    n.ToolTipText = telop.Message;
                    n.Checked = telop.Enabled;

                    this.TelopTreeView.Nodes.Add(n);
                }

                // 標準のスペルタイマーへ変更を反映する
                SpellsController.Instance.ApplyToNormalSpellTimer();

                this.TelopTreeView.ExpandAll();
            }
            finally
            {
                this.TelopTreeView.ResumeLayout();
            }
        }

        /// <summary>
        /// ワンポイントテロップのLoad
        /// </summary>
        private void LoadOnePointTelop()
        {
            // テロップテーブルをロードする
            this.LoadTelopTable();

            this.TelopDetailGroupBox.Visible = false;

            // コンボボックスにアイテムを装填する
            this.TelopMatchSoundComboBox.DataSource = SoundController.Instance.EnumlateWave();
            this.TelopMatchSoundComboBox.ValueMember = "FullPath";
            this.TelopMatchSoundComboBox.DisplayMember = "Name";

            this.TelopDelaySoundComboBox.DataSource = SoundController.Instance.EnumlateWave();
            this.TelopDelaySoundComboBox.ValueMember = "FullPath";
            this.TelopDelaySoundComboBox.DisplayMember = "Name";

            this.TelopPlay1Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play((string)this.TelopMatchSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.TelopPlay2Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play((string)this.TelopDelaySoundComboBox.SelectedValue ?? string.Empty);
            };

            this.TelopSpeak1Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play(this.TelopMatchTTSTextBox.Text);
            };

            this.TelopSpeak2Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play(this.TelopDelayTTSTextBox.Text);
            };

            this.TelopTreeView.AfterCheck += (s1, e1) =>
            {
                var source = e1.Node.Tag as OnePointTelop;
                if (source != null)
                {
                    source.Enabled = e1.Node.Checked;
                }

                // キャッシュを無効にする
                TableCompiler.Instance.RecompileTickers();

                // テロップの有効・無効が変化した際に、標準のスペルタイマーに反映する
                SpellsController.Instance.ApplyToNormalSpellTimer();
            };

            this.TelopTreeView.AfterSelect += (s1, e1) =>
            {
                this.ShowTelopDetail(
                    e1.Node.Tag as OnePointTelop);
            };

            this.TelopSelectJobButton.Click += async (s1, e1) =>
            {
                var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
                if (src != null)
                {
                    using (var f = new SelectJobForm())
                    {
                        f.JobFilter = src.JobFilter;
                        if (await Task.Run(() => f.ShowDialog(this.ParentForm)) ==
                            DialogResult.OK)
                        {
                            src.JobFilter = f.JobFilter;

                            // ジョブ限定ボタンの色を変える（未設定：黒、設定有：青）
                            this.TelopSelectJobButton.ForeColor = src.JobFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;
                        }
                    }
                }
            };

            this.TelopSelectZoneButton.Click += async (s1, e1) =>
            {
                var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
                if (src != null)
                {
                    using (var f = new SelectZoneForm())
                    {
                        f.ZoneFilter = src.ZoneFilter;
                        if (await Task.Run(() => f.ShowDialog(this.ParentForm)) ==
                            DialogResult.OK)
                        {
                            src.ZoneFilter = f.ZoneFilter;

                            // ゾーン限定ボタンの色を変える（未設定：黒、設定有：青）
                            this.TelopSelectZoneButton.ForeColor = src.ZoneFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;
                        }
                    }
                }
            };

            this.TelopSetConditionButton.Click += async (s1, e1) =>
            {
                var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
                if (src != null)
                {
                    using (var f = new SetConditionForm())
                    {
                        f.TimersMustRunning = src.TimersMustRunningForStart;
                        f.TimersMustStopping = src.TimersMustStoppingForStart;
                        if (await Task.Run(() => f.ShowDialog(this.ParentForm)) ==
                            DialogResult.OK)
                        {
                            src.TimersMustRunningForStart = f.TimersMustRunning;
                            src.TimersMustStoppingForStart = f.TimersMustStopping;

                            // 条件設定ボタンの色を変える（未設定：黒、設定有：青）
                            this.TelopSetConditionButton.ForeColor =
                                (src.TimersMustRunningForStart.Length != 0 || src.TimersMustStoppingForStart.Length != 0) ?
                                Color.Blue :
                                Button.DefaultForeColor;
                        }
                    }
                }
            };

            this.TelopExportButton.Click += this.TelopExportButton_Click;
            this.TelopImportButton.Click += this.TelopImportButton_Click;
            this.TelopClearAllButton.Click += this.TelopClearAllButton_Click;
            this.TelopAddButton.Click += this.TelopAddButton_Click;
            this.TelopUpdateButton.Click += this.TelopUpdateButton_Click;
            this.TelopDeleteButton.Click += this.TelopDeleteButton_Click;
        }

        /// <summary>
        /// 詳細を表示する
        /// </summary>
        /// <param name="dataSource"></param>
        private void ShowTelopDetail(
            OnePointTelop dataSource)
        {
            var src = dataSource;
            if (src == null)
            {
                this.TelopDetailGroupBox.Visible = false;
                return;
            }

            this.TelopDetailGroupBox.Visible = true;

            this.TelopTitleTextBox.Text = src.Title;
            this.TelopMessageTextBox.Text = src.Message;
            this.TelopKeywordTextBox.Text = src.Keyword;
            this.TelopKeywordToHideTextBox.Text = src.KeywordToHide;
            this.TelopRegexEnabledCheckBox.Checked = src.RegexEnabled;
            this.TelopDelayNumericUpDown.Value = (decimal)src.Delay;
            this.DisplayTimeNumericUpDown.Value = (decimal)src.DisplayTime;
            this.EnabledAddMessageCheckBox.Checked = src.AddMessageEnabled;
            this.TelopProgressBarEnabledCheckBox.Checked = src.ProgressBarEnabled;

            this.TelopVisualSetting.FontColor = src.FontColor.FromHTML();
            this.TelopVisualSetting.FontOutlineColor = src.FontOutlineColor.FromHTML();
            this.TelopVisualSetting.SetFontInfo(src.Font);
            this.TelopVisualSetting.BackgroundColor = string.IsNullOrWhiteSpace(src.BackgroundColor) ?
                Settings.Default.BackgroundColor :
                Color.FromArgb(src.BackgroundAlpha, src.BackgroundColor.FromHTML());

            this.TelopVisualSetting.RefreshSampleImage();

            var left = (int)src.Left;
            var top = (int)src.Top;

            double x, y;
            TickersController.Instance.GettLocation(
                src.ID,
                out x,
                out y);

            if (x != 0)
            {
                left = (int)x;
            }

            if (y != 0)
            {
                top = (int)y;
            }

            this.TelopLeftNumericUpDown.Value = left;
            this.TelopLeftNumericUpDown.Tag = left;
            this.TelopTopNumericUpDown.Value = top;
            this.TelopTopNumericUpDown.Tag = top;

            this.TelopMatchSoundComboBox.SelectedValue = src.MatchSound;
            this.TelopMatchTTSTextBox.Text = src.MatchTextToSpeak;

            this.TelopDelaySoundComboBox.SelectedValue = src.DelaySound;
            this.TelopDelayTTSTextBox.Text = src.DelayTextToSpeak;

            // データソースをタグに突っ込んでおく
            this.TelopDetailGroupBox.Tag = src;

            // ジョブ限定ボタンの色を変える（未設定：黒、設定有：青）
            this.TelopSelectJobButton.ForeColor = src.JobFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

            // ゾーン限定ボタンの色を変える（未設定：黒、設定有：青）
            this.TelopSelectZoneButton.ForeColor = src.ZoneFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

            // 条件設定ボタンの色を変える（未設定：黒、設定有：青）
            this.TelopSetConditionButton.ForeColor =
                (src.TimersMustRunningForStart.Length != 0 || src.TimersMustStoppingForStart.Length != 0) ?
                Color.Blue :
                Button.DefaultForeColor;
        }

        /// <summary>
        /// テロップ追加 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopAddButton_Click(object sender, EventArgs e)
        {
            var nr = new OnePointTelop();

            nr.ID = OnePointTelopTable.Instance.Table.Any() ?
                OnePointTelopTable.Instance.Table.Max(x => x.ID) + 1 :
                1;
            nr.Guid = Guid.NewGuid();
            nr.Title = Translate.Get("NewTelop");
            nr.DisplayTime = 3;
            nr.FontColor = Settings.Default.FontColor.ToHTML();
            nr.FontOutlineColor = Settings.Default.FontOutlineColor.ToHTML();
            nr.BackgroundColor = Settings.Default.BackgroundColor.ToHTML();
            nr.Left = 10.0d;
            nr.Top = 10.0d;
            nr.JobFilter = string.Empty;
            nr.ZoneFilter = string.Empty;
            nr.TimersMustRunningForStart = new Guid[0];
            nr.TimersMustStoppingForStart = new Guid[0];

            // 現在選択しているノードの情報を一部コピーする
            if (this.TelopTreeView.SelectedNode != null)
            {
                var baseRow = this.TelopTreeView.SelectedNode.Tag != null ?
                    this.TelopTreeView.SelectedNode.Tag as OnePointTelop :
                    this.TelopTreeView.SelectedNode.Nodes[0].Tag as OnePointTelop;

                if (baseRow != null)
                {
                    nr.Title = baseRow.Title + " New";
                    nr.Message = baseRow.Message;
                    nr.Keyword = baseRow.Keyword;
                    nr.KeywordToHide = baseRow.KeywordToHide;
                    nr.RegexEnabled = baseRow.RegexEnabled;
                    nr.Delay = baseRow.Delay;
                    nr.DisplayTime = baseRow.DisplayTime;
                    nr.AddMessageEnabled = baseRow.AddMessageEnabled;
                    nr.ProgressBarEnabled = baseRow.ProgressBarEnabled;
                    nr.FontColor = baseRow.FontColor;
                    nr.FontOutlineColor = baseRow.FontOutlineColor;
                    nr.Font = baseRow.Font;
                    nr.BackgroundColor = baseRow.BackgroundColor;
                    nr.BackgroundAlpha = baseRow.BackgroundAlpha;
                    nr.Left = baseRow.Left;
                    nr.Top = baseRow.Top;
                    nr.JobFilter = baseRow.JobFilter;
                    nr.ZoneFilter = baseRow.ZoneFilter;
                    nr.TimersMustRunningForStart = baseRow.TimersMustRunningForStart;
                    nr.TimersMustStoppingForStart = baseRow.TimersMustStoppingForStart;
                }
            }

            nr.MatchDateTime = DateTime.MinValue;
            nr.Enabled = true;
            nr.Regex = null;
            nr.RegexPattern = string.Empty;
            nr.RegexToHide = null;
            nr.RegexPatternToHide = string.Empty;

            OnePointTelopTable.Instance.Table.Add(nr);

            TableCompiler.Instance.RecompileTickers();
            OnePointTelopTable.Instance.Save(true);

            // 新しいノードを生成する
            var node = new TreeNode(nr.Title)
            {
                Tag = nr,
                ToolTipText = nr.Message,
                Checked = nr.Enabled
            };

            this.TelopTreeView.Nodes.Add(node);
            this.TelopTreeView.SelectedNode = node;

            // ジョブ限定ボタンの色を変える（未設定：黒、設定有：青）
            this.TelopSelectJobButton.ForeColor = nr.JobFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

            // ゾーン限定ボタンの色を変える（未設定：黒、設定有：青）
            this.TelopSelectZoneButton.ForeColor = nr.ZoneFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

            // 条件設定ボタンの色を変える（未設定：黒、設定有：青）
            this.TelopSetConditionButton.ForeColor =
                (nr.TimersMustRunningForStart.Length != 0 || nr.TimersMustStoppingForStart.Length != 0) ?
                Color.Blue :
                Button.DefaultForeColor;

            // 標準のスペルタイマーへ変更を反映する
            SpellsController.Instance.ApplyToNormalSpellTimer();
        }

        /// <summary>
        /// テロップ全て削除 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private async void TelopClearAllButton_Click(object sender, EventArgs e)
        {
            if (await Task.Run(() => MessageBox.Show(
                    this,
                    Translate.Get("TelopClearAllPrompt"),
                    "ACT.SpecialSpellTimer",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2)) == DialogResult.OK)
            {
                lock (OnePointTelopTable.Instance.Table)
                {
                    this.TelopDetailGroupBox.Visible = false;
                    OnePointTelopTable.Instance.Table.Clear();
                }

                TickersController.Instance.CloseTelops();
                this.LoadTelopTable();
            }
        }

        /// <summary>
        /// テロップ削除 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopDeleteButton_Click(object sender, EventArgs e)
        {
            lock (OnePointTelopTable.Instance.Table)
            {
                var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
                if (src != null)
                {
                    OnePointTelopTable.Instance.Table.Remove(src);
                    TableCompiler.Instance.RecompileTickers();
                    OnePointTelopTable.Instance.Save(true);

                    TickersController.Instance.CloseTelops();

                    this.TelopDetailGroupBox.Visible = false;
                }
            }

            // 今の選択ノードを取り出す
            var targetNode = this.TelopTreeView.SelectedNode;
            if (targetNode != null)
            {
                // 1個前のノードを取り出しておく
                var prevNode = targetNode.PrevNode;

                targetNode.Remove();

                if (prevNode != null)
                {
                    this.TelopTreeView.SelectedNode = prevNode;
                }
            }

            // 標準のスペルタイマーへ変更を反映する
            SpellsController.Instance.ApplyToNormalSpellTimer();
        }

        /// <summary>
        /// テロップエクスポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private async void TelopExportButton_Click(object sender, EventArgs e)
        {
            this.SaveFileDialog.FileName = "ACT.SpecialSpellTimer.Telops.xml";
            if (await Task.Run(() => this.SaveFileDialog.ShowDialog(this)) !=
                DialogResult.Cancel)
            {
                OnePointTelopTable.Instance.Save(
                    this.SaveFileDialog.FileName,
                    true);
            }
        }

        /// <summary>
        /// テロップインポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private async void TelopImportButton_Click(object sender, EventArgs e)
        {
            this.OpenFileDialog.FileName = "ACT.SpecialSpellTimer.Telops.xml";
            if (await Task.Run(() => this.OpenFileDialog.ShowDialog(this)) !=
                DialogResult.Cancel)
            {
                OnePointTelopTable.Instance.Load(
                    this.OpenFileDialog.FileName,
                    false);

                this.LoadTelopTable();
            }
        }

        /// <summary>
        /// テロップ更新 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopUpdateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TelopTitleTextBox.Text))
            {
                MessageBox.Show(
                    this,
                    Translate.Get("UpdateTelopNameTitle"),
                    "ACT.SpecialSpellTimer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return;
            }

            var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
            if (src != null)
            {
                src.Title = this.TelopTitleTextBox.Text;
                src.Message = this.TelopMessageTextBox.Text;
                src.Keyword = this.TelopKeywordTextBox.Text;
                src.KeywordToHide = this.TelopKeywordToHideTextBox.Text;
                src.RegexEnabled = this.TelopRegexEnabledCheckBox.Checked;
                src.Delay = (double)this.TelopDelayNumericUpDown.Value;
                src.DisplayTime = (double)this.DisplayTimeNumericUpDown.Value;
                src.AddMessageEnabled = this.EnabledAddMessageCheckBox.Checked;
                src.ProgressBarEnabled = this.TelopProgressBarEnabledCheckBox.Checked;
                src.FontColor = this.TelopVisualSetting.FontColor.ToHTML();
                src.FontOutlineColor = this.TelopVisualSetting.FontOutlineColor.ToHTML();
                src.Font = this.TelopVisualSetting.GetFontInfo();
                src.BackgroundColor = this.TelopVisualSetting.BackgroundColor.ToHTML();
                src.BackgroundAlpha = this.TelopVisualSetting.BackgroundColor.A;
                src.Left = (double)this.TelopLeftNumericUpDown.Value;
                src.Top = (double)this.TelopTopNumericUpDown.Value;
                src.MatchSound = (string)this.TelopMatchSoundComboBox.SelectedValue ?? string.Empty;
                src.MatchTextToSpeak = this.TelopMatchTTSTextBox.Text;
                src.DelaySound = (string)this.TelopDelaySoundComboBox.SelectedValue ?? string.Empty;
                src.DelayTextToSpeak = this.TelopDelayTTSTextBox.Text;

                if ((int)this.TelopLeftNumericUpDown.Tag != src.Left ||
                    (int)this.TelopTopNumericUpDown.Tag != src.Top)
                {
                    TickersController.Instance.SetLocation(
                        src.ID,
                        src.Left,
                        src.Top);
                }

                TableCompiler.Instance.RecompileTickers();
                OnePointTelopTable.Instance.Save(true);
                this.LoadTelopTable();

                // 一度全てのテロップを閉じる
                TickersController.Instance.CloseTelops();

                foreach (TreeNode node in this.TelopTreeView.Nodes)
                {
                    var ds = node.Tag as OnePointTelop;
                    if (ds != null)
                    {
                        if (ds.ID == src.ID)
                        {
                            this.TelopTreeView.SelectedNode = node;
                            break;
                        }
                    }
                }
            }
        }
    }
}
