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
            this.PlaceholderListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.AutoRefreshCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.ActiveTriggerCountTextBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SpellsListView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Controls.Add(this.tabPage2);
            this.TabControl.Location = new System.Drawing.Point(0, 52);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(800, 548);
            this.TabControl.TabIndex = 45;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.PlaceholderListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 522);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "__Placeholders";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            this.PlaceholderListView.Location = new System.Drawing.Point(3, 3);
            this.PlaceholderListView.Name = "PlaceholderListView";
            this.PlaceholderListView.Size = new System.Drawing.Size(786, 516);
            this.PlaceholderListView.TabIndex = 47;
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 522);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "__Spells & Tickers　";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // AutoRefreshCheckBox
            // 
            this.AutoRefreshCheckBox.AutoSize = true;
            this.AutoRefreshCheckBox.Checked = true;
            this.AutoRefreshCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoRefreshCheckBox.Location = new System.Drawing.Point(6, 30);
            this.AutoRefreshCheckBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 3);
            this.AutoRefreshCheckBox.Name = "AutoRefreshCheckBox";
            this.AutoRefreshCheckBox.Size = new System.Drawing.Size(96, 16);
            this.AutoRefreshCheckBox.TabIndex = 46;
            this.AutoRefreshCheckBox.Text = "__Auto refresh";
            this.AutoRefreshCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.ActiveTriggerCountTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 35);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "__Active trigger count";
            // 
            // ActiveTriggerCountTextBox
            // 
            this.ActiveTriggerCountTextBox.Location = new System.Drawing.Point(124, 8);
            this.ActiveTriggerCountTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8);
            this.ActiveTriggerCountTextBox.Name = "ActiveTriggerCountTextBox";
            this.ActiveTriggerCountTextBox.ReadOnly = true;
            this.ActiveTriggerCountTextBox.Size = new System.Drawing.Size(47, 19);
            this.ActiveTriggerCountTextBox.TabIndex = 1;
            this.ActiveTriggerCountTextBox.Text = "1,000";
            this.ActiveTriggerCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.SpellsListView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 38);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(786, 481);
            this.panel2.TabIndex = 2;
            // 
            // SpellsListView
            // 
            this.SpellsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SpellsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.SpellsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpellsListView.FullRowSelect = true;
            this.SpellsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SpellsListView.Location = new System.Drawing.Point(0, 0);
            this.SpellsListView.Name = "SpellsListView";
            this.SpellsListView.Size = new System.Drawing.Size(786, 481);
            this.SpellsListView.TabIndex = 1;
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
            // columnHeader8
            // 
            this.columnHeader8.Text = "__Cost";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ConfigPanelLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AutoRefreshCheckBox);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.HeaderLabel);
            this.Name = "ConfigPanelLog";
            this.Size = new System.Drawing.Size(800, 600);
            this.TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label HeaderLabel;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox AutoRefreshCheckBox;
        private System.Windows.Forms.ListView PlaceholderListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ActiveTriggerCountTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView SpellsListView;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
    }
}
