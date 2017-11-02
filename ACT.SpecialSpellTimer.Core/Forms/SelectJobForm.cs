using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
            foreach (Job item in this.JobsCheckedListBox.CheckedItems)
            {
                jobs.Add(item.ID.ToString());
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

            this.JobsCheckedListBox.Items.Clear();
            foreach (var job in Jobs.List)
            {
                this.JobsCheckedListBox.Items.Add(
                    job,
                    jobs.Any(x => x == job.ID.ToString()));
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
