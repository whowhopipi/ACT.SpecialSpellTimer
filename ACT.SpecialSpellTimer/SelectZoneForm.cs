using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// ゾーン選択Form
    /// </summary>
    public partial class SelectZoneForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SelectZoneForm()
        {
            this.InitializeComponent();

            Translate.TranslateControls(this);

            // ListViewのダブルバッファリングを有効にする
            typeof(CheckedListBox)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(this.ZonesCheckedListBox, true, null);

            this.Load += this.FormLoad;
            this.OKButton.Click += this.OKButton_Click;

            this.AllONButton.Click += (s1, e1) =>
            {
                for (int i = 0; i < this.ZonesCheckedListBox.Items.Count; i++)
                {
                    this.ZonesCheckedListBox.SetItemChecked(i, true);
                }
            };

            this.AllOFFButton.Click += (s1, e1) =>
            {
                for (int i = 0; i < this.ZonesCheckedListBox.Items.Count; i++)
                {
                    this.ZonesCheckedListBox.SetItemChecked(i, false);
                }
            };
        }

        /// <summary>
        /// ゾーンフィルタ
        /// </summary>
        public string ZoneFilter { get; set; }

        /// <summary>
        /// ロード
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void FormLoad(object sender, EventArgs e)
        {
            var zoneFilters = this.ZoneFilter.Split(',');

            try
            {
                this.ZonesCheckedListBox.SuspendLayout();
                this.ZonesCheckedListBox.Items.Clear();

                var query =
                    from x in FFXIV.Instance.ZoneList
                    orderby
                    x.IsAddedByUser ? 0 : 1,
                    x.Rank,
                    x.ID descending
                    select
                    x;

                foreach (var zone in query)
                {
                    this.ZonesCheckedListBox.Items.Add(
                        zone,
                        zoneFilters.Any(x => x == zone.ID.ToString()));
                }
            }
            finally
            {
                this.ZonesCheckedListBox.ResumeLayout();
            }
        }

        /// <summary>
        /// OKボタン Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            var items = new List<string>();
            foreach (Zone item in this.ZonesCheckedListBox.CheckedItems)
            {
                items.Add(item.ID.ToString());
            }

            this.ZoneFilter = string.Join(
                ",",
                items.ToArray());
        }

        /// <summary>
        /// Shown
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SelectZoneForm_Shown(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                this.Font = this.Owner.Font;
            }
        }
    }
}
