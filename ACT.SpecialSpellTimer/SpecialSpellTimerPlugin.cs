using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
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
                    Logger.Write(Translate.Get("ACTAssemblyError"), ex);
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

            SpellTimerCore.Instance.ClosePanels();
            OnePointTelopController.CloseTelops();

            TableCompiler.Instance.RefreshPlayerPlacceholder();
            TableCompiler.Instance.RefreshPartyPlaceholders();
            TableCompiler.Instance.RefreshPetPlaceholder();
            TableCompiler.Instance.RecompileSpells();
            TableCompiler.Instance.RecompileTickers();

            if (Settings.Default.OverlayVisible)
            {
                SpellTimerCore.Instance.ActivatePanels();
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
                SpellTimerCore.Instance.End();
                this.RemoveSwitchVisibleButton();
                this.PluginStatusLabel.Text = "Plugin Exited";

                // 設定ファイルを保存する
                Settings.Default.Save();

                Logger.Write("Plugin Exited.");
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(
                    ex,
                    "Plugin deinit error.");

                Logger.Write("Plugin deinit error.", ex);

                this.PluginStatusLabel.Text = "Plugin Exited Error";
            }

            Logger.End();
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="pluginScreenSpace">Pluginタブ</param>
        /// <param name="pluginStatusText">Pluginステータスラベル</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        void IActPluginV1.InitPlugin(
            TabPage pluginScreenSpace,
            Label pluginStatusText)
        {
            try
            {
                Logger.Begin();
                Logger.Write("Plugin Start.");

                // WPFアプリケーションを開始する
                if (System.Windows.Application.Current == null)
                {
                    new System.Windows.Application();
                    System.Windows.Application.Current.ShutdownMode = 
                        System.Windows.ShutdownMode.OnExplicitShutdown;
                }

                // 設定ファイルを読み込む
                Settings.Default.Load();
                Settings.Default.ApplyRenderMode();

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
                SpecialSpellTimerPlugin.ConfigPanel = new ConfigPanel();
                pluginScreenSpace.Controls.Add(SpecialSpellTimerPlugin.ConfigPanel);
                SpecialSpellTimerPlugin.ConfigPanel.Dock = DockStyle.Fill;

                // 設定ファイルのバックアップを作成する
                SpellTimerTable.Instance.Backup();
                OnePointTelopTable.Instance.Backup();
                PanelSettings.Instance.Backup();

                // 設定ファイルを読み込む
                SpellTimerTable.Instance.Load();
                OnePointTelopTable.Instance.Load();
                PanelSettings.Instance.Load();

                // 本体を開始する
                SpellTimerCore.Instance.Begin();

                this.SetSwitchVisibleButton();
                this.PluginStatusLabel.Text = "Plugin Started";

                Logger.Write("Plugin Started.");
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(
                    ex,
                    "Plugin init error.");

                Logger.Write("Plugin init error.", ex);

                this.PluginStatusLabel.Text = "Plugin Initialize Error";
            }
        }

        /// <summary>
        /// ACTメインフォーム Resize
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void FormActMain_Resize(object sender, EventArgs e)
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
                ActGlobals.oFormActMain.Resize -= this.FormActMain_Resize;
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
            SwitchVisibleButton.Text = Translate.Get("SupeSupe");
            SwitchVisibleButton.TextAlign = ContentAlignment.MiddleCenter;
            SwitchVisibleButton.UseVisualStyleBackColor = true;
            SwitchVisibleButton.Click += (s, e) =>
            {
                var button = s as Button;

                Settings.Default.OverlayVisible = !Settings.Default.OverlayVisible;
                Settings.Default.Save();

                SpellTimerCore.Instance.ClosePanels();
                OnePointTelopController.CloseTelops();

                TableCompiler.Instance.RefreshPlayerPlacceholder();
                TableCompiler.Instance.RefreshPartyPlaceholders();
                TableCompiler.Instance.RefreshPetPlaceholder();
                TableCompiler.Instance.RecompileSpells();
                TableCompiler.Instance.RecompileTickers();

                if (Settings.Default.OverlayVisible)
                {
                    SpellTimerCore.Instance.ActivatePanels();
                    OnePointTelopController.ActivateTelops();
                }

                changeColor(s as Button);
            };

            changeColor(SwitchVisibleButton);

            ActGlobals.oFormActMain.Resize += this.FormActMain_Resize;
            ActGlobals.oFormActMain.Controls.Add(SwitchVisibleButton);
            ActGlobals.oFormActMain.Controls.SetChildIndex(SwitchVisibleButton, 1);

            this.FormActMain_Resize(this, null);
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
