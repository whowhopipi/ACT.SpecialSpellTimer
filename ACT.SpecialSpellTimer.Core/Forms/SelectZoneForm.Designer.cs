namespace ACT.SpecialSpellTimer.Forms
{
    partial class SelectZoneForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ZonesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.AllONButton = new System.Windows.Forms.Button();
            this.AllOFFButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ZonesCheckedListBox
            // 
            this.ZonesCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ZonesCheckedListBox.BackColor = System.Drawing.SystemColors.Control;
            this.ZonesCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ZonesCheckedListBox.CheckOnClick = true;
            this.ZonesCheckedListBox.ColumnWidth = 400;
            this.ZonesCheckedListBox.FormattingEnabled = true;
            this.ZonesCheckedListBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ZonesCheckedListBox.Location = new System.Drawing.Point(12, 40);
            this.ZonesCheckedListBox.MultiColumn = true;
            this.ZonesCheckedListBox.Name = "ZonesCheckedListBox";
            this.ZonesCheckedListBox.Size = new System.Drawing.Size(920, 448);
            this.ZonesCheckedListBox.TabIndex = 0;
            this.ZonesCheckedListBox.ThreeDCheckBoxes = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(732, 520);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(97, 28);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(835, 520);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(97, 28);
            this.CloseButton.TabIndex = 2;
            this.CloseButton.Text = "CancelButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // AllONButton
            // 
            this.AllONButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AllONButton.Location = new System.Drawing.Point(12, 520);
            this.AllONButton.Name = "AllONButton";
            this.AllONButton.Size = new System.Drawing.Size(75, 28);
            this.AllONButton.TabIndex = 3;
            this.AllONButton.Text = "AllOnButton";
            this.AllONButton.UseVisualStyleBackColor = true;
            // 
            // AllOFFButton
            // 
            this.AllOFFButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AllOFFButton.Location = new System.Drawing.Point(93, 520);
            this.AllOFFButton.Name = "AllOFFButton";
            this.AllOFFButton.Size = new System.Drawing.Size(75, 28);
            this.AllOFFButton.TabIndex = 4;
            this.AllOFFButton.Text = "AllOffButton";
            this.AllOFFButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "SelectZoneSubtitle";
            // 
            // SelectZoneForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(944, 561);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AllOFFButton);
            this.Controls.Add(this.AllONButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.ZonesCheckedListBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectZoneForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SelectZoneTitle";
            this.Shown += new System.EventHandler(this.SelectZoneForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox ZonesCheckedListBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button AllONButton;
        private System.Windows.Forms.Button AllOFFButton;
        private System.Windows.Forms.Label label1;
    }
}