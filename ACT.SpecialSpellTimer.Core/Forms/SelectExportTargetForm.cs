using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using ACT.SpecialSpellTimer.Models;

namespace ACT.SpecialSpellTimer.Forms
{
    public partial class SelectExportTargetForm : Form
    {
        #region Singleton

        private static SelectExportTargetForm instance = new SelectExportTargetForm();

        #endregion Singleton

        #region Dialog

        public static Task<DialogResult> ShowDialogAsync(
            Targets target,
            IWin32Window owner = null)
        {
            SelectExportTargetForm.instance.Target = target;
            return Task.Run(() => SelectExportTargetForm.instance.ShowDialog(owner));
        }

        #endregion Dialog

        public Targets Target { get; set; }

        public SelectExportTargetForm()
        {
            this.InitializeComponent();

            this.Load += this.SelectExportTargetForm_Load;

            void switchVisibleList()
            {
                if (this.AllRadioButton.Checked)
                {
                    this.SelectionListView.Visible = false;
                }

                if (this.SelectionRadioButton.Checked)
                {
                    this.SelectionListView.Visible = true;
                }
            }

            this.AllRadioButton.CheckedChanged += (s, e) => switchVisibleList();
            switchVisibleList();

            this.OKButton.Click += this.OKButton_Click;
        }

        private async void OKButton_Click(object sender, EventArgs e)
        {
            var dataName = string.Empty;
            switch (this.Target)
            {
                case Targets.Spells:
                    dataName = "spells";
                    break;

                case Targets.Tickers:
                    dataName = "tickers";
                    break;
            }

            var dataRange = this.AllRadioButton.Checked ?
                "all" :
                "selection";

            var fileName = $"{dataName}.{dataRange}.xml";

            this.saveFileDialog.FileName = fileName;
            var result = await Task.Run(() => this.saveFileDialog.ShowDialog(this));

            if (result != DialogResult.OK)
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            switch (this.Target)
            {
                case Targets.Spells:
                    this.ExportSpells(this.saveFileDialog.FileName);
                    break;

                case Targets.Tickers:
                    this.ExportTickers(this.saveFileDialog.FileName);
                    break;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void ExportSpells(
            string fileName)
        {
            if (this.AllRadioButton.Checked)
            {
                SpellTimerTable.Instance.Save(fileName, true);
                return;
            }

            var list = new List<SpellTimer>();
            for (int i = 0; i < this.SelectionListView.SelectedItems.Count; i++)
            {
                var item = this.SelectionListView.SelectedItems[i];
                list.Add(item.Tag as SpellTimer);
            }

            SpellTimerTable.Instance.Save(fileName, list);
        }

        private void ExportTickers(
            string fileName)
        {
            if (this.AllRadioButton.Checked)
            {
                OnePointTelopTable.Instance.Save(fileName, true);
                return;
            }

            var list = new List<OnePointTelop>();
            for (int i = 0; i < this.SelectionListView.SelectedItems.Count; i++)
            {
                var item = this.SelectionListView.SelectedItems[i];
                list.Add(item.Tag as OnePointTelop);
            }

            OnePointTelopTable.Instance.Save(fileName, list);
        }

        private void SelectExportTargetForm_Load(object sender, EventArgs e)
        {
            switch (this.Target)
            {
                case Targets.Spells:
                    this.LoadSpells();
                    break;

                case Targets.Tickers:
                    this.LoadTickers();
                    break;
            }
        }


        private void LoadSpells()
        {
            var list = new List<ListViewItem>();

            var q =
                from x in SpellTimerTable.Instance.Table
                where
                !x.IsInstance
                orderby
                x.Panel,
                x.DisplayNo,
                x.ID
                select
                x;

            foreach (var spell in q)
            {
                list.Add(new ListViewItem(
                    $"[{spell.Panel}] {spell.SpellTitle}")
                {
                    Tag = spell
                });
            }

            try
            {
                this.SelectionListView.SuspendLayout();

                this.SelectionListView.Items.Clear();
                this.SelectionListView.Items.AddRange(list.ToArray());

                for (int i = 0; i < this.SelectionListView.Columns.Count; i++)
                {
                    this.SelectionListView.Columns[i].AutoResize(
                        ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }
            finally
            {
                this.SelectionListView.ResumeLayout();
            }
        }

        private void LoadTickers()
        {
            var list = new List<ListViewItem>();

            var q =
                from x in OnePointTelopTable.Instance.Table
                orderby
                x.Title,
                x.ID
                select
                x;

            foreach (var ticker in q)
            {
                list.Add(new ListViewItem(ticker.Title)
                {
                    Tag = ticker
                });
            }

            try
            {
                this.SelectionListView.SuspendLayout();

                this.SelectionListView.Items.Clear();
                this.SelectionListView.Items.AddRange(list.ToArray());

                for (int i = 0; i < this.SelectionListView.Columns.Count; i++)
                {
                    this.SelectionListView.Columns[i].AutoResize(
                        ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }
            finally
            {
                this.SelectionListView.ResumeLayout();
            }
        }

        public enum Targets
        {
            Spells,
            Tickers
        }
    }
}
