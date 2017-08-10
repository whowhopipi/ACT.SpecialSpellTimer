namespace ACT.SpecialSpellTimer
{
    partial class SelectIconUserControl
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
            this.IconLabel = new System.Windows.Forms.Label();
            this.IconButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // IconLabel
            // 
            this.IconLabel.Location = new System.Drawing.Point(3, 76);
            this.IconLabel.Name = "IconLabel";
            this.IconLabel.Size = new System.Drawing.Size(108, 27);
            this.IconLabel.TabIndex = 4;
            this.IconLabel.Text = "サンプルテキスト";
            this.IconLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // IconButton
            // 
            this.IconButton.BackColor = System.Drawing.Color.Transparent;
            this.IconButton.FlatAppearance.BorderSize = 0;
            this.IconButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Highlight;
            this.IconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconButton.Location = new System.Drawing.Point(25, 7);
            this.IconButton.Name = "IconButton";
            this.IconButton.Size = new System.Drawing.Size(64, 64);
            this.IconButton.TabIndex = 3;
            this.IconButton.UseVisualStyleBackColor = false;
            // 
            // SelectIconUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.IconLabel);
            this.Controls.Add(this.IconButton);
            this.Name = "SelectIconUserControl";
            this.Size = new System.Drawing.Size(114, 103);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label IconLabel;
        private System.Windows.Forms.Button IconButton;
    }
}
