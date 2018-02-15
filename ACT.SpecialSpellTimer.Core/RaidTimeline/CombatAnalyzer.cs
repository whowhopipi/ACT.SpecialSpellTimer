using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;
using NPOI.SS.UserModel;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    /// <summary>
    /// 分析キーワードの分類
    /// </summary>
    public enum AnalyzeKeywordCategory
    {
        Unknown = 0,
        Me = 0x01,
        PartyMember = 0x02,
        Pet = 0x03,
        Cast = 0x10,
        CastStartsUsing = 0x11,
        Action = 0x20,
        Dialogue = 0x30,
        HPRate = 0x80,
        Added = 0x81,
        Start = 0x90,
        End = 0x91,
    }

    /// <summary>
    /// 分析キーワード
    /// </summary>
    public class AnalyzeKeyword
    {
        /// <summary>
        /// キーワードの分類
        /// </summary>
        public AnalyzeKeywordCategory Category
        {
            get;
            set;
        } = AnalyzeKeywordCategory.Unknown;

        /// <summary>
        /// キーワード
        /// </summary>
        public string Keyword
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// 同一カテゴリのキーワードをまとめて生成する
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <param name="category">カテゴリ</param>
        /// <returns>生成したキーワードオブジェクト</returns>
        public static AnalyzeKeyword[] CreateKeywords(
            string[] keywords,
            AnalyzeKeywordCategory category)
        {
            var list = new List<AnalyzeKeyword>();

            foreach (var k in keywords)
            {
                list.Add(new AnalyzeKeyword()
                {
                    Keyword = k,
                    Category = category
                });
            }

            return list.ToArray();
        }
    }

    /// <summary>
    /// コンバットアナライザ
    /// </summary>
    public class CombatAnalyzer
    {
        #region Singleton

        /// <summary>
        /// シングルトンInstance
        /// </summary>
        private static CombatAnalyzer instance;

        /// <summary>
        /// シングルトンInstance
        /// </summary>
        public static CombatAnalyzer Instance =>
            instance ?? (instance = new CombatAnalyzer());

        #endregion Singleton

        public const string WipeoutLog = "00:0038:wipeout";
        public const string ImportLog = "00:0038:import";

        private static readonly Regex ActionRegex = new Regex(
            @"\[.+?\] 00:....:(?<actor>.+?)の「(?<skill>.+?)」$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex AddedRegex = new Regex(
            @"\[.+?\] 03:Added new combatant (?<actor>.+)\.  ",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex CastRegex = new Regex(
            @"\[.+?\] 00:....:(?<actor>.+?)は「(?<skill>.+?)」(を唱えた。|の構え。)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex HPRateRegex = new Regex(
            @"\[.+?\] ..:(?<actor>.+?) HP at (?<hprate>\d+?)%",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex StartsUsingRegex = new Regex(
            @"14:..:(?<actor>.+?) starts using (?<skill>.+?) on (?<target>.+?)\.$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex DialogRegex = new Regex(
            @"00:0044:(?<dialog>.+?)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex CombatStartRegex = new Regex(
            @"00:(0038|0039):(?<discription>.+?)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex CombatEndRegex = new Regex(
            @"00:....:(?<discription>.+?)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly IList<AnalyzeKeyword> Keywords = new[]
        {
            new AnalyzeKeyword() { Keyword = "・エギ", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "フェアリー・", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "カーバンクル・", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "オートタレット", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "デミ・バハムート", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "を唱えた。", Category = AnalyzeKeywordCategory.Cast },
            new AnalyzeKeyword() { Keyword = "の構え。", Category = AnalyzeKeywordCategory.Cast },
            new AnalyzeKeyword() { Keyword = "starts using", Category = AnalyzeKeywordCategory.CastStartsUsing },
            new AnalyzeKeyword() { Keyword = "HP at", Category = AnalyzeKeywordCategory.HPRate },
            new AnalyzeKeyword() { Keyword = "Added new combatant", Category = AnalyzeKeywordCategory.Added },
            new AnalyzeKeyword() { Keyword = "00:0044:", Category = AnalyzeKeywordCategory.Dialogue },
            new AnalyzeKeyword() { Keyword = ImportLog, Category = AnalyzeKeywordCategory.Start },
            new AnalyzeKeyword() { Keyword = "00:0039:戦闘開始", Category = AnalyzeKeywordCategory.Start },
            new AnalyzeKeyword() { Keyword = "の攻略を終了した。", Category = AnalyzeKeywordCategory.End },
            new AnalyzeKeyword() { Keyword = "ロットを行ってください。", Category = AnalyzeKeywordCategory.End },
            new AnalyzeKeyword() { Keyword = WipeoutLog, Category = AnalyzeKeywordCategory.End },
            new AnalyzeKeyword() { Keyword = "「", Category = AnalyzeKeywordCategory.Action },
            new AnalyzeKeyword() { Keyword = "」", Category = AnalyzeKeywordCategory.Action },
        };

        /// <summary>
        /// ログ一時バッファ
        /// </summary>
        private readonly ConcurrentQueue<LogLineEventArgs> logInfoQueue = new ConcurrentQueue<LogLineEventArgs>();

        /// <summary>
        /// ログのID
        /// </summary>
        private long id;

        /// <summary>
        /// ログ格納スレッド
        /// </summary>
        private DispatcherTimer storeLogWorker;

        /// <summary>
        /// ログ格納中？
        /// </summary>
        private bool isStoreLogAnalyzing;

        /// <summary>
        /// 戦闘ログのリスト
        /// </summary>
        public ObservableCollection<CombatLog> CurrentCombatLogList
        {
            get;
            private set;
        } = new ObservableCollection<CombatLog>();

        /// <summary>
        /// アクターのHP率
        /// </summary>
        private Dictionary<string, decimal> ActorHPRate
        {
            get;
            set;
        } = new Dictionary<string, decimal>();

        /// <summary>
        /// 分析を開始する
        /// </summary>
        public void Start()
        {
            this.ClearLogBuffer();

            if (Settings.Default.AutoCombatLogAnalyze)
            {
                this.StartPoller();
                ActGlobals.oFormActMain.OnLogLineRead -= this.FormActMain_OnLogLineRead;
                ActGlobals.oFormActMain.OnLogLineRead += this.FormActMain_OnLogLineRead;
                Logger.Write("Start Timeline Analyze.");
            }
        }

        /// <summary>
        /// 分析を停止する
        /// </summary>
        public void End()
        {
            ActGlobals.oFormActMain.OnLogLineRead -= this.FormActMain_OnLogLineRead;

            this.EndPoller();
            this.ClearLogBuffer();
            Logger.Write("End Timeline Analyze.");
        }

        /// <summary>
        /// ログのポーリングを開始する
        /// </summary>
        private void StartPoller()
        {
            this.ClearLogInfoQueue();

            if (this.storeLogWorker != null &&
                this.storeLogWorker.IsEnabled)
            {
                return;
            }

            this.storeLogWorker = new DispatcherTimer(DispatcherPriority.Background);
            this.storeLogWorker.Interval = TimeSpan.FromSeconds(3);
            this.storeLogWorker.Tick += (s, e) =>
            {
                lock (this)
                {
                    if (this.isStoreLogAnalyzing)
                    {
                        return;
                    }

                    this.isStoreLogAnalyzing = true;

                    try
                    {
                        this.StoreLogPoller();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(
                            "catch exception at StoreLog.",
                            ex);
                    }
                    finally
                    {
                        this.isStoreLogAnalyzing = false;
                    }
                }
            };

            this.isStoreLogAnalyzing = false;
            this.storeLogWorker.Start();

            this.inCombat = false;
        }

        /// <summary>
        /// ログのポーリングを終了する
        /// </summary>
        private void EndPoller()
        {
            lock (this)
            {
                this.storeLogWorker?.Stop();
                this.storeLogWorker = null;
                this.isStoreLogAnalyzing = false;
            }

            this.ClearLogInfoQueue();
        }

        /// <summary>
        /// ログバッファをクリアする
        /// </summary>
        private void ClearLogBuffer()
        {
            lock (this.CurrentCombatLogList)
            {
                this.ClearLogInfoQueue();
                this.CurrentCombatLogList.Clear();
                this.ActorHPRate.Clear();
            }
        }

        /// <summary>
        /// ログキューを消去する
        /// </summary>
        private void ClearLogInfoQueue()
        {
            while (this.logInfoQueue.TryDequeue(out LogLineEventArgs l))
            {
            }
        }

        /// <summary>
        /// ログを1行読取った
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="logInfo">ログ情報</param>
        private void FormActMain_OnLogLineRead(
            bool isImport,
            LogLineEventArgs logInfo)
        {
            try
            {
                if (!Settings.Default.AutoCombatLogAnalyze)
                {
                    return;
                }

                // キューに貯める
                this.logInfoQueue.Enqueue(logInfo);
            }
            catch (Exception ex)
            {
                Logger.Write(
                    "catch exception at Timeline Analyzer OnLogLineRead.",
                    ex);
            }
        }

        /// <summary>
        /// 分析用のキーワードを取得する
        /// </summary>
        /// <returns>キーワードコレクション</returns>
        private IList<string> GetPartyMemberNames()
        {
            var names = new List<string>();

            // プレイヤ情報とパーティリストを取得する
            var player = FFXIVPlugin.Instance.GetPlayer();
            var ptlist = FFXIVPlugin.Instance.GetPartyList();

            if (player != null)
            {
                names.Add(player.Name);
                names.Add(player.NameFI);
                names.Add(player.NameIF);
                names.Add(player.NameII);
            }

            if (ptlist != null)
            {
                names.AddRange(ptlist.Select(x => x.Name));
                names.AddRange(ptlist.Select(x => x.NameFI));
                names.AddRange(ptlist.Select(x => x.NameIF));
                names.AddRange(ptlist.Select(x => x.NameII));
            }

            return names;
        }

        private IList<string> partyNames = null;

        private static readonly Regex PCNameRegex = new Regex(
            @"[a-zA-Z'\.]+ [a-zA-Z'\.]+",
            RegexOptions.Compiled);

        /// <summary>
        /// ログを保存する対象のActorか？
        /// </summary>
        /// <param name="actor">アクター</param>
        /// <returns>保存対象か？</returns>
        private bool ToStoreActor(
            string actor)
        {
            if (string.IsNullOrEmpty(actor))
            {
                return true;
            }

            if (this.partyNames == null)
            {
                this.partyNames = this.GetPartyMemberNames();
            }

#if !DEBUG
            if (this.partyNames.Any())
            {
                return !this.partyNames.Contains(actor);
            }
#endif
            return !PCNameRegex.Match(actor).Success;
        }

        /// <summary>
        /// アクションログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreActionLog(
            LogLineEventArgs logInfo)
        {
            var match = ActionRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                Activity = $"{match.Groups["skill"].ToString()}",
                LogType = CombatLogType.Action
            };

            if (this.ToStoreActor(log.Actor))
            {
                this.StoreLog(log);
            }
        }

        /// <summary>
        /// Addedのログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreAddedLog(
            LogLineEventArgs logInfo)
        {
            var match = AddedRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                Activity = "Added",
                LogType = CombatLogType.Added
            };

            if (this.ToStoreActor(log.Actor))
            {
                this.StoreLog(log);
            }
        }

        /// <summary>
        /// キャストログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreCastLog(
            LogLineEventArgs logInfo)
        {
            var match = CastRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                Activity = $"{match.Groups["skill"].ToString()} Start",
                LogType = CombatLogType.CastStart
            };

            if (this.ToStoreActor(log.Actor))
            {
                this.StoreLog(log);
            }
        }

        /// <summary>
        /// キャストログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreCastStartsUsingLog(
            LogLineEventArgs logInfo)
        {
            var match = StartsUsingRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                Activity = $"starts using {match.Groups["skill"].ToString()}",
                LogType = CombatLogType.CastStart
            };

            if (this.ToStoreActor(log.Actor))
            {
                this.StoreLog(log);
            }
        }

        /// <summary>
        /// HP率のログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreHPRateLog(
            LogLineEventArgs logInfo)
        {
            var match = HPRateRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var actor = match.Groups["actor"].ToString();

            if (this.ToStoreActor(actor))
            {
                decimal hprate;
                if (!decimal.TryParse(match.Groups["hprate"].ToString(), out hprate))
                {
                    hprate = 0m;
                }

                this.ActorHPRate[actor] = hprate;
            }
        }

        /// <summary>
        /// セリフを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreDialog(
            LogLineEventArgs logInfo)
        {
            var match = DialogRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var dialog = match.Groups["dialog"].ToString();

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine,
                Actor = string.Empty,
                Activity = "Dialog",
                LogType = CombatLogType.Dialog
            };

            this.StoreLog(log);
        }

        /// <summary>
        /// 戦闘開始を格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreStartCombat(
            LogLineEventArgs logInfo)
        {
            var match = CombatStartRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var discription = match.Groups["discription"].ToString();

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine,
                Actor = string.Empty,
                Activity = discription,
                LogType = CombatLogType.CombatStart
            };

            this.StoreLog(log);
        }

        /// <summary>
        /// 戦闘終了を格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreEndCombat(
            LogLineEventArgs logInfo)
        {
            var match = CombatEndRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var discription = match.Groups["discription"].ToString();

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine,
                Actor = string.Empty,
                Activity = discription,
                LogType = CombatLogType.CombatEnd
            };

            this.StoreLog(log);
        }

        /// <summary>
        /// ログを格納する
        /// </summary>
        /// <param name="log">ログ</param>
        private void StoreLog(
            CombatLog log)
        {
            var zone = ActGlobals.oFormActMain.CurrentZone;
            zone = string.IsNullOrEmpty(zone) ?
                "UNKNOWN" :
                zone;

            lock (this.CurrentCombatLogList)
            {
                // IDを発番する
                log.ID = this.id;
                this.id++;

                // 今回の分析の連番を付与する
                log.No = this.no;
                this.no++;

                // 経過秒を求める
                var origin = this.CurrentCombatLogList.FirstOrDefault(x => x.IsOrigin);
                if (origin != null)
                {
                    var ts = log.TimeStamp - origin.TimeStamp;
                    if (ts.TotalMinutes <= 60 &&
                        ts.TotalMinutes >= -60)
                    {
                        log.TimeStampElapted = ts;
                    }
                }

                // アクター別の残HP率をセットする
                if (this.ActorHPRate.ContainsKey(log.Actor))
                {
                    log.HPRate = this.ActorHPRate[log.Actor];
                }

                if (!this.CurrentCombatLogList.Any() &&
                    log.RawWithoutTimestamp != ImportLog)
                {
                    log.IsOrigin = true;
                }

                // ゾーンを保存する
                log.Zone = zone;

                this.CurrentCombatLogList.Add(log);
            }
        }

        private long no;
        private bool inCombat;
        private bool isImporting;

        /// <summary>
        /// ログを格納するスレッド
        /// </summary>
        private void StoreLogPoller()
        {
            while (this.logInfoQueue.TryDequeue(out LogLineEventArgs log))
            {
                Thread.Yield();
                this.AnalyzeLogLine(log);
            }
        }

        /// <summary>
        /// ログ行を分析する
        /// </summary>
        /// <param name="logLine">ログ行</param>
        private void AnalyzeLogLine(
            LogLineEventArgs logLine)
        {
            if (logLine == null)
            {
                return;
            }

            // ログを分類する
            var category = analyzeLogLine(logLine.logLine, Keywords);
            switch (category)
            {
                case AnalyzeKeywordCategory.Pet:
                    break;

                case AnalyzeKeywordCategory.Cast:
                    if (this.inCombat)
                    {
                        this.StoreCastLog(logLine);
                    }
                    break;

                case AnalyzeKeywordCategory.CastStartsUsing:
                    /*
                    starts using は準備動作をかぶるので無視する
                    if (this.inCombat)
                    {
                        this.StoreCastStartsUsingLog(log);
                    }
                    */
                    break;

                case AnalyzeKeywordCategory.Action:
                    if (this.inCombat)
                    {
                        this.StoreActionLog(logLine);
                    }
                    break;

                case AnalyzeKeywordCategory.HPRate:
                    if (this.inCombat)
                    {
                        this.StoreHPRateLog(logLine);
                    }
                    break;

                case AnalyzeKeywordCategory.Added:
                    if (this.inCombat)
                    {
                        this.StoreAddedLog(logLine);
                    }
                    break;

                case AnalyzeKeywordCategory.Dialogue:
                    if (this.inCombat)
                    {
                        this.StoreDialog(logLine);
                    }
                    break;

                case AnalyzeKeywordCategory.Start:
                    lock (this.CurrentCombatLogList)
                    {
                        if (!this.inCombat)
                        {
                            if (!this.isImporting)
                            {
                                this.CurrentCombatLogList.Clear();
                                this.ActorHPRate.Clear();
                                this.partyNames = null;
                                this.no = 1;
                            }

                            Logger.Write("Start Combat");
                        }

                        this.inCombat = true;
                    }

                    this.StoreStartCombat(logLine);
                    break;

                case AnalyzeKeywordCategory.End:
                    lock (this.CurrentCombatLogList)
                    {
                        if (this.inCombat)
                        {
                            this.inCombat = false;
                            this.StoreEndCombat(logLine);

                            this.AutoSaveToSpreadsheet();

                            Logger.Write("End Combat");
                        }
                    }
                    break;

                default:
                    break;
            }

            AnalyzeKeywordCategory analyzeLogLine(string log, IList<AnalyzeKeyword> keywords)
            {
                var key = (
                    from x in keywords
                    where
                    log.Contains(x.Keyword)
                    select
                    x).FirstOrDefault();

                return key != null ?
                    key.Category :
                    AnalyzeKeywordCategory.Unknown;
            }
        }

        /// <summary>
        /// ログ行をインポートして解析する
        /// </summary>
        /// <param name="logLines">インポートするログ行</param>
        public void ImportLogLines(
            List<string> logLines)
        {
            try
            {
                this.isImporting = true;

                // 冒頭にインポートを示すログを加える
                logLines.Insert(0, $"[00:00:00.000] {ImportLog}");

                var now = DateTime.Now;

                // 各種初期化
                this.inCombat = false;
                this.CurrentCombatLogList.Clear();
                this.ActorHPRate.Clear();
                this.partyNames = null;
                this.no = 1;

                foreach (var line in logLines)
                {
                    if (line.Length < 14)
                    {
                        continue;
                    }

                    var timeAsText = line.Substring(0, 14)
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty);

                    DateTime time;
                    if (!DateTime.TryParse(timeAsText, out time))
                    {
                        continue;
                    }

                    var detectTime = new DateTime(
                        now.Year,
                        now.Month,
                        now.Day,
                        time.Hour,
                        time.Minute,
                        time.Second,
                        time.Millisecond);

                    var arg = new LogLineEventArgs(
                        line,
                        0,
                        detectTime,
                        string.Empty,
                        true);

                    this.AnalyzeLogLine(arg);
                }
            }
            finally
            {
                this.isImporting = false;
            }
        }

        private void AutoSaveToSpreadsheet()
        {
            if (!Settings.Default.AutoCombatLogSave ||
                string.IsNullOrEmpty(Settings.Default.CombatLogSaveDirectory))
            {
                return;
            }

            var logs = default(IList<CombatLog>);
            lock (this.CurrentCombatLogList)
            {
                logs = this.CurrentCombatLogList.ToArray();
            }

            if (!logs.Any())
            {
                return;
            }

            var zone = logs.First().Zone;
            var timeStamp = logs.Last().TimeStamp;

            var file = $"{timeStamp.ToString("yyyy-MM-dd_HHmm")}.[{zone}].AutoAnalyzedLog.xlsx";

            this.SaveToSpreadsheet(
                Path.Combine(Settings.Default.CombatLogSaveDirectory, file),
                logs);
        }

        public void SaveToSpreadsheet(
            string file,
            IList<CombatLog> combatLogs)
        {
            var master = Path.Combine(
                DirectoryHelper.FindSubDirectory("resources"),
                "CombatLogBase.xlsx");

            var work = Path.Combine(
                DirectoryHelper.FindSubDirectory("resources"),
                "CombatLogBase_work.xlsx");

            if (!File.Exists(master))
            {
                throw new FileNotFoundException(
                   $"CombatLog Master File Not Found. {master}");
            }

            File.Copy(master, work, true);

            var book = WorkbookFactory.Create(work);
            var sheet = book.GetSheet("CombatLog");

            try
            {
                // セルの書式を生成する
                var noStyle = book.CreateCellStyle();
                var timeStyle = book.CreateCellStyle();
                var textStyle = book.CreateCellStyle();
                var perStyle = book.CreateCellStyle();
                noStyle.DataFormat = book.CreateDataFormat().GetFormat("#,##0_ ");
                timeStyle.DataFormat = book.CreateDataFormat().GetFormat("mm:ss");
                textStyle.DataFormat = book.CreateDataFormat().GetFormat("@");
                perStyle.DataFormat = book.CreateDataFormat().GetFormat("0.0%");

                var now = DateTime.Now;

                var row = 1;
                foreach (var data in combatLogs)
                {
                    var timeAsDateTime = new DateTime(
                        now.Year,
                        now.Month,
                        now.Day,
                        0,
                        data.TimeStampElapted.Minutes >= 0 ? data.TimeStampElapted.Minutes : 0,
                        data.TimeStampElapted.Seconds >= 0 ? data.TimeStampElapted.Seconds : 0);

                    var col = 0;

                    writeCell<long>(sheet, row, col++, data.No, noStyle);
                    writeCell<DateTime>(sheet, row, col++, timeAsDateTime, timeStyle);
                    writeCell<double>(sheet, row, col++, data.TimeStampElapted.TotalSeconds, noStyle);
                    writeCell<string>(sheet, row, col++, data.LogTypeName, textStyle);
                    writeCell<string>(sheet, row, col++, data.Actor, textStyle);
                    writeCell<decimal>(sheet, row, col++, data.HPRate, perStyle);
                    writeCell<string>(sheet, row, col++, data.Activity, textStyle);
                    writeCell<string>(sheet, row, col++, data.RawWithoutTimestamp, textStyle);
                    writeCell<string>(sheet, row, col++, data.Zone, textStyle);

                    row++;
                }

                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                using (var fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    book.Write(fs);
                }
            }
            finally
            {
                book?.Close();
                File.Delete(work);
            }

            // セルの編集内部メソッド
            void writeCell<T>(ISheet sh, int r, int c, T v, ICellStyle style)
            {
                var rowObj = sh.GetRow(r) ?? sheet.CreateRow(r);
                var cellObj = rowObj.GetCell(c) ?? rowObj.CreateCell(c);

                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        cellObj.SetCellValue(Convert.ToDouble(v));
                        cellObj.CellStyle = style;
                        break;

                    case TypeCode.DateTime:
                        cellObj.SetCellValue(Convert.ToDateTime(v));
                        cellObj.CellStyle = style;
                        break;

                    case TypeCode.String:
                        cellObj.SetCellValue(Convert.ToString(v));
                        cellObj.CellStyle = style;
                        break;
                }
            }
        }
    }
}
