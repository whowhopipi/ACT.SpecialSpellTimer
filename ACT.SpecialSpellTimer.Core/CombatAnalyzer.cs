using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
    /// <summary>
    /// 分析キーワードの分類
    /// </summary>
    public enum AnalyzeKeywordCategory
    {
        /// <summary>
        /// 未使用
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 自分
        /// </summary>
        Me = 0x01,

        /// <summary>
        /// パーティメンバ
        /// </summary>
        PartyMember = 0x02,

        /// <summary>
        /// ペット
        /// </summary>
        Pet = 0x03,

        /// <summary>
        /// 構え等準備動作
        /// </summary>
        Cast = 0x10,

        /// <summary>
        /// 構え等準備動作 starts using
        /// </summary>
        CastStartsUsing = 0x11,

        /// <summary>
        /// アクションの発動
        /// </summary>
        Action = 0x20,

        /// <summary>
        /// 残HP率
        /// </summary>
        HPRate = 0x30,

        /// <summary>
        /// Added Combatant
        /// </summary>
        Added = 0x40
    }

    /// <summary>
    /// 分析キーワード
    /// </summary>
    public class AnalyzeKeyword
    {
        /// <summary>
        /// キーワードの分類
        /// </summary>
        public AnalyzeKeywordCategory Category { get; set; }

        /// <summary>
        /// キーワード
        /// </summary>
        public string Keyword { get; set; }

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

        private static readonly IReadOnlyCollection<AnalyzeKeyword> Keywords = new List<AnalyzeKeyword>()
        {
            new AnalyzeKeyword() { Keyword = "・エギ", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "フェアリー・", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "カーバンクル・", Category = AnalyzeKeywordCategory.Pet },
            new AnalyzeKeyword() { Keyword = "を唱えた。", Category = AnalyzeKeywordCategory.Cast },
            new AnalyzeKeyword() { Keyword = "の構え。", Category = AnalyzeKeywordCategory.Cast },
            new AnalyzeKeyword() { Keyword = "starts using", Category = AnalyzeKeywordCategory.CastStartsUsing },
            new AnalyzeKeyword() { Keyword = "「", Category = AnalyzeKeywordCategory.Action },
            new AnalyzeKeyword() { Keyword = "」", Category = AnalyzeKeywordCategory.Action },
            new AnalyzeKeyword() { Keyword = "HP at", Category = AnalyzeKeywordCategory.HPRate },
            new AnalyzeKeyword() { Keyword = "Added new combatant", Category = AnalyzeKeywordCategory.Added },
        };

        private static readonly Regex StartsUsingRegex = new Regex(
            @"14:..:(?<actor>.+?) starts using (?<skill>.+?) on (?<target>.+?)\.$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

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
        private System.Timers.Timer storeLogWorker;

        /// <summary>
        /// 戦闘ログのリスト
        /// </summary>
        public List<CombatLog> CurrentCombatLogList { get; private set; } = new List<CombatLog>();

        /// <summary>
        /// アクターのHP率
        /// </summary>
        private Dictionary<string, decimal> ActorHPRate { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// ログを分析する
        /// </summary>
        public void AnalyzeLog()
        {
            this.AnalyzeLogAsync(this.CurrentCombatLogList);
        }

        /// <summary>
        /// ログを分析する
        /// </summary>
        /// <param name="logList">ログのリスト</param>
        public async void AnalyzeLogAsync(
            List<CombatLog> logList)
        {
            CombatLog[] logs;

            lock (this.CurrentCombatLogList)
            {
                if (logList == null ||
                    logList.Count < 1)
                {
                    return;
                }

                logs = logList.OrderBy(x => x.ID).ToArray();
            }

            var previouseAction = new Dictionary<string, DateTime>();

            await Task.Run(() =>
            {
                foreach (var log in logs)
                {
                    if (log.LogType == CombatLogType.AnalyzeStart ||
                        log.LogType == CombatLogType.AnalyzeEnd ||
                        log.LogType == CombatLogType.HPRate)
                    {
                        continue;
                    }

                    var key = log.LogType.ToString() + "-" + log.Actor + "-" + log.Action;

                    // 直前の同じログを探す
                    if (previouseAction.ContainsKey(key))
                    {
                        log.Span = (log.TimeStamp - previouseAction[key]).TotalSeconds;
                    }

                    // 記録しておく
                    previouseAction[key] = log.TimeStamp;
                }
            });
        }

        /// <summary>
        /// ログバッファをクリアする
        /// </summary>
        public void ClearLogBuffer()
        {
            lock (this.CurrentCombatLogList)
            {
                this.ClearLogInfoQueue();
                this.CurrentCombatLogList.Clear();
                this.ActorHPRate.Clear();
            }
        }

        /// <summary>
        /// 分析を停止する
        /// </summary>
        public void Denitialize()
        {
            ActGlobals.oFormActMain.OnLogLineRead -= this.FormActMain_OnLogLineRead;

            this.EndPoller();

            this.CurrentCombatLogList.Clear();
            Logger.Write("end combat analyze.");
        }

        /// <summary>
        /// ログのポーリングを終了する
        /// </summary>
        public void EndPoller()
        {
            this.storeLogWorker?.Stop();
            this.storeLogWorker?.Dispose();
            this.storeLogWorker = null;
            this.ClearLogInfoQueue();
        }

        /// <summary>
        /// 分析を開始する
        /// </summary>
        public void Initialize()
        {
            this.ClearLogBuffer();

            if (Settings.Default.CombatLogEnabled)
            {
                this.StartPoller();
            }

            ActGlobals.oFormActMain.OnLogLineRead += this.FormActMain_OnLogLineRead;
            Logger.Write("start combat analyze.");
        }

        /// <summary>
        /// ログのポーリングを開始する
        /// </summary>
        public void StartPoller()
        {
            this.ClearLogInfoQueue();

            this.storeLogWorker = new System.Timers.Timer();
            this.storeLogWorker.AutoReset = true;
            this.storeLogWorker.Interval = 1000;
            this.storeLogWorker.Elapsed += (s, e) =>
            {
                this.StoreLogPoller();
            };

            this.storeLogWorker.Start();
        }

        /// <summary>
        /// ログキューを消去する
        /// </summary>
        private void ClearLogInfoQueue()
        {
            while (!this.logInfoQueue.IsEmpty)
            {
                this.logInfoQueue.TryDequeue(out LogLineEventArgs l);
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
                if (!Settings.Default.CombatLogEnabled)
                {
                    return;
                }

                // キューに貯める
                this.logInfoQueue.Enqueue(logInfo);
            }
            catch (Exception ex)
            {
                Logger.Write(
                    "catch exception at Combat Analyzer OnLogLineRead.\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 分析用のキーワードを取得する
        /// </summary>
        /// <returns>キーワードコレクション</returns>
        private IReadOnlyCollection<string> GetPartyMemberNames()
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
                Action = $"{match.Groups["skill"].ToString()} の発動",
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
                Action = "Added",
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
                Action = $"{match.Groups["skill"].ToString()} の準備動作",
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
                Action = $"starts using {match.Groups["skill"].ToString()}",
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
        /// ログを格納する
        /// </summary>
        /// <param name="log">ログ</param>
        private void StoreLog(
            CombatLog log)
        {
            switch (log.LogType)
            {
                case CombatLogType.AnalyzeStart:
                    log.LogTypeName = "開始";
                    break;

                case CombatLogType.AnalyzeEnd:
                    log.LogTypeName = "終了";
                    break;

                case CombatLogType.CastStart:
                    log.LogTypeName = "準備動作";
                    break;

                case CombatLogType.Action:
                    log.LogTypeName = "アクション";
                    break;

                case CombatLogType.Added:
                    log.LogTypeName = "Added";
                    break;

                case CombatLogType.HPRate:
                    log.LogTypeName = "残HP率";
                    break;
            }

            lock (this.CurrentCombatLogList)
            {
                // IDを発番する
                log.ID = this.id;
                this.id++;

                // バッファサイズを超えた？
                if (this.CurrentCombatLogList.Count >
                    (Settings.Default.CombatLogBufferSize * 1.1m))
                {
                    // オーバー分を消去する
                    var over = (int)(this.CurrentCombatLogList.Count - Settings.Default.CombatLogBufferSize);
                    this.CurrentCombatLogList.RemoveRange(0, over);
                }

                // 経過秒を求める
                if (this.CurrentCombatLogList.Count > 0)
                {
                    log.TimeStampElapted =
                        (log.TimeStamp - this.CurrentCombatLogList.First().TimeStamp).TotalSeconds;
                }
                else
                {
                    log.TimeStampElapted = 0;
                }

                // アクター別の残HP率をセットする
                if (this.ActorHPRate.ContainsKey(log.Actor))
                {
                    log.HPRate = this.ActorHPRate[log.Actor];
                }

                this.CurrentCombatLogList.Add(log);
            }
        }

        /// <summary>
        /// ログを格納するスレッド
        /// </summary>
        private void StoreLogPoller()
        {
            var analyzeLogLine = new Func<string, IReadOnlyCollection<AnalyzeKeyword>, AnalyzeKeywordCategory>(
                (log, keywords) =>
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
                });

            while (!this.logInfoQueue.IsEmpty)
            {
                Thread.Sleep(0);

                this.logInfoQueue.TryDequeue(out LogLineEventArgs log);

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

                    default:
                        break;
                }
            }
        }
    }
}
