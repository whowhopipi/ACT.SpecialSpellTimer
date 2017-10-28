namespace ACT.SpecialSpellTimer.Forms
{
    partial class SelectIconForm
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
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.FolderTreeView = new System.Windows.Forms.TreeView();
            this.IconsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.BottomTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ClearButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.BottomTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Controls.Add(this.SplitContainer, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.BottomTableLayoutPanel, 0, 1);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 2;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(784, 411);
            this.TableLayoutPanel.TabIndex = 4;
            // 
            // SplitContainer
            // 
            this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer.Location = new System.Drawing.Point(3, 3);
            this.SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.FolderTreeView);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.IconsFlowLayoutPanel);
            this.SplitContainer.Size = new System.Drawing.Size(778, 374);
            this.SplitContainer.SplitterDistance = 248;
            this.SplitContainer.TabIndex = 5;
            // 
            // FolderTreeView
            // 
            this.FolderTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FolderTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FolderTreeView.FullRowSelect = true;
            this.FolderTreeView.HideSelection = false;
            this.FolderTreeView.ItemHeight = 20;
            this.FolderTreeView.Location = new System.Drawing.Point(0, 0);
            this.FolderTreeView.Name = "FolderTreeView";
            this.FolderTreeView.ShowLines = false;
            this.FolderTreeView.Size = new System.Drawing.Size(248, 374);
            this.FolderTreeView.TabIndex = 0;
            // 
            // IconsFlowLayoutPanel
            // 
            this.IconsFlowLayoutPanel.AutoScroll = true;
            this.IconsFlowLayoutPanel.AutoScrollMargin = new System.Drawing.Size(0, 60);
            this.IconsFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IconsFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.IconsFlowLayoutPanel.Name = "IconsFlowLayoutPanel";
            this.IconsFlowLayoutPanel.Size = new System.Drawing.Size(526, 374);
            this.IconsFlowLayoutPanel.TabIndex = 0;
            // 
            // BottomTableLayoutPanel
            // 
            this.BottomTableLayoutPanel.AutoSize = true;
            this.BottomTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BottomTableLayoutPanel.ColumnCount = 2;
            this.BottomTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BottomTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BottomTableLayoutPanel.Controls.Add(this.ClearButton, 0, 0);
            this.BottomTableLayoutPanel.Controls.Add(this.CloseButton, 1, 0);
            this.BottomTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomTableLayoutPanel.Location = new System.Drawing.Point(0, 380);
            this.BottomTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BottomTableLayoutPanel.Name = "BottomTableLayoutPanel";
            this.BottomTableLayoutPanel.RowCount = 1;
            this.BottomTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.BottomTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.BottomTableLayoutPanel.Size = new System.Drawing.Size(784, 31);
            this.BottomTableLayoutPanel.TabIndex = 6;
            // 
            // ClearButton
            // 
            this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ClearButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ClearButton.Location = new System.Drawing.Point(3, 3);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(118, 25);
            this.ClearButton.TabIndex = 9;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(663, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(118, 25);
            this.CloseButton.TabIndex = 10;
            this.CloseButton.Text = "Cancel";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // SelectIconForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.TableLayoutPanel);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectIconForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ICON..";
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.BottomTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.TreeView FolderTreeView;
        private System.Windows.Forms.FlowLayoutPanel IconsFlowLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel BottomTableLayoutPanel;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Button CloseButton;
    }
}