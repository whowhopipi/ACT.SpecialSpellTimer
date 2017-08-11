namespace ACT.SpecialSpellTimer.Forms
{
    using System;
    using System.Windows.Forms;

    public partial class VisualSettingControlBackgoundColorForm : Form
    {
        public VisualSettingControlBackgoundColorForm()
        {
            this.InitializeComponent();
            Utility.Translate.TranslateControls(this);

            this.OpacityNumericUpDown.ValueChanged += (s1, e1) =>
            {
                this.AlphaRateLabel.Text =
                    (this.OpacityNumericUpDown.Value / 255m * 100m).ToString("N0") + "%";
            };

            this.Load += (s, e) =>
            {
                this.OpacityNumericUpDown.Value = this.Alpha;
                this.OpacityNumericUpDown.Focus();
            };
        }

        public int Alpha { get; set; }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Alpha = (int)this.OpacityNumericUpDown.Value;
        }
    }
}
