using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Sound;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Common;

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
        private static PluginMainWorker instance;

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static PluginMainWorker Instance =>
            instance ?? (instance = new PluginMainWorker());

        public static void Free() => instance = null;

        #endregion Singleton

        #region Thread

        private System.Timers.Timer backgroudWorker;
        private ThreadWorker detectLogsWorker;
        private volatile bool isOver;
        private DispatcherTimer refreshSpellOverlaysWorker;
        private DispatcherTimer refreshTickerOverlaysWorker;

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
            FFXIVPlugin.Initialize();
            FFXIVPlugin.Instance.Start();

            // ログバッファを生成する
            this.LogBuffer = new LogBuffer();

            // テーブルコンパイラを開始する
            TableCompiler.Initialize();
            TableCompiler.Instance.Begin();

            // 戦闘分析を初期化する
            CombatAnalyzer.Default.Initialize();

            // サウンドコントローラを開始する
            SoundController.Instance.Begin();

            // Overlayの更新スレッドを開始する
            this.BeginOverlaysThread();

            // ログ監視タイマを開始する
            this.detectLogsWorker = new ThreadWorker(() =>
            {
                this.DetectLogsCore();
            },
            0,
            nameof(this.detectLogsWorker));

            // Backgroudスレッドを開始する
            this.backgroudWorker = new System.Timers.Timer();
            this.backgroudWorker.AutoReset = true;
            this.backgroudWorker.Interval = 5000;
            this.backgroudWorker.Elapsed += (s, e) =>
            {
                this.BackgroundCore();
            };

            this.detectLogsWorker.Run();
            this.backgroudWorker.Start();
        }

        public void BeginOverlaysThread()
        {
            // スペルのスレッドを開始する
            this.refreshSpellOverlaysWorker = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.Default.RefreshInterval)
            };

            this.refreshSpellOverlaysWorker.Tick += (s, e) =>
            {
                try
                {
                    this.RefreshSpellOverlaysCore();
                }
                catch (Exception ex)
                {
                    Logger.Write("refresh spell overlays error:", ex);
                }
            };

            this.refreshSpellOverlaysWorker.Start();

            // テロップのスレッドを開始する
            this.refreshTickerOverlaysWorker = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.Default.RefreshInterval)
            };

            this.refreshTickerOverlaysWorker.Tick += (s, e) =>
            {
                try
                {
                    this.RefreshTickerOverlaysCore();
                }
                catch (Exception ex)
                {
                    Logger.Write("refresh ticker overlays error:", ex);
                }
            };

            this.refreshTickerOverlaysWorker.Start();
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
            this.refreshSpellOverlaysWorker?.Stop();
            this.refreshTickerOverlaysWorker?.Stop();
            this.detectLogsWorker?.Abort();

            this.refreshSpellOverlaysWorker = null;
            this.refreshTickerOverlaysWorker = null;
            this.detectLogsWorker = null;

            this.backgroudWorker?.Stop();
            this.backgroudWorker?.Dispose();
            this.backgroudWorker = null;

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
            TableCompiler.Free();

            // FFXIVのスキャンを停止する
            FFXIVPlugin.Instance.End();
            FFXIVPlugin.Free();
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
#if !DEBUG
                // importログの解析用にログを取り出しておく
                if (!this.LogBuffer.IsEmpty)
                {
                    this.LogBuffer.GetLogLines();
                }
                Thread.Sleep(TimeSpan.FromSeconds(3));
                return;
#endif
            }

            // 全滅によるリセットを判定する
            var resetTask = Task.Run(() => this.ResetCountAtRestart());

            // ログがないなら抜ける
            if (this.LogBuffer.IsEmpty)
            {
                resetTask.Wait();
                Thread.Sleep(TimeSpan.FromMilliseconds(Settings.Default.LogPollSleepInterval));
                return;
            }

#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            // ログを取り出す
            var logsTask = Task.Run(() => this.LogBuffer.GetLogLines());

            // 有効なスペルとテロップのリストを取得する
            var triggers = TableCompiler.Instance.TriggerList;

            var logs = logsTask.Result;
            if (logs.Count > 0)
            {
                var doneCommand = false;
                logs.AsParallel().AsOrdered().ForAll((logLine) =>
                {
                    // 冒頭のタイムスタンプを除去する
                    logLine = logLine.Remove(0, 15);

                    triggers.AsParallel().ForAll((trigger) =>
                    {
                        switch (trigger)
                        {
                            case OnePointTelop telop:
                                TickersController.Instance.MatchCore(
                                    telop,
                                    logLine);
                                break;

                            case Models.SpellTimer spell:
                                SpellsController.Instance.MatchCore(
                                    spell,
                                    logLine);
                                break;
                        }
                    });

                    // コマンドとマッチングする
                    doneCommand |= TextCommandController.MatchCommandCore(logLine);

                    Thread.Yield();
                });

                if (doneCommand)
                {
                    SystemSounds.Asterisk.Play();
                }
#if false
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
#endif

                existsLog = true;
            }
#if DEBUG
            sw.Stop();
            if (logs.Count != 0)
            {
                var time = sw.ElapsedMilliseconds;
                var count = logs.Count;
                Debug.WriteLine($"●DetectLogs\t{time:N1} ms\t{count:N0} lines\tavg {time / count:N2}");
            }
#endif

            resetTask.Wait();

            if (existsLog)
            {
                Thread.Sleep(0);
            }
            else
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(Settings.Default.LogPollSleepInterval));
            }
        }

        private void RefreshSpellOverlaysCore()
        {
            // Close要求を処理する
            SpellsController.Instance.ExecuteClosePanels();

            if (this.isOver)
            {
                return;
            }

            // 有効なスペルを取得する
            var spells = TableCompiler.Instance.SpellList;

            // FFXIVでの使用？
            if (!Settings.Default.UseOtherThanFFXIV &&
                !this.existFFXIVProcess &&
                !Settings.Default.OverlayForceVisible)
            {
                // 一時表示スペルがない？
                if (!spells.Any(x => x.IsTemporaryDisplay))
                {
                    SpellsController.Instance.ClosePanels();
                    return;
                }
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

            // 有効なテロップを取得する
            var telops = TableCompiler.Instance.TickerList;

            // FFXIVでの使用？
            if (!Settings.Default.UseOtherThanFFXIV &&
                !this.existFFXIVProcess &&
                !Settings.Default.OverlayForceVisible)
            {
                // 一時表示テロップがない？
                if (!telops.Any(x => x.IsTemporaryDisplay))
                {
                    TickersController.Instance.CloseTelops();
                    return;
                }
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
