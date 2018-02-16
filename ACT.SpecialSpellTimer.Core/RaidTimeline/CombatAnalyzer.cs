using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;
using NPOI.SS.UserModel;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    #region Enum

    /// <summary>
    /// 分析キーワードの分類
    /// </summary>
    public enum KewordTypes
    {
        Unknown = 0,
        Me,
        PartyMember,
        Pet,
        Cast,
        CastStartsUsing,
        Action,
        Effect,
        Marker,
        Dialogue,
        HPRate,
        Added,
        Start,
        End,
    }

    /// <summary>
    /// 戦闘ログの種類
    /// </summary>
    public enum LogTypes
    {
        Unknown = 0,
        CombatStart,
        CombatEnd,
        CastStart,
        Action,
        Effect,
        Marker,
        Added,
        HPRate,
        Dialog,
    }

    public static class LogTypesExtensions
    {
        public static string ToText(
            this LogTypes t)
            => new[]
            {
                "UNKNOWN",
                "Combat Start",
                "Combat End",
                "Starts Using",
                "Action",
                "Effect",
                "Marker",
                "Added",
                "HP Rate",
                "Dialog",
            }[(int)t];

        public static Color ToColor(
            this LogTypes t)
            => new[]
            {
                Colors.White,
                Colors.Wheat,
                Colors.Wheat,
                Colors.OrangeRed,
                Colors.Sienna,
                Colors.DarkGray,
                Colors.DarkGray,
                Colors.DarkGreen,
                Colors.Silver,
                Colors.Peru,
            }[(int)t];
    }

    #endregion Enum

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

        #region Keywords

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

        private static readonly Regex EffectRegex = new Regex(
            @"1A:(?<victim>.+?) gains the effect of (?<effect>.+?) from (?<actor>.+?) for (?<duration>[0-9\.]*?) Seconds.$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex MarkerRegex = new Regex(
            @"1B:(?<id>.{8}):(?<target>.+?):0000:0000:(?<type>....):0000:0000:0000:$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly IList<AnalyzeKeyword> Keywords = new[]
        {
            new AnalyzeKeyword() { Keyword = "・エギ", Category = KewordTypes.Pet },
            new AnalyzeKeyword() { Keyword = "フェアリー・", Category = KewordTypes.Pet },
            new AnalyzeKeyword() { Keyword = "カーバンクル・", Category = KewordTypes.Pet },
            new AnalyzeKeyword() { Keyword = "オートタレット", Category = KewordTypes.Pet },
            new AnalyzeKeyword() { Keyword = "デミ・バハムート", Category = KewordTypes.Pet },
            new AnalyzeKeyword() { Keyword = "アーサリースター", Category = KewordTypes.Pet },
            new AnalyzeKeyword() { Keyword = "を唱えた。", Category = KewordTypes.Cast },
            new AnalyzeKeyword() { Keyword = "の構え。", Category = KewordTypes.Cast },
            new AnalyzeKeyword() { Keyword = "starts using", Category = KewordTypes.CastStartsUsing },
            new AnalyzeKeyword() { Keyword = "HP at", Category = KewordTypes.HPRate },
            new AnalyzeKeyword() { Keyword = "Added new combatant", Category = KewordTypes.Added },
            new AnalyzeKeyword() { Keyword = "] 1B:", Category = KewordTypes.Marker },
            new AnalyzeKeyword() { Keyword = "] 1A:", Category = KewordTypes.Effect },
            new AnalyzeKeyword() { Keyword = "00:0044:", Category = KewordTypes.Dialogue },
            new AnalyzeKeyword() { Keyword = ImportLog, Category = KewordTypes.Start },
            new AnalyzeKeyword() { Keyword = "00:0039:戦闘開始", Category = KewordTypes.Start },
            new AnalyzeKeyword() { Keyword = "の攻略を終了した。", Category = KewordTypes.End },
            new AnalyzeKeyword() { Keyword = "ロットを行ってください。", Category = KewordTypes.End },
            new AnalyzeKeyword() { Keyword = WipeoutLog, Category = KewordTypes.End },
            new AnalyzeKeyword() { Keyword = "「", Category = KewordTypes.Action },
            new AnalyzeKeyword() { Keyword = "」", Category = KewordTypes.Action },
        };

        #endregion Keywords

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

        #region PC Name

        private IList<string> partyNames = null;
        private IList<Combatant> combatants = null;

        private static readonly Regex PCNameRegex = new Regex(
            @"[a-zA-Z'\.]+ [a-zA-Z'\.]+",
            RegexOptions.Compiled);

        /// <summary>
        /// Combatantsを取得する
        /// </summary>
        /// <returns>Combatants</returns>
        private IList<Combatant> GetCombatants()
        {
            lock (this)
            {
                if (this.combatants == null)
                {
                    // プレイヤ情報とパーティリストを取得する
                    var player = FFXIVPlugin.Instance.GetPlayer();
                    var ptlist = FFXIVPlugin.Instance.GetPartyList();

                    var list = new List<Combatant>();
                    list.Add(player);
                    list.AddRange(ptlist);

                    this.combatants = list;
                }
            }

            return this.combatants;
        }

        /// <summary>
        /// パーティメンバの名前リストを取得する
        /// </summary>
        /// <returns>名前リスト</returns>
        private IList<string> GetPartyMemberNames()
        {
            lock (this)
            {
                if (this.partyNames == null)
                {
                    var names = new List<string>();

                    foreach (var combatant in this.GetCombatants())
                    {
                        names.Add(combatant.Name);
                        names.Add(combatant.NameFI);
                        names.Add(combatant.NameIF);
                        names.Add(combatant.NameII);
                    }

                    this.partyNames = names;
                }
            }

            return this.partyNames;
        }

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

            var names = this.GetPartyMemberNames();

            if (names != null &&
                names.Any() &&
                names.Contains(actor))
            {
                return false;
            }

            return !PCNameRegex.Match(actor).Success;
        }

        /// <summary>
        /// PC名をJOB名に置換える
        /// </summary>
        /// <param name="name">PC名</param>
        /// <returns>JOB名</returns>
        private string ToNameToJob(
            string name)
        {
            var jobName = "<PC>";

            var combs = this.GetCombatants();

            var com = combs.FirstOrDefault(x =>
                x.Name == name ||
                x.NameFI == name ||
                x.NameIF == name ||
                x.NameII == name);

            if (com != null)
            {
                jobName = $"<{com.JobID.ToString()}>";
            }

            return jobName;
        }

        #endregion PC Name

        #region Store Log

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
                case KewordTypes.Pet:
                    break;

                case KewordTypes.Cast:
                    if (this.inCombat)
                    {
                        this.StoreCastLog(logLine);
                    }
                    break;

                case KewordTypes.CastStartsUsing:
                    /*
                    starts using は準備動作をかぶるので無視する
                    if (this.inCombat)
                    {
                        this.StoreCastStartsUsingLog(log);
                    }
                    */
                    break;

                case KewordTypes.Action:
                    if (this.inCombat)
                    {
                        this.StoreActionLog(logLine);
                    }
                    break;

                case KewordTypes.Effect:
                    if (this.inCombat)
                    {
                        this.StoreEffectLog(logLine);
                    }
                    break;

                case KewordTypes.Marker:
                    if (this.inCombat)
                    {
                        this.StoreMarkerLog(logLine);
                    }
                    break;

                case KewordTypes.HPRate:
                    if (this.inCombat)
                    {
                        this.StoreHPRateLog(logLine);
                    }
                    break;

                case KewordTypes.Added:
                    if (this.inCombat)
                    {
                        this.StoreAddedLog(logLine);
                    }
                    break;

                case KewordTypes.Dialogue:
                    if (this.inCombat)
                    {
                        this.StoreDialog(logLine);
                    }
                    break;

                case KewordTypes.Start:
                    lock (this.CurrentCombatLogList)
                    {
                        if (!this.inCombat)
                        {
                            if (!this.isImporting)
                            {
                                this.CurrentCombatLogList.Clear();
                                this.ActorHPRate.Clear();
                                this.partyNames = null;
                                this.combatants = null;
                                this.no = 1;
                            }

                            Logger.Write("Start Combat");
                        }

                        this.inCombat = true;
                    }

                    this.StoreStartCombat(logLine);
                    break;

                case KewordTypes.End:
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

            KewordTypes analyzeLogLine(string log, IList<AnalyzeKeyword> keywords)
            {
                var key = (
                    from x in keywords
                    where
                    log.Contains(x.Keyword)
                    select
                    x).FirstOrDefault();

                return key != null ?
                    key.Category :
                    KewordTypes.Unknown;
            }
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
                Skill = match.Groups["skill"].ToString(),
                LogType = LogTypes.Action
            };

            log.Text = log.Skill;
            log.SyncKeyword = log.RawWithoutTimestamp.Substring(8);

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
                Activity = $"Added",
                LogType = LogTypes.Added,
            };

            log.Text = $"Add {log.Actor}";
            log.SyncKeyword = log.RawWithoutTimestamp.Substring(0, log.RawWithoutTimestamp.IndexOf('.'));

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
                Skill = match.Groups["skill"].ToString(),
                LogType = LogTypes.CastStart
            };

            log.Text = log.Skill;
            log.SyncKeyword = log.RawWithoutTimestamp.Substring(8);

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
                Skill = match.Groups["skill"].ToString(),
                LogType = LogTypes.CastStart
            };

            log.Text = log.Skill;
            log.SyncKeyword = log.RawWithoutTimestamp.Substring(6);

            if (this.ToStoreActor(log.Actor))
            {
                this.StoreLog(log);
            }
        }

        /// <summary>
        /// Effectログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreEffectLog(
            LogLineEventArgs logInfo)
        {
            var match = EffectRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var victim = match.Groups["victim"].ToString();
            var victimJobName = this.ToNameToJob(victim);

            var effect = match.Groups["effect"].ToString();
            var actor = match.Groups["actor"].ToString();
            var duration = match.Groups["duration"].ToString();

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine
                    .Replace(victim, victimJobName),
                Actor = actor,
                Activity = $"effect {effect}",
                LogType = LogTypes.Effect
            };

            log.Text = log.Activity;
            log.SyncKeyword = log.RawWithoutTimestamp;

            if (victim != actor)
            {
                if (this.ToStoreActor(log.Actor))
                {
                    this.StoreLog(log);
                }
            }
        }

        /// <summary>
        /// マーカーログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreMarkerLog(
            LogLineEventArgs logInfo)
        {
            const string PCIDPlaceholder = "(?<pcid>.{8})";

            var match = MarkerRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            var id = match.Groups["id"].ToString();
            var target = match.Groups["target"].ToString();
            var targetJobName = this.ToNameToJob(target);

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                Raw = logInfo.logLine
                    .Replace(id, PCIDPlaceholder)
                    .Replace(target, targetJobName),
                Activity = $"Marker:{match.Groups["type"].ToString()}",
                LogType = LogTypes.Marker
            };

            log.Text = log.Activity;
            log.SyncKeyword = log.RawWithoutTimestamp;

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
                LogType = LogTypes.Dialog
            };

            log.Text = null;
            log.SyncKeyword = log.RawWithoutTimestamp.Substring(8);

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
                Activity = LogTypes.CombatStart.ToString(),
                LogType = LogTypes.CombatStart
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
                Activity = LogTypes.CombatEnd.ToString(),
                LogType = LogTypes.CombatEnd
            };

            this.StoreLog(log);
        }

        #endregion Store Log

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
                this.combatants = null;
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

            var timeStamp = logs.Last().TimeStamp;
            var zone = logs.First().Zone;
            zone = zone.Replace(" ", "_");
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                zone = zone.Replace(c, '_');
            }

            var file = $"{timeStamp.ToString("yyyy-MM-dd_HHmm")}.{zone}.AutoAnalyzedLog.xlsx";

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
                var timestampStyle = book.CreateCellStyle();
                var timeStyle = book.CreateCellStyle();
                var textStyle = book.CreateCellStyle();
                var perStyle = book.CreateCellStyle();
                noStyle.DataFormat = book.CreateDataFormat().GetFormat("#,##0_ ");
                timestampStyle.DataFormat = book.CreateDataFormat().GetFormat("yyyy-MM-dd HH:mm:ss.000");
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
                    writeCell<DateTime>(sheet, row, col++, data.TimeStamp, timestampStyle);
                    writeCell<DateTime>(sheet, row, col++, timeAsDateTime, timeStyle);
                    writeCell<double>(sheet, row, col++, data.TimeStampElapted.TotalSeconds, noStyle);
                    writeCell<string>(sheet, row, col++, data.LogTypeName, textStyle);
                    writeCell<string>(sheet, row, col++, data.Actor, textStyle);
                    writeCell<decimal>(sheet, row, col++, data.HPRate, perStyle);
                    writeCell<string>(sheet, row, col++, data.Activity, textStyle);
                    writeCell<string>(sheet, row, col++, data.RawWithoutTimestamp, textStyle);
                    writeCell<string>(sheet, row, col++, data.Zone, textStyle);
                    writeCell<string>(sheet, row, col++, data.Text, textStyle);
                    writeCell<string>(sheet, row, col++, data.SyncKeyword, textStyle);

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

        public void SaveToDraftTimeline(
            string file,
            IList<CombatLog> combatLogs)
        {
            if (!combatLogs.Any())
            {
                return;
            }

            var outputTypes = new[]
            {
                LogTypes.CastStart,
                LogTypes.Action,
                LogTypes.Dialog,
                LogTypes.Added,
                LogTypes.Marker,
                LogTypes.Effect,
            };

            var timeline = new TimelineModel();

            timeline.Zone = combatLogs.First().Zone;

            foreach (var log in combatLogs.Where(x =>
                outputTypes.Contains(x.LogType)))
            {
                var a = new TimelineActivityModel()
                {
                    Time = log.TimeStampElapted,
                    Text = log.Text,
                    SyncKeyword = log.SyncKeyword,
                };

                switch (log.LogType)
                {
                    case LogTypes.CastStart:
                        a.Notice = $"次は、{log.Skill}。";
                        break;

                    case LogTypes.Action:
                        // 構えのないアクションか？
                        if (!combatLogs.Any(x =>
                            x.ID < log.ID &&
                            x.Skill == log.Skill &&
                            x.LogType == LogTypes.CastStart &&
                            (log.TimeStamp - x.TimeStamp).TotalSeconds <= 12))
                        {
                            a.Notice = $"次は、{log.Skill}。";
                        }
                        else
                        {
                            continue;
                        }

                        break;

                    case LogTypes.Added:
                        a.Notice = $"次は、{log.Actor}。";

                        if (timeline.Activities.Any(x =>
                            x.Time == a.Time &&
                            x.Text == a.Text &&
                            x.SyncKeyword == a.SyncKeyword))
                        {
                            continue;
                        }

                        break;

                    case LogTypes.Marker:
                    case LogTypes.Effect:
                        a.Enabled = false;
                        if (timeline.Activities.Any(x =>
                            x.Time == a.Time &&
                            x.Text == a.Text))
                        {
                            continue;
                        }

                        break;
                }

                timeline.Add(a);
            }

            timeline.Save(file);
        }
    }

    /// <summary>
    /// 分析キーワード
    /// </summary>
    public class AnalyzeKeyword
    {
        /// <summary>
        /// キーワードの分類
        /// </summary>
        public KewordTypes Category
        {
            get;
            set;
        } = KewordTypes.Unknown;

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
            KewordTypes category)
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
}
