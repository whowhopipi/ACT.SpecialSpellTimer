namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

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
            Utility.Translate.TranslateControls(this);

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
            var items = this.ZoneFilter.Split(',');

            this.ZonesCheckedListBox.Items.Clear();
            foreach (var item in FF14PluginHelper.GetZoneList())
            {
                this.ZonesCheckedListBox.Items.Add(
                    item,
                    items.Any(x => x == item.ID.ToString()));
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