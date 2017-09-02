using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// ログのバッファ
    /// </summary>
    public class LogBuffer :
        IDisposable
    {
        /// <summary>
        /// 空のログリスト
        /// </summary>
        private static readonly List<string> EmptyLogLineList = new List<string>();

        /// <summary>
        /// ツールチップ文字を除去するための正規表現
        /// </summary>
        private static readonly Regex TooltipCharsRegex =
            new Regex(@".\u0001\u0001\uFFFD",
                RegexOptions.Compiled);

        /// <summary>
        /// Unknownスキルを補完するための正規表現
        /// </summary>
        private static readonly Regex UnknownSkillRegex =
            new Regex(@"(?<UnknownSkill>Unknown_(?<UnknownSkillID>\w\w\w\w))",
                RegexOptions.Compiled);

        /// <summary>
        /// 内部バッファ
        /// </summary>
        private readonly ConcurrentQueue<LogLineEventArgs> logInfoQueue = new ConcurrentQueue<LogLineEventArgs>();

        /// <summary>
        /// 最初のログが到着したか？
        /// </summary>
        private bool firstLogArrived;

        /// <summary>
        /// 最後のログのタイムスタンプ
        /// </summary>
        private DateTime lastLogineTimestamp;

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

            this.logInfoQueue.Enqueue(logInfo);

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

            var list = new List<string>(logInfoQueue.Count);
            var partyChangedAtDQX = false;
            var summoned = false;

            while (logInfoQueue.TryDequeue(
                out LogLineEventArgs logInfo))
            {
                var logLine = logInfo.logLine.Trim();

                // エフェクトに付与されるツールチップ文字を除去する
                if (Settings.Default.RemoveTooltipSymbols)
                {
                    logLine = TooltipCharsRegex.Replace(logLine, string.Empty);
                }

                // Unknownスキルを補完する？
                if (Settings.Default.ToComplementUnknownSkill)
                {
                    if (logLine.Contains("Unknown_"))
                    {
                        var match = LogBuffer.UnknownSkillRegex.Match(logLine);
                        if (match.Success)
                        {
                            var unknownSkill = match.Groups["UnknownSkill"].Value;
                            var unknownSkillID = match.Groups["UnknownSkillID"].Value;

                            var skill = XIVDB.Instance.FindSkill(unknownSkillID);
                            if (skill != null)
                            {
                                logLine = logLine.Replace(
                                    unknownSkill,
                                    skill.Name);
                            }
                        }
                    }
                }

                // FFXIVでの使用？
                if (!Settings.Default.UseOtherThanFFXIV)
                {
                    if (!summoned)
                    {
                        // ペットIDのCacheを更新する
                        if (palyerIsSummoner)
                        {
                            if (logLine.Contains(player.Name + "の「サモン") ||
                                logLine.Contains(player.NameFI + "の「サモン") ||
                                logLine.Contains(player.NameIF + "の「サモン") ||
                                logLine.Contains(player.NameII + "の「サモン") ||
                                logLine.Contains("You cast Summon"))
                            {
                                summoned = true;
                            }
                        }
                    }
                }

                // パーティに変化があるか？（対DQX）
                var r = DQXUtility.IsPartyChanged(logLine);
                if (!partyChangedAtDQX)
                {
                    partyChangedAtDQX = r;
                }

                list.Add(logLine);
            }

            if (partyChangedAtDQX)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    DQXUtility.RefeshKeywords();
                });
            }

            if (summoned)
            {
                TableCompiler.Instance.RefreshPetPlaceholder();
            }

            // ログファイルに出力する
            if (Settings.Default.SaveLogEnabled)
            {
                Task.Run(() =>
                {
                    ChatLogWorker.Instance.AppendLines(list);
                });
            }

            // ログのタイムスタンプを記録する
            this.lastLogineTimestamp = DateTime.Now;

            return list;
        }

        #endregion ログ処理
    }
}
