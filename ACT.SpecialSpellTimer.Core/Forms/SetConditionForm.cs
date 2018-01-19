namespace ACT.SpecialSpellTimer.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Models;

    public partial class SetConditionForm : Form
    {
        public SetConditionForm()
        {
            InitializeComponent();
            Utility.Translate.TranslateControls(this);
        }

        public Guid[] TimersMustRunning { get; set; }

        public Guid[] TimersMustStopping { get; set; }

        private void AllOFFButton_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in this.SpellMustRunningTreeView.Nodes)
            {
                node.Checked = false;
                foreach (TreeNode child in node.Nodes)
                {
                    child.Checked = false;
                }
            }

            foreach (TreeNode node in this.SpellMustStoppingTreeView.Nodes)
            {
                node.Checked = false;
                foreach (TreeNode child in node.Nodes)
                {
                    child.Checked = false;
                }
            }

            foreach (TreeNode node in this.TelopMustRunningTreeView.Nodes)
            {
                node.Checked = false;
            }

            foreach (TreeNode node in this.TelopMustStoppingTreeView.Nodes)
            {
                node.Checked = false;
            }
        }

        /// <summary>
        /// TreeViewからチェックされたSpellTimerの一覧を取得する
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <returns>チェックされたSpellTimerを表すGuidの配列</returns>
        private Guid[] GetCheckedSpells(TreeView treeView)
        {
            var spells = new List<Guid>();
            foreach (TreeNode parent in treeView.Nodes)
            {
                foreach (TreeNode node in parent.Nodes)
                {
                    if (node.Checked)
                    {
                        var spell = (Spell)node.Tag;
                        spells.Add(spell.Guid);
                    }
                }
            }

            return spells.ToArray();
        }

        /// <summary>
        /// TreeViewからチェックされたOnePointTelopの一覧を取得する
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <returns>チェックされたOnePointTelopを表すGuidの配列</returns>
        private Guid[] GetCheckedTelops(TreeView treeView)
        {
            var telops = new List<Guid>();
            foreach (TreeNode node in treeView.Nodes)
            {
                if (node.Checked)
                {
                    var telop = (Ticker)node.Tag;
                    telops.Add(telop.Guid);
                }
            }

            return telops.ToArray();
        }

        /// <summary>
        /// TreeViewにSpellTimer一覧を読み込む
        /// その際、指定されたGuidに対応するSpellTimerにチェックを付ける
        /// </summary>
        /// <param name="treeView">読み込み先のTreeView</param>
        /// <param name="checks">チェックをつけるSpellTimerのGuid</param>
        private void LoadSpells(TreeView treeView, Guid[] checks)
        {
            treeView.Nodes.Clear();

            var panels = SpellTable.Instance.Table
                .OrderBy(x => x.Panel.PanelName)
                .Select(x => x.PanelID)
                .Distinct();
            foreach (var panelID in panels)
            {
                var children = new List<TreeNode>();
                var spells = SpellTable.Instance.Table
                    .OrderBy(x => x.DisplayNo)
                    .Where(x => x.PanelID == panelID);
                foreach (var spell in spells)
                {
                    var nc = new TreeNode()
                    {
                        Text = spell.SpellTitle,
                        ToolTipText = spell.Keyword,
                        Checked = Array.IndexOf(checks, spell.Guid) != -1,
                        Tag = spell,
                    };

                    children.Add(nc);
                }

                var n = new TreeNode(
                    spells.First().Panel.PanelName,
                    children.ToArray());

                n.Checked = false;

                treeView.Nodes.Add(n);
            }

            treeView.ExpandAll();
        }

        /// <summary>
        /// TreeViewにOnePointTelop一覧を読み込む
        /// その際、指定されたGuidに対応するOnePointTelopにチェックを付ける
        /// </summary>
        /// <param name="treeView">読み込み先のTreeView</param>
        /// <param name="checks">チェックをつけるOnePointTelopのGuid</param>
        private void LoadTelops(TreeView treeView, Guid[] checks)
        {
            treeView.Nodes.Clear();

            var telops = TickerTable.Instance.Table.OrderBy(x => x.Title);
            foreach (var telop in telops)
            {
                var n = new TreeNode();

                n.Tag = telop;
                n.Text = telop.Title;
                n.ToolTipText = telop.Message;
                n.Checked = Array.IndexOf(checks, telop.Guid) != -1;

                treeView.Nodes.Add(n);
            }

            treeView.ExpandAll();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            var s1 = this.GetCheckedSpells(this.SpellMustRunningTreeView);
            var s2 = this.GetCheckedSpells(this.SpellMustStoppingTreeView);
            var t1 = this.GetCheckedTelops(this.TelopMustRunningTreeView);
            var t2 = this.GetCheckedTelops(this.TelopMustStoppingTreeView);

            this.TimersMustRunning = s1.Concat(t1).ToArray();
            this.TimersMustStopping = s2.Concat(t2).ToArray();
        }

        private void SelectConditionForm_Load(object sender, EventArgs e)
        {
            this.LoadSpells(this.SpellMustRunningTreeView, this.TimersMustRunning);
            this.LoadSpells(this.SpellMustStoppingTreeView, this.TimersMustStopping);
            this.LoadTelops(this.TelopMustRunningTreeView, this.TimersMustRunning);
            this.LoadTelops(this.TelopMustStoppingTreeView, this.TimersMustStopping);
        }

        /// <summary>
        /// Shown
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SetConditionForm_Shown(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                this.Font = this.Owner.Font;
            }
        }
    }
}
