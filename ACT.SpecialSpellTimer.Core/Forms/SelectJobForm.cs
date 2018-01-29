using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ACT.SpecialSpellTimer.Config.Models;
using FFXIV.Framework.FFXIVHelper;

namespace ACT.SpecialSpellTimer.Forms
{
    /// <summary>
    /// ジョブ選択Form
    /// </summary>
    public partial class SelectJobForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SelectJobForm()
        {
            this.InitializeComponent();
            Utility.Translate.TranslateControls(this);

            this.Load += this.SelectJobForm_Load;
            this.OKButton.Click += this.OKButton_Click;

            this.AllONButton.Click += (s1, e1) =>
            {
                for (int i = 0; i < this.JobsCheckedListBox.Items.Count; i++)
                {
                    this.JobsCheckedListBox.SetItemChecked(i, true);
                }
            };

            this.AllOFFButton.Click += (s1, e1) =>
            {
                for (int i = 0; i < this.JobsCheckedListBox.Items.Count; i++)
                {
                    this.JobsCheckedListBox.SetItemChecked(i, false);
                }
            };
        }

        /// <summary>
        /// ジョブフィルタ
        /// </summary>
        public string JobFilter { get; set; }

        /// <summary>
        /// OKボタン Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            var jobs = new List<string>();
            foreach (JobSelector container in this.JobsCheckedListBox.CheckedItems)
            {
                var jobId = (int)container.Job.ID;
                jobs.Add(jobId.ToString());
            }

            this.JobFilter = string.Join(
                ",",
                jobs.ToArray());
        }

        /// <summary>
        /// ロード
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SelectJobForm_Load(object sender, EventArgs e)
        {
            var jobs = this.JobFilter.Split(',');

            try
            {
                this.JobsCheckedListBox.SuspendLayout();

                this.JobsCheckedListBox.Items.Clear();
                foreach (var job in Jobs.List
                    .OrderBy(x => x.Role))
                {
                    if (job.ID != JobIDs.Unknown)
                    {
                        var jobId = (int)job.ID;
                        var container = new JobSelector(job);
                        this.JobsCheckedListBox.Items.Add(
                            container,
                            jobs.Any(x => x == jobId.ToString()));

                        this.DummyCheckBox.Text = container.ToString();
                        if (this.DummyCheckBox.Width >
                            this.JobsCheckedListBox.ColumnWidth)
                        {
                            this.JobsCheckedListBox.ColumnWidth = (int)(this.DummyCheckBox.Width * 1.5d);
                        }
                    }
                }
            }
            finally
            {
                this.JobsCheckedListBox.ResumeLayout();
            }
        }

        /// <summary>
        /// Shown
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SelectJobForm_Shown(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                this.Font = this.Owner.Font;
            }
        }
    }
}
