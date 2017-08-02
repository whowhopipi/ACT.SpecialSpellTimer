namespace ACT.SpecialSpellTimer
{
    partial class SetConditionForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SpellMustStoppingTreeView = new System.Windows.Forms.TreeView();
            this.SpellMustRunningTreeView = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TelopMustStoppingTreeView = new System.Windows.Forms.TreeView();
            this.TelopMustRunningTreeView = new System.Windows.Forms.TreeView();
            this.CloseButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.AllOFFButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.SpellMustStoppingTreeView);
            this.groupBox1.Controls.Add(this.SpellMustRunningTreeView);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 485);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SpellTimerTabTitle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "IsStopping";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "IsRunning";
            // 
            // SpellMustStoppingTreeView
            // 
            this.SpellMustStoppingTreeView.CheckBoxes = true;
            this.SpellMustStoppingTreeView.Location = new System.Drawing.Point(244, 53);
            this.SpellMustStoppingTreeView.Name = "SpellMustStoppingTreeView";
            this.SpellMustStoppingTreeView.ShowNodeToolTips = true;
            this.SpellMustStoppingTreeView.Size = new System.Drawing.Size(230, 423);
            this.SpellMustStoppingTreeView.TabIndex = 1;
            // 
            // SpellMustRunningTreeView
            // 
            this.SpellMustRunningTreeView.CheckBoxes = true;
            this.SpellMustRunningTreeView.Location = new System.Drawing.Point(6, 53);
            this.SpellMustRunningTreeView.Name = "SpellMustRunningTreeView";
            this.SpellMustRunningTreeView.ShowNodeToolTips = true;
            this.SpellMustRunningTreeView.Size = new System.Drawing.Size(230, 423);
            this.SpellMustRunningTreeView.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.TelopMustStoppingTreeView);
            this.groupBox2.Controls.Add(this.TelopMustRunningTreeView);
            this.groupBox2.Location = new System.Drawing.Point(516, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 485);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "TelopTabPageTitle";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(254, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "IsStopping";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "IsRunning";
            // 
            // TelopMustStoppingTreeView
            // 
            this.TelopMustStoppingTreeView.CheckBoxes = true;
            this.TelopMustStoppingTreeView.Location = new System.Drawing.Point(244, 53);
            this.TelopMustStoppingTreeView.Name = "TelopMustStoppingTreeView";
            this.TelopMustStoppingTreeView.ShowNodeToolTips = true;
            this.TelopMustStoppingTreeView.Size = new System.Drawing.Size(230, 423);
            this.TelopMustStoppingTreeView.TabIndex = 1;
            // 
            // TelopMustRunningTreeView
            // 
            this.TelopMustRunningTreeView.CheckBoxes = true;
            this.TelopMustRunningTreeView.Location = new System.Drawing.Point(6, 53);
            this.TelopMustRunningTreeView.Name = "TelopMustRunningTreeView";
            this.TelopMustRunningTreeView.ShowNodeToolTips = true;
            this.TelopMustRunningTreeView.Size = new System.Drawing.Size(230, 423);
            this.TelopMustRunningTreeView.TabIndex = 0;
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(878, 510);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(118, 25);
            this.CloseButton.TabIndex = 2;
            this.CloseButton.Text = "CancelButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(754, 510);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(118, 25);
            this.OKButton.TabIndex = 3;
            this.OKButton.Text = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // AllOFFButton
            // 
            this.AllOFFButton.Location = new System.Drawing.Point(12, 510);
            this.AllOFFButton.Name = "AllOFFButton";
            this.AllOFFButton.Size = new System.Drawing.Size(118, 25);
            this.AllOFFButton.TabIndex = 4;
            this.AllOFFButton.Text = "AllOffButton";
            this.AllOFFButton.UseVisualStyleBackColor = true;
            this.AllOFFButton.Click += new System.EventHandler(this.AllOFFButton_Click);
            // 
            // SetConditionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1008, 547);
            this.Controls.Add(this.AllOFFButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SetConditionForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SetConditionTitle";
            this.Load += new System.EventHandler(this.SelectConditionForm_Load);
            this.Shown += new System.EventHandler(this.SetConditionForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView SpellMustStoppingTreeView;
        private System.Windows.Forms.TreeView SpellMustRunningTreeView;
        private System.Windows.Forms.TreeView TelopMustStoppingTreeView;
        private System.Windows.Forms.TreeView TelopMustRunningTreeView;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button AllOFFButton;
    }
}