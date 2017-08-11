namespace ACT.SpecialSpellTimer.Forms
{
    partial class ConfigPanelLog
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.HeaderLabel = new System.Windows.Forms.Label();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.PlaceholderListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.SpellsListView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // HeaderLabel
            // 
            this.HeaderLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.HeaderLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderLabel.Location = new System.Drawing.Point(0, 0);
            this.HeaderLabel.Name = "HeaderLabel";
            this.HeaderLabel.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.HeaderLabel.Size = new System.Drawing.Size(800, 24);
            this.HeaderLabel.TabIndex = 44;
            this.HeaderLabel.Text = "__DEBUG Info";
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Controls.Add(this.tabPage2);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 24);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(800, 576);
            this.TabControl.TabIndex = 45;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.SplitContainer);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 550);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "__Placeholders & Logs";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.SpellsListView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 550);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "__Spells & Tickers";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // SplitContainer
            // 
            this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer.Location = new System.Drawing.Point(3, 3);
            this.SplitContainer.Name = "SplitContainer";
            this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.PlaceholderListView);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.LogTextBox);
            this.SplitContainer.Size = new System.Drawing.Size(786, 544);
            this.SplitContainer.SplitterDistance = 251;
            this.SplitContainer.TabIndex = 46;
            // 
            // PlaceholderListView
            // 
            this.PlaceholderListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PlaceholderListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.PlaceholderListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlaceholderListView.FullRowSelect = true;
            this.PlaceholderListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PlaceholderListView.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.PlaceholderListView.Location = new System.Drawing.Point(0, 0);
            this.PlaceholderListView.Name = "PlaceholderListView";
            this.PlaceholderListView.Size = new System.Drawing.Size(786, 251);
            this.PlaceholderListView.TabIndex = 46;
            this.PlaceholderListView.UseCompatibleStateImageBehavior = false;
            this.PlaceholderListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "__Placeholder";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "__Value";
            this.columnHeader2.Width = 430;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "__Type";
            this.columnHeader3.Width = 100;
            // 
            // LogTextBox
            // 
            this.LogTextBox.BackColor = System.Drawing.Color.White;
            this.LogTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTextBox.Location = new System.Drawing.Point(0, 0);
            this.LogTextBox.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(786, 289);
            this.LogTextBox.TabIndex = 44;
            // 
            // SpellsListView
            // 
            this.SpellsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SpellsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.SpellsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpellsListView.FullRowSelect = true;
            this.SpellsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SpellsListView.Location = new System.Drawing.Point(3, 3);
            this.SpellsListView.Name = "SpellsListView";
            this.SpellsListView.Size = new System.Drawing.Size(786, 544);
            this.SpellsListView.TabIndex = 0;
            this.SpellsListView.UseCompatibleStateImageBehavior = false;
            this.SpellsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "__Type";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "__Title";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "__Pattern";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "__Regex";
            // 
            // ConfigPanelLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.HeaderLabel);
            this.Name = "ConfigPanelLog";
            this.Size = new System.Drawing.Size(800, 600);
            this.TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            this.SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label HeaderLabel;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.ListView PlaceholderListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView SpellsListView;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
    }
}
