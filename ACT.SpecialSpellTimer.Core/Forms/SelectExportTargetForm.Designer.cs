namespace ACT.SpecialSpellTimer.Forms
{
    partial class SelectExportTargetForm
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
            this.CloseButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.SelectionRadioButton = new System.Windows.Forms.RadioButton();
            this.AllRadioButton = new System.Windows.Forms.RadioButton();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.SelectionPanel = new System.Windows.Forms.Panel();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SelectionListView = new System.Windows.Forms.ListView();
            this.ItemColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TopPanel.SuspendLayout();
            this.BottomPanel.SuspendLayout();
            this.SelectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(379, 5);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(102, 23);
            this.CloseButton.TabIndex = 1;
            this.CloseButton.Text = "Cancel";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(271, 5);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(102, 23);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // TopPanel
            // 
            this.TopPanel.AutoSize = true;
            this.TopPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TopPanel.Controls.Add(this.SelectionRadioButton);
            this.TopPanel.Controls.Add(this.AllRadioButton);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(484, 55);
            this.TopPanel.TabIndex = 3;
            // 
            // SelectionRadioButton
            // 
            this.SelectionRadioButton.AutoSize = true;
            this.SelectionRadioButton.Location = new System.Drawing.Point(20, 36);
            this.SelectionRadioButton.Name = "SelectionRadioButton";
            this.SelectionRadioButton.Size = new System.Drawing.Size(70, 16);
            this.SelectionRadioButton.TabIndex = 1;
            this.SelectionRadioButton.Text = "Selection";
            this.SelectionRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllRadioButton
            // 
            this.AllRadioButton.AutoSize = true;
            this.AllRadioButton.Checked = true;
            this.AllRadioButton.Location = new System.Drawing.Point(20, 13);
            this.AllRadioButton.Name = "AllRadioButton";
            this.AllRadioButton.Size = new System.Drawing.Size(43, 16);
            this.AllRadioButton.TabIndex = 0;
            this.AllRadioButton.TabStop = true;
            this.AllRadioButton.Text = "ALL";
            this.AllRadioButton.UseVisualStyleBackColor = true;
            // 
            // BottomPanel
            // 
            this.BottomPanel.AutoSize = true;
            this.BottomPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BottomPanel.Controls.Add(this.OKButton);
            this.BottomPanel.Controls.Add(this.CloseButton);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(0, 280);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(484, 31);
            this.BottomPanel.TabIndex = 5;
            // 
            // SelectionPanel
            // 
            this.SelectionPanel.AutoSize = true;
            this.SelectionPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SelectionPanel.Controls.Add(this.SelectionListView);
            this.SelectionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectionPanel.Location = new System.Drawing.Point(0, 55);
            this.SelectionPanel.Name = "SelectionPanel";
            this.SelectionPanel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 6);
            this.SelectionPanel.Size = new System.Drawing.Size(484, 225);
            this.SelectionPanel.TabIndex = 6;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            this.saveFileDialog.RestoreDirectory = true;
            this.saveFileDialog.SupportMultiDottedExtensions = true;
            this.saveFileDialog.Title = "Export ...";
            // 
            // SelectionListView
            // 
            this.SelectionListView.BackColor = System.Drawing.SystemColors.Window;
            this.SelectionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ItemColumnHeader});
            this.SelectionListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectionListView.FullRowSelect = true;
            this.SelectionListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.SelectionListView.Location = new System.Drawing.Point(10, 8);
            this.SelectionListView.Name = "SelectionListView";
            this.SelectionListView.Size = new System.Drawing.Size(464, 211);
            this.SelectionListView.TabIndex = 1;
            this.SelectionListView.UseCompatibleStateImageBehavior = false;
            this.SelectionListView.View = System.Windows.Forms.View.List;
            // 
            // ItemColumnHeader
            // 
            this.ItemColumnHeader.Text = "Item";
            this.ItemColumnHeader.Width = 220;
            // 
            // SelectExportTargetForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(484, 311);
            this.Controls.Add(this.SelectionPanel);
            this.Controls.Add(this.BottomPanel);
            this.Controls.Add(this.TopPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectExportTargetForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export ...";
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            this.BottomPanel.ResumeLayout(false);
            this.SelectionPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.RadioButton SelectionRadioButton;
        private System.Windows.Forms.RadioButton AllRadioButton;
        private System.Windows.Forms.Panel BottomPanel;
        private System.Windows.Forms.Panel SelectionPanel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ListView SelectionListView;
        private System.Windows.Forms.ColumnHeader ItemColumnHeader;
    }
}