using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Sound;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// SpellTimerの中核
    /// </summary>
    public class PluginMainWorker
    {
        private const int INVALID = 1;

        private const int VALID = 0;

        #region Singleton

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static PluginMainWorker instance = new PluginMainWorker();

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static PluginMainWorker Instance => instance;

        #endregion Singleton

        #region Thread

        private BackgroundWorker backgroudWorker;
        private BackgroundWorker detectLogsWorker;
        private volatile bool isOver;
        private BackgroundWorker refreshSpellOverlaysWorker;
        private BackgroundWorker refreshTickerOverlaysWorker;

        #endregion Thread

        private volatile bool existFFXIVProcess;
        private volatile bool isFFXIVActive;

        /// <summary>
        /// 最後にテロップテーブルを保存した日時
        /// </summary>
        private DateTime lastSaveTickerTableDateTime = DateTime.Now;

        /// <summary>
        /// 最後に全滅した日時
        /// </summary>
        private DateTime lastWipeOutDateTime = DateTime.MinValue;

        /// <summary>
        /// ログバッファ
        /// </summary>
        private volatile LogBuffer LogBuffer;

        #region Begin / End

        /// <summary>
        /// 開始する
        /// </summary>
        public void Begin()
        {
            this.isOver = false;

            // FFXIVのスキャンを開始する
            FFXIVPlugin.Instance.Start();

            // ログバッファを生成する
            this.LogBuffer = new LogBuffer();

            // テーブルコンパイラを開始する
            TableCompiler.Instance.Begin();

            // 戦闘分析を初期化する
            CombatAnalyzer.Default.Initialize();

            // サウンドコントローラを開始する
            SoundController.Instance.Begin();

            // Overlayの更新スレッドを開始する
            this.BeginOverlaysThread();

            // ログ監視タイマを開始する
            this.detectLogsWorker = new BackgroundWorker();
            this.detectLogsWorker.WorkerSupportsCancellation = true;
            this.detectLogsWorker.DoWork += (s, e) =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Logger.Write("start detect logs.");

                while (true)
                {
                    try
                    {
                        if (this.detectLogsWorker.CancellationPending)
                        {
                            Logger.Write("end detect logs.");
                            e.Cancel = true;
                            return;
                        }

                        this.DetectLogsCore();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("detect logs error:", ex);
                    }
                }
            };

            this.detectLogsWorker.RunWorkerAsync();

            // Backgroudスレッドを開始する
            this.backgroudWorker = new BackgroundWorker();
            this.backgroudWorker.WorkerSupportsCancellation = true;
            this.backgroudWorker.DoWork += (s, e) =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));

                while (true)
                {
                    try
                    {
                        if (this.backgroudWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        this.BackgroundCore();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("background thread for UI error:", ex);
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            };

            this.backgroudWorker.RunWorkerAsync();
        }

        public void BeginOverlaysThread()
        {
            // スペルのスレッドを開始する
            this.refreshSpellOverlaysWorker = new BackgroundWorker();
            this.refreshSpellOverlaysWorker.WorkerSupportsCancellation = true;
            this.refreshSpellOverlaysWorker.DoWork += (s, e) =>
            {
                Logger.Write("start refresh spell overlays.");

                while (true)
                {
                    try
                    {
                        if (this.refreshSpellOverlaysWorker.CancellationPending)
                        {
                            Logger.Write("end refresh spell overlays.");
                            e.Cancel = true;
                            return;
                        }

                        Application.Current.Dispatcher.BeginInvoke(
                            (Action)this.RefreshSpellOverlaysCore,
                            DispatcherPriority.Normal);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("refresh spell overlays error:", ex);
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(Settings.Default.RefreshInterval));
                }
            };

            this.refreshSpellOverlaysWorker.RunWorkerAsync();

            // テロップのスレッドを開始する
            this.refreshTickerOverlaysWorker = new BackgroundWorker();
            this.refreshTickerOverlaysWorker.WorkerSupportsCancellation = true;
            this.refreshTickerOverlaysWorker.DoWork += (s, e) =>
            {
                Logger.Write("start refresh ticker overlays.");

                while (true)
                {
                    try
                    {
                        if (this.refreshTickerOverlaysWorker.CancellationPending)
                        {
                            Logger.Write("end refresh ticker overlays.");
                            e.Cancel = true;
                            return;
                        }

                        Application.Current.Dispatcher.BeginInvoke(
                            (Action)this.RefreshTickerOverlaysCore,
                            DispatcherPriority.Normal);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("refresh ticker overlays error:", ex);
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(Settings.Default.RefreshInterval));
                }
            };

            this.refreshTickerOverlaysWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 終了する
        /// </summary>
        public void End()
        {
            this.isOver = true;

            // 戦闘分析を開放する
            CombatAnalyzer.Default.Denitialize();

            // Workerを開放する
            this.refreshSpellOverlaysWorker?.Cancel();
            this.refreshTickerOverlaysWorker?.Cancel();
            this.detectLogsWorker?.Cancel();
            this.backgroudWorker?.Cancel();

            // ログバッファを開放する
            if (this.LogBuffer != null)
            {
                this.LogBuffer.Dispose();
                this.LogBuffer = null;
            }

            // Windowを閉じる
            SpellsController.Instance.ClosePanels();
            TickersController.Instance.CloseTelops();
            SpellsController.Instance.ExecuteClosePanels();
            TickersController.Instance.ExecuteCloseTelops();

            // 設定を保存する
            Settings.Default.Save();
            SpellTimerTable.Instance.Save();
            OnePointTelopTable.Instance.Save();

            // サウンドコントローラを停止する
            SoundController.Instance.End();

            // テーブルコンパイラを停止する
            TableCompiler.Instance.End();

            // FFXIVのスキャンを停止する
            FFXIVPlugin.Instance.End();
        }

        #endregion Begin / End

        #region Core

        private void BackgroundCore()
        {
            // FFXIVプロセスの有無を取得する
            this.existFFXIVProcess = FFXIVPlugin.Instance.Process != null;

            // FFXIV及びACTがアクティブか取得する
            this.isFFXIVActive = this.IsActive();

            // テロップの位置を保存するためテロップテーブルを保存する
            if ((DateTime.Now - this.lastSaveTickerTableDateTime).TotalMinutes >= 1)
            {
                this.lastSaveTickerTableDateTime = DateTime.Now;

                Task.Run(() =>
                {
                    OnePointTelopTable.Instance.Save();
                    Debug.WriteLine("●Save telop table.");
                });
            }
        }

        /// <summary>
        /// ログを監視する
        /// </summary>
        private void DetectLogsCore()
        {
            var existsLog = false;

            // FFXIVがいない？
            if (!FFXIVPlugin.Instance.IsAvalable)
            {
                // importログの解析用にログを取り出しておく
                if (!this.LogBuffer.IsEmpty)
                {
                    this.LogBuffer.GetLogLines();
                }

                Thread.Sleep(TimeSpan.FromSeconds(3));
                return;
            }

            // 全滅によるリセットを判定する
            var resetTask = Task.Run(() => this.ResetCountAtRestart());

            if (!this.LogBuffer.IsEmpty)
            {
                // ログを取り出す
                var logsTask = Task.Run(() => this.LogBuffer.GetLogLines());

                // 有効なスペルとテロップのリストを取得する
                var spells = TableCompiler.Instance.SpellList;
                var telops = TableCompiler.Instance.TickerList;

                var logs = logsTask.Result;
                if (logs.Count > 0)
                {
                    // テロップとマッチングする
                    var t1 = Task.Run(() => TickersController.Instance.Match(
                        telops,
                        logs));

                    // スペルリストとマッチングする
                    var t2 = Task.Run(() => SpellsController.Instance.Match(
                        spells,
                        logs));

                    // コマンドとマッチングする
                    var t3 = Task.Run(() => TextCommandController.MatchCommand(
                        logs));

                    Task.WaitAll(t1, t2, t3);

                    existsLog = true;
                }
            }

            resetTask.Wait();

            var interval = !existsLog ?
                TimeSpan.FromMilliseconds(Settings.Default.LogPollSleepInterval) :
                TimeSpan.FromMilliseconds(0);

            Thread.Sleep(interval);
        }

        private void RefreshSpellOverlaysCore()
        {
            // Close要求を処理する
            SpellsController.Instance.ExecuteClosePanels();

            if (this.isOver)
            {
                return;
            }

            // FFXIVでの使用？
            if (!Settings.Default.UseOtherThanFFXIV &&
                !this.existFFXIVProcess &&
                !Settings.Default.OverlayForceVisible)
            {
                SpellsController.Instance.ClosePanels();
                return;
            }

            // オーバーレイが非表示？
            if (!Settings.Default.OverlayVisible)
            {
                SpellsController.Instance.HidePanels();
                return;
            }

            // 非アクティブのとき非表示にする？
            if (Settings.Default.HideWhenNotActive &&
                !this.isFFXIVActive)
            {
                SpellsController.Instance.HidePanels();
                return;
            }

            // スペルWindowを表示する
            var spells = TableCompiler.Instance.SpellList;
            SpellsController.Instance.RefreshSpellOverlays(spells);
        }

        private void RefreshTickerOverlaysCore()
        {
            // Close要求を処理する
            TickersController.Instance.ExecuteCloseTelops();

            if (this.isOver)
            {
                return;
            }

            // FFXIVでの使用？
            if (!Settings.Default.UseOtherThanFFXIV &&
                !this.existFFXIVProcess &&
                !Settings.Default.OverlayForceVisible)
            {
                TickersController.Instance.CloseTelops();
                return;
            }

            // オーバーレイが非表示？
            if (!Settings.Default.OverlayVisible)
            {
                TickersController.Instance.HideTelops();
                return;
            }

            // 非アクティブのとき非表示にする？
            if (Settings.Default.HideWhenNotActive &&
                !this.isFFXIVActive)
            {
                TickersController.Instance.HideTelops();
                return;
            }

            // テロップWindowを表示する
            var telops = TableCompiler.Instance.TickerList;
            TickersController.Instance.RefreshTelopOverlays(telops);
        }

        #endregion Core

        /// <summary>
        /// リスタートのときスペルのカウントをリセットする
        /// </summary>
        private void ResetCountAtRestart()
        {
            // FFXIV以外での使用ならば何もしない
            if (Settings.Default.UseOtherThanFFXIV)
            {
                return;
            }

            // 無効？
            if (!Settings.Default.ResetOnWipeOut)
            {
                return;
            }

            var combatants = FFXIVPlugin.Instance.GetPartyList();

            if (combatants == null ||
                combatants.Count < 1)
            {
                return;
            }

            // 関係者が全員死んでる？
            if (combatants.Count ==
                combatants.Count(x => x.CurrentHP <= 0))
            {
                // リセットするのは15秒に1回にする
                // 暗転中もずっとリセットし続けてしまうので
                if ((DateTime.Now - this.lastWipeOutDateTime).TotalSeconds >= 15.0)
                {
                    SpellTimerTable.ResetCount();
                    OnePointTelopTable.Instance.ResetCount();

                    // ACT本体に戦闘終了を通知する
                    if (Settings.Default.WipeoutNotifyToACT)
                    {
                        ActInvoker.Invoke(() =>
                        {
                            ActGlobals.oFormActMain.ActCommands("end");
                        });
                    }

                    this.lastWipeOutDateTime = DateTime.Now;
                }
            }
        }

        #region Misc

        /// <summary>
        /// FFXIVまたはACTがアクティブか？
        /// </summary>
        /// <returns>
        /// FFXIVまたはACTがアクティブか？</returns>
        private bool IsActive()
        {
            var r = true;

            try
            {
                // フォアグラウンドWindowのハンドルを取得する
                var hWnd = GetForegroundWindow();

                // プロセスIDに変換する
                int pid;
                GetWindowThreadProcessId(hWnd, out pid);

                // メインモジュールのファイル名を取得する
                var p = Process.GetProcessById(pid);
                if (p != null)
                {
                    var fileName = Path.GetFileName(
                        p.MainModule.FileName);

                    var actFileName = Path.GetFileName(
                        Process.GetCurrentProcess().MainModule.FileName);

                    Debug.WriteLine(
                        DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]") + " " +
                        "●WatchActive " + fileName);

                    if (fileName.ToLower() == "ffxiv.exe" ||
                        fileName.ToLower() == "ffxiv_dx11.exe" ||
                        fileName.ToLower() == "dqx.exe" ||
                        fileName.ToLower() == actFileName.ToLower())
                    {
                        r = true;
                    }
                    else
                    {
                        r = false;
                    }
                }
            }
            catch (Win32Exception)
            {
                // ignore
            }
            catch (Exception ex)
            {
                Logger.Write(Translate.Get("WatchActiveError"), ex);
            }

            return r;
        }

        #endregion Misc

        #region NativeMethods

        /// <summary>
        /// フォアグラウンドWindowのハンドルを取得する
        /// </summary>
        /// <returns>
        /// フォアグラウンドWindowのハンドル</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// WindowハンドルからそのプロセスIDを取得する
        /// </summary>
        /// <param name="hWnd">
        /// プロセスIDを取得するWindowハンドル</param>
        /// <param name="lpdwProcessId">
        /// プロセスID</param>
        /// <returns>
        /// Windowを作成したスレッドのID</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        #endregion NativeMethods
    }
}
