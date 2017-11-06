using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// ログのバッファ
    /// </summary>
    public class LogBuffer :
        IDisposable
    {
        #region Constants

        /// <summary>
        /// 空のログリスト
        /// </summary>
        private static readonly List<string> EmptyLogLineList = new List<string>();

        /// <summary>
        /// ツールチップのサフィックス
        /// </summary>
        /// <remarks>
        /// ツールチップは計4charsで構成されるが先頭1文字目が可変で残り3文字が固定となっている</remarks>
        private const string TooltipSuffix = "\u0001\u0001\uFFFD";

        /// <summary>
        /// ツールチップで残るリプレースメントキャラ
        /// </summary>
        private const string TooltipReplacementChar = "\uFFFD";

        #endregion Constants

        /// <summary>
        /// 内部バッファ
        /// </summary>
        private readonly ConcurrentQueue<LogLineEventArgs> logInfoQueue = new ConcurrentQueue<LogLineEventArgs>();

        /// <summary>
        /// 最初のログが到着したか？
        /// </summary>
        private bool firstLogArrived;

        #region コンストラクター/デストラクター/Dispose

        private const int FALSE = 0;

        private const int TRUE = 1;

        private int disposed = FALSE;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogBuffer()
        {
            // BeforeLogLineReadイベントを登録する 無理やり一番目に処理されるようにする
            this.AddOnBeforeLogLineRead();

            // LogLineReadイベントを登録する
            ActGlobals.oFormActMain.OnLogLineRead -= this.OnLogLineRead;
            ActGlobals.oFormActMain.OnLogLineRead += this.OnLogLineRead;

            // 生ログの書き出しバッファを開始する
            ChatLogWorker.Instance.Begin();
        }

        /// <summary>
        /// デストラクター
        /// </summary>
        ~LogBuffer()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, TRUE, FALSE) != FALSE)
            {
                return;
            }

            ActGlobals.oFormActMain.BeforeLogLineRead -= this.OnBeforeLogLineRead;
            ActGlobals.oFormActMain.OnLogLineRead -= this.OnLogLineRead;

            // 生ログの書き出しバッファを停止する
            ChatLogWorker.Instance.End();

            GC.SuppressFinalize(this);
        }

        #endregion コンストラクター/デストラクター/Dispose

        #region ACT event hander

        /// <summary>
        /// OnBeforeLogLineRead イベントを追加する
        /// </summary>
        /// <remarks>
        /// スペスペのOnBeforeLogLineReadをACT本体に登録する。
        /// ただし、FFXIVプラグインよりも先に処理する必要があるのでイベントを一旦除去して
        /// スペスペのイベントを登録した後に元のイベントを登録する
        /// </remarks>
        private void AddOnBeforeLogLineRead()
        {
            if (!Settings.Default.DetectPacketDump)
            {
                return;
            }

            try
            {
                var fi = ActGlobals.oFormActMain.GetType().GetField(
                    "BeforeLogLineRead",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.GetField |
                    BindingFlags.Public |
                    BindingFlags.Static);

                var beforeLogLineReadDelegate =
                    fi.GetValue(ActGlobals.oFormActMain)
                    as Delegate;

                if (beforeLogLineReadDelegate != null)
                {
                    var handlers = beforeLogLineReadDelegate.GetInvocationList();

                    // 全てのイベントハンドラを一度解除する
                    foreach (var handler in handlers)
                    {
                        ActGlobals.oFormActMain.BeforeLogLineRead -= (LogLineEventDelegate)handler;
                    }

                    // スペスペのイベントハンドラを最初に登録する
                    ActGlobals.oFormActMain.BeforeLogLineRead += this.OnBeforeLogLineRead;

                    // 解除したイベントハンドラを登録し直す
                    foreach (var handler in handlers)
                    {
                        ActGlobals.oFormActMain.BeforeLogLineRead += (LogLineEventDelegate)handler;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("AddOnBeforeLogLineRead error:", ex);
            }
        }

        /// <summary>
        /// OnBeforeLogLineRead
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="logInfo">ログ情報</param>
        /// <remarks>
        /// FFXIVプラグインが加工する前のログが通知されるイベント こちらは一部カットされてしまうログがカットされずに通知される
        /// またログのデリミタが異なるため、通常のログと同様に扱えるようにデリミタを変換して取り込む
        /// </remarks>
        private void OnBeforeLogLineRead(
            bool isImport,
            LogLineEventArgs logInfo)
        {
#if !DEBUG
            if (isImport)
            {
                return;
            }
#endif
            // PacketDumpを解析対象にしていないならば何もしない
            if (!Settings.Default.DetectPacketDump)
            {
                return;
            }

            try
            {
                var data = logInfo.logLine.Split('|');

                if (data.Length >= 2)
                {
                    var messageType = int.Parse(data[0]);
                    var timeStamp = DateTime.Parse(data[1]);

                    switch (messageType)
                    {
                        // 251:Debug, 252:PacketDump, 253:Version
                        case 251:
                        case 252:
                        case 253:
                            // ログオブジェクトをコピーする
                            var copyLogInfo = new LogLineEventArgs(
                                logInfo.logLine,
                                logInfo.detectedType,
                                logInfo.detectedTime,
                                logInfo.detectedZone,
                                logInfo.inCombat);

                            // ログを出力用に書き換える
                            copyLogInfo.logLine =
                                $"[{timeStamp:HH:mm:ss.fff}] {messageType:X2}:{string.Join(":", data)}";

                            this.logInfoQueue.Enqueue(copyLogInfo);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // 例外は握りつぶす
            }
        }

        private double[] lpss = new double[60];
        private int currentLpsIndex;
        private long currentLineCount;
        private Stopwatch lineCountTimer = new Stopwatch();

        /// <summary>
        /// LPS lines/s
        /// </summary>
        public double LPS
        {
            get
            {
                var avalableLPSs = this.lpss.Where(x => x > 0);
                if (!avalableLPSs.Any())
                {
                    return 0;
                }

                return avalableLPSs.Sum() / avalableLPSs.Count();
            }
        }

        /// <summary>
        /// LPSを計測する
        /// </summary>
        private void CountLPS()
        {
            this.currentLineCount++;

            if (!this.lineCountTimer.IsRunning)
            {
                this.lineCountTimer.Restart();
            }

            if (this.lineCountTimer.Elapsed >= TimeSpan.FromSeconds(1))
            {
                this.lineCountTimer.Stop();

                var lps = this.currentLineCount / this.lineCountTimer.Elapsed.TotalSeconds;
                if (lps > 0)
                {
                    if (this.currentLpsIndex > this.lpss.GetUpperBound(0))
                    {
                        this.currentLpsIndex = 0;
                    }

                    this.lpss[this.currentLpsIndex] = lps;
                    this.currentLpsIndex++;
                }

                this.currentLineCount = 0;
            }
        }

        /// <summary>
        /// OnLogLineRead
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="logInfo">ログ情報</param>
        /// <remarks>FFXIVプラグインが加工した後のログが通知されるイベント</remarks>
        private void OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            // 18文字以下のログは読み捨てる
            // なぜならば、タイムスタンプ＋ログタイプのみのログだから
            if (logInfo.logLine.Length <= 18)
            {
                return;
            }

            // ログをキューに格納する
            this.logInfoQueue.Enqueue(logInfo);

            // LPSを計測する
            this.CountLPS();

            // 最初のログならば動作ログに出力する
            if (!this.firstLogArrived)
            {
                Logger.Write("First log has arrived.");
            }

            this.firstLogArrived = true;
        }

        #endregion ACT event hander

        #region ログ処理

        public bool IsEmpty => this.logInfoQueue.IsEmpty;

        /// <summary>
        /// ログ行を返す
        /// </summary>
        /// <returns>ログ行の配列</returns>
        public IReadOnlyList<string> GetLogLines()
        {
            if (this.logInfoQueue.IsEmpty)
            {
                return EmptyLogLineList;
            }

            // プレイヤー情報を取得する
            var player = FFXIVPlugin.Instance.GetPlayer();

            // プレイヤーが召喚士か？
            var palyerIsSummoner = false;
            if (player != null)
            {
                var job = player.AsJob();
                if (job != null)
                {
                    palyerIsSummoner = job.IsSummoner();
                }
            }

            // マッチング用のログリスト
            var list = new List<string>(logInfoQueue.Count);

            var partyChangedAtDQX = false;
            var summoned = false;
            var doneCommand = false;

#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            while (logInfoQueue.TryDequeue(
                out LogLineEventArgs logInfo))
            {
                // 冒頭のタイムスタンプを除去する
                var logLine = logInfo.logLine.Remove(0, 15);

                // エフェクトに付与されるツールチップ文字を除去する
                if (Settings.Default.RemoveTooltipSymbols)
                {
                    // 4文字分のツールチップ文字を除去する
                    int index;
                    if ((index = logLine.IndexOf(
                        TooltipSuffix,
                        0,
                        StringComparison.Ordinal)) > -1)
                    {
                        logLine = logLine.Remove(index - 1, 4);
                    }

                    // 残ったReplacementCharを除去する
                    logLine = logLine.Replace(TooltipReplacementChar, string.Empty);
                }

                // FFXIVでの使用？
                if (!Settings.Default.UseOtherThanFFXIV &&
                    !summoned &&
                    palyerIsSummoner)
                {
                    summoned = isSummoned(logLine);
                }

                // パーティに変化があるか？（対DQX）
                if (!partyChangedAtDQX)
                {
                    partyChangedAtDQX = DQXUtility.IsPartyChanged(logLine);
                }

                // コマンドとマッチングする
                doneCommand |= TextCommandController.MatchCommandCore(logLine);

                list.Add(logLine);
            }

            if (summoned)
            {
                TableCompiler.Instance.RefreshPetPlaceholder();
            }

            if (partyChangedAtDQX)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    DQXUtility.RefeshKeywords();
                });
            }

            if (doneCommand)
            {
                SystemSounds.Asterisk.Play();
            }

            // ログファイルに出力する
            if (Settings.Default.SaveLogEnabled)
            {
                Task.Run(() => ChatLogWorker.Instance.AppendLines(list));
            }

#if DEBUG
            sw.Stop();
            System.Diagnostics.Debug.WriteLine($"★GetLogLines {sw.Elapsed.TotalMilliseconds:N1} ms");
#endif
            // リストを返す
            return list;

            // 召喚したか？
            bool isSummoned(string logLine)
            {
                var r = false;

                if (logLine.Contains("You cast Summon", StringComparison.OrdinalIgnoreCase))
                {
                    r = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(player.Name))
                    {
                        r = logLine.Contains(player.Name + "の「サモン", StringComparison.OrdinalIgnoreCase);
                    }

                    if (!string.IsNullOrEmpty(player.NameFI))
                    {
                        r = logLine.Contains(player.NameFI + "の「サモン", StringComparison.OrdinalIgnoreCase);
                    }

                    if (!string.IsNullOrEmpty(player.NameIF))
                    {
                        r = logLine.Contains(player.NameIF + "の「サモン", StringComparison.OrdinalIgnoreCase);
                    }

                    if (!string.IsNullOrEmpty(player.NameII))
                    {
                        r = logLine.Contains(player.NameII + "の「サモン", StringComparison.OrdinalIgnoreCase);
                    }
                }

                return r;
            }
        }

        #endregion ログ処理
    }
}
