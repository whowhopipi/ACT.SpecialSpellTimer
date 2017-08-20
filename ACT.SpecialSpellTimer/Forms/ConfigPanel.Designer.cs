namespace ACT.SpecialSpellTimer.Forms
{
    partial class ConfigPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
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
            this.components = new System.ComponentModel.Container();
            this.CombatAnalyzerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CASelectAllItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.CACopyLogItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CACopyLogDetailItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CASetOriginItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.CombatAnalyzingTimer = new System.Windows.Forms.Timer(this.components);
            this.EnabledSpellTimerNoDecimal = new System.Windows.Forms.CheckBox();
            this.TabControl = new ACT.SpecialSpellTimer.Forms.TabControlExt();
            this.SpecialSpellTabPage = new System.Windows.Forms.TabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.DetailPanelGroupBox = new System.Windows.Forms.GroupBox();
            this.FixedPositionSpellCheckBox = new System.Windows.Forms.CheckBox();
            this.HorizontalLayoutCheckBox = new System.Windows.Forms.CheckBox();
            this.label62 = new System.Windows.Forms.Label();
            this.MarginUpDown = new System.Windows.Forms.NumericUpDown();
            this.PanelTopNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.PanelLeftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.UpdatePanelButton = new System.Windows.Forms.Button();
            this.SpellTimerTreeView = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ExportButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearAllButton = new System.Windows.Forms.ToolStripMenuItem();
            this.AddButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DetailGroupBox = new System.Windows.Forms.Panel();
            this.tabControlExtHoriz2 = new ACT.SpecialSpellTimer.Forms.TabControlExtHoriz();
            this.GeneralTab = new System.Windows.Forms.TabPage();
            this.SpellDetailPanel = new System.Windows.Forms.Panel();
            this.SelectIconButton = new System.Windows.Forms.Button();
            this.label58 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.WarningTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.WarningTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label105 = new System.Windows.Forms.Label();
            this.PanelNameTextBox = new System.Windows.Forms.TextBox();
            this.ExpandSecounds1NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.KeywordToExpand1TextBox = new System.Windows.Forms.TextBox();
            this.KeywordToExpand2TextBox = new System.Windows.Forms.TextBox();
            this.RegexEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.ExpandSecounds2NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.SpellTitleTextBox = new System.Windows.Forms.TextBox();
            this.DisplayNoNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.SpellIconSizeUpDown = new System.Windows.Forms.NumericUpDown();
            this.KeywordTextBox = new System.Windows.Forms.TextBox();
            this.RecastTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.UpperLimitOfExtensionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.RepeatCheckBox = new System.Windows.Forms.CheckBox();
            this.SpellVisualSetting = new ACT.SpecialSpellTimer.Forms.VisualSettingControl();
            this.SetConditionButton = new System.Windows.Forms.Button();
            this.ToInstanceCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.ReduceIconBrightnessCheckBox = new System.Windows.Forms.CheckBox();
            this.label50 = new System.Windows.Forms.Label();
            this.SelectZoneButton = new System.Windows.Forms.Button();
            this.ExtendBeyondOriginalRecastTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.SelectJobButton = new System.Windows.Forms.Button();
            this.OverlapRecastTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DontHideCheckBox = new System.Windows.Forms.CheckBox();
            this.HideSpellNameCheckBox = new System.Windows.Forms.CheckBox();
            this.label55 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.IsReverseCheckBox = new System.Windows.Forms.CheckBox();
            this.ShowProgressBarCheckBox = new System.Windows.Forms.CheckBox();
            this.label60 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.AlarmTab = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BeforeTextToSpeakTextBox = new System.Windows.Forms.TextBox();
            this.BeforeSoundComboBox = new System.Windows.Forms.ComboBox();
            this.label52 = new System.Windows.Forms.Label();
            this.BeforeTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Speak4Button = new System.Windows.Forms.Button();
            this.Play4Button = new System.Windows.Forms.Button();
            this.label53 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MatchTextToSpeakTextBox = new System.Windows.Forms.TextBox();
            this.MatchSoundComboBox = new System.Windows.Forms.ComboBox();
            this.Speak1Button = new System.Windows.Forms.Button();
            this.Play1Button = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.OverTextToSpeakTextBox = new System.Windows.Forms.TextBox();
            this.OverSoundComboBox = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.OverTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Speak2Button = new System.Windows.Forms.Button();
            this.Play2Button = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TimeupTextToSpeakTextBox = new System.Windows.Forms.TextBox();
            this.TimeupSoundComboBox = new System.Windows.Forms.ComboBox();
            this.Speak3Button = new System.Windows.Forms.Button();
            this.Play3Button = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SpellButtonsPanel = new System.Windows.Forms.Panel();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.OnPointTelopTabPage = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.TelopTreeView = new System.Windows.Forms.TreeView();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.TelopExportButton = new System.Windows.Forms.ToolStripMenuItem();
            this.TelopImportButton = new System.Windows.Forms.ToolStripMenuItem();
            this.TelopClearAllButton = new System.Windows.Forms.ToolStripMenuItem();
            this.TelopAddButton = new System.Windows.Forms.ToolStripMenuItem();
            this.TelopDetailGroupBox = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.TelopUpdateButton = new System.Windows.Forms.Button();
            this.TelopDeleteButton = new System.Windows.Forms.Button();
            this.tabControlExtHoriz3 = new ACT.SpecialSpellTimer.Forms.TabControlExtHoriz();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel6 = new System.Windows.Forms.Panel();
            this.TelopTitleTextBox = new System.Windows.Forms.TextBox();
            this.TelopMessageTextBox = new System.Windows.Forms.TextBox();
            this.TelopKeywordTextBox = new System.Windows.Forms.TextBox();
            this.DisplayTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TelopKeywordToHideTextBox = new System.Windows.Forms.TextBox();
            this.TelopDelayNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TelopSetConditionButton = new System.Windows.Forms.Button();
            this.label47 = new System.Windows.Forms.Label();
            this.TelopSelectZoneButton = new System.Windows.Forms.Button();
            this.TelopVisualSetting = new ACT.SpecialSpellTimer.Forms.VisualSettingControl();
            this.TelopSelectJobButton = new System.Windows.Forms.Button();
            this.label46 = new System.Windows.Forms.Label();
            this.TelopProgressBarEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.EnabledAddMessageCheckBox = new System.Windows.Forms.CheckBox();
            this.label45 = new System.Windows.Forms.Label();
            this.TelopTopNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label35 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.TelopLeftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.TelopRegexEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.label44 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.TelopDelayTTSTextBox = new System.Windows.Forms.TextBox();
            this.TelopDelaySoundComboBox = new System.Windows.Forms.ComboBox();
            this.TelopSpeak2Button = new System.Windows.Forms.Button();
            this.TelopPlay2Button = new System.Windows.Forms.Button();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.TelopMatchTTSTextBox = new System.Windows.Forms.TextBox();
            this.TelopMatchSoundComboBox = new System.Windows.Forms.ComboBox();
            this.TelopSpeak1Button = new System.Windows.Forms.Button();
            this.TelopPlay1Button = new System.Windows.Forms.Button();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.CombatAnalyzerTabPage = new System.Windows.Forms.TabPage();
            this.CombatLogListView = new System.Windows.Forms.ListView();
            this.DummyColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NoColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TimeStampColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ElapsedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LogTypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ActorColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HPRateColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ActionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SpanColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LogColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.CombatLogEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.CombatLogBufferSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ExportCSVButton = new System.Windows.Forms.Button();
            this.AnalyzeCombatButton = new System.Windows.Forms.Button();
            this.CombatAnalyzingLabel = new System.Windows.Forms.Label();
            this.lblCombatAnalyzer = new System.Windows.Forms.Label();
            this.NameStyleTabPage = new System.Windows.Forms.TabPage();
            this.OptionTabPage = new System.Windows.Forms.TabPage();
            this.tabControlExtHoriz1 = new ACT.SpecialSpellTimer.Forms.TabControlExtHoriz();
            this.tabOverlayOptions = new System.Windows.Forms.TabPage();
            this.RenderWithCPUOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label107 = new System.Windows.Forms.Label();
            this.label106 = new System.Windows.Forms.Label();
            this.TextBlurRateNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TextOutlineThicknessRateNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.DefaultVisualSetting = new ACT.SpecialSpellTimer.Forms.VisualSettingControl();
            this.label18 = new System.Windows.Forms.Label();
            this.OpacityNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.ClickThroughCheckBox = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.EnabledSpellTimerNoDecimalCheckBox = new System.Windows.Forms.CheckBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label93 = new System.Windows.Forms.Label();
            this.UseOtherThanFFXIVCheckbox = new System.Windows.Forms.CheckBox();
            this.ReduceIconBrightnessNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label94 = new System.Windows.Forms.Label();
            this.SwitchOverlayButton = new System.Windows.Forms.Button();
            this.SwitchTelopButton = new System.Windows.Forms.Button();
            this.OverlayForceVisibleCheckBox = new System.Windows.Forms.CheckBox();
            this.HideWhenNotActiceCheckBox = new System.Windows.Forms.CheckBox();
            this.tabDetailOptions = new System.Windows.Forms.TabPage();
            this.ToComplementUnknownSkillCheckBox = new System.Windows.Forms.CheckBox();
            this.label57 = new System.Windows.Forms.Label();
            this.NotifyToACTCheckBox = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.AutoSortCheckBox = new System.Windows.Forms.CheckBox();
            this.TimeOfHideNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label104 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.DetectPacketDumpcheckBox = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.RemoveTooltipSymbolsCheckBox = new System.Windows.Forms.CheckBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label103 = new System.Windows.Forms.Label();
            this.AutoSortReverseCheckBox = new System.Windows.Forms.CheckBox();
            this.SimpleRegexCheckBox = new System.Windows.Forms.CheckBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label102 = new System.Windows.Forms.Label();
            this.RefreshIntervalNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ResetOnWipeOutLabel = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.ResetOnWipeOutCheckBox = new System.Windows.Forms.CheckBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label92 = new System.Windows.Forms.Label();
            this.EnabledPTPlaceholderCheckBox = new System.Windows.Forms.CheckBox();
            this.LogPollSleepNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label48 = new System.Windows.Forms.Label();
            this.label91 = new System.Windows.Forms.Label();
            this.label63 = new System.Windows.Forms.Label();
            this.SaveLogButton = new System.Windows.Forms.Button();
            this.EnabledNotifyNormalSpellTimerCheckBox = new System.Windows.Forms.CheckBox();
            this.SaveLogCheckBox = new System.Windows.Forms.CheckBox();
            this.label64 = new System.Windows.Forms.Label();
            this.SaveLogTextBox = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.ReadyTextBox = new System.Windows.Forms.TextBox();
            this.OverTextBox = new System.Windows.Forms.TextBox();
            this.label66 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.TekiyoButton = new System.Windows.Forms.Button();
            this.ShokikaButton = new System.Windows.Forms.Button();
            this.pnlLanguage = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.LanguageComboBox = new System.Windows.Forms.ComboBox();
            this.LanguageRestartLabel = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.DQXOptionTabPage = new System.Windows.Forms.TabPage();
            this.lblDQX = new System.Windows.Forms.Label();
            this.DQXOptionEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.label101 = new System.Windows.Forms.Label();
            this.label100 = new System.Windows.Forms.Label();
            this.label99 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.label97 = new System.Windows.Forms.Label();
            this.label96 = new System.Windows.Forms.Label();
            this.label95 = new System.Windows.Forms.Label();
            this.DQXPTMember8TextBox = new System.Windows.Forms.TextBox();
            this.DQXPTMember7TextBox = new System.Windows.Forms.TextBox();
            this.DQXPTMember6TextBox = new System.Windows.Forms.TextBox();
            this.DQXPTMember5TextBox = new System.Windows.Forms.TextBox();
            this.DQXPTMember4TextBox = new System.Windows.Forms.TextBox();
            this.DQXPTMember3TextBox = new System.Windows.Forms.TextBox();
            this.DQXPTMember2TextBox = new System.Windows.Forms.TextBox();
            this.DQXPlayerNameTextBox = new System.Windows.Forms.TextBox();
            this.DQXPlayerNameLabel = new System.Windows.Forms.Label();
            this.DQXAppleyButton = new System.Windows.Forms.Button();
            this.LogTabPage = new System.Windows.Forms.TabPage();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.BlinkTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label68 = new System.Windows.Forms.Label();
            this.CombatAnalyzerContextMenuStrip.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.SpecialSpellTabPage.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.DetailPanelGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MarginUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PanelTopNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PanelLeftNumericUpDown)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.DetailGroupBox.SuspendLayout();
            this.tabControlExtHoriz2.SuspendLayout();
            this.GeneralTab.SuspendLayout();
            this.SpellDetailPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WarningTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpandSecounds1NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpandSecounds2NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayNoNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpellIconSizeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecastTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpperLimitOfExtensionNumericUpDown)).BeginInit();
            this.AlarmTab.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BeforeTimeNumericUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverTimeNumericUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SpellButtonsPanel.SuspendLayout();
            this.OnPointTelopTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.TelopDetailGroupBox.SuspendLayout();
            this.panel9.SuspendLayout();
            this.tabControlExtHoriz3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TelopDelayNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TelopTopNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TelopLeftNumericUpDown)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.CombatAnalyzerTabPage.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CombatLogBufferSizeNumericUpDown)).BeginInit();
            this.panel1.SuspendLayout();
            this.OptionTabPage.SuspendLayout();
            this.tabControlExtHoriz1.SuspendLayout();
            this.tabOverlayOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextBlurRateNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextOutlineThicknessRateNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpacityNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReduceIconBrightnessNumericUpDown)).BeginInit();
            this.tabDetailOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeOfHideNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RefreshIntervalNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogPollSleepNumericUpDown)).BeginInit();
            this.panel7.SuspendLayout();
            this.pnlLanguage.SuspendLayout();
            this.DQXOptionTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlinkTimeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // CombatAnalyzerContextMenuStrip
            // 
            this.CombatAnalyzerContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.CombatAnalyzerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CASelectAllItem,
            this.toolStripSeparator1,
            this.CACopyLogItem,
            this.CACopyLogDetailItem,
            this.toolStripSeparator2,
            this.CASetOriginItem});
            this.CombatAnalyzerContextMenuStrip.Name = "CombatAnalyzerContextMenuStrip";
            this.CombatAnalyzerContextMenuStrip.Size = new System.Drawing.Size(224, 104);
            // 
            // CASelectAllItem
            // 
            this.CASelectAllItem.Name = "CASelectAllItem";
            this.CASelectAllItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.CASelectAllItem.Size = new System.Drawing.Size(223, 22);
            this.CASelectAllItem.Text = "SelectAll";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // CACopyLogItem
            // 
            this.CACopyLogItem.Name = "CACopyLogItem";
            this.CACopyLogItem.ShortcutKeyDisplayString = "";
            this.CACopyLogItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CACopyLogItem.Size = new System.Drawing.Size(223, 22);
            this.CACopyLogItem.Text = "CopyLog";
            // 
            // CACopyLogDetailItem
            // 
            this.CACopyLogDetailItem.Name = "CACopyLogDetailItem";
            this.CACopyLogDetailItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.CACopyLogDetailItem.Size = new System.Drawing.Size(223, 22);
            this.CACopyLogDetailItem.Text = "CopyLogDetail";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // CASetOriginItem
            // 
            this.CASetOriginItem.Name = "CASetOriginItem";
            this.CASetOriginItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.CASetOriginItem.Size = new System.Drawing.Size(223, 22);
            this.CASetOriginItem.Text = "SetLogOrigin";
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.DefaultExt = "xml";
            this.OpenFileDialog.FileName = "ACT.SpecialSpellTimer.Spells.xml";
            this.OpenFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            this.OpenFileDialog.RestoreDirectory = true;
            // 
            // SaveFileDialog
            // 
            this.SaveFileDialog.DefaultExt = "xml";
            this.SaveFileDialog.FileName = "ACT.SpecialSpellTimer.Spells.xml";
            this.SaveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            this.SaveFileDialog.RestoreDirectory = true;
            // 
            // ToolTip
            // 
            this.ToolTip.IsBalloon = true;
            // 
            // CombatAnalyzingTimer
            // 
            this.CombatAnalyzingTimer.Interval = 600;
            // 
            // EnabledSpellTimerNoDecimal
            // 
            this.EnabledSpellTimerNoDecimal.AutoSize = true;
            this.EnabledSpellTimerNoDecimal.Location = new System.Drawing.Point(213, 335);
            this.EnabledSpellTimerNoDecimal.Name = "EnabledSpellTimerNoDecimal";
            this.EnabledSpellTimerNoDecimal.Size = new System.Drawing.Size(64, 16);
            this.EnabledSpellTimerNoDecimal.TabIndex = 42;
            this.EnabledSpellTimerNoDecimal.Text = "Enabled";
            this.EnabledSpellTimerNoDecimal.UseVisualStyleBackColor = true;
            // 
            // TabControl
            // 
            this.TabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.TabControl.Controls.Add(this.SpecialSpellTabPage);
            this.TabControl.Controls.Add(this.OnPointTelopTabPage);
            this.TabControl.Controls.Add(this.CombatAnalyzerTabPage);
            this.TabControl.Controls.Add(this.NameStyleTabPage);
            this.TabControl.Controls.Add(this.OptionTabPage);
            this.TabControl.Controls.Add(this.DQXOptionTabPage);
            this.TabControl.Controls.Add(this.LogTabPage);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.ItemSize = new System.Drawing.Size(32, 260);
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(1333, 837);
            this.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControl.TabIndex = 0;
            // 
            // SpecialSpellTabPage
            // 
            this.SpecialSpellTabPage.BackColor = System.Drawing.Color.White;
            this.SpecialSpellTabPage.Controls.Add(this.panel5);
            this.SpecialSpellTabPage.Location = new System.Drawing.Point(264, 4);
            this.SpecialSpellTabPage.Name = "SpecialSpellTabPage";
            this.SpecialSpellTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.SpecialSpellTabPage.Size = new System.Drawing.Size(1065, 829);
            this.SpecialSpellTabPage.TabIndex = 0;
            this.SpecialSpellTabPage.Text = "SpellTimerTabTitle";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.splitContainer1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(2, 2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1061, 825);
            this.panel5.TabIndex = 35;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.DetailPanelGroupBox);
            this.splitContainer1.Panel1.Controls.Add(this.SpellTimerTreeView);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            this.splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.DetailGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(1061, 825);
            this.splitContainer1.SplitterDistance = 520;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 34;
            // 
            // DetailPanelGroupBox
            // 
            this.DetailPanelGroupBox.Controls.Add(this.FixedPositionSpellCheckBox);
            this.DetailPanelGroupBox.Controls.Add(this.HorizontalLayoutCheckBox);
            this.DetailPanelGroupBox.Controls.Add(this.label62);
            this.DetailPanelGroupBox.Controls.Add(this.MarginUpDown);
            this.DetailPanelGroupBox.Controls.Add(this.PanelTopNumericUpDown);
            this.DetailPanelGroupBox.Controls.Add(this.label13);
            this.DetailPanelGroupBox.Controls.Add(this.label15);
            this.DetailPanelGroupBox.Controls.Add(this.PanelLeftNumericUpDown);
            this.DetailPanelGroupBox.Controls.Add(this.label16);
            this.DetailPanelGroupBox.Controls.Add(this.UpdatePanelButton);
            this.DetailPanelGroupBox.Location = new System.Drawing.Point(93, 257);
            this.DetailPanelGroupBox.Name = "DetailPanelGroupBox";
            this.DetailPanelGroupBox.Size = new System.Drawing.Size(312, 257);
            this.DetailPanelGroupBox.TabIndex = 6;
            this.DetailPanelGroupBox.TabStop = false;
            // 
            // FixedPositionSpellCheckBox
            // 
            this.FixedPositionSpellCheckBox.AutoSize = true;
            this.FixedPositionSpellCheckBox.Location = new System.Drawing.Point(9, 113);
            this.FixedPositionSpellCheckBox.Name = "FixedPositionSpellCheckBox";
            this.FixedPositionSpellCheckBox.Size = new System.Drawing.Size(118, 16);
            this.FixedPositionSpellCheckBox.TabIndex = 47;
            this.FixedPositionSpellCheckBox.Text = "FixedPositionSpell";
            this.FixedPositionSpellCheckBox.UseVisualStyleBackColor = true;
            // 
            // HorizontalLayoutCheckBox
            // 
            this.HorizontalLayoutCheckBox.AutoSize = true;
            this.HorizontalLayoutCheckBox.Location = new System.Drawing.Point(9, 88);
            this.HorizontalLayoutCheckBox.Name = "HorizontalLayoutCheckBox";
            this.HorizontalLayoutCheckBox.Size = new System.Drawing.Size(109, 16);
            this.HorizontalLayoutCheckBox.TabIndex = 46;
            this.HorizontalLayoutCheckBox.Text = "HorizontalLayout";
            this.HorizontalLayoutCheckBox.UseVisualStyleBackColor = true;
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(113, 47);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(39, 12);
            this.label62.TabIndex = 45;
            this.label62.Text = "Margin";
            // 
            // MarginUpDown
            // 
            this.MarginUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.MarginUpDown.Location = new System.Drawing.Point(178, 45);
            this.MarginUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.MarginUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.MarginUpDown.Name = "MarginUpDown";
            this.MarginUpDown.Size = new System.Drawing.Size(68, 19);
            this.MarginUpDown.TabIndex = 44;
            this.MarginUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // PanelTopNumericUpDown
            // 
            this.PanelTopNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.PanelTopNumericUpDown.Location = new System.Drawing.Point(178, 17);
            this.PanelTopNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.PanelTopNumericUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.PanelTopNumericUpDown.Name = "PanelTopNumericUpDown";
            this.PanelTopNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.PanelTopNumericUpDown.TabIndex = 43;
            this.PanelTopNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(160, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(12, 12);
            this.label13.TabIndex = 42;
            this.label13.Text = "Y";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(63, 20);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(12, 12);
            this.label15.TabIndex = 41;
            this.label15.Text = "X";
            // 
            // PanelLeftNumericUpDown
            // 
            this.PanelLeftNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.PanelLeftNumericUpDown.Location = new System.Drawing.Point(81, 17);
            this.PanelLeftNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.PanelLeftNumericUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.PanelLeftNumericUpDown.Name = "PanelLeftNumericUpDown";
            this.PanelLeftNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.PanelLeftNumericUpDown.TabIndex = 40;
            this.PanelLeftNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(7, 19);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 12);
            this.label16.TabIndex = 39;
            this.label16.Text = "Location";
            // 
            // UpdatePanelButton
            // 
            this.UpdatePanelButton.Location = new System.Drawing.Point(6, 148);
            this.UpdatePanelButton.Name = "UpdatePanelButton";
            this.UpdatePanelButton.Size = new System.Drawing.Size(102, 25);
            this.UpdatePanelButton.TabIndex = 13;
            this.UpdatePanelButton.Text = "Update";
            this.UpdatePanelButton.UseVisualStyleBackColor = true;
            // 
            // SpellTimerTreeView
            // 
            this.SpellTimerTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SpellTimerTreeView.CheckBoxes = true;
            this.SpellTimerTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpellTimerTreeView.HideSelection = false;
            this.SpellTimerTreeView.Location = new System.Drawing.Point(0, 24);
            this.SpellTimerTreeView.Name = "SpellTimerTreeView";
            this.SpellTimerTreeView.ShowNodeToolTips = true;
            this.SpellTimerTreeView.Size = new System.Drawing.Size(520, 801);
            this.SpellTimerTreeView.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExportButton,
            this.ImportButton,
            this.ClearAllButton,
            this.AddButton});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(520, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ExportButton
            // 
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(89, 20);
            this.ExportButton.Text = "ExportButton";
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // ImportButton
            // 
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(90, 20);
            this.ImportButton.Text = "ImportButton";
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // ClearAllButton
            // 
            this.ClearAllButton.Name = "ClearAllButton";
            this.ClearAllButton.Size = new System.Drawing.Size(95, 20);
            this.ClearAllButton.Text = "ClearAllButton";
            this.ClearAllButton.Click += new System.EventHandler(this.ClearAllButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(77, 20);
            this.AddButton.Text = "AddButton";
            // 
            // DetailGroupBox
            // 
            this.DetailGroupBox.Controls.Add(this.tabControlExtHoriz2);
            this.DetailGroupBox.Controls.Add(this.SpellButtonsPanel);
            this.DetailGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DetailGroupBox.Location = new System.Drawing.Point(0, 0);
            this.DetailGroupBox.Name = "DetailGroupBox";
            this.DetailGroupBox.Size = new System.Drawing.Size(539, 825);
            this.DetailGroupBox.TabIndex = 6;
            // 
            // tabControlExtHoriz2
            // 
            this.tabControlExtHoriz2.Controls.Add(this.GeneralTab);
            this.tabControlExtHoriz2.Controls.Add(this.AlarmTab);
            this.tabControlExtHoriz2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlExtHoriz2.ItemSize = new System.Drawing.Size(160, 32);
            this.tabControlExtHoriz2.Location = new System.Drawing.Point(0, 0);
            this.tabControlExtHoriz2.Name = "tabControlExtHoriz2";
            this.tabControlExtHoriz2.SelectedIndex = 0;
            this.tabControlExtHoriz2.Size = new System.Drawing.Size(539, 793);
            this.tabControlExtHoriz2.TabIndex = 3;
            // 
            // GeneralTab
            // 
            this.GeneralTab.Controls.Add(this.SpellDetailPanel);
            this.GeneralTab.Location = new System.Drawing.Point(-1, 31);
            this.GeneralTab.Name = "GeneralTab";
            this.GeneralTab.Padding = new System.Windows.Forms.Padding(2);
            this.GeneralTab.Size = new System.Drawing.Size(541, 763);
            this.GeneralTab.TabIndex = 0;
            this.GeneralTab.Text = "GeneralTab";
            this.GeneralTab.UseVisualStyleBackColor = true;
            // 
            // SpellDetailPanel
            // 
            this.SpellDetailPanel.AutoScroll = true;
            this.SpellDetailPanel.Controls.Add(this.label68);
            this.SpellDetailPanel.Controls.Add(this.BlinkTimeNumericUpDown);
            this.SpellDetailPanel.Controls.Add(this.SelectIconButton);
            this.SpellDetailPanel.Controls.Add(this.label58);
            this.SpellDetailPanel.Controls.Add(this.label56);
            this.SpellDetailPanel.Controls.Add(this.WarningTimeCheckBox);
            this.SpellDetailPanel.Controls.Add(this.WarningTimeNumericUpDown);
            this.SpellDetailPanel.Controls.Add(this.label105);
            this.SpellDetailPanel.Controls.Add(this.PanelNameTextBox);
            this.SpellDetailPanel.Controls.Add(this.ExpandSecounds1NumericUpDown);
            this.SpellDetailPanel.Controls.Add(this.KeywordToExpand1TextBox);
            this.SpellDetailPanel.Controls.Add(this.KeywordToExpand2TextBox);
            this.SpellDetailPanel.Controls.Add(this.RegexEnabledCheckBox);
            this.SpellDetailPanel.Controls.Add(this.ExpandSecounds2NumericUpDown);
            this.SpellDetailPanel.Controls.Add(this.SpellTitleTextBox);
            this.SpellDetailPanel.Controls.Add(this.DisplayNoNumericUpDown);
            this.SpellDetailPanel.Controls.Add(this.SpellIconSizeUpDown);
            this.SpellDetailPanel.Controls.Add(this.KeywordTextBox);
            this.SpellDetailPanel.Controls.Add(this.RecastTimeNumericUpDown);
            this.SpellDetailPanel.Controls.Add(this.UpperLimitOfExtensionNumericUpDown);
            this.SpellDetailPanel.Controls.Add(this.RepeatCheckBox);
            this.SpellDetailPanel.Controls.Add(this.SpellVisualSetting);
            this.SpellDetailPanel.Controls.Add(this.SetConditionButton);
            this.SpellDetailPanel.Controls.Add(this.ToInstanceCheckBox);
            this.SpellDetailPanel.Controls.Add(this.label1);
            this.SpellDetailPanel.Controls.Add(this.label51);
            this.SpellDetailPanel.Controls.Add(this.ReduceIconBrightnessCheckBox);
            this.SpellDetailPanel.Controls.Add(this.label50);
            this.SpellDetailPanel.Controls.Add(this.SelectZoneButton);
            this.SpellDetailPanel.Controls.Add(this.ExtendBeyondOriginalRecastTimeCheckBox);
            this.SpellDetailPanel.Controls.Add(this.SelectJobButton);
            this.SpellDetailPanel.Controls.Add(this.OverlapRecastTimeCheckBox);
            this.SpellDetailPanel.Controls.Add(this.label2);
            this.SpellDetailPanel.Controls.Add(this.DontHideCheckBox);
            this.SpellDetailPanel.Controls.Add(this.HideSpellNameCheckBox);
            this.SpellDetailPanel.Controls.Add(this.label55);
            this.SpellDetailPanel.Controls.Add(this.label19);
            this.SpellDetailPanel.Controls.Add(this.label61);
            this.SpellDetailPanel.Controls.Add(this.label3);
            this.SpellDetailPanel.Controls.Add(this.IsReverseCheckBox);
            this.SpellDetailPanel.Controls.Add(this.ShowProgressBarCheckBox);
            this.SpellDetailPanel.Controls.Add(this.label60);
            this.SpellDetailPanel.Controls.Add(this.label4);
            this.SpellDetailPanel.Controls.Add(this.label59);
            this.SpellDetailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpellDetailPanel.Location = new System.Drawing.Point(2, 2);
            this.SpellDetailPanel.Name = "SpellDetailPanel";
            this.SpellDetailPanel.Size = new System.Drawing.Size(537, 759);
            this.SpellDetailPanel.TabIndex = 74;
            // 
            // SelectIconButton
            // 
            this.SelectIconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelectIconButton.Location = new System.Drawing.Point(374, 433);
            this.SelectIconButton.Name = "SelectIconButton";
            this.SelectIconButton.Size = new System.Drawing.Size(64, 64);
            this.SelectIconButton.TabIndex = 77;
            this.SelectIconButton.UseVisualStyleBackColor = true;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(3, 289);
            this.label58.Margin = new System.Windows.Forms.Padding(0);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(52, 12);
            this.label58.TabIndex = 62;
            this.label58.Text = "No2Label";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(3, 261);
            this.label56.Margin = new System.Windows.Forms.Padding(0);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(52, 12);
            this.label56.TabIndex = 60;
            this.label56.Text = "No1Label";
            // 
            // WarningTimeCheckBox
            // 
            this.WarningTimeCheckBox.AutoSize = true;
            this.WarningTimeCheckBox.Location = new System.Drawing.Point(259, 364);
            this.WarningTimeCheckBox.Name = "WarningTimeCheckBox";
            this.WarningTimeCheckBox.Size = new System.Drawing.Size(141, 16);
            this.WarningTimeCheckBox.TabIndex = 76;
            this.WarningTimeCheckBox.Text = "WarningTimeCheckBox";
            this.WarningTimeCheckBox.UseVisualStyleBackColor = true;
            // 
            // WarningTimeNumericUpDown
            // 
            this.WarningTimeNumericUpDown.DecimalPlaces = 1;
            this.WarningTimeNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.WarningTimeNumericUpDown.Location = new System.Drawing.Point(5, 364);
            this.WarningTimeNumericUpDown.Margin = new System.Windows.Forms.Padding(6);
            this.WarningTimeNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.WarningTimeNumericUpDown.Name = "WarningTimeNumericUpDown";
            this.WarningTimeNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.WarningTimeNumericUpDown.TabIndex = 74;
            this.WarningTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label105
            // 
            this.label105.AutoSize = true;
            this.label105.Location = new System.Drawing.Point(85, 365);
            this.label105.Margin = new System.Windows.Forms.Padding(0);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(97, 12);
            this.label105.TabIndex = 75;
            this.label105.Text = "WarningTimeLabel";
            // 
            // PanelNameTextBox
            // 
            this.PanelNameTextBox.Location = new System.Drawing.Point(5, 24);
            this.PanelNameTextBox.Name = "PanelNameTextBox";
            this.PanelNameTextBox.Size = new System.Drawing.Size(498, 19);
            this.PanelNameTextBox.TabIndex = 36;
            // 
            // ExpandSecounds1NumericUpDown
            // 
            this.ExpandSecounds1NumericUpDown.DecimalPlaces = 1;
            this.ExpandSecounds1NumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ExpandSecounds1NumericUpDown.Location = new System.Drawing.Point(426, 260);
            this.ExpandSecounds1NumericUpDown.Margin = new System.Windows.Forms.Padding(6);
            this.ExpandSecounds1NumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ExpandSecounds1NumericUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.ExpandSecounds1NumericUpDown.Name = "ExpandSecounds1NumericUpDown";
            this.ExpandSecounds1NumericUpDown.Size = new System.Drawing.Size(77, 19);
            this.ExpandSecounds1NumericUpDown.TabIndex = 54;
            this.ExpandSecounds1NumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // KeywordToExpand1TextBox
            // 
            this.KeywordToExpand1TextBox.Location = new System.Drawing.Point(33, 260);
            this.KeywordToExpand1TextBox.Name = "KeywordToExpand1TextBox";
            this.KeywordToExpand1TextBox.Size = new System.Drawing.Size(387, 19);
            this.KeywordToExpand1TextBox.TabIndex = 53;
            // 
            // KeywordToExpand2TextBox
            // 
            this.KeywordToExpand2TextBox.Location = new System.Drawing.Point(33, 286);
            this.KeywordToExpand2TextBox.Name = "KeywordToExpand2TextBox";
            this.KeywordToExpand2TextBox.Size = new System.Drawing.Size(387, 19);
            this.KeywordToExpand2TextBox.TabIndex = 57;
            // 
            // RegexEnabledCheckBox
            // 
            this.RegexEnabledCheckBox.AutoSize = true;
            this.RegexEnabledCheckBox.Location = new System.Drawing.Point(5, 163);
            this.RegexEnabledCheckBox.Name = "RegexEnabledCheckBox";
            this.RegexEnabledCheckBox.Size = new System.Drawing.Size(76, 16);
            this.RegexEnabledCheckBox.TabIndex = 41;
            this.RegexEnabledCheckBox.Text = "UseRegex";
            this.RegexEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // ExpandSecounds2NumericUpDown
            // 
            this.ExpandSecounds2NumericUpDown.DecimalPlaces = 1;
            this.ExpandSecounds2NumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ExpandSecounds2NumericUpDown.Location = new System.Drawing.Point(426, 286);
            this.ExpandSecounds2NumericUpDown.Margin = new System.Windows.Forms.Padding(6);
            this.ExpandSecounds2NumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ExpandSecounds2NumericUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.ExpandSecounds2NumericUpDown.Name = "ExpandSecounds2NumericUpDown";
            this.ExpandSecounds2NumericUpDown.Size = new System.Drawing.Size(77, 19);
            this.ExpandSecounds2NumericUpDown.TabIndex = 58;
            this.ExpandSecounds2NumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // SpellTitleTextBox
            // 
            this.SpellTitleTextBox.Location = new System.Drawing.Point(5, 64);
            this.SpellTitleTextBox.Name = "SpellTitleTextBox";
            this.SpellTitleTextBox.Size = new System.Drawing.Size(415, 19);
            this.SpellTitleTextBox.TabIndex = 37;
            // 
            // DisplayNoNumericUpDown
            // 
            this.DisplayNoNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.DisplayNoNumericUpDown.Location = new System.Drawing.Point(426, 64);
            this.DisplayNoNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.DisplayNoNumericUpDown.Name = "DisplayNoNumericUpDown";
            this.DisplayNoNumericUpDown.Size = new System.Drawing.Size(77, 19);
            this.DisplayNoNumericUpDown.TabIndex = 39;
            this.DisplayNoNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // SpellIconSizeUpDown
            // 
            this.SpellIconSizeUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.SpellIconSizeUpDown.Location = new System.Drawing.Point(447, 478);
            this.SpellIconSizeUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.SpellIconSizeUpDown.Name = "SpellIconSizeUpDown";
            this.SpellIconSizeUpDown.Size = new System.Drawing.Size(52, 19);
            this.SpellIconSizeUpDown.TabIndex = 68;
            this.SpellIconSizeUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // KeywordTextBox
            // 
            this.KeywordTextBox.Location = new System.Drawing.Point(5, 138);
            this.KeywordTextBox.Name = "KeywordTextBox";
            this.KeywordTextBox.Size = new System.Drawing.Size(498, 19);
            this.KeywordTextBox.TabIndex = 40;
            // 
            // RecastTimeNumericUpDown
            // 
            this.RecastTimeNumericUpDown.DecimalPlaces = 1;
            this.RecastTimeNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.RecastTimeNumericUpDown.Location = new System.Drawing.Point(5, 207);
            this.RecastTimeNumericUpDown.Margin = new System.Windows.Forms.Padding(6);
            this.RecastTimeNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.RecastTimeNumericUpDown.Name = "RecastTimeNumericUpDown";
            this.RecastTimeNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.RecastTimeNumericUpDown.TabIndex = 43;
            this.RecastTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // UpperLimitOfExtensionNumericUpDown
            // 
            this.UpperLimitOfExtensionNumericUpDown.DecimalPlaces = 1;
            this.UpperLimitOfExtensionNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.UpperLimitOfExtensionNumericUpDown.Location = new System.Drawing.Point(5, 337);
            this.UpperLimitOfExtensionNumericUpDown.Margin = new System.Windows.Forms.Padding(6);
            this.UpperLimitOfExtensionNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.UpperLimitOfExtensionNumericUpDown.Name = "UpperLimitOfExtensionNumericUpDown";
            this.UpperLimitOfExtensionNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.UpperLimitOfExtensionNumericUpDown.TabIndex = 63;
            this.UpperLimitOfExtensionNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // RepeatCheckBox
            // 
            this.RepeatCheckBox.AutoSize = true;
            this.RepeatCheckBox.Location = new System.Drawing.Point(79, 210);
            this.RepeatCheckBox.Name = "RepeatCheckBox";
            this.RepeatCheckBox.Size = new System.Drawing.Size(112, 16);
            this.RepeatCheckBox.TabIndex = 44;
            this.RepeatCheckBox.Text = "RepeatCheckBox";
            this.RepeatCheckBox.UseVisualStyleBackColor = true;
            this.RepeatCheckBox.Visible = false;
            // 
            // SpellVisualSetting
            // 
            this.SpellVisualSetting.BackgroundColor = System.Drawing.Color.Empty;
            this.SpellVisualSetting.BarColor = System.Drawing.Color.White;
            this.SpellVisualSetting.BarEnabled = true;
            this.SpellVisualSetting.BarOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(120)))), ((int)(((byte)(157)))));
            this.SpellVisualSetting.BarSize = new System.Drawing.Size(190, 8);
            this.SpellVisualSetting.FontColor = System.Drawing.Color.White;
            this.SpellVisualSetting.FontOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(120)))), ((int)(((byte)(157)))));
            this.SpellVisualSetting.HideSpellName = false;
            this.SpellVisualSetting.Location = new System.Drawing.Point(5, 418);
            this.SpellVisualSetting.Name = "SpellVisualSetting";
            this.SpellVisualSetting.OverlapRecastTime = false;
            this.SpellVisualSetting.Size = new System.Drawing.Size(355, 88);
            this.SpellVisualSetting.SpellIcon = "";
            this.SpellVisualSetting.SpellIconSize = 0;
            this.SpellVisualSetting.TabIndex = 51;
            this.SpellVisualSetting.WarningFontColor = System.Drawing.Color.OrangeRed;
            this.SpellVisualSetting.WarningFontOutlineColor = System.Drawing.Color.DarkRed;
            // 
            // SetConditionButton
            // 
            this.SetConditionButton.Location = new System.Drawing.Point(308, 655);
            this.SetConditionButton.Name = "SetConditionButton";
            this.SetConditionButton.Size = new System.Drawing.Size(144, 26);
            this.SetConditionButton.TabIndex = 36;
            this.SetConditionButton.Text = "SetConditionForStartButton";
            this.SetConditionButton.UseVisualStyleBackColor = true;
            // 
            // ToInstanceCheckBox
            // 
            this.ToInstanceCheckBox.AutoSize = true;
            this.ToInstanceCheckBox.Location = new System.Drawing.Point(5, 89);
            this.ToInstanceCheckBox.Name = "ToInstanceCheckBox";
            this.ToInstanceCheckBox.Size = new System.Drawing.Size(132, 16);
            this.ToInstanceCheckBox.TabIndex = 73;
            this.ToInstanceCheckBox.Text = "ToInstanceCheckBox";
            this.ToInstanceCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 35;
            this.label1.Text = "PanelNameLabel";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(513, 261);
            this.label51.Margin = new System.Windows.Forms.Padding(0);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(75, 12);
            this.label51.TabIndex = 55;
            this.label51.Text = "SecondsLabel";
            // 
            // ReduceIconBrightnessCheckBox
            // 
            this.ReduceIconBrightnessCheckBox.AutoSize = true;
            this.ReduceIconBrightnessCheckBox.Location = new System.Drawing.Point(8, 629);
            this.ReduceIconBrightnessCheckBox.Name = "ReduceIconBrightnessCheckBox";
            this.ReduceIconBrightnessCheckBox.Size = new System.Drawing.Size(190, 16);
            this.ReduceIconBrightnessCheckBox.TabIndex = 72;
            this.ReduceIconBrightnessCheckBox.Text = "ReduceIconBrightnessCheckBox";
            this.ReduceIconBrightnessCheckBox.UseVisualStyleBackColor = true;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(2, 239);
            this.label50.Margin = new System.Windows.Forms.Padding(0);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(171, 12);
            this.label50.TabIndex = 52;
            this.label50.Text = "MatchingLogWordToExpandLabel";
            // 
            // SelectZoneButton
            // 
            this.SelectZoneButton.Location = new System.Drawing.Point(158, 655);
            this.SelectZoneButton.Name = "SelectZoneButton";
            this.SelectZoneButton.Size = new System.Drawing.Size(144, 26);
            this.SelectZoneButton.TabIndex = 29;
            this.SelectZoneButton.Text = "SelectZoneButton";
            this.SelectZoneButton.UseVisualStyleBackColor = true;
            // 
            // ExtendBeyondOriginalRecastTimeCheckBox
            // 
            this.ExtendBeyondOriginalRecastTimeCheckBox.AutoSize = true;
            this.ExtendBeyondOriginalRecastTimeCheckBox.Location = new System.Drawing.Point(5, 312);
            this.ExtendBeyondOriginalRecastTimeCheckBox.Name = "ExtendBeyondOriginalRecastTimeCheckBox";
            this.ExtendBeyondOriginalRecastTimeCheckBox.Size = new System.Drawing.Size(249, 16);
            this.ExtendBeyondOriginalRecastTimeCheckBox.TabIndex = 56;
            this.ExtendBeyondOriginalRecastTimeCheckBox.Text = "ExtendBeyondOriginalRecastTimeCheckBox";
            this.ExtendBeyondOriginalRecastTimeCheckBox.UseVisualStyleBackColor = true;
            // 
            // SelectJobButton
            // 
            this.SelectJobButton.Location = new System.Drawing.Point(8, 655);
            this.SelectJobButton.Name = "SelectJobButton";
            this.SelectJobButton.Size = new System.Drawing.Size(144, 26);
            this.SelectJobButton.TabIndex = 27;
            this.SelectJobButton.Text = "SelectJobButton";
            this.SelectJobButton.UseVisualStyleBackColor = true;
            // 
            // OverlapRecastTimeCheckBox
            // 
            this.OverlapRecastTimeCheckBox.AutoSize = true;
            this.OverlapRecastTimeCheckBox.Location = new System.Drawing.Point(8, 607);
            this.OverlapRecastTimeCheckBox.Name = "OverlapRecastTimeCheckBox";
            this.OverlapRecastTimeCheckBox.Size = new System.Drawing.Size(176, 16);
            this.OverlapRecastTimeCheckBox.TabIndex = 71;
            this.OverlapRecastTimeCheckBox.Text = "OverlapRecastTimeCheckBox";
            this.OverlapRecastTimeCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 48);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 12);
            this.label2.TabIndex = 38;
            this.label2.Text = "SpellNameLabel";
            // 
            // DontHideCheckBox
            // 
            this.DontHideCheckBox.AutoSize = true;
            this.DontHideCheckBox.Location = new System.Drawing.Point(8, 562);
            this.DontHideCheckBox.Name = "DontHideCheckBox";
            this.DontHideCheckBox.Size = new System.Drawing.Size(123, 16);
            this.DontHideCheckBox.TabIndex = 48;
            this.DontHideCheckBox.Text = "DontHideCheckBox";
            this.DontHideCheckBox.UseVisualStyleBackColor = true;
            // 
            // HideSpellNameCheckBox
            // 
            this.HideSpellNameCheckBox.AutoSize = true;
            this.HideSpellNameCheckBox.Location = new System.Drawing.Point(8, 585);
            this.HideSpellNameCheckBox.Name = "HideSpellNameCheckBox";
            this.HideSpellNameCheckBox.Size = new System.Drawing.Size(153, 16);
            this.HideSpellNameCheckBox.TabIndex = 70;
            this.HideSpellNameCheckBox.Text = "HideSpellNameCheckBox";
            this.HideSpellNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(513, 289);
            this.label55.Margin = new System.Windows.Forms.Padding(0);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(75, 12);
            this.label55.TabIndex = 59;
            this.label55.Text = "SecondsLabel";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(423, 49);
            this.label19.Margin = new System.Windows.Forms.Padding(0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(98, 12);
            this.label19.TabIndex = 49;
            this.label19.Text = "DisplayOrderLabel";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(445, 459);
            this.label61.Margin = new System.Windows.Forms.Padding(0);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(99, 12);
            this.label61.TabIndex = 67;
            this.label61.Text = "SpellIconSizeLabel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 119);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 12);
            this.label3.TabIndex = 42;
            this.label3.Text = "MatchingLogWordLabel";
            // 
            // IsReverseCheckBox
            // 
            this.IsReverseCheckBox.AutoSize = true;
            this.IsReverseCheckBox.Location = new System.Drawing.Point(8, 541);
            this.IsReverseCheckBox.Name = "IsReverseCheckBox";
            this.IsReverseCheckBox.Size = new System.Drawing.Size(162, 16);
            this.IsReverseCheckBox.TabIndex = 47;
            this.IsReverseCheckBox.Text = "ReverseDirectionCheckbox";
            this.IsReverseCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShowProgressBarCheckBox
            // 
            this.ShowProgressBarCheckBox.AutoSize = true;
            this.ShowProgressBarCheckBox.Location = new System.Drawing.Point(8, 518);
            this.ShowProgressBarCheckBox.Name = "ShowProgressBarCheckBox";
            this.ShowProgressBarCheckBox.Size = new System.Drawing.Size(166, 16);
            this.ShowProgressBarCheckBox.TabIndex = 46;
            this.ShowProgressBarCheckBox.Text = "ShowProgressBarCheckBox";
            this.ShowProgressBarCheckBox.UseVisualStyleBackColor = true;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(375, 418);
            this.label60.Margin = new System.Windows.Forms.Padding(0);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(78, 12);
            this.label60.TabIndex = 65;
            this.label60.Text = "SpellIconLabel";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 189);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 12);
            this.label4.TabIndex = 45;
            this.label4.Text = "RecastTimeLabel";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(85, 340);
            this.label59.Margin = new System.Windows.Forms.Padding(0);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(92, 12);
            this.label59.TabIndex = 64;
            this.label59.Text = "ExtendLimitLabel";
            // 
            // AlarmTab
            // 
            this.AlarmTab.Controls.Add(this.groupBox4);
            this.AlarmTab.Controls.Add(this.groupBox1);
            this.AlarmTab.Controls.Add(this.groupBox3);
            this.AlarmTab.Controls.Add(this.groupBox2);
            this.AlarmTab.Location = new System.Drawing.Point(-1, 31);
            this.AlarmTab.Name = "AlarmTab";
            this.AlarmTab.Padding = new System.Windows.Forms.Padding(3);
            this.AlarmTab.Size = new System.Drawing.Size(541, 763);
            this.AlarmTab.TabIndex = 1;
            this.AlarmTab.Text = "AlarmTab";
            this.AlarmTab.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.BeforeTextToSpeakTextBox);
            this.groupBox4.Controls.Add(this.BeforeSoundComboBox);
            this.groupBox4.Controls.Add(this.label52);
            this.groupBox4.Controls.Add(this.BeforeTimeNumericUpDown);
            this.groupBox4.Controls.Add(this.Speak4Button);
            this.groupBox4.Controls.Add(this.Play4Button);
            this.groupBox4.Controls.Add(this.label53);
            this.groupBox4.Controls.Add(this.label54);
            this.groupBox4.Location = new System.Drawing.Point(5, 213);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(530, 110);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "SoundEffectBeforeComplete";
            // 
            // BeforeTextToSpeakTextBox
            // 
            this.BeforeTextToSpeakTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BeforeTextToSpeakTextBox.Location = new System.Drawing.Point(255, 44);
            this.BeforeTextToSpeakTextBox.Name = "BeforeTextToSpeakTextBox";
            this.BeforeTextToSpeakTextBox.Size = new System.Drawing.Size(268, 19);
            this.BeforeTextToSpeakTextBox.TabIndex = 1;
            // 
            // BeforeSoundComboBox
            // 
            this.BeforeSoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BeforeSoundComboBox.FormattingEnabled = true;
            this.BeforeSoundComboBox.Location = new System.Drawing.Point(6, 44);
            this.BeforeSoundComboBox.MaxDropDownItems = 16;
            this.BeforeSoundComboBox.Name = "BeforeSoundComboBox";
            this.BeforeSoundComboBox.Size = new System.Drawing.Size(243, 20);
            this.BeforeSoundComboBox.TabIndex = 0;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(81, 73);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(122, 12);
            this.label52.TabIndex = 20;
            this.label52.Text = "SoundInSecondsBefore";
            // 
            // BeforeTimeNumericUpDown
            // 
            this.BeforeTimeNumericUpDown.DecimalPlaces = 1;
            this.BeforeTimeNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.BeforeTimeNumericUpDown.Location = new System.Drawing.Point(6, 70);
            this.BeforeTimeNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.BeforeTimeNumericUpDown.Name = "BeforeTimeNumericUpDown";
            this.BeforeTimeNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.BeforeTimeNumericUpDown.TabIndex = 2;
            this.BeforeTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Speak4Button
            // 
            this.Speak4Button.Location = new System.Drawing.Point(255, 18);
            this.Speak4Button.Name = "Speak4Button";
            this.Speak4Button.Size = new System.Drawing.Size(33, 20);
            this.Speak4Button.TabIndex = 3;
            this.Speak4Button.Text = "SpeakButton";
            this.Speak4Button.UseVisualStyleBackColor = true;
            // 
            // Play4Button
            // 
            this.Play4Button.Location = new System.Drawing.Point(6, 18);
            this.Play4Button.Name = "Play4Button";
            this.Play4Button.Size = new System.Drawing.Size(33, 20);
            this.Play4Button.TabIndex = 1;
            this.Play4Button.Text = "PlayButton";
            this.Play4Button.UseVisualStyleBackColor = true;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(294, 22);
            this.label53.Margin = new System.Windows.Forms.Padding(0);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(99, 12);
            this.label53.TabIndex = 15;
            this.label53.Text = "TextToSpeakLabel";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(45, 22);
            this.label54.Margin = new System.Windows.Forms.Padding(0);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(90, 12);
            this.label54.TabIndex = 13;
            this.label54.Text = "WaveSoundLabel";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.MatchTextToSpeakTextBox);
            this.groupBox1.Controls.Add(this.MatchSoundComboBox);
            this.groupBox1.Controls.Add(this.Speak1Button);
            this.groupBox1.Controls.Add(this.Play1Button);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(5, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 75);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ImmediateSoundEffect";
            // 
            // MatchTextToSpeakTextBox
            // 
            this.MatchTextToSpeakTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MatchTextToSpeakTextBox.Location = new System.Drawing.Point(255, 44);
            this.MatchTextToSpeakTextBox.Name = "MatchTextToSpeakTextBox";
            this.MatchTextToSpeakTextBox.Size = new System.Drawing.Size(268, 19);
            this.MatchTextToSpeakTextBox.TabIndex = 1;
            // 
            // MatchSoundComboBox
            // 
            this.MatchSoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MatchSoundComboBox.FormattingEnabled = true;
            this.MatchSoundComboBox.Location = new System.Drawing.Point(6, 44);
            this.MatchSoundComboBox.MaxDropDownItems = 16;
            this.MatchSoundComboBox.Name = "MatchSoundComboBox";
            this.MatchSoundComboBox.Size = new System.Drawing.Size(243, 20);
            this.MatchSoundComboBox.TabIndex = 0;
            // 
            // Speak1Button
            // 
            this.Speak1Button.Location = new System.Drawing.Point(255, 18);
            this.Speak1Button.Name = "Speak1Button";
            this.Speak1Button.Size = new System.Drawing.Size(33, 20);
            this.Speak1Button.TabIndex = 3;
            this.Speak1Button.Text = "SpeakButton";
            this.Speak1Button.UseVisualStyleBackColor = true;
            // 
            // Play1Button
            // 
            this.Play1Button.Location = new System.Drawing.Point(6, 18);
            this.Play1Button.Name = "Play1Button";
            this.Play1Button.Size = new System.Drawing.Size(33, 20);
            this.Play1Button.TabIndex = 1;
            this.Play1Button.Text = "PlayButton";
            this.Play1Button.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(294, 22);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "TextToSpeakLabel";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(45, 22);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "WaveSoundLabel";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.OverTextToSpeakTextBox);
            this.groupBox3.Controls.Add(this.OverSoundComboBox);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.OverTimeNumericUpDown);
            this.groupBox3.Controls.Add(this.Speak2Button);
            this.groupBox3.Controls.Add(this.Play2Button);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Location = new System.Drawing.Point(5, 93);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(530, 110);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "DelayedSoundEffectAfterMatch";
            // 
            // OverTextToSpeakTextBox
            // 
            this.OverTextToSpeakTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OverTextToSpeakTextBox.Location = new System.Drawing.Point(255, 44);
            this.OverTextToSpeakTextBox.Name = "OverTextToSpeakTextBox";
            this.OverTextToSpeakTextBox.Size = new System.Drawing.Size(268, 19);
            this.OverTextToSpeakTextBox.TabIndex = 1;
            // 
            // OverSoundComboBox
            // 
            this.OverSoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OverSoundComboBox.FormattingEnabled = true;
            this.OverSoundComboBox.Location = new System.Drawing.Point(6, 44);
            this.OverSoundComboBox.MaxDropDownItems = 16;
            this.OverSoundComboBox.Name = "OverSoundComboBox";
            this.OverSoundComboBox.Size = new System.Drawing.Size(243, 20);
            this.OverSoundComboBox.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(81, 73);
            this.label14.Margin = new System.Windows.Forms.Padding(0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 12);
            this.label14.TabIndex = 20;
            this.label14.Text = "SoundInSeconds";
            // 
            // OverTimeNumericUpDown
            // 
            this.OverTimeNumericUpDown.DecimalPlaces = 1;
            this.OverTimeNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.OverTimeNumericUpDown.Location = new System.Drawing.Point(6, 70);
            this.OverTimeNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.OverTimeNumericUpDown.Name = "OverTimeNumericUpDown";
            this.OverTimeNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.OverTimeNumericUpDown.TabIndex = 2;
            this.OverTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Speak2Button
            // 
            this.Speak2Button.Location = new System.Drawing.Point(255, 18);
            this.Speak2Button.Name = "Speak2Button";
            this.Speak2Button.Size = new System.Drawing.Size(33, 20);
            this.Speak2Button.TabIndex = 3;
            this.Speak2Button.Text = "SpeakButton";
            this.Speak2Button.UseVisualStyleBackColor = true;
            // 
            // Play2Button
            // 
            this.Play2Button.Location = new System.Drawing.Point(6, 18);
            this.Play2Button.Name = "Play2Button";
            this.Play2Button.Size = new System.Drawing.Size(33, 20);
            this.Play2Button.TabIndex = 1;
            this.Play2Button.Text = "PlayButton";
            this.Play2Button.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(294, 22);
            this.label11.Margin = new System.Windows.Forms.Padding(0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 12);
            this.label11.TabIndex = 15;
            this.label11.Text = "TextToSpeakLabel";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(45, 22);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(90, 12);
            this.label12.TabIndex = 13;
            this.label12.Text = "WaveSoundLabel";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.TimeupTextToSpeakTextBox);
            this.groupBox2.Controls.Add(this.TimeupSoundComboBox);
            this.groupBox2.Controls.Add(this.Speak3Button);
            this.groupBox2.Controls.Add(this.Play3Button);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(5, 332);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(530, 75);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RecastCompleteSoundEffect";
            // 
            // TimeupTextToSpeakTextBox
            // 
            this.TimeupTextToSpeakTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TimeupTextToSpeakTextBox.Location = new System.Drawing.Point(255, 44);
            this.TimeupTextToSpeakTextBox.Name = "TimeupTextToSpeakTextBox";
            this.TimeupTextToSpeakTextBox.Size = new System.Drawing.Size(268, 19);
            this.TimeupTextToSpeakTextBox.TabIndex = 1;
            // 
            // TimeupSoundComboBox
            // 
            this.TimeupSoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TimeupSoundComboBox.FormattingEnabled = true;
            this.TimeupSoundComboBox.Location = new System.Drawing.Point(6, 44);
            this.TimeupSoundComboBox.MaxDropDownItems = 16;
            this.TimeupSoundComboBox.Name = "TimeupSoundComboBox";
            this.TimeupSoundComboBox.Size = new System.Drawing.Size(243, 20);
            this.TimeupSoundComboBox.TabIndex = 0;
            // 
            // Speak3Button
            // 
            this.Speak3Button.Location = new System.Drawing.Point(255, 18);
            this.Speak3Button.Name = "Speak3Button";
            this.Speak3Button.Size = new System.Drawing.Size(33, 20);
            this.Speak3Button.TabIndex = 3;
            this.Speak3Button.Text = "SpeakButton";
            this.Speak3Button.UseVisualStyleBackColor = true;
            // 
            // Play3Button
            // 
            this.Play3Button.Location = new System.Drawing.Point(6, 18);
            this.Play3Button.Name = "Play3Button";
            this.Play3Button.Size = new System.Drawing.Size(33, 20);
            this.Play3Button.TabIndex = 1;
            this.Play3Button.Text = "PlayButton";
            this.Play3Button.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(294, 22);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 12);
            this.label9.TabIndex = 15;
            this.label9.Text = "TextToSpeakLabel";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(45, 22);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(90, 12);
            this.label10.TabIndex = 13;
            this.label10.Text = "WaveSoundLabel";
            // 
            // SpellButtonsPanel
            // 
            this.SpellButtonsPanel.Controls.Add(this.DeleteButton);
            this.SpellButtonsPanel.Controls.Add(this.UpdateButton);
            this.SpellButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SpellButtonsPanel.Location = new System.Drawing.Point(0, 793);
            this.SpellButtonsPanel.Name = "SpellButtonsPanel";
            this.SpellButtonsPanel.Size = new System.Drawing.Size(539, 32);
            this.SpellButtonsPanel.TabIndex = 0;
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteButton.Location = new System.Drawing.Point(434, 3);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(102, 26);
            this.DeleteButton.TabIndex = 13;
            this.DeleteButton.Text = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            // 
            // UpdateButton
            // 
            this.UpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateButton.Location = new System.Drawing.Point(326, 3);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(102, 26);
            this.UpdateButton.TabIndex = 12;
            this.UpdateButton.Text = "UpdateButton";
            this.UpdateButton.UseVisualStyleBackColor = true;
            // 
            // OnPointTelopTabPage
            // 
            this.OnPointTelopTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.OnPointTelopTabPage.Controls.Add(this.splitContainer2);
            this.OnPointTelopTabPage.Location = new System.Drawing.Point(264, 4);
            this.OnPointTelopTabPage.Name = "OnPointTelopTabPage";
            this.OnPointTelopTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.OnPointTelopTabPage.Size = new System.Drawing.Size(1065, 829);
            this.OnPointTelopTabPage.TabIndex = 2;
            this.OnPointTelopTabPage.Text = "TelopTabPageTitle";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(2, 2);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.TelopTreeView);
            this.splitContainer2.Panel1.Controls.Add(this.menuStrip2);
            this.splitContainer2.Panel1MinSize = 400;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel2.Controls.Add(this.TelopDetailGroupBox);
            this.splitContainer2.Size = new System.Drawing.Size(1061, 825);
            this.splitContainer2.SplitterDistance = 520;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 6;
            // 
            // TelopTreeView
            // 
            this.TelopTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TelopTreeView.CheckBoxes = true;
            this.TelopTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TelopTreeView.HideSelection = false;
            this.TelopTreeView.Location = new System.Drawing.Point(0, 24);
            this.TelopTreeView.Name = "TelopTreeView";
            this.TelopTreeView.ShowNodeToolTips = true;
            this.TelopTreeView.Size = new System.Drawing.Size(520, 801);
            this.TelopTreeView.TabIndex = 0;
            // 
            // menuStrip2
            // 
            this.menuStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TelopExportButton,
            this.TelopImportButton,
            this.TelopClearAllButton,
            this.TelopAddButton});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip2.Size = new System.Drawing.Size(520, 24);
            this.menuStrip2.TabIndex = 0;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // TelopExportButton
            // 
            this.TelopExportButton.Name = "TelopExportButton";
            this.TelopExportButton.Size = new System.Drawing.Size(118, 20);
            this.TelopExportButton.Text = "TelopExportButton";
            // 
            // TelopImportButton
            // 
            this.TelopImportButton.Name = "TelopImportButton";
            this.TelopImportButton.Size = new System.Drawing.Size(119, 20);
            this.TelopImportButton.Text = "TelopImportButton";
            // 
            // TelopClearAllButton
            // 
            this.TelopClearAllButton.Name = "TelopClearAllButton";
            this.TelopClearAllButton.Size = new System.Drawing.Size(124, 20);
            this.TelopClearAllButton.Text = "TelopClearAllButton";
            // 
            // TelopAddButton
            // 
            this.TelopAddButton.Name = "TelopAddButton";
            this.TelopAddButton.Size = new System.Drawing.Size(106, 20);
            this.TelopAddButton.Text = "TelopAddButton";
            // 
            // TelopDetailGroupBox
            // 
            this.TelopDetailGroupBox.BackColor = System.Drawing.Color.White;
            this.TelopDetailGroupBox.Controls.Add(this.panel9);
            this.TelopDetailGroupBox.Controls.Add(this.tabControlExtHoriz3);
            this.TelopDetailGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TelopDetailGroupBox.Location = new System.Drawing.Point(0, 0);
            this.TelopDetailGroupBox.Name = "TelopDetailGroupBox";
            this.TelopDetailGroupBox.Size = new System.Drawing.Size(539, 825);
            this.TelopDetailGroupBox.TabIndex = 5;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.White;
            this.panel9.Controls.Add(this.TelopUpdateButton);
            this.panel9.Controls.Add(this.TelopDeleteButton);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel9.Location = new System.Drawing.Point(0, 793);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(539, 32);
            this.panel9.TabIndex = 2;
            // 
            // TelopUpdateButton
            // 
            this.TelopUpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TelopUpdateButton.Location = new System.Drawing.Point(326, 4);
            this.TelopUpdateButton.Name = "TelopUpdateButton";
            this.TelopUpdateButton.Size = new System.Drawing.Size(102, 25);
            this.TelopUpdateButton.TabIndex = 15;
            this.TelopUpdateButton.Text = "UpdateButton";
            this.TelopUpdateButton.UseVisualStyleBackColor = true;
            // 
            // TelopDeleteButton
            // 
            this.TelopDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TelopDeleteButton.Location = new System.Drawing.Point(434, 4);
            this.TelopDeleteButton.Name = "TelopDeleteButton";
            this.TelopDeleteButton.Size = new System.Drawing.Size(102, 25);
            this.TelopDeleteButton.TabIndex = 14;
            this.TelopDeleteButton.Text = "DeleteButton";
            this.TelopDeleteButton.UseVisualStyleBackColor = true;
            // 
            // tabControlExtHoriz3
            // 
            this.tabControlExtHoriz3.Controls.Add(this.tabPage1);
            this.tabControlExtHoriz3.Controls.Add(this.tabPage2);
            this.tabControlExtHoriz3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlExtHoriz3.ItemSize = new System.Drawing.Size(160, 32);
            this.tabControlExtHoriz3.Location = new System.Drawing.Point(0, 0);
            this.tabControlExtHoriz3.Name = "tabControlExtHoriz3";
            this.tabControlExtHoriz3.SelectedIndex = 0;
            this.tabControlExtHoriz3.Size = new System.Drawing.Size(539, 825);
            this.tabControlExtHoriz3.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.panel6);
            this.tabPage1.Controls.Add(this.TelopTitleTextBox);
            this.tabPage1.Controls.Add(this.TelopMessageTextBox);
            this.tabPage1.Controls.Add(this.TelopKeywordTextBox);
            this.tabPage1.Controls.Add(this.DisplayTimeNumericUpDown);
            this.tabPage1.Controls.Add(this.TelopKeywordToHideTextBox);
            this.tabPage1.Controls.Add(this.TelopDelayNumericUpDown);
            this.tabPage1.Controls.Add(this.TelopSetConditionButton);
            this.tabPage1.Controls.Add(this.label47);
            this.tabPage1.Controls.Add(this.TelopSelectZoneButton);
            this.tabPage1.Controls.Add(this.TelopVisualSetting);
            this.tabPage1.Controls.Add(this.TelopSelectJobButton);
            this.tabPage1.Controls.Add(this.label46);
            this.tabPage1.Controls.Add(this.TelopProgressBarEnabledCheckBox);
            this.tabPage1.Controls.Add(this.EnabledAddMessageCheckBox);
            this.tabPage1.Controls.Add(this.label45);
            this.tabPage1.Controls.Add(this.TelopTopNumericUpDown);
            this.tabPage1.Controls.Add(this.label35);
            this.tabPage1.Controls.Add(this.label40);
            this.tabPage1.Controls.Add(this.label34);
            this.tabPage1.Controls.Add(this.label36);
            this.tabPage1.Controls.Add(this.TelopLeftNumericUpDown);
            this.tabPage1.Controls.Add(this.label39);
            this.tabPage1.Controls.Add(this.label33);
            this.tabPage1.Controls.Add(this.label32);
            this.tabPage1.Controls.Add(this.TelopRegexEnabledCheckBox);
            this.tabPage1.Controls.Add(this.label44);
            this.tabPage1.Location = new System.Drawing.Point(-1, 31);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(541, 795);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "GeneralTab";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            this.panel6.Location = new System.Drawing.Point(6, 445);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(529, 68);
            this.panel6.TabIndex = 76;
            // 
            // TelopTitleTextBox
            // 
            this.TelopTitleTextBox.Location = new System.Drawing.Point(5, 27);
            this.TelopTitleTextBox.Name = "TelopTitleTextBox";
            this.TelopTitleTextBox.Size = new System.Drawing.Size(498, 19);
            this.TelopTitleTextBox.TabIndex = 50;
            // 
            // TelopMessageTextBox
            // 
            this.TelopMessageTextBox.Location = new System.Drawing.Point(5, 70);
            this.TelopMessageTextBox.Name = "TelopMessageTextBox";
            this.TelopMessageTextBox.Size = new System.Drawing.Size(498, 19);
            this.TelopMessageTextBox.TabIndex = 52;
            // 
            // TelopKeywordTextBox
            // 
            this.TelopKeywordTextBox.Location = new System.Drawing.Point(5, 113);
            this.TelopKeywordTextBox.Name = "TelopKeywordTextBox";
            this.TelopKeywordTextBox.Size = new System.Drawing.Size(498, 19);
            this.TelopKeywordTextBox.TabIndex = 54;
            // 
            // DisplayTimeNumericUpDown
            // 
            this.DisplayTimeNumericUpDown.DecimalPlaces = 1;
            this.DisplayTimeNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.DisplayTimeNumericUpDown.Location = new System.Drawing.Point(145, 223);
            this.DisplayTimeNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.DisplayTimeNumericUpDown.Name = "DisplayTimeNumericUpDown";
            this.DisplayTimeNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.DisplayTimeNumericUpDown.TabIndex = 68;
            this.DisplayTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.DisplayTimeNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TelopKeywordToHideTextBox
            // 
            this.TelopKeywordToHideTextBox.Location = new System.Drawing.Point(5, 156);
            this.TelopKeywordToHideTextBox.Name = "TelopKeywordToHideTextBox";
            this.TelopKeywordToHideTextBox.Size = new System.Drawing.Size(498, 19);
            this.TelopKeywordToHideTextBox.TabIndex = 55;
            // 
            // TelopDelayNumericUpDown
            // 
            this.TelopDelayNumericUpDown.DecimalPlaces = 1;
            this.TelopDelayNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TelopDelayNumericUpDown.Location = new System.Drawing.Point(5, 223);
            this.TelopDelayNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TelopDelayNumericUpDown.Name = "TelopDelayNumericUpDown";
            this.TelopDelayNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.TelopDelayNumericUpDown.TabIndex = 58;
            this.TelopDelayNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TelopSetConditionButton
            // 
            this.TelopSetConditionButton.Location = new System.Drawing.Point(306, 414);
            this.TelopSetConditionButton.Name = "TelopSetConditionButton";
            this.TelopSetConditionButton.Size = new System.Drawing.Size(144, 25);
            this.TelopSetConditionButton.TabIndex = 75;
            this.TelopSetConditionButton.Text = "SetConditionForStartButton";
            this.TelopSetConditionButton.UseVisualStyleBackColor = true;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(2, 12);
            this.label47.Margin = new System.Windows.Forms.Padding(0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(66, 12);
            this.label47.TabIndex = 51;
            this.label47.Text = "TickerName";
            // 
            // TelopSelectZoneButton
            // 
            this.TelopSelectZoneButton.Location = new System.Drawing.Point(156, 414);
            this.TelopSelectZoneButton.Name = "TelopSelectZoneButton";
            this.TelopSelectZoneButton.Size = new System.Drawing.Size(144, 25);
            this.TelopSelectZoneButton.TabIndex = 74;
            this.TelopSelectZoneButton.Text = "SelectZoneButton";
            this.TelopSelectZoneButton.UseVisualStyleBackColor = true;
            // 
            // TelopVisualSetting
            // 
            this.TelopVisualSetting.BackgroundColor = System.Drawing.Color.Empty;
            this.TelopVisualSetting.BarColor = System.Drawing.Color.White;
            this.TelopVisualSetting.BarEnabled = false;
            this.TelopVisualSetting.BarOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(120)))), ((int)(((byte)(157)))));
            this.TelopVisualSetting.BarSize = new System.Drawing.Size(190, 8);
            this.TelopVisualSetting.FontColor = System.Drawing.Color.White;
            this.TelopVisualSetting.FontOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(120)))), ((int)(((byte)(157)))));
            this.TelopVisualSetting.HideSpellName = false;
            this.TelopVisualSetting.Location = new System.Drawing.Point(5, 253);
            this.TelopVisualSetting.Name = "TelopVisualSetting";
            this.TelopVisualSetting.OverlapRecastTime = false;
            this.TelopVisualSetting.Size = new System.Drawing.Size(306, 71);
            this.TelopVisualSetting.SpellIcon = "";
            this.TelopVisualSetting.SpellIconSize = 0;
            this.TelopVisualSetting.TabIndex = 59;
            this.TelopVisualSetting.WarningFontColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(69)))), ((int)(((byte)(0)))));
            this.TelopVisualSetting.WarningFontOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            // 
            // TelopSelectJobButton
            // 
            this.TelopSelectJobButton.Location = new System.Drawing.Point(6, 414);
            this.TelopSelectJobButton.Name = "TelopSelectJobButton";
            this.TelopSelectJobButton.Size = new System.Drawing.Size(144, 25);
            this.TelopSelectJobButton.TabIndex = 73;
            this.TelopSelectJobButton.Text = "SelectJobButton";
            this.TelopSelectJobButton.UseVisualStyleBackColor = true;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(2, 55);
            this.label46.Margin = new System.Windows.Forms.Padding(0);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(123, 12);
            this.label46.TabIndex = 53;
            this.label46.Text = "MessageOnTickerLabel";
            // 
            // TelopProgressBarEnabledCheckBox
            // 
            this.TelopProgressBarEnabledCheckBox.AutoSize = true;
            this.TelopProgressBarEnabledCheckBox.Location = new System.Drawing.Point(5, 353);
            this.TelopProgressBarEnabledCheckBox.Name = "TelopProgressBarEnabledCheckBox";
            this.TelopProgressBarEnabledCheckBox.Size = new System.Drawing.Size(192, 16);
            this.TelopProgressBarEnabledCheckBox.TabIndex = 72;
            this.TelopProgressBarEnabledCheckBox.Text = "DisplayRemainingTimeCheckBox";
            this.TelopProgressBarEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // EnabledAddMessageCheckBox
            // 
            this.EnabledAddMessageCheckBox.AutoSize = true;
            this.EnabledAddMessageCheckBox.Location = new System.Drawing.Point(5, 330);
            this.EnabledAddMessageCheckBox.Name = "EnabledAddMessageCheckBox";
            this.EnabledAddMessageCheckBox.Size = new System.Drawing.Size(141, 16);
            this.EnabledAddMessageCheckBox.TabIndex = 71;
            this.EnabledAddMessageCheckBox.Text = "AddMessageCheckBox";
            this.EnabledAddMessageCheckBox.UseVisualStyleBackColor = true;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(3, 98);
            this.label45.Margin = new System.Windows.Forms.Padding(0);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(121, 12);
            this.label45.TabIndex = 57;
            this.label45.Text = "MatchingLogWordLabel";
            // 
            // TelopTopNumericUpDown
            // 
            this.TelopTopNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TelopTopNumericUpDown.Location = new System.Drawing.Point(160, 381);
            this.TelopTopNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TelopTopNumericUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.TelopTopNumericUpDown.Name = "TelopTopNumericUpDown";
            this.TelopTopNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.TelopTopNumericUpDown.TabIndex = 66;
            this.TelopTopNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(142, 383);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(12, 12);
            this.label35.TabIndex = 65;
            this.label35.Text = "Y";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(218, 225);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(86, 12);
            this.label40.TabIndex = 70;
            this.label40.Text = "DisplaySeconds";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(49, 383);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(12, 12);
            this.label34.TabIndex = 64;
            this.label34.Text = "X";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(372, 55);
            this.label36.Margin = new System.Windows.Forms.Padding(0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(131, 12);
            this.label36.TabIndex = 67;
            this.label36.Text = "CommaLineBreakExplain";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TelopLeftNumericUpDown
            // 
            this.TelopLeftNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TelopLeftNumericUpDown.Location = new System.Drawing.Point(67, 381);
            this.TelopLeftNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TelopLeftNumericUpDown.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.TelopLeftNumericUpDown.Name = "TelopLeftNumericUpDown";
            this.TelopLeftNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.TelopLeftNumericUpDown.TabIndex = 63;
            this.TelopLeftNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(79, 225);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(48, 12);
            this.label39.TabIndex = 69;
            this.label39.Text = "Duration";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(3, 384);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(48, 12);
            this.label33.TabIndex = 62;
            this.label33.Text = "Location";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(3, 141);
            this.label32.Margin = new System.Windows.Forms.Padding(0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(118, 12);
            this.label32.TabIndex = 61;
            this.label32.Text = "MatchingWordsToHide";
            // 
            // TelopRegexEnabledCheckBox
            // 
            this.TelopRegexEnabledCheckBox.AutoSize = true;
            this.TelopRegexEnabledCheckBox.Location = new System.Drawing.Point(6, 181);
            this.TelopRegexEnabledCheckBox.Name = "TelopRegexEnabledCheckBox";
            this.TelopRegexEnabledCheckBox.Size = new System.Drawing.Size(76, 16);
            this.TelopRegexEnabledCheckBox.TabIndex = 56;
            this.TelopRegexEnabledCheckBox.Text = "UseRegex";
            this.TelopRegexEnabledCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.TelopRegexEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(3, 206);
            this.label44.Margin = new System.Windows.Forms.Padding(0);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(81, 12);
            this.label44.TabIndex = 60;
            this.label44.Text = "MatchLogLabel";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Location = new System.Drawing.Point(-1, 31);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(541, 795);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "AlarmTab";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.TelopDelayTTSTextBox);
            this.groupBox5.Controls.Add(this.TelopDelaySoundComboBox);
            this.groupBox5.Controls.Add(this.TelopSpeak2Button);
            this.groupBox5.Controls.Add(this.TelopPlay2Button);
            this.groupBox5.Controls.Add(this.label37);
            this.groupBox5.Controls.Add(this.label38);
            this.groupBox5.Location = new System.Drawing.Point(6, 93);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(530, 75);
            this.groupBox5.TabIndex = 79;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "DelayedSoundEffect";
            // 
            // TelopDelayTTSTextBox
            // 
            this.TelopDelayTTSTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TelopDelayTTSTextBox.Location = new System.Drawing.Point(255, 44);
            this.TelopDelayTTSTextBox.Name = "TelopDelayTTSTextBox";
            this.TelopDelayTTSTextBox.Size = new System.Drawing.Size(268, 19);
            this.TelopDelayTTSTextBox.TabIndex = 1;
            // 
            // TelopDelaySoundComboBox
            // 
            this.TelopDelaySoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TelopDelaySoundComboBox.FormattingEnabled = true;
            this.TelopDelaySoundComboBox.Location = new System.Drawing.Point(6, 44);
            this.TelopDelaySoundComboBox.MaxDropDownItems = 16;
            this.TelopDelaySoundComboBox.Name = "TelopDelaySoundComboBox";
            this.TelopDelaySoundComboBox.Size = new System.Drawing.Size(243, 20);
            this.TelopDelaySoundComboBox.TabIndex = 0;
            // 
            // TelopSpeak2Button
            // 
            this.TelopSpeak2Button.Location = new System.Drawing.Point(255, 18);
            this.TelopSpeak2Button.Name = "TelopSpeak2Button";
            this.TelopSpeak2Button.Size = new System.Drawing.Size(33, 20);
            this.TelopSpeak2Button.TabIndex = 3;
            this.TelopSpeak2Button.Text = "SpeakButton";
            this.TelopSpeak2Button.UseVisualStyleBackColor = true;
            // 
            // TelopPlay2Button
            // 
            this.TelopPlay2Button.Location = new System.Drawing.Point(6, 18);
            this.TelopPlay2Button.Name = "TelopPlay2Button";
            this.TelopPlay2Button.Size = new System.Drawing.Size(33, 20);
            this.TelopPlay2Button.TabIndex = 1;
            this.TelopPlay2Button.Text = "PlayButton";
            this.TelopPlay2Button.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(294, 22);
            this.label37.Margin = new System.Windows.Forms.Padding(0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(99, 12);
            this.label37.TabIndex = 15;
            this.label37.Text = "TextToSpeakLabel";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(45, 22);
            this.label38.Margin = new System.Windows.Forms.Padding(0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(90, 12);
            this.label38.TabIndex = 13;
            this.label38.Text = "WaveSoundLabel";
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.TelopMatchTTSTextBox);
            this.groupBox7.Controls.Add(this.TelopMatchSoundComboBox);
            this.groupBox7.Controls.Add(this.TelopSpeak1Button);
            this.groupBox7.Controls.Add(this.TelopPlay1Button);
            this.groupBox7.Controls.Add(this.label41);
            this.groupBox7.Controls.Add(this.label42);
            this.groupBox7.Location = new System.Drawing.Point(6, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(530, 75);
            this.groupBox7.TabIndex = 78;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "ImmediateSoundEffect";
            // 
            // TelopMatchTTSTextBox
            // 
            this.TelopMatchTTSTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TelopMatchTTSTextBox.Location = new System.Drawing.Point(255, 44);
            this.TelopMatchTTSTextBox.Name = "TelopMatchTTSTextBox";
            this.TelopMatchTTSTextBox.Size = new System.Drawing.Size(268, 19);
            this.TelopMatchTTSTextBox.TabIndex = 1;
            // 
            // TelopMatchSoundComboBox
            // 
            this.TelopMatchSoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TelopMatchSoundComboBox.FormattingEnabled = true;
            this.TelopMatchSoundComboBox.Location = new System.Drawing.Point(6, 44);
            this.TelopMatchSoundComboBox.MaxDropDownItems = 16;
            this.TelopMatchSoundComboBox.Name = "TelopMatchSoundComboBox";
            this.TelopMatchSoundComboBox.Size = new System.Drawing.Size(243, 20);
            this.TelopMatchSoundComboBox.TabIndex = 0;
            // 
            // TelopSpeak1Button
            // 
            this.TelopSpeak1Button.Location = new System.Drawing.Point(255, 18);
            this.TelopSpeak1Button.Name = "TelopSpeak1Button";
            this.TelopSpeak1Button.Size = new System.Drawing.Size(33, 20);
            this.TelopSpeak1Button.TabIndex = 3;
            this.TelopSpeak1Button.Text = "SpeakButton";
            this.TelopSpeak1Button.UseVisualStyleBackColor = true;
            // 
            // TelopPlay1Button
            // 
            this.TelopPlay1Button.Location = new System.Drawing.Point(6, 18);
            this.TelopPlay1Button.Name = "TelopPlay1Button";
            this.TelopPlay1Button.Size = new System.Drawing.Size(33, 20);
            this.TelopPlay1Button.TabIndex = 1;
            this.TelopPlay1Button.Text = "PlayButton";
            this.TelopPlay1Button.UseVisualStyleBackColor = true;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(294, 22);
            this.label41.Margin = new System.Windows.Forms.Padding(0);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(99, 12);
            this.label41.TabIndex = 15;
            this.label41.Text = "TextToSpeakLabel";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(45, 22);
            this.label42.Margin = new System.Windows.Forms.Padding(0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(90, 12);
            this.label42.TabIndex = 13;
            this.label42.Text = "WaveSoundLabel";
            // 
            // CombatAnalyzerTabPage
            // 
            this.CombatAnalyzerTabPage.BackColor = System.Drawing.Color.White;
            this.CombatAnalyzerTabPage.Controls.Add(this.CombatLogListView);
            this.CombatAnalyzerTabPage.Controls.Add(this.panel2);
            this.CombatAnalyzerTabPage.Controls.Add(this.panel1);
            this.CombatAnalyzerTabPage.Controls.Add(this.lblCombatAnalyzer);
            this.CombatAnalyzerTabPage.Location = new System.Drawing.Point(264, 4);
            this.CombatAnalyzerTabPage.Name = "CombatAnalyzerTabPage";
            this.CombatAnalyzerTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.CombatAnalyzerTabPage.Size = new System.Drawing.Size(1065, 829);
            this.CombatAnalyzerTabPage.TabIndex = 3;
            this.CombatAnalyzerTabPage.Text = "CombatAnalyzerTabTitle";
            // 
            // CombatLogListView
            // 
            this.CombatLogListView.BackColor = System.Drawing.SystemColors.Window;
            this.CombatLogListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CombatLogListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DummyColumnHeader,
            this.NoColumnHeader,
            this.TimeStampColumnHeader,
            this.ElapsedColumnHeader,
            this.LogTypeColumnHeader,
            this.ActorColumnHeader,
            this.HPRateColumnHeader,
            this.ActionColumnHeader,
            this.SpanColumnHeader,
            this.LogColumnHeader});
            this.CombatLogListView.ContextMenuStrip = this.CombatAnalyzerContextMenuStrip;
            this.CombatLogListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CombatLogListView.FullRowSelect = true;
            this.CombatLogListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.CombatLogListView.HideSelection = false;
            this.CombatLogListView.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.CombatLogListView.Location = new System.Drawing.Point(2, 100);
            this.CombatLogListView.Name = "CombatLogListView";
            this.CombatLogListView.Size = new System.Drawing.Size(1061, 689);
            this.CombatLogListView.TabIndex = 19;
            this.CombatLogListView.UseCompatibleStateImageBehavior = false;
            this.CombatLogListView.View = System.Windows.Forms.View.Details;
            // 
            // DummyColumnHeader
            // 
            this.DummyColumnHeader.Text = "";
            this.DummyColumnHeader.Width = 0;
            // 
            // NoColumnHeader
            // 
            this.NoColumnHeader.Text = "NumberHeader";
            this.NoColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NoColumnHeader.Width = 50;
            // 
            // TimeStampColumnHeader
            // 
            this.TimeStampColumnHeader.Text = "TimestampHeader";
            this.TimeStampColumnHeader.Width = 140;
            // 
            // ElapsedColumnHeader
            // 
            this.ElapsedColumnHeader.Text = "ElapsedHeader";
            this.ElapsedColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ElapsedColumnHeader.Width = 80;
            // 
            // LogTypeColumnHeader
            // 
            this.LogTypeColumnHeader.Text = "LogTypeHeader";
            this.LogTypeColumnHeader.Width = 100;
            // 
            // ActorColumnHeader
            // 
            this.ActorColumnHeader.Text = "ActorHeader";
            this.ActorColumnHeader.Width = 120;
            // 
            // HPRateColumnHeader
            // 
            this.HPRateColumnHeader.Text = "HPHeader";
            this.HPRateColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.HPRateColumnHeader.Width = 80;
            // 
            // ActionColumnHeader
            // 
            this.ActionColumnHeader.Text = "ActionHeader";
            this.ActionColumnHeader.Width = 300;
            // 
            // SpanColumnHeader
            // 
            this.SpanColumnHeader.Text = "SpanHeader";
            this.SpanColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.SpanColumnHeader.Width = 80;
            // 
            // LogColumnHeader
            // 
            this.LogColumnHeader.Text = "LogHeader";
            this.LogColumnHeader.Width = 600;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CombatLogEnabledCheckBox);
            this.panel2.Controls.Add(this.CombatLogBufferSizeNumericUpDown);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(2, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1061, 75);
            this.panel2.TabIndex = 22;
            // 
            // CombatLogEnabledCheckBox
            // 
            this.CombatLogEnabledCheckBox.AutoSize = true;
            this.CombatLogEnabledCheckBox.Location = new System.Drawing.Point(8, 9);
            this.CombatLogEnabledCheckBox.Name = "CombatLogEnabledCheckBox";
            this.CombatLogEnabledCheckBox.Size = new System.Drawing.Size(173, 16);
            this.CombatLogEnabledCheckBox.TabIndex = 15;
            this.CombatLogEnabledCheckBox.Text = "CombatLogEnabledCheckBox";
            this.CombatLogEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // CombatLogBufferSizeNumericUpDown
            // 
            this.CombatLogBufferSizeNumericUpDown.Location = new System.Drawing.Point(131, 40);
            this.CombatLogBufferSizeNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.CombatLogBufferSizeNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.CombatLogBufferSizeNumericUpDown.Name = "CombatLogBufferSizeNumericUpDown";
            this.CombatLogBufferSizeNumericUpDown.Size = new System.Drawing.Size(67, 19);
            this.CombatLogBufferSizeNumericUpDown.TabIndex = 16;
            this.CombatLogBufferSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.CombatLogBufferSizeNumericUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 42);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 12);
            this.label5.TabIndex = 17;
            this.label5.Text = "BufferSize";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.Location = new System.Drawing.Point(204, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "LinesLabel";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ExportCSVButton);
            this.panel1.Controls.Add(this.AnalyzeCombatButton);
            this.panel1.Controls.Add(this.CombatAnalyzingLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(2, 789);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1061, 38);
            this.panel1.TabIndex = 21;
            // 
            // ExportCSVButton
            // 
            this.ExportCSVButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportCSVButton.Location = new System.Drawing.Point(953, 6);
            this.ExportCSVButton.Name = "ExportCSVButton";
            this.ExportCSVButton.Size = new System.Drawing.Size(102, 25);
            this.ExportCSVButton.TabIndex = 20;
            this.ExportCSVButton.Text = "ExportCSVButton";
            this.ExportCSVButton.UseVisualStyleBackColor = true;
            this.ExportCSVButton.Click += new System.EventHandler(this.ExportCSVButton_Click);
            // 
            // AnalyzeCombatButton
            // 
            this.AnalyzeCombatButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AnalyzeCombatButton.Location = new System.Drawing.Point(845, 6);
            this.AnalyzeCombatButton.Name = "AnalyzeCombatButton";
            this.AnalyzeCombatButton.Size = new System.Drawing.Size(102, 25);
            this.AnalyzeCombatButton.TabIndex = 1;
            this.AnalyzeCombatButton.Text = "AnalyzeCombat";
            this.AnalyzeCombatButton.UseVisualStyleBackColor = true;
            // 
            // CombatAnalyzingLabel
            // 
            this.CombatAnalyzingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CombatAnalyzingLabel.AutoSize = true;
            this.CombatAnalyzingLabel.Location = new System.Drawing.Point(714, 13);
            this.CombatAnalyzingLabel.Name = "CombatAnalyzingLabel";
            this.CombatAnalyzingLabel.Size = new System.Drawing.Size(93, 12);
            this.CombatAnalyzingLabel.TabIndex = 14;
            this.CombatAnalyzingLabel.Text = "CombatAnalyzing";
            // 
            // lblCombatAnalyzer
            // 
            this.lblCombatAnalyzer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblCombatAnalyzer.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCombatAnalyzer.Location = new System.Drawing.Point(2, 2);
            this.lblCombatAnalyzer.Name = "lblCombatAnalyzer";
            this.lblCombatAnalyzer.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblCombatAnalyzer.Size = new System.Drawing.Size(1061, 23);
            this.lblCombatAnalyzer.TabIndex = 32;
            this.lblCombatAnalyzer.Text = "lblCombatAnalyzer";
            this.lblCombatAnalyzer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NameStyleTabPage
            // 
            this.NameStyleTabPage.Location = new System.Drawing.Point(264, 4);
            this.NameStyleTabPage.Name = "NameStyleTabPage";
            this.NameStyleTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.NameStyleTabPage.Size = new System.Drawing.Size(1065, 829);
            this.NameStyleTabPage.TabIndex = 7;
            this.NameStyleTabPage.Text = "NameStyleTabPage";
            this.NameStyleTabPage.UseVisualStyleBackColor = true;
            // 
            // OptionTabPage
            // 
            this.OptionTabPage.BackColor = System.Drawing.Color.White;
            this.OptionTabPage.Controls.Add(this.tabControlExtHoriz1);
            this.OptionTabPage.Controls.Add(this.panel7);
            this.OptionTabPage.Controls.Add(this.pnlLanguage);
            this.OptionTabPage.Controls.Add(this.lblLanguage);
            this.OptionTabPage.Location = new System.Drawing.Point(264, 4);
            this.OptionTabPage.Name = "OptionTabPage";
            this.OptionTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.OptionTabPage.Size = new System.Drawing.Size(1065, 829);
            this.OptionTabPage.TabIndex = 1;
            this.OptionTabPage.Text = "OptionTabPageTitle";
            // 
            // tabControlExtHoriz1
            // 
            this.tabControlExtHoriz1.Controls.Add(this.tabOverlayOptions);
            this.tabControlExtHoriz1.Controls.Add(this.tabDetailOptions);
            this.tabControlExtHoriz1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlExtHoriz1.ItemSize = new System.Drawing.Size(160, 32);
            this.tabControlExtHoriz1.Location = new System.Drawing.Point(2, 58);
            this.tabControlExtHoriz1.Name = "tabControlExtHoriz1";
            this.tabControlExtHoriz1.SelectedIndex = 0;
            this.tabControlExtHoriz1.Size = new System.Drawing.Size(1061, 738);
            this.tabControlExtHoriz1.TabIndex = 76;
            // 
            // tabOverlayOptions
            // 
            this.tabOverlayOptions.AutoScroll = true;
            this.tabOverlayOptions.BackColor = System.Drawing.Color.White;
            this.tabOverlayOptions.Controls.Add(this.RenderWithCPUOnlyCheckBox);
            this.tabOverlayOptions.Controls.Add(this.label27);
            this.tabOverlayOptions.Controls.Add(this.label107);
            this.tabOverlayOptions.Controls.Add(this.label106);
            this.tabOverlayOptions.Controls.Add(this.TextBlurRateNumericUpDown);
            this.tabOverlayOptions.Controls.Add(this.TextOutlineThicknessRateNumericUpDown);
            this.tabOverlayOptions.Controls.Add(this.DefaultVisualSetting);
            this.tabOverlayOptions.Controls.Add(this.label18);
            this.tabOverlayOptions.Controls.Add(this.OpacityNumericUpDown);
            this.tabOverlayOptions.Controls.Add(this.label20);
            this.tabOverlayOptions.Controls.Add(this.label21);
            this.tabOverlayOptions.Controls.Add(this.ClickThroughCheckBox);
            this.tabOverlayOptions.Controls.Add(this.label22);
            this.tabOverlayOptions.Controls.Add(this.EnabledSpellTimerNoDecimalCheckBox);
            this.tabOverlayOptions.Controls.Add(this.label31);
            this.tabOverlayOptions.Controls.Add(this.label49);
            this.tabOverlayOptions.Controls.Add(this.label93);
            this.tabOverlayOptions.Controls.Add(this.UseOtherThanFFXIVCheckbox);
            this.tabOverlayOptions.Controls.Add(this.ReduceIconBrightnessNumericUpDown);
            this.tabOverlayOptions.Controls.Add(this.label94);
            this.tabOverlayOptions.Controls.Add(this.SwitchOverlayButton);
            this.tabOverlayOptions.Controls.Add(this.SwitchTelopButton);
            this.tabOverlayOptions.Controls.Add(this.OverlayForceVisibleCheckBox);
            this.tabOverlayOptions.Controls.Add(this.HideWhenNotActiceCheckBox);
            this.tabOverlayOptions.Location = new System.Drawing.Point(-1, 31);
            this.tabOverlayOptions.Name = "tabOverlayOptions";
            this.tabOverlayOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabOverlayOptions.Size = new System.Drawing.Size(1063, 708);
            this.tabOverlayOptions.TabIndex = 0;
            this.tabOverlayOptions.Text = "tabOverlayOptions";
            // 
            // RenderWithCPUOnlyCheckBox
            // 
            this.RenderWithCPUOnlyCheckBox.AutoSize = true;
            this.RenderWithCPUOnlyCheckBox.Location = new System.Drawing.Point(291, 415);
            this.RenderWithCPUOnlyCheckBox.Name = "RenderWithCPUOnlyCheckBox";
            this.RenderWithCPUOnlyCheckBox.Size = new System.Drawing.Size(64, 16);
            this.RenderWithCPUOnlyCheckBox.TabIndex = 70;
            this.RenderWithCPUOnlyCheckBox.Text = "Enabled";
            this.RenderWithCPUOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(15, 416);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(109, 12);
            this.label27.TabIndex = 69;
            this.label27.Text = "RenderWithCPUOnly";
            // 
            // label107
            // 
            this.label107.AutoSize = true;
            this.label107.Location = new System.Drawing.Point(15, 377);
            this.label107.Name = "label107";
            this.label107.Size = new System.Drawing.Size(73, 12);
            this.label107.TabIndex = 68;
            this.label107.Text = "TextBlurRate";
            // 
            // label106
            // 
            this.label106.AutoSize = true;
            this.label106.Location = new System.Drawing.Point(15, 349);
            this.label106.Name = "label106";
            this.label106.Size = new System.Drawing.Size(140, 12);
            this.label106.TabIndex = 67;
            this.label106.Text = "TextOutlineThicknessRate";
            // 
            // TextBlurRateNumericUpDown
            // 
            this.TextBlurRateNumericUpDown.DecimalPlaces = 1;
            this.TextBlurRateNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextBlurRateNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.TextBlurRateNumericUpDown.Location = new System.Drawing.Point(291, 375);
            this.TextBlurRateNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.TextBlurRateNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.TextBlurRateNumericUpDown.Name = "TextBlurRateNumericUpDown";
            this.TextBlurRateNumericUpDown.Size = new System.Drawing.Size(59, 19);
            this.TextBlurRateNumericUpDown.TabIndex = 66;
            this.TextBlurRateNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TextOutlineThicknessRateNumericUpDown
            // 
            this.TextOutlineThicknessRateNumericUpDown.DecimalPlaces = 1;
            this.TextOutlineThicknessRateNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TextOutlineThicknessRateNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.TextOutlineThicknessRateNumericUpDown.Location = new System.Drawing.Point(291, 347);
            this.TextOutlineThicknessRateNumericUpDown.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.TextOutlineThicknessRateNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.TextOutlineThicknessRateNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.TextOutlineThicknessRateNumericUpDown.Name = "TextOutlineThicknessRateNumericUpDown";
            this.TextOutlineThicknessRateNumericUpDown.Size = new System.Drawing.Size(59, 19);
            this.TextOutlineThicknessRateNumericUpDown.TabIndex = 65;
            this.TextOutlineThicknessRateNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DefaultVisualSetting
            // 
            this.DefaultVisualSetting.BackgroundColor = System.Drawing.Color.Empty;
            this.DefaultVisualSetting.BarColor = System.Drawing.Color.White;
            this.DefaultVisualSetting.BarEnabled = true;
            this.DefaultVisualSetting.BarOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(120)))), ((int)(((byte)(157)))));
            this.DefaultVisualSetting.BarSize = new System.Drawing.Size(190, 8);
            this.DefaultVisualSetting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DefaultVisualSetting.FontColor = System.Drawing.Color.OrangeRed;
            this.DefaultVisualSetting.FontOutlineColor = System.Drawing.Color.DarkRed;
            this.DefaultVisualSetting.HideSpellName = false;
            this.DefaultVisualSetting.Location = new System.Drawing.Point(5, 125);
            this.DefaultVisualSetting.Name = "DefaultVisualSetting";
            this.DefaultVisualSetting.OverlapRecastTime = false;
            this.DefaultVisualSetting.Size = new System.Drawing.Size(347, 90);
            this.DefaultVisualSetting.SpellIcon = "";
            this.DefaultVisualSetting.SpellIconSize = 0;
            this.DefaultVisualSetting.TabIndex = 37;
            this.DefaultVisualSetting.WarningFontColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(69)))), ((int)(((byte)(0)))));
            this.DefaultVisualSetting.WarningFontOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(14, 105);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(128, 12);
            this.label18.TabIndex = 10;
            this.label18.Text = "ProgressBarAppearance";
            // 
            // OpacityNumericUpDown
            // 
            this.OpacityNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.OpacityNumericUpDown.Location = new System.Drawing.Point(291, 228);
            this.OpacityNumericUpDown.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.OpacityNumericUpDown.Name = "OpacityNumericUpDown";
            this.OpacityNumericUpDown.Size = new System.Drawing.Size(59, 19);
            this.OpacityNumericUpDown.TabIndex = 6;
            this.OpacityNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(356, 229);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(19, 12);
            this.label20.TabIndex = 16;
            this.label20.Text = "__%";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(14, 229);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(89, 12);
            this.label21.TabIndex = 17;
            this.label21.Text = "TransmitOverlay";
            // 
            // ClickThroughCheckBox
            // 
            this.ClickThroughCheckBox.AutoSize = true;
            this.ClickThroughCheckBox.Location = new System.Drawing.Point(291, 288);
            this.ClickThroughCheckBox.Name = "ClickThroughCheckBox";
            this.ClickThroughCheckBox.Size = new System.Drawing.Size(64, 16);
            this.ClickThroughCheckBox.TabIndex = 7;
            this.ClickThroughCheckBox.Text = "Enabled";
            this.ClickThroughCheckBox.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(15, 289);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(72, 12);
            this.label22.TabIndex = 20;
            this.label22.Text = "ClickThrough";
            // 
            // EnabledSpellTimerNoDecimalCheckBox
            // 
            this.EnabledSpellTimerNoDecimalCheckBox.AutoSize = true;
            this.EnabledSpellTimerNoDecimalCheckBox.Location = new System.Drawing.Point(291, 316);
            this.EnabledSpellTimerNoDecimalCheckBox.Name = "EnabledSpellTimerNoDecimalCheckBox";
            this.EnabledSpellTimerNoDecimalCheckBox.Size = new System.Drawing.Size(64, 16);
            this.EnabledSpellTimerNoDecimalCheckBox.TabIndex = 42;
            this.EnabledSpellTimerNoDecimalCheckBox.Text = "Enabled";
            this.EnabledSpellTimerNoDecimalCheckBox.UseVisualStyleBackColor = true;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(15, 317);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(122, 12);
            this.label31.TabIndex = 43;
            this.label31.Text = "SpellTimerFormatLabel";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(373, 317);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(159, 12);
            this.label49.TabIndex = 44;
            this.label49.Text = "SpellTimerFormatExplainLabel";
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Location = new System.Drawing.Point(14, 258);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(119, 12);
            this.label93.TabIndex = 61;
            this.label93.Text = "ReduceIconBrightness";
            // 
            // UseOtherThanFFXIVCheckbox
            // 
            this.UseOtherThanFFXIVCheckbox.AutoSize = true;
            this.UseOtherThanFFXIVCheckbox.ForeColor = System.Drawing.Color.OrangeRed;
            this.UseOtherThanFFXIVCheckbox.Location = new System.Drawing.Point(269, 70);
            this.UseOtherThanFFXIVCheckbox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.UseOtherThanFFXIVCheckbox.Name = "UseOtherThanFFXIVCheckbox";
            this.UseOtherThanFFXIVCheckbox.Size = new System.Drawing.Size(179, 16);
            this.UseOtherThanFFXIVCheckbox.TabIndex = 64;
            this.UseOtherThanFFXIVCheckbox.Text = "UseOtherThanFFXIVCheckbox";
            this.UseOtherThanFFXIVCheckbox.UseVisualStyleBackColor = true;
            // 
            // ReduceIconBrightnessNumericUpDown
            // 
            this.ReduceIconBrightnessNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ReduceIconBrightnessNumericUpDown.Location = new System.Drawing.Point(291, 256);
            this.ReduceIconBrightnessNumericUpDown.Name = "ReduceIconBrightnessNumericUpDown";
            this.ReduceIconBrightnessNumericUpDown.Size = new System.Drawing.Size(59, 19);
            this.ReduceIconBrightnessNumericUpDown.TabIndex = 62;
            this.ReduceIconBrightnessNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(356, 258);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(19, 12);
            this.label94.TabIndex = 63;
            this.label94.Text = "__%";
            // 
            // SwitchOverlayButton
            // 
            this.SwitchOverlayButton.Location = new System.Drawing.Point(4, 6);
            this.SwitchOverlayButton.Name = "SwitchOverlayButton";
            this.SwitchOverlayButton.Size = new System.Drawing.Size(250, 42);
            this.SwitchOverlayButton.TabIndex = 0;
            this.SwitchOverlayButton.Text = "SwitchOverlayButton";
            this.SwitchOverlayButton.UseVisualStyleBackColor = true;
            // 
            // SwitchTelopButton
            // 
            this.SwitchTelopButton.Location = new System.Drawing.Point(4, 54);
            this.SwitchTelopButton.Name = "SwitchTelopButton";
            this.SwitchTelopButton.Size = new System.Drawing.Size(250, 42);
            this.SwitchTelopButton.TabIndex = 29;
            this.SwitchTelopButton.Text = "AlwaysDisplayTelop";
            this.SwitchTelopButton.UseVisualStyleBackColor = true;
            // 
            // OverlayForceVisibleCheckBox
            // 
            this.OverlayForceVisibleCheckBox.AutoSize = true;
            this.OverlayForceVisibleCheckBox.Location = new System.Drawing.Point(269, 20);
            this.OverlayForceVisibleCheckBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.OverlayForceVisibleCheckBox.Name = "OverlayForceVisibleCheckBox";
            this.OverlayForceVisibleCheckBox.Size = new System.Drawing.Size(179, 16);
            this.OverlayForceVisibleCheckBox.TabIndex = 38;
            this.OverlayForceVisibleCheckBox.Text = "OverlayForceVisibleCheckBox";
            this.OverlayForceVisibleCheckBox.UseVisualStyleBackColor = true;
            // 
            // HideWhenNotActiceCheckBox
            // 
            this.HideWhenNotActiceCheckBox.AutoSize = true;
            this.HideWhenNotActiceCheckBox.Location = new System.Drawing.Point(269, 45);
            this.HideWhenNotActiceCheckBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.HideWhenNotActiceCheckBox.Name = "HideWhenNotActiceCheckBox";
            this.HideWhenNotActiceCheckBox.Size = new System.Drawing.Size(177, 16);
            this.HideWhenNotActiceCheckBox.TabIndex = 56;
            this.HideWhenNotActiceCheckBox.Text = "HideWhenNotActiceCheckBox";
            this.HideWhenNotActiceCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabDetailOptions
            // 
            this.tabDetailOptions.AutoScroll = true;
            this.tabDetailOptions.BackColor = System.Drawing.Color.White;
            this.tabDetailOptions.Controls.Add(this.ToComplementUnknownSkillCheckBox);
            this.tabDetailOptions.Controls.Add(this.label57);
            this.tabDetailOptions.Controls.Add(this.NotifyToACTCheckBox);
            this.tabDetailOptions.Controls.Add(this.label23);
            this.tabDetailOptions.Controls.Add(this.AutoSortCheckBox);
            this.tabDetailOptions.Controls.Add(this.TimeOfHideNumericUpDown);
            this.tabDetailOptions.Controls.Add(this.label104);
            this.tabDetailOptions.Controls.Add(this.label24);
            this.tabDetailOptions.Controls.Add(this.DetectPacketDumpcheckBox);
            this.tabDetailOptions.Controls.Add(this.label25);
            this.tabDetailOptions.Controls.Add(this.RemoveTooltipSymbolsCheckBox);
            this.tabDetailOptions.Controls.Add(this.label26);
            this.tabDetailOptions.Controls.Add(this.label103);
            this.tabDetailOptions.Controls.Add(this.AutoSortReverseCheckBox);
            this.tabDetailOptions.Controls.Add(this.SimpleRegexCheckBox);
            this.tabDetailOptions.Controls.Add(this.label28);
            this.tabDetailOptions.Controls.Add(this.label102);
            this.tabDetailOptions.Controls.Add(this.RefreshIntervalNumericUpDown);
            this.tabDetailOptions.Controls.Add(this.ResetOnWipeOutLabel);
            this.tabDetailOptions.Controls.Add(this.label29);
            this.tabDetailOptions.Controls.Add(this.ResetOnWipeOutCheckBox);
            this.tabDetailOptions.Controls.Add(this.label30);
            this.tabDetailOptions.Controls.Add(this.label90);
            this.tabDetailOptions.Controls.Add(this.label43);
            this.tabDetailOptions.Controls.Add(this.label92);
            this.tabDetailOptions.Controls.Add(this.EnabledPTPlaceholderCheckBox);
            this.tabDetailOptions.Controls.Add(this.LogPollSleepNumericUpDown);
            this.tabDetailOptions.Controls.Add(this.label48);
            this.tabDetailOptions.Controls.Add(this.label91);
            this.tabDetailOptions.Controls.Add(this.label63);
            this.tabDetailOptions.Controls.Add(this.SaveLogButton);
            this.tabDetailOptions.Controls.Add(this.EnabledNotifyNormalSpellTimerCheckBox);
            this.tabDetailOptions.Controls.Add(this.SaveLogCheckBox);
            this.tabDetailOptions.Controls.Add(this.label64);
            this.tabDetailOptions.Controls.Add(this.SaveLogTextBox);
            this.tabDetailOptions.Controls.Add(this.label65);
            this.tabDetailOptions.Controls.Add(this.label67);
            this.tabDetailOptions.Controls.Add(this.ReadyTextBox);
            this.tabDetailOptions.Controls.Add(this.OverTextBox);
            this.tabDetailOptions.Controls.Add(this.label66);
            this.tabDetailOptions.Location = new System.Drawing.Point(-1, 31);
            this.tabDetailOptions.Name = "tabDetailOptions";
            this.tabDetailOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabDetailOptions.Size = new System.Drawing.Size(1063, 708);
            this.tabDetailOptions.TabIndex = 1;
            this.tabDetailOptions.Text = "tabDetailOptions";
            // 
            // ToComplementUnknownSkillCheckBox
            // 
            this.ToComplementUnknownSkillCheckBox.AutoSize = true;
            this.ToComplementUnknownSkillCheckBox.Location = new System.Drawing.Point(325, 381);
            this.ToComplementUnknownSkillCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.ToComplementUnknownSkillCheckBox.Name = "ToComplementUnknownSkillCheckBox";
            this.ToComplementUnknownSkillCheckBox.Size = new System.Drawing.Size(64, 16);
            this.ToComplementUnknownSkillCheckBox.TabIndex = 75;
            this.ToComplementUnknownSkillCheckBox.Text = "Enabled";
            this.ToComplementUnknownSkillCheckBox.UseVisualStyleBackColor = true;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(8, 382);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(149, 12);
            this.label57.TabIndex = 74;
            this.label57.Text = "ToComplementUnknownSkill";
            // 
            // NotifyToACTCheckBox
            // 
            this.NotifyToACTCheckBox.AutoSize = true;
            this.NotifyToACTCheckBox.Location = new System.Drawing.Point(419, 297);
            this.NotifyToACTCheckBox.Name = "NotifyToACTCheckBox";
            this.NotifyToACTCheckBox.Size = new System.Drawing.Size(143, 16);
            this.NotifyToACTCheckBox.TabIndex = 73;
            this.NotifyToACTCheckBox.Text = "NotifyToACTCheckBox";
            this.NotifyToACTCheckBox.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(8, 13);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(104, 12);
            this.label23.TabIndex = 21;
            this.label23.Text = "SortByRecastOrder";
            // 
            // AutoSortCheckBox
            // 
            this.AutoSortCheckBox.AutoSize = true;
            this.AutoSortCheckBox.Location = new System.Drawing.Point(326, 12);
            this.AutoSortCheckBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.AutoSortCheckBox.Name = "AutoSortCheckBox";
            this.AutoSortCheckBox.Size = new System.Drawing.Size(64, 16);
            this.AutoSortCheckBox.TabIndex = 8;
            this.AutoSortCheckBox.Text = "Enabled";
            this.AutoSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // TimeOfHideNumericUpDown
            // 
            this.TimeOfHideNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.TimeOfHideNumericUpDown.Location = new System.Drawing.Point(326, 37);
            this.TimeOfHideNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TimeOfHideNumericUpDown.Name = "TimeOfHideNumericUpDown";
            this.TimeOfHideNumericUpDown.Size = new System.Drawing.Size(59, 19);
            this.TimeOfHideNumericUpDown.TabIndex = 10;
            this.TimeOfHideNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label104
            // 
            this.label104.AutoSize = true;
            this.label104.Location = new System.Drawing.Point(8, 410);
            this.label104.Name = "label104";
            this.label104.Size = new System.Drawing.Size(130, 12);
            this.label104.TabIndex = 72;
            this.label104.Text = "DetectPacketDumpLabel";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(391, 38);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(74, 12);
            this.label24.TabIndex = 23;
            this.label24.Text = "SecondsLater";
            // 
            // DetectPacketDumpcheckBox
            // 
            this.DetectPacketDumpcheckBox.AutoSize = true;
            this.DetectPacketDumpcheckBox.Location = new System.Drawing.Point(325, 409);
            this.DetectPacketDumpcheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.DetectPacketDumpcheckBox.Name = "DetectPacketDumpcheckBox";
            this.DetectPacketDumpcheckBox.Size = new System.Drawing.Size(64, 16);
            this.DetectPacketDumpcheckBox.TabIndex = 71;
            this.DetectPacketDumpcheckBox.Text = "Enabled";
            this.DetectPacketDumpcheckBox.UseVisualStyleBackColor = true;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 39);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(96, 12);
            this.label25.TabIndex = 24;
            this.label25.Text = "EraseAfterRecast";
            // 
            // RemoveTooltipSymbolsCheckBox
            // 
            this.RemoveTooltipSymbolsCheckBox.AutoSize = true;
            this.RemoveTooltipSymbolsCheckBox.Location = new System.Drawing.Point(325, 353);
            this.RemoveTooltipSymbolsCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.RemoveTooltipSymbolsCheckBox.Name = "RemoveTooltipSymbolsCheckBox";
            this.RemoveTooltipSymbolsCheckBox.Size = new System.Drawing.Size(64, 16);
            this.RemoveTooltipSymbolsCheckBox.TabIndex = 70;
            this.RemoveTooltipSymbolsCheckBox.Text = "Enabled";
            this.RemoveTooltipSymbolsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(441, 38);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(97, 12);
            this.label26.TabIndex = 11;
            this.label26.Text = "DisableWhenClear";
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(8, 354);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(151, 12);
            this.label103.TabIndex = 69;
            this.label103.Text = "RemoveTooltipSymbolsLabel";
            // 
            // AutoSortReverseCheckBox
            // 
            this.AutoSortReverseCheckBox.AutoSize = true;
            this.AutoSortReverseCheckBox.Location = new System.Drawing.Point(404, 12);
            this.AutoSortReverseCheckBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.AutoSortReverseCheckBox.Name = "AutoSortReverseCheckBox";
            this.AutoSortReverseCheckBox.Size = new System.Drawing.Size(161, 16);
            this.AutoSortReverseCheckBox.TabIndex = 9;
            this.AutoSortReverseCheckBox.Text = "AutoSortReverseCheckbox";
            this.AutoSortReverseCheckBox.UseVisualStyleBackColor = true;
            // 
            // SimpleRegexCheckBox
            // 
            this.SimpleRegexCheckBox.AutoSize = true;
            this.SimpleRegexCheckBox.Location = new System.Drawing.Point(325, 325);
            this.SimpleRegexCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.SimpleRegexCheckBox.Name = "SimpleRegexCheckBox";
            this.SimpleRegexCheckBox.Size = new System.Drawing.Size(64, 16);
            this.SimpleRegexCheckBox.TabIndex = 68;
            this.SimpleRegexCheckBox.Text = "Enabled";
            this.SimpleRegexCheckBox.UseVisualStyleBackColor = true;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(6, 66);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(128, 12);
            this.label28.TabIndex = 25;
            this.label28.Text = "ScreenUpdateTimeLabel";
            // 
            // label102
            // 
            this.label102.AutoSize = true;
            this.label102.Location = new System.Drawing.Point(8, 326);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(98, 12);
            this.label102.TabIndex = 67;
            this.label102.Text = "SimpleRegexLabel";
            // 
            // RefreshIntervalNumericUpDown
            // 
            this.RefreshIntervalNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.RefreshIntervalNumericUpDown.Location = new System.Drawing.Point(326, 64);
            this.RefreshIntervalNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.RefreshIntervalNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.RefreshIntervalNumericUpDown.Name = "RefreshIntervalNumericUpDown";
            this.RefreshIntervalNumericUpDown.Size = new System.Drawing.Size(59, 19);
            this.RefreshIntervalNumericUpDown.TabIndex = 26;
            this.RefreshIntervalNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RefreshIntervalNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // ResetOnWipeOutLabel
            // 
            this.ResetOnWipeOutLabel.AutoSize = true;
            this.ResetOnWipeOutLabel.Location = new System.Drawing.Point(8, 298);
            this.ResetOnWipeOutLabel.Name = "ResetOnWipeOutLabel";
            this.ResetOnWipeOutLabel.Size = new System.Drawing.Size(118, 12);
            this.ResetOnWipeOutLabel.TabIndex = 66;
            this.ResetOnWipeOutLabel.Text = "ResetOnWipeOutLabel";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(391, 65);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(62, 12);
            this.label29.TabIndex = 27;
            this.label29.Text = "Millisecond";
            // 
            // ResetOnWipeOutCheckBox
            // 
            this.ResetOnWipeOutCheckBox.AutoSize = true;
            this.ResetOnWipeOutCheckBox.Location = new System.Drawing.Point(325, 297);
            this.ResetOnWipeOutCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.ResetOnWipeOutCheckBox.Name = "ResetOnWipeOutCheckBox";
            this.ResetOnWipeOutCheckBox.Size = new System.Drawing.Size(64, 16);
            this.ResetOnWipeOutCheckBox.TabIndex = 65;
            this.ResetOnWipeOutCheckBox.Text = "Enabled";
            this.ResetOnWipeOutCheckBox.UseVisualStyleBackColor = true;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(444, 65);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(63, 12);
            this.label30.TabIndex = 28;
            this.label30.Text = "SpecsLabel";
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(444, 92);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(63, 12);
            this.label90.TabIndex = 59;
            this.label90.Text = "SpecsLabel";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(6, 121);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(100, 12);
            this.label43.TabIndex = 30;
            this.label43.Text = "PartyPronounLabel";
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(391, 92);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(62, 12);
            this.label92.TabIndex = 60;
            this.label92.Text = "Millisecond";
            // 
            // EnabledPTPlaceholderCheckBox
            // 
            this.EnabledPTPlaceholderCheckBox.AutoSize = true;
            this.EnabledPTPlaceholderCheckBox.Location = new System.Drawing.Point(326, 121);
            this.EnabledPTPlaceholderCheckBox.Name = "EnabledPTPlaceholderCheckBox";
            this.EnabledPTPlaceholderCheckBox.Size = new System.Drawing.Size(64, 16);
            this.EnabledPTPlaceholderCheckBox.TabIndex = 31;
            this.EnabledPTPlaceholderCheckBox.Text = "Enabled";
            this.EnabledPTPlaceholderCheckBox.UseVisualStyleBackColor = true;
            // 
            // LogPollSleepNumericUpDown
            // 
            this.LogPollSleepNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.LogPollSleepNumericUpDown.Location = new System.Drawing.Point(326, 91);
            this.LogPollSleepNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.LogPollSleepNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.LogPollSleepNumericUpDown.Name = "LogPollSleepNumericUpDown";
            this.LogPollSleepNumericUpDown.Size = new System.Drawing.Size(59, 19);
            this.LogPollSleepNumericUpDown.TabIndex = 58;
            this.LogPollSleepNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.LogPollSleepNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(391, 122);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(175, 12);
            this.label48.TabIndex = 32;
            this.label48.Text = "PartyMemberNumberExplainLabel";
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(6, 93);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(135, 12);
            this.label91.TabIndex = 57;
            this.label91.Text = "LogPollSleepIntervalLabel";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(6, 145);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(153, 12);
            this.label63.TabIndex = 45;
            this.label63.Text = "NotifyNormalSpellTimerLabel";
            // 
            // SaveLogButton
            // 
            this.SaveLogButton.Enabled = false;
            this.SaveLogButton.Location = new System.Drawing.Point(318, 259);
            this.SaveLogButton.Name = "SaveLogButton";
            this.SaveLogButton.Size = new System.Drawing.Size(66, 23);
            this.SaveLogButton.TabIndex = 55;
            this.SaveLogButton.Text = "ReferenceButton";
            this.SaveLogButton.UseVisualStyleBackColor = true;
            // 
            // EnabledNotifyNormalSpellTimerCheckBox
            // 
            this.EnabledNotifyNormalSpellTimerCheckBox.AutoSize = true;
            this.EnabledNotifyNormalSpellTimerCheckBox.Location = new System.Drawing.Point(326, 145);
            this.EnabledNotifyNormalSpellTimerCheckBox.Name = "EnabledNotifyNormalSpellTimerCheckBox";
            this.EnabledNotifyNormalSpellTimerCheckBox.Size = new System.Drawing.Size(64, 16);
            this.EnabledNotifyNormalSpellTimerCheckBox.TabIndex = 46;
            this.EnabledNotifyNormalSpellTimerCheckBox.Text = "Enabled";
            this.EnabledNotifyNormalSpellTimerCheckBox.UseVisualStyleBackColor = true;
            // 
            // SaveLogCheckBox
            // 
            this.SaveLogCheckBox.AutoSize = true;
            this.SaveLogCheckBox.Location = new System.Drawing.Point(326, 231);
            this.SaveLogCheckBox.Name = "SaveLogCheckBox";
            this.SaveLogCheckBox.Size = new System.Drawing.Size(64, 16);
            this.SaveLogCheckBox.TabIndex = 54;
            this.SaveLogCheckBox.Text = "Enabled";
            this.SaveLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(391, 146);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(190, 12);
            this.label64.TabIndex = 47;
            this.label64.Text = "NotifyNormalSpellTimerExplainLabel";
            // 
            // SaveLogTextBox
            // 
            this.SaveLogTextBox.Enabled = false;
            this.SaveLogTextBox.Location = new System.Drawing.Point(7, 260);
            this.SaveLogTextBox.Name = "SaveLogTextBox";
            this.SaveLogTextBox.Size = new System.Drawing.Size(306, 19);
            this.SaveLogTextBox.TabIndex = 53;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(6, 177);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(87, 12);
            this.label65.TabIndex = 48;
            this.label65.Text = "ReadyTextLabel";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(6, 231);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(75, 12);
            this.label67.TabIndex = 52;
            this.label67.Text = "SaveLogLabel";
            // 
            // ReadyTextBox
            // 
            this.ReadyTextBox.Location = new System.Drawing.Point(326, 174);
            this.ReadyTextBox.Name = "ReadyTextBox";
            this.ReadyTextBox.Size = new System.Drawing.Size(100, 19);
            this.ReadyTextBox.TabIndex = 49;
            // 
            // OverTextBox
            // 
            this.OverTextBox.Location = new System.Drawing.Point(326, 203);
            this.OverTextBox.Name = "OverTextBox";
            this.OverTextBox.Size = new System.Drawing.Size(100, 19);
            this.OverTextBox.TabIndex = 51;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(6, 206);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(79, 12);
            this.label66.TabIndex = 50;
            this.label66.Text = "OverTextLabel";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.TekiyoButton);
            this.panel7.Controls.Add(this.ShokikaButton);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(2, 796);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1061, 31);
            this.panel7.TabIndex = 77;
            // 
            // TekiyoButton
            // 
            this.TekiyoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TekiyoButton.Location = new System.Drawing.Point(801, 3);
            this.TekiyoButton.Name = "TekiyoButton";
            this.TekiyoButton.Size = new System.Drawing.Size(126, 25);
            this.TekiyoButton.TabIndex = 12;
            this.TekiyoButton.Text = "ApplyButton";
            this.TekiyoButton.UseVisualStyleBackColor = true;
            this.TekiyoButton.Click += new System.EventHandler(this.TekiyoButton_Click);
            // 
            // ShokikaButton
            // 
            this.ShokikaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShokikaButton.Location = new System.Drawing.Point(931, 3);
            this.ShokikaButton.Name = "ShokikaButton";
            this.ShokikaButton.Size = new System.Drawing.Size(126, 25);
            this.ShokikaButton.TabIndex = 13;
            this.ShokikaButton.Text = "InitializationButton";
            this.ShokikaButton.UseVisualStyleBackColor = true;
            this.ShokikaButton.Click += new System.EventHandler(this.ShokikaButton_Click);
            // 
            // pnlLanguage
            // 
            this.pnlLanguage.Controls.Add(this.label17);
            this.pnlLanguage.Controls.Add(this.LanguageComboBox);
            this.pnlLanguage.Controls.Add(this.LanguageRestartLabel);
            this.pnlLanguage.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLanguage.Location = new System.Drawing.Point(2, 26);
            this.pnlLanguage.Name = "pnlLanguage";
            this.pnlLanguage.Size = new System.Drawing.Size(1061, 32);
            this.pnlLanguage.TabIndex = 74;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(7, 9);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(80, 12);
            this.label17.TabIndex = 39;
            this.label17.Text = "LanguageLabel";
            // 
            // LanguageComboBox
            // 
            this.LanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageComboBox.FormattingEnabled = true;
            this.LanguageComboBox.Location = new System.Drawing.Point(163, 6);
            this.LanguageComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.LanguageComboBox.Name = "LanguageComboBox";
            this.LanguageComboBox.Size = new System.Drawing.Size(142, 20);
            this.LanguageComboBox.TabIndex = 40;
            // 
            // LanguageRestartLabel
            // 
            this.LanguageRestartLabel.AutoSize = true;
            this.LanguageRestartLabel.Location = new System.Drawing.Point(316, 9);
            this.LanguageRestartLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LanguageRestartLabel.Name = "LanguageRestartLabel";
            this.LanguageRestartLabel.Size = new System.Drawing.Size(88, 12);
            this.LanguageRestartLabel.TabIndex = 41;
            this.LanguageRestartLabel.Text = "RequiresRestart";
            // 
            // lblLanguage
            // 
            this.lblLanguage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLanguage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLanguage.Location = new System.Drawing.Point(2, 2);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblLanguage.Size = new System.Drawing.Size(1061, 24);
            this.lblLanguage.TabIndex = 73;
            this.lblLanguage.Text = "lblLanguage";
            this.lblLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DQXOptionTabPage
            // 
            this.DQXOptionTabPage.BackColor = System.Drawing.Color.White;
            this.DQXOptionTabPage.Controls.Add(this.lblDQX);
            this.DQXOptionTabPage.Controls.Add(this.DQXOptionEnabledCheckBox);
            this.DQXOptionTabPage.Controls.Add(this.label101);
            this.DQXOptionTabPage.Controls.Add(this.label100);
            this.DQXOptionTabPage.Controls.Add(this.label99);
            this.DQXOptionTabPage.Controls.Add(this.label98);
            this.DQXOptionTabPage.Controls.Add(this.label97);
            this.DQXOptionTabPage.Controls.Add(this.label96);
            this.DQXOptionTabPage.Controls.Add(this.label95);
            this.DQXOptionTabPage.Controls.Add(this.DQXPTMember8TextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPTMember7TextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPTMember6TextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPTMember5TextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPTMember4TextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPTMember3TextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPTMember2TextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPlayerNameTextBox);
            this.DQXOptionTabPage.Controls.Add(this.DQXPlayerNameLabel);
            this.DQXOptionTabPage.Controls.Add(this.DQXAppleyButton);
            this.DQXOptionTabPage.Location = new System.Drawing.Point(264, 4);
            this.DQXOptionTabPage.Name = "DQXOptionTabPage";
            this.DQXOptionTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.DQXOptionTabPage.Size = new System.Drawing.Size(1065, 829);
            this.DQXOptionTabPage.TabIndex = 5;
            this.DQXOptionTabPage.Text = "DQXOptionTabPage";
            // 
            // lblDQX
            // 
            this.lblDQX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblDQX.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDQX.Location = new System.Drawing.Point(2, 2);
            this.lblDQX.Name = "lblDQX";
            this.lblDQX.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblDQX.Size = new System.Drawing.Size(1061, 23);
            this.lblDQX.TabIndex = 31;
            this.lblDQX.Text = "lblDQX";
            this.lblDQX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DQXOptionEnabledCheckBox
            // 
            this.DQXOptionEnabledCheckBox.AutoSize = true;
            this.DQXOptionEnabledCheckBox.Location = new System.Drawing.Point(11, 35);
            this.DQXOptionEnabledCheckBox.Name = "DQXOptionEnabledCheckBox";
            this.DQXOptionEnabledCheckBox.Size = new System.Drawing.Size(172, 16);
            this.DQXOptionEnabledCheckBox.TabIndex = 30;
            this.DQXOptionEnabledCheckBox.Text = "DQXOptionEnabledCheckBox";
            this.DQXOptionEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // label101
            // 
            this.label101.AutoSize = true;
            this.label101.Location = new System.Drawing.Point(9, 279);
            this.label101.Name = "label101";
            this.label101.Size = new System.Drawing.Size(115, 12);
            this.label101.TabIndex = 29;
            this.label101.Text = "DQXPTMember8Label";
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Location = new System.Drawing.Point(9, 249);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(115, 12);
            this.label100.TabIndex = 28;
            this.label100.Text = "DQXPTMember7Label";
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(9, 219);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(115, 12);
            this.label99.TabIndex = 27;
            this.label99.Text = "DQXPTMember6Label";
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(9, 189);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(115, 12);
            this.label98.TabIndex = 26;
            this.label98.Text = "DQXPTMember5Label";
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(9, 159);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(115, 12);
            this.label97.TabIndex = 25;
            this.label97.Text = "DQXPTMember4Label";
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(9, 129);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(115, 12);
            this.label96.TabIndex = 24;
            this.label96.Text = "DQXPTMember3Label";
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(9, 99);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(115, 12);
            this.label95.TabIndex = 23;
            this.label95.Text = "DQXPTMember2Label";
            // 
            // DQXPTMember8TextBox
            // 
            this.DQXPTMember8TextBox.Location = new System.Drawing.Point(294, 276);
            this.DQXPTMember8TextBox.Name = "DQXPTMember8TextBox";
            this.DQXPTMember8TextBox.ReadOnly = true;
            this.DQXPTMember8TextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPTMember8TextBox.TabIndex = 22;
            // 
            // DQXPTMember7TextBox
            // 
            this.DQXPTMember7TextBox.Location = new System.Drawing.Point(294, 246);
            this.DQXPTMember7TextBox.Name = "DQXPTMember7TextBox";
            this.DQXPTMember7TextBox.ReadOnly = true;
            this.DQXPTMember7TextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPTMember7TextBox.TabIndex = 21;
            // 
            // DQXPTMember6TextBox
            // 
            this.DQXPTMember6TextBox.Location = new System.Drawing.Point(294, 216);
            this.DQXPTMember6TextBox.Name = "DQXPTMember6TextBox";
            this.DQXPTMember6TextBox.ReadOnly = true;
            this.DQXPTMember6TextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPTMember6TextBox.TabIndex = 20;
            // 
            // DQXPTMember5TextBox
            // 
            this.DQXPTMember5TextBox.Location = new System.Drawing.Point(294, 186);
            this.DQXPTMember5TextBox.Name = "DQXPTMember5TextBox";
            this.DQXPTMember5TextBox.ReadOnly = true;
            this.DQXPTMember5TextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPTMember5TextBox.TabIndex = 19;
            // 
            // DQXPTMember4TextBox
            // 
            this.DQXPTMember4TextBox.Location = new System.Drawing.Point(294, 156);
            this.DQXPTMember4TextBox.Name = "DQXPTMember4TextBox";
            this.DQXPTMember4TextBox.ReadOnly = true;
            this.DQXPTMember4TextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPTMember4TextBox.TabIndex = 18;
            // 
            // DQXPTMember3TextBox
            // 
            this.DQXPTMember3TextBox.Location = new System.Drawing.Point(294, 126);
            this.DQXPTMember3TextBox.Name = "DQXPTMember3TextBox";
            this.DQXPTMember3TextBox.ReadOnly = true;
            this.DQXPTMember3TextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPTMember3TextBox.TabIndex = 17;
            // 
            // DQXPTMember2TextBox
            // 
            this.DQXPTMember2TextBox.Location = new System.Drawing.Point(294, 96);
            this.DQXPTMember2TextBox.Name = "DQXPTMember2TextBox";
            this.DQXPTMember2TextBox.ReadOnly = true;
            this.DQXPTMember2TextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPTMember2TextBox.TabIndex = 16;
            // 
            // DQXPlayerNameTextBox
            // 
            this.DQXPlayerNameTextBox.Location = new System.Drawing.Point(294, 66);
            this.DQXPlayerNameTextBox.Name = "DQXPlayerNameTextBox";
            this.DQXPlayerNameTextBox.Size = new System.Drawing.Size(185, 19);
            this.DQXPlayerNameTextBox.TabIndex = 15;
            // 
            // DQXPlayerNameLabel
            // 
            this.DQXPlayerNameLabel.AutoSize = true;
            this.DQXPlayerNameLabel.Location = new System.Drawing.Point(9, 69);
            this.DQXPlayerNameLabel.Name = "DQXPlayerNameLabel";
            this.DQXPlayerNameLabel.Size = new System.Drawing.Size(116, 12);
            this.DQXPlayerNameLabel.TabIndex = 14;
            this.DQXPlayerNameLabel.Text = "DQXPlayerNameLabel";
            // 
            // DQXAppleyButton
            // 
            this.DQXAppleyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DQXAppleyButton.Location = new System.Drawing.Point(1094, 828);
            this.DQXAppleyButton.Name = "DQXAppleyButton";
            this.DQXAppleyButton.Size = new System.Drawing.Size(126, 25);
            this.DQXAppleyButton.TabIndex = 13;
            this.DQXAppleyButton.Text = "ApplyButton";
            this.DQXAppleyButton.UseVisualStyleBackColor = true;
            // 
            // LogTabPage
            // 
            this.LogTabPage.Location = new System.Drawing.Point(264, 4);
            this.LogTabPage.Name = "LogTabPage";
            this.LogTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.LogTabPage.Size = new System.Drawing.Size(1065, 829);
            this.LogTabPage.TabIndex = 6;
            this.LogTabPage.Text = "LogTabPage";
            this.LogTabPage.UseVisualStyleBackColor = true;
            // 
            // BlinkTimeNumericUpDown
            // 
            this.BlinkTimeNumericUpDown.DecimalPlaces = 1;
            this.BlinkTimeNumericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.BlinkTimeNumericUpDown.Location = new System.Drawing.Point(5, 392);
            this.BlinkTimeNumericUpDown.Margin = new System.Windows.Forms.Padding(6);
            this.BlinkTimeNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.BlinkTimeNumericUpDown.Name = "BlinkTimeNumericUpDown";
            this.BlinkTimeNumericUpDown.Size = new System.Drawing.Size(68, 19);
            this.BlinkTimeNumericUpDown.TabIndex = 78;
            this.BlinkTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(85, 394);
            this.label68.Margin = new System.Windows.Forms.Padding(0);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(83, 12);
            this.label68.TabIndex = 79;
            this.label68.Text = "BlinkTimeLabel";
            // 
            // ConfigPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.TabControl);
            this.Name = "ConfigPanel";
            this.Size = new System.Drawing.Size(1333, 837);
            this.CombatAnalyzerContextMenuStrip.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.SpecialSpellTabPage.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.DetailPanelGroupBox.ResumeLayout(false);
            this.DetailPanelGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MarginUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PanelTopNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PanelLeftNumericUpDown)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.DetailGroupBox.ResumeLayout(false);
            this.tabControlExtHoriz2.ResumeLayout(false);
            this.GeneralTab.ResumeLayout(false);
            this.SpellDetailPanel.ResumeLayout(false);
            this.SpellDetailPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WarningTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpandSecounds1NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpandSecounds2NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayNoNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpellIconSizeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecastTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpperLimitOfExtensionNumericUpDown)).EndInit();
            this.AlarmTab.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BeforeTimeNumericUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverTimeNumericUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.SpellButtonsPanel.ResumeLayout(false);
            this.OnPointTelopTabPage.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.TelopDetailGroupBox.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.tabControlExtHoriz3.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TelopDelayNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TelopTopNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TelopLeftNumericUpDown)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.CombatAnalyzerTabPage.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CombatLogBufferSizeNumericUpDown)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.OptionTabPage.ResumeLayout(false);
            this.tabControlExtHoriz1.ResumeLayout(false);
            this.tabOverlayOptions.ResumeLayout(false);
            this.tabOverlayOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextBlurRateNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextOutlineThicknessRateNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpacityNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReduceIconBrightnessNumericUpDown)).EndInit();
            this.tabDetailOptions.ResumeLayout(false);
            this.tabDetailOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeOfHideNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RefreshIntervalNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogPollSleepNumericUpDown)).EndInit();
            this.panel7.ResumeLayout(false);
            this.pnlLanguage.ResumeLayout(false);
            this.pnlLanguage.PerformLayout();
            this.DQXOptionTabPage.ResumeLayout(false);
            this.DQXOptionTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlinkTimeNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControlExt TabControl;
        private System.Windows.Forms.TabPage SpecialSpellTabPage;
        private System.Windows.Forms.TabPage OptionTabPage;
        private System.Windows.Forms.TreeView SpellTimerTreeView;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button ShokikaButton;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.NumericUpDown TimeOfHideNumericUpDown;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox AutoSortCheckBox;
        private System.Windows.Forms.CheckBox ClickThroughCheckBox;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown OpacityNumericUpDown;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button TekiyoButton;
        private System.Windows.Forms.CheckBox AutoSortReverseCheckBox;
        private System.Windows.Forms.NumericUpDown RefreshIntervalNumericUpDown;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.TabPage OnPointTelopTabPage;
        private System.Windows.Forms.TreeView TelopTreeView;
        private System.Windows.Forms.Button SwitchTelopButton;
        private System.Windows.Forms.CheckBox EnabledPTPlaceholderCheckBox;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label48;
        internal System.Windows.Forms.Button SwitchOverlayButton;
        private VisualSettingControl DefaultVisualSetting;
        private System.Windows.Forms.TabPage CombatAnalyzerTabPage;
        internal System.Windows.Forms.Button AnalyzeCombatButton;
        private System.Windows.Forms.Label CombatAnalyzingLabel;
        private System.Windows.Forms.Timer CombatAnalyzingTimer;
        private System.Windows.Forms.CheckBox CombatLogEnabledCheckBox;
        private System.Windows.Forms.NumericUpDown CombatLogBufferSizeNumericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView CombatLogListView;
        private System.Windows.Forms.ColumnHeader NoColumnHeader;
        private System.Windows.Forms.ColumnHeader TimeStampColumnHeader;
        private System.Windows.Forms.ColumnHeader ElapsedColumnHeader;
        private System.Windows.Forms.ColumnHeader LogTypeColumnHeader;
        private System.Windows.Forms.ColumnHeader ActorColumnHeader;
        private System.Windows.Forms.ColumnHeader ActionColumnHeader;
        private System.Windows.Forms.ColumnHeader SpanColumnHeader;
        private System.Windows.Forms.ColumnHeader LogColumnHeader;
        private System.Windows.Forms.ColumnHeader DummyColumnHeader;
        private System.Windows.Forms.ContextMenuStrip CombatAnalyzerContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem CASelectAllItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem CACopyLogItem;
        private System.Windows.Forms.ToolStripMenuItem CACopyLogDetailItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem CASetOriginItem;
        private System.Windows.Forms.CheckBox OverlayForceVisibleCheckBox;
        private System.Windows.Forms.ColumnHeader HPRateColumnHeader;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label LanguageRestartLabel;
        private System.Windows.Forms.ComboBox LanguageComboBox;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox EnabledSpellTimerNoDecimalCheckBox;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.CheckBox EnabledSpellTimerNoDecimal;
        internal System.Windows.Forms.Button ExportCSVButton;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.CheckBox EnabledNotifyNormalSpellTimerCheckBox;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.TextBox ReadyTextBox;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.TextBox OverTextBox;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Button SaveLogButton;
        private System.Windows.Forms.CheckBox SaveLogCheckBox;
        private System.Windows.Forms.TextBox SaveLogTextBox;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.CheckBox HideWhenNotActiceCheckBox;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.Label label92;
        private System.Windows.Forms.NumericUpDown LogPollSleepNumericUpDown;
        private System.Windows.Forms.Label label91;
        private System.Windows.Forms.Label label94;
        private System.Windows.Forms.NumericUpDown ReduceIconBrightnessNumericUpDown;
        private System.Windows.Forms.Label label93;
        private System.Windows.Forms.CheckBox UseOtherThanFFXIVCheckbox;
        private System.Windows.Forms.TabPage DQXOptionTabPage;
        private System.Windows.Forms.Button DQXAppleyButton;
        private System.Windows.Forms.TextBox DQXPTMember8TextBox;
        private System.Windows.Forms.TextBox DQXPTMember7TextBox;
        private System.Windows.Forms.TextBox DQXPTMember6TextBox;
        private System.Windows.Forms.TextBox DQXPTMember5TextBox;
        private System.Windows.Forms.TextBox DQXPTMember4TextBox;
        private System.Windows.Forms.TextBox DQXPTMember3TextBox;
        private System.Windows.Forms.TextBox DQXPTMember2TextBox;
        private System.Windows.Forms.TextBox DQXPlayerNameTextBox;
        private System.Windows.Forms.Label DQXPlayerNameLabel;
        private System.Windows.Forms.Label label101;
        private System.Windows.Forms.Label label100;
        private System.Windows.Forms.Label label99;
        private System.Windows.Forms.Label label98;
        private System.Windows.Forms.Label label97;
        private System.Windows.Forms.Label label96;
        private System.Windows.Forms.Label label95;
        private System.Windows.Forms.CheckBox DQXOptionEnabledCheckBox;
        private System.Windows.Forms.Label ResetOnWipeOutLabel;
        private System.Windows.Forms.CheckBox ResetOnWipeOutCheckBox;
        private System.Windows.Forms.CheckBox SimpleRegexCheckBox;
        private System.Windows.Forms.Label label102;
        private System.Windows.Forms.CheckBox RemoveTooltipSymbolsCheckBox;
        private System.Windows.Forms.Label label103;
        private System.Windows.Forms.Label label104;
        private System.Windows.Forms.CheckBox DetectPacketDumpcheckBox;
        private System.Windows.Forms.Label lblDQX;
        private System.Windows.Forms.Panel pnlLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private TabControlExtHoriz tabControlExtHoriz1;
        private System.Windows.Forms.TabPage tabOverlayOptions;
        private System.Windows.Forms.TabPage tabDetailOptions;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblCombatAnalyzer;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ExportButton;
        private System.Windows.Forms.ToolStripMenuItem ImportButton;
        private System.Windows.Forms.ToolStripMenuItem ClearAllButton;
        private System.Windows.Forms.ToolStripMenuItem AddButton;
        private System.Windows.Forms.Panel DetailGroupBox;
        private System.Windows.Forms.Panel SpellButtonsPanel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem TelopExportButton;
        private System.Windows.Forms.ToolStripMenuItem TelopImportButton;
        private System.Windows.Forms.ToolStripMenuItem TelopClearAllButton;
        private System.Windows.Forms.ToolStripMenuItem TelopAddButton;
        private System.Windows.Forms.Panel TelopDetailGroupBox;
        private TabControlExtHoriz tabControlExtHoriz2;
        private System.Windows.Forms.TabPage GeneralTab;
        private System.Windows.Forms.Panel SpellDetailPanel;
        private System.Windows.Forms.TextBox PanelNameTextBox;
        private System.Windows.Forms.NumericUpDown ExpandSecounds1NumericUpDown;
        private System.Windows.Forms.TextBox KeywordToExpand1TextBox;
        private System.Windows.Forms.TextBox KeywordToExpand2TextBox;
        private System.Windows.Forms.CheckBox RegexEnabledCheckBox;
        private System.Windows.Forms.NumericUpDown ExpandSecounds2NumericUpDown;
        private System.Windows.Forms.TextBox SpellTitleTextBox;
        private System.Windows.Forms.NumericUpDown DisplayNoNumericUpDown;
        private System.Windows.Forms.NumericUpDown SpellIconSizeUpDown;
        private System.Windows.Forms.TextBox KeywordTextBox;
        private System.Windows.Forms.NumericUpDown RecastTimeNumericUpDown;
        private System.Windows.Forms.NumericUpDown UpperLimitOfExtensionNumericUpDown;
        private System.Windows.Forms.CheckBox RepeatCheckBox;
        private VisualSettingControl SpellVisualSetting;
        private System.Windows.Forms.Button SetConditionButton;
        private System.Windows.Forms.CheckBox ToInstanceCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.CheckBox ReduceIconBrightnessCheckBox;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Button SelectZoneButton;
        private System.Windows.Forms.CheckBox ExtendBeyondOriginalRecastTimeCheckBox;
        private System.Windows.Forms.Button SelectJobButton;
        private System.Windows.Forms.CheckBox OverlapRecastTimeCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox DontHideCheckBox;
        private System.Windows.Forms.CheckBox HideSpellNameCheckBox;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox IsReverseCheckBox;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.CheckBox ShowProgressBarCheckBox;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.TabPage AlarmTab;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox BeforeTextToSpeakTextBox;
        private System.Windows.Forms.ComboBox BeforeSoundComboBox;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.NumericUpDown BeforeTimeNumericUpDown;
        private System.Windows.Forms.Button Speak4Button;
        private System.Windows.Forms.Button Play4Button;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox MatchTextToSpeakTextBox;
        private System.Windows.Forms.ComboBox MatchSoundComboBox;
        private System.Windows.Forms.Button Speak1Button;
        private System.Windows.Forms.Button Play1Button;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox OverTextToSpeakTextBox;
        private System.Windows.Forms.ComboBox OverSoundComboBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown OverTimeNumericUpDown;
        private System.Windows.Forms.Button Speak2Button;
        private System.Windows.Forms.Button Play2Button;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TimeupTextToSpeakTextBox;
        private System.Windows.Forms.ComboBox TimeupSoundComboBox;
        private System.Windows.Forms.Button Speak3Button;
        private System.Windows.Forms.Button Play3Button;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Button TelopUpdateButton;
        private System.Windows.Forms.Button TelopDeleteButton;
        private TabControlExtHoriz tabControlExtHoriz3;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox TelopTitleTextBox;
        private System.Windows.Forms.TextBox TelopMessageTextBox;
        private System.Windows.Forms.TextBox TelopKeywordTextBox;
        private System.Windows.Forms.NumericUpDown DisplayTimeNumericUpDown;
        private System.Windows.Forms.TextBox TelopKeywordToHideTextBox;
        private System.Windows.Forms.NumericUpDown TelopDelayNumericUpDown;
        private System.Windows.Forms.Button TelopSetConditionButton;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Button TelopSelectZoneButton;
        private VisualSettingControl TelopVisualSetting;
        private System.Windows.Forms.Button TelopSelectJobButton;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.CheckBox TelopProgressBarEnabledCheckBox;
        private System.Windows.Forms.CheckBox EnabledAddMessageCheckBox;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.NumericUpDown TelopTopNumericUpDown;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.NumericUpDown TelopLeftNumericUpDown;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.CheckBox TelopRegexEnabledCheckBox;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox TelopDelayTTSTextBox;
        private System.Windows.Forms.ComboBox TelopDelaySoundComboBox;
        private System.Windows.Forms.Button TelopSpeak2Button;
        private System.Windows.Forms.Button TelopPlay2Button;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox TelopMatchTTSTextBox;
        private System.Windows.Forms.ComboBox TelopMatchSoundComboBox;
        private System.Windows.Forms.Button TelopSpeak1Button;
        private System.Windows.Forms.Button TelopPlay1Button;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.GroupBox DetailPanelGroupBox;
        private System.Windows.Forms.CheckBox FixedPositionSpellCheckBox;
        private System.Windows.Forms.CheckBox HorizontalLayoutCheckBox;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.NumericUpDown MarginUpDown;
        private System.Windows.Forms.NumericUpDown PanelTopNumericUpDown;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown PanelLeftNumericUpDown;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button UpdatePanelButton;
        private System.Windows.Forms.NumericUpDown WarningTimeNumericUpDown;
        private System.Windows.Forms.Label label105;
        private System.Windows.Forms.CheckBox WarningTimeCheckBox;
        private System.Windows.Forms.Label label107;
        private System.Windows.Forms.Label label106;
        private System.Windows.Forms.NumericUpDown TextBlurRateNumericUpDown;
        private System.Windows.Forms.NumericUpDown TextOutlineThicknessRateNumericUpDown;
        private System.Windows.Forms.CheckBox NotifyToACTCheckBox;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TabPage NameStyleTabPage;
        private System.Windows.Forms.TabPage LogTabPage;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.CheckBox RenderWithCPUOnlyCheckBox;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.CheckBox ToComplementUnknownSkillCheckBox;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Button SelectIconButton;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.NumericUpDown BlinkTimeNumericUpDown;
    }
}
