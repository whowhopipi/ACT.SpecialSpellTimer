using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

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
        private static CombatAnalyzer instance = new CombatAnalyzer();

        /// <summary>
        /// シングルトンInstance
        /// </summary>
        public static CombatAnalyzer Default => instance;

        #endregion Singleton

        public const string WipeoutLog = "00:0038:wipeout";

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
            @"00:0039:(?<discription>.+?)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex CombatEndRegex = new Regex(
            @"00:....:(?<discription>.+?)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly IList<AnalyzeKeyword> Keywords = new[]
        {
            new AnalyzeKeyword() { Keyword = "・エギ", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "フェアリー・", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "カーバンクル・", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "を唱えた。", Category = AnalyzeKeywordCategory.Cast },
            new AnalyzeKeyword() { Keyword = "の構え。", Category = AnalyzeKeywordCategory.Cast },
            new AnalyzeKeyword() { Keyword = "starts using", Category = AnalyzeKeywordCategory.CastStartsUsing },
            new AnalyzeKeyword() { Keyword = "HP at", Category = AnalyzeKeywordCategory.HPRate },
            new AnalyzeKeyword() { Keyword = "Added new combatant", Category = AnalyzeKeywordCategory.Added },
            new AnalyzeKeyword() { Keyword = "00:0044:", Category = AnalyzeKeywordCategory.Dialogue },
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
                Activity = $"{match.Groups["skill"].ToString()} の発動",
                LogType = CombatLogType.Action
            };

            if (!this.GetPartyMemberNames().Any(x => x == log.Actor))
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

            if (!this.GetPartyMemberNames().Any(x => x == log.Actor))
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
                Activity = $"{match.Groups["skill"].ToString()} の準備動作",
                LogType = CombatLogType.CastStart
            };

            if (!this.GetPartyMemberNames().Any(x => x == log.Actor))
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

            if (!this.GetPartyMemberNames().Any(x => x == log.Actor))
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

            if (!string.IsNullOrWhiteSpace(actor))
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
                Activity = dialog,
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
            lock (this.CurrentCombatLogList)
            {
                // IDを発番する
                log.ID = this.id;
                this.id++;

                // 経過秒を求める
                var origin = this.CurrentCombatLogList.FirstOrDefault();
                if (origin != null)
                {
                    log.TimeStampElapted = log.TimeStamp - origin.TimeStamp;
                }

                // アクター別の残HP率をセットする
                if (this.ActorHPRate.ContainsKey(log.Actor))
                {
                    log.HPRate = this.ActorHPRate[log.Actor];
                }

                if (!this.CurrentCombatLogList.Any())
                {
                    log.IsOrigin = true;
                }

                this.CurrentCombatLogList.Add(log);
            }
        }

        /// <summary>
        /// ログを格納するスレッド
        /// </summary>
        private void StoreLogPoller()
        {
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

            while (this.logInfoQueue.TryDequeue(out LogLineEventArgs log))
            {
                Thread.Yield();

                if (log == null)
                {
                    continue;
                }

                // ログを分類する
                var category = analyzeLogLine(log.logLine, Keywords);
                switch (category)
                {
                    case AnalyzeKeywordCategory.Pet:
                        break;

                    case AnalyzeKeywordCategory.Cast:
                        this.StoreCastLog(log);
                        break;

                    case AnalyzeKeywordCategory.CastStartsUsing:
                        this.StoreCastStartsUsingLog(log);
                        break;

                    case AnalyzeKeywordCategory.Action:
                        this.StoreActionLog(log);
                        break;

                    case AnalyzeKeywordCategory.HPRate:
                        this.StoreHPRateLog(log);
                        break;

                    case AnalyzeKeywordCategory.Added:
                        this.StoreAddedLog(log);
                        break;

                    case AnalyzeKeywordCategory.Dialogue:
                        this.StoreDialog(log);
                        break;

                    case AnalyzeKeywordCategory.Start:
                        // 戦闘開始でバッファを消去する
                        lock (this.CurrentCombatLogList)
                        {
                            if (this.CurrentCombatLogList.Count > 32)
                            {
                                this.CurrentCombatLogList.Clear();
                                this.ActorHPRate.Clear();
                            }

                            if (!this.CurrentCombatLogList.Any())
                            {
                                Logger.Write("Start Combat");
                            }
                        }

                        this.StoreStartCombat(log);
                        break;

                    case AnalyzeKeywordCategory.End:
                        Logger.Write("End Combat");
                        this.StoreEndCombat(log);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
