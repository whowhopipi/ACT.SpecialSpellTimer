using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Image;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Sound;
using ACT.SpecialSpellTimer.Utility;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.Forms
{
    /// <summary>
    /// 設定Panel
    /// </summary>
    public partial class ConfigPanel :
        UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigPanel()
        {
            this.InitializeComponent();

            // PC名設定タブの中身を設定する
            this.NameStyleTabPage.Controls.Add(ConfigPanelNameStyle.Instance);
            ConfigPanelNameStyle.Instance.Dock = DockStyle.Fill;

            // ログタブの中身を設定する
            this.LogTabPage.Controls.Add(ConfigPanelLog.Instance);
            ConfigPanelLog.Instance.Dock = DockStyle.Fill;
            ConfigPanelLog.Instance.IsLogTabActiveDelegate = () => this.TabControl.SelectedTab == this.LogTabPage;

            // 翻訳する
            Translate.TranslateControls(this);

            this.LanguageComboBox.Items.AddRange(Language.GetLanguageList());

            this.ToolTip.SetToolTip(this.KeywordTextBox, Translate.Get("MatchingKeywordExplanationTooltip"));
            this.ToolTip.SetToolTip(this.RegexEnabledCheckBox, Translate.Get("RegularExpressionExplanationTooltip"));
            this.ToolTip.SetToolTip(this.TelopRegexEnabledCheckBox, Translate.Get("RegularExpressionExplanationTooltip"));
            this.ToolTip.SetToolTip(this.TelopKeywordTextBox, Translate.Get("MatchingKeywordExplanationTooltip"));
            this.ToolTip.SetToolTip(this.TelopMessageTextBox, Translate.Get("TelopMessageExplanationTooltip"));
            this.ToolTip.SetToolTip(this.label46, Translate.Get("TelopMessageExplanationTooltip"));

            // ListViewのダブルバッファリングを有効にする
            typeof(ListView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(this.CombatLogListView, true, null);

            // インスタンス化に伴う正規表現のON/OFFを制限する
            this.ToInstanceCheckBox.CheckedChanged += (s, e) =>
            {
                if (this.ToInstanceCheckBox.Checked)
                {
                    this.RegexEnabledCheckBox.Checked = true;
                    this.RegexEnabledCheckBox.Enabled = false;
                }
                else
                {
                    this.RegexEnabledCheckBox.Enabled = true;
                }
            };

            this.Load += this.ConfigPanel_Load;
        }

        /// <summary>
        /// スペルタイマテーブルを読み込む
        /// </summary>
        public void LoadSpellTimerTable()
        {
            try
            {
                this.SpellTimerTreeView.SuspendLayout();

                this.SpellTimerTreeView.Nodes.Clear();

                var panels = SpellTimerTable.Instance.Table
                    .Where(x => !x.IsInstance)
                    .OrderBy(x => x.Panel)
                    .Select(x => x.Panel)
                    .Distinct();
                foreach (var panelName in panels)
                {
                    var children = new List<TreeNode>();
                    var spells = SpellTimerTable.Instance.Table
                        .Where(x => !x.IsInstance)
                        .OrderBy(x => x.DisplayNo)
                        .Where(x => x.Panel == panelName);
                    foreach (var spell in spells)
                    {
                        var nc = new TreeNode()
                        {
                            Text = spell.SpellTitle,
                            ToolTipText = spell.Keyword,
                            Checked = spell.Enabled,
                            Tag = spell,
                        };

                        children.Add(nc);
                    }

                    var n = new TreeNode(
                        panelName,
                        children.ToArray());

                    n.Checked = children.Any(x => x.Checked);

                    this.SpellTimerTreeView.Nodes.Add(n);
                }

                // スペルの再描画を行わせる
                SpellTimerTable.Instance.ClearUpdateFlags();

                // 標準のスペルタイマーへ変更を反映する
                SpellsController.Instance.ApplyToNormalSpellTimer();

                this.SpellTimerTreeView.ExpandAll();
            }
            finally
            {
                this.SpellTimerTreeView.ResumeLayout();
            }
        }

        /// <summary>
        /// 追加 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            lock (SpellTimerTable.Instance.Table)
            {
                var nr = new SpellTimer();

                nr.ID = SpellTimerTable.Instance.Table.Any() ?
                    SpellTimerTable.Instance.Table.Max(x => x.ID) + 1 :
                    1;
                nr.Guid = Guid.NewGuid();
                nr.Panel = "General";
                nr.SpellTitle = "New Spell";
                nr.SpellIcon = string.Empty;
                nr.SpellIconSize = 24;
                nr.ProgressBarVisible = true;
                nr.FontColor = Settings.Default.FontColor.ToHTML();
                nr.FontOutlineColor = Settings.Default.FontOutlineColor.ToHTML();
                nr.WarningFontColor = Settings.Default.WarningFontColor.ToHTML();
                nr.WarningFontOutlineColor = Settings.Default.WarningFontOutlineColor.ToHTML();
                nr.BarColor = Settings.Default.ProgressBarColor.ToHTML();
                nr.BarOutlineColor = Settings.Default.ProgressBarOutlineColor.ToHTML();
                nr.BarWidth = Settings.Default.ProgressBarSize.Width;
                nr.BarHeight = Settings.Default.ProgressBarSize.Height;
                nr.BackgroundColor = Settings.Default.BackgroundColor.ToHTML();
                nr.JobFilter = string.Empty;
                nr.ZoneFilter = string.Empty;
                nr.TimersMustRunningForStart = new Guid[0];
                nr.TimersMustStoppingForStart = new Guid[0];

                // 現在選択しているノードの情報を一部コピーする
                if (this.SpellTimerTreeView.SelectedNode != null)
                {
                    var baseRow = this.SpellTimerTreeView.SelectedNode.Tag != null ?
                        this.SpellTimerTreeView.SelectedNode.Tag as SpellTimer :
                        this.SpellTimerTreeView.SelectedNode.Nodes[0].Tag as SpellTimer;

                    if (baseRow != null)
                    {
                        nr.Panel = baseRow.Panel;
                        nr.SpellTitle = baseRow.SpellTitle + " New";
                        nr.SpellIcon = baseRow.SpellIcon;
                        nr.SpellIconSize = baseRow.SpellIconSize;
                        nr.Keyword = baseRow.Keyword;
                        nr.RegexEnabled = baseRow.RegexEnabled;
                        nr.RecastTime = baseRow.RecastTime;
                        nr.KeywordForExtend1 = baseRow.KeywordForExtend1;
                        nr.RecastTimeExtending1 = baseRow.RecastTimeExtending1;
                        nr.KeywordForExtend2 = baseRow.KeywordForExtend2;
                        nr.RecastTimeExtending2 = baseRow.RecastTimeExtending2;
                        nr.ExtendBeyondOriginalRecastTime = baseRow.ExtendBeyondOriginalRecastTime;
                        nr.UpperLimitOfExtension = baseRow.UpperLimitOfExtension;
                        nr.RepeatEnabled = baseRow.RepeatEnabled;
                        nr.ProgressBarVisible = baseRow.ProgressBarVisible;
                        nr.IsReverse = baseRow.IsReverse;
                        nr.FontColor = baseRow.FontColor;
                        nr.FontOutlineColor = baseRow.FontOutlineColor;
                        nr.WarningFontColor = baseRow.WarningFontColor;
                        nr.WarningFontOutlineColor = baseRow.WarningFontOutlineColor;
                        nr.BarColor = baseRow.BarColor;
                        nr.BarOutlineColor = baseRow.BarOutlineColor;
                        nr.DontHide = baseRow.DontHide;
                        nr.HideSpellName = baseRow.HideSpellName;
                        nr.WarningTime = baseRow.WarningTime;
                        nr.BlinkTime = baseRow.BlinkTime;
                        nr.BlinkIcon = baseRow.BlinkIcon;
                        nr.BlinkBar = baseRow.BlinkBar;
                        nr.ChangeFontColorsWhenWarning = baseRow.ChangeFontColorsWhenWarning;
                        nr.OverlapRecastTime = baseRow.OverlapRecastTime;
                        nr.ReduceIconBrightness = baseRow.ReduceIconBrightness;
                        nr.Font = baseRow.Font;
                        nr.BarWidth = baseRow.BarWidth;
                        nr.BarHeight = baseRow.BarHeight;
                        nr.BackgroundColor = baseRow.BackgroundColor;
                        nr.BackgroundAlpha = baseRow.BackgroundAlpha;
                        nr.JobFilter = baseRow.JobFilter;
                        nr.ZoneFilter = baseRow.ZoneFilter;
                        nr.TimersMustRunningForStart = baseRow.TimersMustRunningForStart;
                        nr.TimersMustStoppingForStart = baseRow.TimersMustStoppingForStart;
                        nr.ToInstance = baseRow.ToInstance;
                    }
                }

                nr.MatchDateTime = DateTime.MinValue;
                nr.UpdateDone = false;
                nr.Enabled = true;
                nr.DisplayNo = SpellTimerTable.Instance.Table.Any() ?
                    SpellTimerTable.Instance.Table.Max(x => x.DisplayNo) + 1 :
                    50;
                nr.Regex = null;
                nr.RegexPattern = string.Empty;
                SpellTimerTable.Instance.Table.Add(nr);

                TableCompiler.Instance.RecompileSpells();
                SpellTimerTable.Instance.RemoveAllInstanceSpells();
                SpellTimerTable.Instance.Save(true);

                // 新しいノードを生成する
                var node = new TreeNode(nr.SpellTitle)
                {
                    Tag = nr,
                    ToolTipText = nr.Keyword,
                    Checked = nr.Enabled
                };

                // 親ノードがあれば追加する
                foreach (TreeNode item in this.SpellTimerTreeView.Nodes)
                {
                    if (item.Text == nr.Panel)
                    {
                        item.Nodes.Add(node);
                        this.SpellTimerTreeView.SelectedNode = node;
                        this.SpellTimerTreeView.SelectedNode.EnsureVisible();
                        break;
                    }
                }

                // 親ノードがなかった
                if (this.SpellTimerTreeView.SelectedNode != node)
                {
                    var parentNode = new TreeNode(nr.Panel, new TreeNode[] { node })
                    {
                        Checked = true
                    };

                    SpellsController.Instance.GetPanelSettings(nr.Panel);

                    this.SpellTimerTreeView.Nodes.Add(parentNode);
                    this.SpellTimerTreeView.SelectedNode = node;
                    this.SpellTimerTreeView.SelectedNode.EnsureVisible();
                }

                // ゾーン限定ボタンの色を変える（未設定：黒、設定有：青）
                this.SelectZoneButton.ForeColor = nr.ZoneFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

                // ジョブ限定ボタンの色を変える（未設定：黒、設定有：青）
                this.SelectJobButton.ForeColor = nr.JobFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

                // 条件設定ボタンの色を変える（未設定：黒、設定有：青）
                this.SetConditionButton.ForeColor =
                    (nr.TimersMustRunningForStart.Length != 0 || nr.TimersMustStoppingForStart.Length != 0) ?
                    Color.Blue :
                    Button.DefaultForeColor;
            }

            // 標準のスペルタイマーへ変更を反映する
            SpellsController.Instance.ApplyToNormalSpellTimer();
        }

        /// <summary>
        /// 全て削除
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private async void ClearAllButton_Click(object sender, EventArgs e)
        {
            if (await Task.Run(() => MessageBox.Show(
                this,
                Translate.Get("SpellClearAllPrompt"),
                "ACT.SpecialSpellTimer",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2)) == DialogResult.OK)
            {
                lock (SpellTimerTable.Instance.Table)
                {
                    this.DetailGroupBox.Visible = false;
                    this.DetailPanelGroupBox.Visible = false;
                    SpellTimerTable.Instance.Table.Clear();
                }

                this.LoadSpellTimerTable();
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ConfigPanel_Load(object sender, EventArgs e)
        {
            this.LoadSpellTimerTable();

            this.DetailGroupBox.Visible = false;
            this.DetailPanelGroupBox.Visible = false;

            // コンボボックスにアイテムを装填する
            this.MatchSoundComboBox.DataSource = SoundController.Instance.EnumlateWave();
            this.MatchSoundComboBox.ValueMember = "FullPath";
            this.MatchSoundComboBox.DisplayMember = "Name";

            this.OverSoundComboBox.DataSource = SoundController.Instance.EnumlateWave();
            this.OverSoundComboBox.ValueMember = "FullPath";
            this.OverSoundComboBox.DisplayMember = "Name";

            this.BeforeSoundComboBox.DataSource = SoundController.Instance.EnumlateWave();
            this.BeforeSoundComboBox.ValueMember = "FullPath";
            this.BeforeSoundComboBox.DisplayMember = "Name";

            this.TimeupSoundComboBox.DataSource = SoundController.Instance.EnumlateWave();
            this.TimeupSoundComboBox.ValueMember = "FullPath";
            this.TimeupSoundComboBox.DisplayMember = "Name";

            // イベントを設定する
            this.SpellTimerTreeView.AfterSelect += this.SpellTimerTreeView_AfterSelect;
            this.AddButton.Click += this.AddButton_Click;
            this.UpdateButton.Click += this.UpdateButton_Click;
            this.DeleteButton.Click += this.DeleteButton_Click;

            this.Play1Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play((string)this.MatchSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.Play2Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play((string)this.OverSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.Play3Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play((string)this.TimeupSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.Play4Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play((string)this.BeforeSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.Speak1Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play(this.MatchTextToSpeakTextBox.Text);
            };

            this.Speak2Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play(this.OverTextToSpeakTextBox.Text);
            };

            this.Speak3Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play(this.TimeupTextToSpeakTextBox.Text);
            };

            this.Speak4Button.Click += (s1, e1) =>
            {
                SoundController.Instance.Play(this.BeforeTextToSpeakTextBox.Text);
            };

            this.SpellTimerTreeView.AfterCheck += (s1, e1) =>
            {
                var source = e1.Node.Tag as SpellTimer;
                if (source != null)
                {
                    source.Enabled = e1.Node.Checked;
                    source.UpdateDone = false;
                }
                else
                {
                    foreach (TreeNode node in e1.Node.Nodes)
                    {
                        var sourceChild = node.Tag as SpellTimer;
                        if (sourceChild != null)
                        {
                            sourceChild.Enabled = e1.Node.Checked;
                        }

                        node.Checked = e1.Node.Checked;
                    }
                }

                // キャッシュを無効にする
                TableCompiler.Instance.RecompileSpells();

                // スペルの有効・無効が変化した際に、標準のスペルタイマーに反映する
                SpellsController.Instance.ApplyToNormalSpellTimer();
            };

            this.SelectJobButton.Click += async (s1, e1) =>
            {
                var src = this.DetailGroupBox.Tag as SpellTimer;
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
                            this.SelectJobButton.ForeColor = src.JobFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;
                        }
                    }
                }
            };

            this.SelectZoneButton.Click += async (s1, e1) =>
            {
                var src = this.DetailGroupBox.Tag as SpellTimer;
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
                            this.SelectZoneButton.ForeColor = src.ZoneFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;
                        }
                    }
                }
            };

            this.SetConditionButton.Click += async (s1, e1) =>
            {
                var src = this.DetailGroupBox.Tag as SpellTimer;
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
                            this.SetConditionButton.ForeColor =
                                (src.TimersMustRunningForStart.Length != 0 || src.TimersMustStoppingForStart.Length != 0) ?
                                Color.Blue :
                                Button.DefaultForeColor;
                        }
                    }
                }
            };

            // アイコン選択ボタンの挙動を設定する
            this.SelectIconButton.Click += async (s1, e1) =>
            {
                var selectedIcon = (string)this.SelectIconButton.Tag;
                var spell = this.DetailGroupBox.Tag as SpellTimer;

                var result = await SelectIconForm.ShowDialogAsync(
                    selectedIcon,
                    this,
                    spell);

                if (result.Result)
                {
                    ActInvoker.Invoke(() =>
                    {
                        this.SelectIconButton.Tag = result.Icon;
                        this.SelectIconButton.BackgroundImageLayout = ImageLayout.Zoom;

                        this.SelectIconButton.BackgroundImage = null;
                        this.SelectIconButton.FlatAppearance.BorderSize = 1;
                        var icon = IconController.Instance.GetIconFile(result.Icon);
                        if (icon != null &&
                            File.Exists(icon.FullPath))
                        {
                            this.SelectIconButton.BackgroundImage = System.Drawing.Image.FromFile(
                                icon.FullPath);
                            this.SelectIconButton.FlatAppearance.BorderSize = 0;
                        }

                        this.SpellVisualSetting.SpellIcon = result.Icon;
                        this.SpellVisualSetting.RefreshSampleImage();
                    });
                }
            };

            this.SpellIconSizeUpDown.ValueChanged += (s1, e1) =>
            {
                this.SpellVisualSetting.SpellIconSize = (int)this.SpellIconSizeUpDown.Value;
                this.SpellVisualSetting.RefreshSampleImage();
            };

            this.HideSpellNameCheckBox.CheckedChanged += (s1, e1) =>
            {
                this.SpellVisualSetting.HideSpellName = this.HideSpellNameCheckBox.Checked;
                this.SpellVisualSetting.RefreshSampleImage();
            };

            this.OverlapRecastTimeCheckBox.CheckedChanged += (s1, e1) =>
            {
                this.SpellVisualSetting.OverlapRecastTime = this.OverlapRecastTimeCheckBox.Checked;
                this.SpellVisualSetting.RefreshSampleImage();
            };

            // スペルの一時表示チェックボックス
            this.TemporarilyDisplaySpellCheckBox.CheckedChanged += (s1, e1) =>
            {
                var src = this.DetailGroupBox.Tag as SpellTimer;
                if (src == null)
                {
                    return;
                }

                src.IsTemporarilyDisplay = this.TemporarilyDisplaySpellCheckBox.Checked;
                src.UpdateDone = false;

                TableCompiler.Instance.RecompileSpells();
            };

            // オプションのロードメソッドを呼ぶ
            this.LoadOption();
            this.LoadDQXOption();

            // ワンポイントテロップのロードメソッドを呼ぶ
            this.LoadOnePointTelop();

            // 戦闘アナライザのロードメソッドを呼ぶ
            this.LoadCombatAnalyzer();
        }

        /// <summary>
        /// 削除 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            lock (SpellTimerTable.Instance.Table)
            {
                var src = this.DetailGroupBox.Tag as SpellTimer;
                if (src != null)
                {
                    SpellTimerTable.Instance.Table.Remove(src);
                    TableCompiler.Instance.RecompileSpells();
                    SpellTimerTable.Instance.RemoveAllInstanceSpells();
                    SpellTimerTable.Instance.Save(true);

                    this.DetailGroupBox.Visible = false;
                    this.DetailPanelGroupBox.Visible = false;
                }
            }

            // 今の選択ノードを取り出す
            var targetNode = this.SpellTimerTreeView.SelectedNode;
            if (targetNode != null)
            {
                // 1個前のノードを取り出しておく
                var prevNode = targetNode.PrevNode;

                if (targetNode.Parent != null &&
                    targetNode.Parent.Nodes.Count > 1)
                {
                    targetNode.Remove();

                    if (prevNode != null)
                    {
                        this.SpellTimerTreeView.SelectedNode = prevNode;
                        this.SpellTimerTreeView.SelectedNode.EnsureVisible();
                    }
                }
                else
                {
                    targetNode.Parent.Remove();
                }
            }

            // 標準のスペルタイマーへ変更を反映する
            SpellsController.Instance.ApplyToNormalSpellTimer();
        }

        /// <summary>
        /// エクスポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private async void ExportButton_Click(object sender, EventArgs e)
        {
            this.SaveFileDialog.FileName = "ACT.SpecialSpellTimer.Spells.xml";

            var result = await Task.Run(() =>
            {
                return this.SaveFileDialog.ShowDialog(this);
            });

            if (result != DialogResult.Cancel)
            {
                SpellTimerTable.Instance.Save(
                    this.SaveFileDialog.FileName,
                    true);
            }
        }

        /// <summary>
        /// インポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private async void ImportButton_Click(object sender, EventArgs e)
        {
            this.OpenFileDialog.FileName = "ACT.SpecialSpellTimer.Spells.xml";

            var result = await Task.Run(() =>
            {
                return this.OpenFileDialog.ShowDialog(this);
            });

            if (result != DialogResult.Cancel)
            {
                SpellTimerTable.Instance.Load(
                    this.OpenFileDialog.FileName,
                    false);

                this.LoadSpellTimerTable();
            }
        }

        /// <summary>
        /// 初期化 Button
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private async void ShokikaButton_Click(object sender, EventArgs e)
        {
            var result = await Task.Run(() =>
            {
                return MessageBox.Show(
                    this,
                    Translate.Get("ResetAllPrompt"),
                    "ACT.SpecialSpellTimer",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
            });

            if (result != DialogResult.OK)
            {
                return;
            }

            Settings.Default.Reset();
            Settings.Default.Save();

            this.LoadSettingsOption();
        }

        /// <summary>
        /// 詳細を表示する
        /// </summary>
        /// <param name="dataSource">データソース</param>
        private void ShowDetail(
            SpellTimer dataSource)
        {
            var src = dataSource;
            if (src == null)
            {
                this.DetailGroupBox.Visible = false;
                return;
            }

            this.DetailGroupBox.Visible = true;

            // データソースをタグに突っ込んでおく
            this.DetailGroupBox.Tag = src;

            // アイコンを設定する
            this.SelectIconButton.Tag = src.SpellIcon;
            this.SelectIconButton.BackgroundImageLayout = ImageLayout.Zoom;
            this.SelectIconButton.BackgroundImage = null;
            this.SelectIconButton.FlatAppearance.BorderSize = 1;
            var icon = IconController.Instance.GetIconFile(src.SpellIcon);
            if (icon != null &&
                File.Exists(icon.FullPath))
            {
                this.SelectIconButton.BackgroundImage = System.Drawing.Image.FromFile(
                    icon.FullPath);
                this.SelectIconButton.FlatAppearance.BorderSize = 0;
            }

            this.PanelNameTextBox.Text = src.Panel;
            this.SpellTitleTextBox.Text = src.SpellTitle;
            this.SpellIconSizeUpDown.Value = src.SpellIconSize;
            this.DisplayNoNumericUpDown.Value = src.DisplayNo;
            this.KeywordTextBox.Text = src.Keyword;
            this.RegexEnabledCheckBox.Checked = src.RegexEnabled;
            this.RecastTimeNumericUpDown.Value = (decimal)src.RecastTime;
            this.RepeatCheckBox.Checked = src.RepeatEnabled;
            this.ShowProgressBarCheckBox.Checked = src.ProgressBarVisible;

            this.KeywordToExpand1TextBox.Text = src.KeywordForExtend1;
            this.ExpandSecounds1NumericUpDown.Value = (decimal)src.RecastTimeExtending1;
            this.KeywordToExpand2TextBox.Text = src.KeywordForExtend2;
            this.ExpandSecounds2NumericUpDown.Value = (decimal)src.RecastTimeExtending2;
            this.ExtendBeyondOriginalRecastTimeCheckBox.Checked = src.ExtendBeyondOriginalRecastTime;
            this.UpperLimitOfExtensionNumericUpDown.Value = (decimal)src.UpperLimitOfExtension;
            this.WarningTimeNumericUpDown.Value = (decimal)src.WarningTime;
            this.WarningTimeCheckBox.Checked = src.ChangeFontColorsWhenWarning;

            this.MatchSoundComboBox.SelectedValue = src.MatchSound;
            this.MatchTextToSpeakTextBox.Text = src.MatchTextToSpeak;

            this.OverSoundComboBox.SelectedValue = src.OverSound;
            this.OverTextToSpeakTextBox.Text = src.OverTextToSpeak;
            this.OverTimeNumericUpDown.Value = (decimal)src.OverTime;

            this.BeforeSoundComboBox.SelectedValue = src.BeforeSound;
            this.BeforeTextToSpeakTextBox.Text = src.BeforeTextToSpeak;
            this.BeforeTimeNumericUpDown.Value = (decimal)src.BeforeTime;

            this.TimeupSoundComboBox.SelectedValue = src.TimeupSound;
            this.TimeupTextToSpeakTextBox.Text = src.TimeupTextToSpeak;

            this.IsReverseCheckBox.Checked = src.IsReverse;
            this.DontHideCheckBox.Checked = src.DontHide;
            this.HideSpellNameCheckBox.Checked = src.HideSpellName;
            this.OverlapRecastTimeCheckBox.Checked = src.OverlapRecastTime;
            this.ReduceIconBrightnessCheckBox.Checked = src.ReduceIconBrightness;
            this.ToInstanceCheckBox.Checked = src.ToInstance;

            this.WarningTimeCheckBox.Checked = src.ChangeFontColorsWhenWarning;
            this.WarningTimeNumericUpDown.Value = (decimal)src.WarningTime;
            this.BlinkTimeNumericUpDown.Value = (decimal)src.BlinkTime;
            this.BlinkIconCheckBox.Checked = src.BlinkIcon;
            this.BlinkBarCheckBox.Checked = src.BlinkBar;

            this.SpellVisualSetting.SetFontInfo(src.Font);
            this.SpellVisualSetting.BarColor = string.IsNullOrWhiteSpace(src.BarColor) ?
                Settings.Default.ProgressBarColor :
                src.BarColor.FromHTML();
            this.SpellVisualSetting.BarOutlineColor = string.IsNullOrWhiteSpace(src.BarOutlineColor) ?
                Settings.Default.ProgressBarOutlineColor :
                src.BarOutlineColor.FromHTML();
            this.SpellVisualSetting.FontColor = string.IsNullOrWhiteSpace(src.FontColor) ?
                Settings.Default.FontColor :
                src.FontColor.FromHTML();
            this.SpellVisualSetting.FontOutlineColor = string.IsNullOrWhiteSpace(src.FontOutlineColor) ?
                Settings.Default.FontOutlineColor :
                src.FontOutlineColor.FromHTML();
            this.SpellVisualSetting.WarningFontColor = string.IsNullOrWhiteSpace(src.WarningFontColor) ?
                Settings.Default.WarningFontColor :
                src.WarningFontColor.FromHTML();
            this.SpellVisualSetting.WarningFontOutlineColor = string.IsNullOrWhiteSpace(src.WarningFontOutlineColor) ?
                Settings.Default.WarningFontOutlineColor :
                src.WarningFontOutlineColor.FromHTML();
            this.SpellVisualSetting.BarSize = new Size(src.BarWidth, src.BarHeight);
            this.SpellVisualSetting.BackgroundColor = string.IsNullOrWhiteSpace(src.BackgroundColor) ?
                Settings.Default.BackgroundColor :
                Color.FromArgb(src.BackgroundAlpha, src.BackgroundColor.FromHTML());

            this.SpellVisualSetting.SpellIcon = src.SpellIcon;
            this.SpellVisualSetting.SpellIconSize = src.SpellIconSize;
            this.SpellVisualSetting.HideSpellName = src.HideSpellName;
            this.SpellVisualSetting.OverlapRecastTime = src.OverlapRecastTime;

            this.TemporarilyDisplaySpellCheckBox.Checked = src.IsTemporarilyDisplay;

            this.SpellVisualSetting.RefreshSampleImage();

            // ゾーン限定ボタンの色を変える（未設定：黒、設定有：青）
            this.SelectZoneButton.ForeColor = src.ZoneFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

            // ジョブ限定ボタンの色を変える（未設定：黒、設定有：青）
            this.SelectJobButton.ForeColor = src.JobFilter != string.Empty ? Color.Blue : Button.DefaultForeColor;

            // 条件設定ボタンの色を変える（未設定：黒、設定有：青）
            this.SetConditionButton.ForeColor =
                (src.TimersMustRunningForStart.Length != 0 || src.TimersMustStoppingForStart.Length != 0) ?
                Color.Blue :
                Button.DefaultForeColor;
        }

        /// <summary>
        /// スペルタイマツリー AfterSelect
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SpellTimerTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var ds = e.Node.Tag as SpellTimer;

            // スペルの詳細？
            if (ds != null)
            {
                this.DetailPanelGroupBox.Visible = false;

                this.ShowDetail(
                    e.Node.Tag as SpellTimer);

                return;
            }

            // パネルの位置を調整する
            this.DetailPanelGroupBox.Parent = this.DetailGroupBox.Parent;
            this.DetailPanelGroupBox.Location = this.DetailGroupBox.Location;
            this.DetailPanelGroupBox.Dock = DockStyle.Fill;

            // パネルの詳細を表示する
            this.DetailGroupBox.Visible = false;
            this.DetailPanelGroupBox.Visible = true;

            // パネル名を取り出す
            var panelName = e.Node.Text;
            this.DetailPanelGroupBox.Tag = panelName;

            // パネルの設定を取り出す
            var panelSettings = SpellsController.Instance.GetPanelSettings(
                panelName);

            this.PanelLeftNumericUpDown.Value = (int)panelSettings.Left;
            this.PanelTopNumericUpDown.Value = (int)panelSettings.Top;
            this.MarginUpDown.Value = (int)panelSettings.Margin;
            this.HorizontalLayoutCheckBox.Checked = panelSettings.Horizontal;
            this.FixedPositionSpellCheckBox.Checked = panelSettings.FixedPositionSpell;

            // 更新ボタンの挙動をセットする
            if (this.UpdatePanelButton.Tag == null ||
                !(bool)(this.UpdatePanelButton.Tag))
            {
                this.UpdatePanelButton.Click += new EventHandler((s1, e1) =>
                {
                    var left = (double)this.PanelLeftNumericUpDown.Value;
                    var top = (double)this.PanelTopNumericUpDown.Value;
                    var margin = (int)this.MarginUpDown.Value;
                    var horizontal = this.HorizontalLayoutCheckBox.Checked;
                    var fixedPositionSpell = this.FixedPositionSpellCheckBox.Checked;

                    if (this.DetailPanelGroupBox.Tag != null)
                    {
                        var panelNameToUpdate = (string)this.DetailPanelGroupBox.Tag;
                        SpellsController.Instance.SetPanelSettings(
                            panelNameToUpdate,
                            left,
                            top,
                            margin,
                            horizontal,
                            fixedPositionSpell);
                    }
                });

                this.UpdatePanelButton.Tag = true;
            }
        }

        /// <summary>
        /// 適用する Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TekiyoButton_Click(object sender, EventArgs e)
        {
            this.ApplySettingsOption();

            // Windowを一旦すべて閉じる
            SpellsController.Instance.ClosePanels();
            TickersController.Instance.CloseTelops();
        }

        /// <summary>
        /// 更新 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            lock (SpellTimerTable.Instance.Table)
            {
                if (string.IsNullOrWhiteSpace(this.PanelNameTextBox.Text))
                {
                    MessageBox.Show(
                        this,
                        Translate.Get("UpdateSpellPanelTitle"),
                        "ACT.SpecialSpellTimer",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.SpellTitleTextBox.Text))
                {
                    MessageBox.Show(
                        this,
                        Translate.Get("UpdateSpellNameTitle"),
                        "ACT.SpecialSpellTimer",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                var src = this.DetailGroupBox.Tag as SpellTimer;
                if (src != null)
                {
                    src.Panel = this.PanelNameTextBox.Text;
                    src.SpellTitle = this.SpellTitleTextBox.Text;
                    src.SpellIcon = (string)this.SelectIconButton.Tag;
                    src.SpellIconSize = (int)this.SpellIconSizeUpDown.Value;
                    src.DisplayNo = (int)this.DisplayNoNumericUpDown.Value;
                    src.Keyword = this.KeywordTextBox.Text;
                    src.RegexEnabled = this.RegexEnabledCheckBox.Checked;
                    src.RecastTime = (double)this.RecastTimeNumericUpDown.Value;
                    src.RepeatEnabled = this.RepeatCheckBox.Checked;
                    src.ProgressBarVisible = this.ShowProgressBarCheckBox.Checked;

                    src.KeywordForExtend1 = this.KeywordToExpand1TextBox.Text;
                    src.RecastTimeExtending1 = (double)this.ExpandSecounds1NumericUpDown.Value;
                    src.KeywordForExtend2 = this.KeywordToExpand2TextBox.Text;
                    src.RecastTimeExtending2 = (double)this.ExpandSecounds2NumericUpDown.Value;
                    src.ExtendBeyondOriginalRecastTime = this.ExtendBeyondOriginalRecastTimeCheckBox.Checked;
                    src.UpperLimitOfExtension = (double)this.UpperLimitOfExtensionNumericUpDown.Value;

                    src.MatchSound = (string)this.MatchSoundComboBox.SelectedValue ?? string.Empty;
                    src.MatchTextToSpeak = this.MatchTextToSpeakTextBox.Text;

                    src.OverSound = (string)this.OverSoundComboBox.SelectedValue ?? string.Empty;
                    src.OverTextToSpeak = this.OverTextToSpeakTextBox.Text;
                    src.OverTime = (double)this.OverTimeNumericUpDown.Value;

                    src.BeforeSound = (string)this.BeforeSoundComboBox.SelectedValue ?? string.Empty;
                    src.BeforeTextToSpeak = this.BeforeTextToSpeakTextBox.Text;
                    src.BeforeTime = (double)this.BeforeTimeNumericUpDown.Value;

                    src.TimeupSound = (string)this.TimeupSoundComboBox.SelectedValue ?? string.Empty;
                    src.TimeupTextToSpeak = this.TimeupTextToSpeakTextBox.Text;

                    src.IsReverse = this.IsReverseCheckBox.Checked;
                    src.DontHide = this.DontHideCheckBox.Checked;
                    src.HideSpellName = this.HideSpellNameCheckBox.Checked;
                    src.OverlapRecastTime = this.OverlapRecastTimeCheckBox.Checked;
                    src.ReduceIconBrightness = this.ReduceIconBrightnessCheckBox.Checked;
                    src.ToInstance = this.ToInstanceCheckBox.Checked;

                    src.Font = this.SpellVisualSetting.GetFontInfo();
                    src.FontColor = this.SpellVisualSetting.FontColor.ToHTML();
                    src.FontOutlineColor = this.SpellVisualSetting.FontOutlineColor.ToHTML();
                    src.WarningFontColor = this.SpellVisualSetting.WarningFontColor.ToHTML();
                    src.WarningFontOutlineColor = this.SpellVisualSetting.WarningFontOutlineColor.ToHTML();
                    src.BarColor = this.SpellVisualSetting.BarColor.ToHTML();
                    src.BarOutlineColor = this.SpellVisualSetting.BarOutlineColor.ToHTML();
                    src.BarWidth = this.SpellVisualSetting.BarSize.Width;
                    src.BarHeight = this.SpellVisualSetting.BarSize.Height;
                    src.BackgroundColor = this.SpellVisualSetting.BackgroundColor.ToHTML();
                    src.BackgroundAlpha = this.SpellVisualSetting.BackgroundColor.A;

                    src.WarningTime = (double)this.WarningTimeNumericUpDown.Value;
                    src.ChangeFontColorsWhenWarning = this.WarningTimeCheckBox.Checked;
                    src.BlinkTime = (double)this.BlinkTimeNumericUpDown.Value;
                    src.BlinkIcon = this.BlinkIconCheckBox.Checked;
                    src.BlinkBar = this.BlinkBarCheckBox.Checked;

                    var panel = SpellTimerTable.Instance.Table.Where(x => x.Panel == src.Panel);
                    foreach (var s in panel)
                    {
                        s.BackgroundColor = src.BackgroundColor;
                    }

                    TableCompiler.Instance.RecompileSpells();

                    SpellTimerTable.Instance.Save(true);
                    this.LoadSpellTimerTable();

                    // 一度全てのパネルを閉じる
                    SpellsController.Instance.ClosePanels();

                    foreach (TreeNode root in this.SpellTimerTreeView.Nodes)
                    {
                        if (root.Nodes != null)
                        {
                            foreach (TreeNode node in root.Nodes)
                            {
                                var ds = node.Tag as SpellTimer;
                                if (ds != null)
                                {
                                    if (ds.ID == src.ID)
                                    {
                                        this.SpellTimerTreeView.SelectedNode = node;
                                        this.SpellTimerTreeView.SelectedNode.EnsureVisible();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
