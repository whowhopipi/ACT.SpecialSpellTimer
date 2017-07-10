namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ACT.SpecialSpellTimer.Config;
    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// SpecialSpellTimer Plugin
    /// </summary>
    public class SpecialSpellTimerPlugin : IActPluginV1
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpecialSpellTimerPlugin()
        {
            // このDLLの配置場所とACT標準のPluginディレクトリも解決の対象にする
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                try
                {
                    var asm = new AssemblyName(e.Name);

                    var plugin = ActGlobals.oFormActMain.PluginGetSelfData(this);
                    if (plugin != null)
                    {
                        var thisDirectory = plugin.pluginFile.DirectoryName;
                        var path1 = Path.Combine(thisDirectory, asm.Name + ".dll");
                        if (File.Exists(path1))
                        {
                            return Assembly.LoadFrom(path1);
                        }
                    }

                    var pluginDirectory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        @"Advanced Combat Tracker\Plugins");

                    var path = Path.Combine(pluginDirectory, asm.Name + ".dll");

                    if (File.Exists(path))
                    {
                        return Assembly.LoadFrom(path);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(Utility.Translate.Get("ACTAssemblyError"), ex);
                }

                return null;
            };
        }

        /// <summary>
        /// 設定パネル
        /// </summary>
        public static ConfigPanel ConfigPanel { get; private set; }

        /// <summary>
        /// 自身の場所
        /// </summary>
        public static string Location
        {
            get;
            private set;
        }

        /// <summary>
        /// 表示切り替えボタン
        /// </summary>
        private static Button SwitchVisibleButton { get; set; }

        /// <summary>
        /// プラグインステータス表示ラベル
        /// </summary>
        private Label PluginStatusLabel
        {
            get;
            set;
        }

        /// <summary>
        /// 表示切り替えボタン（スペスペボタン）の状態を切り替える
        /// </summary>
        /// <param name="visible">
        /// 切り替える状態</param>
        public static void ChangeSwitchVisibleButton(
            bool visible)
        {
            Settings.Default.OverlayVisible = visible;
            Settings.Default.Save();

            SpellTimerCore.Default.ClosePanels();
            OnePointTelopController.CloseTelops();

            FF14PluginHelper.RefreshPlayer();
            LogBuffer.RefreshPartyList();
            LogBuffer.RefreshPetID();

            if (Settings.Default.OverlayVisible)
            {
                SpellTimerCore.Default.ActivatePanels();
                OnePointTelopController.ActivateTelops();
            }

            ActInvoker.Invoke(() =>
            {
                if (Settings.Default.OverlayVisible)
                {
                    SwitchVisibleButton.BackColor = Color.OrangeRed;
                    SwitchVisibleButton.ForeColor = Color.WhiteSmoke;
                }
                else
                {
                    SwitchVisibleButton.BackColor = SystemColors.Control;
                    SwitchVisibleButton.ForeColor = Color.Black;
                }
            });
        }

        /// <summary>
        /// 後片付けをする
        /// </summary>
        void IActPluginV1.DeInitPlugin()
        {
            try
            {
                SpellTimerCore.Default.End();
                this.RemoveSwitchVisibleButton();
                this.PluginStatusLabel.Text = "Plugin Exited";

                // 設定ファイルを保存する
                Settings.Default.Save();

                Logger.Write("Plugin Exited.");
            }
            catch (Exception ex)
            {
                Logger.Write(Utility.Translate.Get("ACTPluginStopError"), ex);

                ActGlobals.oFormActMain.WriteExceptionLog(
                    ex,
                    Utility.Translate.Get("ACTPluginStopError"));

                this.PluginStatusLabel.Text = "Plugin Exited Error";
            }

            Logger.End();
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="pluginScreenSpace">Pluginタブ</param>
        /// <param name="pluginStatusText">Pluginステータスラベル</param>
        void IActPluginV1.InitPlugin(
            TabPage pluginScreenSpace,
            Label pluginStatusText)
        {
            Logger.Begin();

            try
            {
                Logger.Write("Plugin Start.");

                // 設定ファイルを読み込む
                Settings.Default.Load();

                pluginScreenSpace.Text = Translate.Get("LN_Tabname");
                this.PluginStatusLabel = pluginStatusText;

                // アップデートを確認する
                Task.Run(() =>
                {
                    this.Update();
                });

                // 自身の場所を格納しておく
                var plugin = ActGlobals.oFormActMain.PluginGetSelfData(this);
                if (plugin != null)
                {
                    SpecialSpellTimerPlugin.Location = plugin.pluginFile.DirectoryName;
                }

                // 設定Panelを追加する
                ConfigPanel = new ConfigPanel();
                pluginScreenSpace.Controls.Add(ConfigPanel);
                ConfigPanel.Size = pluginScreenSpace.Size;
                ConfigPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

                // 設定ファイルのバックアップを作成する
                SpellTimerTable.Backup();
                OnePointTelopTable.Default.Backup();
                PanelSettings.Default.Backup();

                // 本体を開始する
                SpellTimerCore.Default.Begin();

                this.SetSwitchVisibleButton();
                this.PluginStatusLabel.Text = "Plugin Started";

                Logger.Write("Plugin Started.");
            }
            catch (Exception ex)
            {
                Logger.Write(Utility.Translate.Get("ACTPluginStartError"), ex);

                this.PluginStatusLabel.Text = "Plugin Initialize Error";
            }
        }

        /// <summary>
        /// ACTメインフォーム Resize
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void oFormActMain_Resize(object sender, EventArgs e)
        {
            SwitchVisibleButton.Location = new Point(
                ActGlobals.oFormActMain.Width - 533,
                0);
        }

        /// <summary>
        /// 表示切り替えボタンを除去する
        /// </summary>
        private void RemoveSwitchVisibleButton()
        {
            if (SwitchVisibleButton != null)
            {
                ActGlobals.oFormActMain.Resize -= this.oFormActMain_Resize;
                ActGlobals.oFormActMain.Controls.Remove(SwitchVisibleButton);
            }
        }

        /// <summary>
        /// 表示切り替えボタンを配置する
        /// </summary>
        private void SetSwitchVisibleButton()
        {
            var changeColor = new Action<Button>((button) =>
            {
                if (Settings.Default.OverlayVisible)
                {
                    button.BackColor = Color.OrangeRed;
                    button.ForeColor = Color.WhiteSmoke;
                }
                else
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = Color.Black;
                }
            });

            SwitchVisibleButton = new Button();
            SwitchVisibleButton.Name = "SpecialSpellTimerSwitchVisibleButton";
            SwitchVisibleButton.Size = new Size(90, 24);
            SwitchVisibleButton.Text = Utility.Translate.Get("SupeSupe");
            SwitchVisibleButton.TextAlign = ContentAlignment.MiddleCenter;
            SwitchVisibleButton.UseVisualStyleBackColor = true;
            SwitchVisibleButton.Click += (s, e) =>
            {
                var button = s as Button;

                Settings.Default.OverlayVisible = !Settings.Default.OverlayVisible;
                Settings.Default.Save();

                SpellTimerCore.Default.ClosePanels();
                OnePointTelopController.CloseTelops();

                FF14PluginHelper.RefreshPlayer();
                LogBuffer.RefreshPartyList();
                LogBuffer.RefreshPetID();

                if (Settings.Default.OverlayVisible)
                {
                    SpellTimerCore.Default.ActivatePanels();
                    OnePointTelopController.ActivateTelops();
                }

                changeColor(s as Button);
            };

            changeColor(SwitchVisibleButton);

            ActGlobals.oFormActMain.Resize += this.oFormActMain_Resize;
            ActGlobals.oFormActMain.Controls.Add(SwitchVisibleButton);
            ActGlobals.oFormActMain.Controls.SetChildIndex(SwitchVisibleButton, 1);

            this.oFormActMain_Resize(this, null);
        }

        /// <summary>
        /// アップデートを行う
        /// </summary>
        private void Update()
        {
            if (Settings.Default.UpdateCheckInterval >= 0.0d)
            {
                if ((DateTime.Now - Settings.Default.LastUpdateDateTime).TotalHours >=
                    Settings.Default.UpdateCheckInterval)
                {
                    var message = UpdateChecker.Update();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Logger.Write(message);
                    }

                    Settings.Default.LastUpdateDateTime = DateTime.Now;
                    Settings.Default.Save();
                }
            }
        }
    }
}