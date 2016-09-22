namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// ログのバッファ
    /// </summary>
    public class LogBuffer : IDisposable
    {
        #region private static fields

        /// <summary>
        /// ペットID更新ロックオブジェクト
        /// </summary>
        private static readonly object lockPetidObject = new object();

        /// <summary>
        /// 空の文字列リスト
        /// </summary>
        private static readonly IReadOnlyList<string> EMPTY_STRING_LIST =
            new List<string>();

        /// <summary>
        /// 空の(文字列 -> 文字列)マップ
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> EMPTY_STRING_PAIR_MAP =
            new Dictionary<string, string>();

        /// <summary>
        /// パーティ名プレースホルダーのキャッシュ "<2>" ～ "<8>"
        /// </summary>
        private static readonly IReadOnlyList<string> PARTY_PLACEHOLDERS =
            Enumerable.Range(2, 7).Select(ordinal => "<" + ordinal + ">").ToList();

        /// <summary>
        /// パーティ変更をチェックするワードパターン
        /// </summary>
        private static readonly IReadOnlyCollection<IReadOnlyCollection<string>> PARTY_CHANGED_WORDS =
            new List<IReadOnlyCollection<string>>
            {
                new List<string> { "パーティを解散しました。" },
                new List<string> { "がパーティに参加しました。" },
                new List<string> { "がパーティから離脱しました。" },
                new List<string> { "をパーティから離脱させました。" },
                new List<string> { "の攻略を開始した。" },
                new List<string> { "の攻略を終了した。" },
                new List<string> { "You join ", "'s party." },
                new List<string> { "You left the party." },
                new List<string> { "You dissolve the party." },
                new List<string> { "The party has been disbanded." },
                new List<string> { "joins the party." },
                new List<string> { "has left the party." },
                new List<string> { "was removed from the party." },
            };

        /// <summary>
        /// 現在のパーティメンバー名リストのキャッシュ
        /// </summary>
        private static volatile IReadOnlyList<string> partyList = EMPTY_STRING_LIST;

        /// <summary>
        /// ジョブ名プレースホルダー -> プレイヤー名
        /// </summary>
        private static volatile IReadOnlyDictionary<string, string> placeholderToJobNameDictionaly = EMPTY_STRING_PAIR_MAP;

        /// <summary>
        /// ペットのID
        /// </summary>
        private static volatile string currentPetId = string.Empty;

        /// <summary>
        /// ペットのIDを取得したゾーン
        /// </summary>
        private static volatile string petIdCheckedZone = string.Empty;

        /// <summary>
        /// パーティメンバの代名詞が有効か？
        /// </summary>
        private static bool enabledPartyMemberPlaceHolder => Settings.Default.EnabledPartyMemberPlaceholder;

        #endregion

        #region public static properties

        /// <summary>
        /// パーティメンバー名
        /// </summary>
        public static IReadOnlyList<string> PartyList => partyList;

        /// <summary>
        /// ジョブ代名詞による置換文字列セットのリスト
        /// </summary>
        public static IReadOnlyDictionary<string, string> PlaceholderToJobNameDictionaly => placeholderToJobNameDictionaly;

        #endregion

        #region private fields

        /// <summary>
        /// 内部バッファ
        /// </summary>
        private readonly ConcurrentQueue<LogLineEventArgs> logInfoQueue = new ConcurrentQueue<LogLineEventArgs>();

        /// <summary>
        /// ログファイル出力用のバッファ
        /// </summary>
        private readonly StringBuilder fileOutputLogBuffer = new StringBuilder();

        /// <summary>
        /// 現在走っているゾーン/ペットID情報更新タスクのキャンセルトークンソース
        /// </summary>
        private volatile CancellationTokenSource petIdRefreshTaskCancelTokenSource;

        /// <summary>
        /// 最後のログのタイムスタンプ
        /// </summary>
        private DateTime lastLogineTimestamp;

        /// <summary>
        /// 最初のログが到着したか？
        /// </summary>
        private bool firstLogArrived;

        #endregion

        #region コンストラクター/デストラクター/Dispose

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogBuffer()
        {
            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;
        }

        /// <summary>
        /// デストラクター
        /// </summary>
        ~LogBuffer()
        {
            Dispose();
        }

        private const int FALSE = 0;
        private const int TRUE = 1;
        private int disposed = FALSE;

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, TRUE, FALSE) != FALSE)
            {
                return;
            }

            ActGlobals.oFormActMain.OnLogLineRead -= this.oFormActMain_OnLogLineRead;

            LogLineEventArgs ignore;
            while (logInfoQueue.TryDequeue(out ignore)) ;

            this.FlushLogFile();

            partyList = EMPTY_STRING_LIST;
            placeholderToJobNameDictionaly = EMPTY_STRING_PAIR_MAP;
            currentPetId = string.Empty;
            petIdCheckedZone = string.Empty;

            GC.SuppressFinalize(this);
        }

        #endregion

        #region ACT event hander

        /// <summary>
        /// ログを一行読取った
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="logInfo">ログ情報</param>
        private void oFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            if (isImport)
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

        #endregion

        #region ログ処理

        /// <summary>
        /// バッファーにログがない場合 true
        /// </summary>
        /// <returns>バッファーにログがない場合 true</returns>
        public bool nonEmpty()
        {
            return !logInfoQueue.IsEmpty;
        }

        /// <summary>
        /// ログ行を返す
        /// </summary>
        /// <returns>
        /// ログ行の配列</returns>
        public IReadOnlyList<string> GetLogLines()
        {
            var playerRefreshed = false;
            var partyRefreshed = false;

            // 最後のログから1min間が空いた？
            if ((DateTime.Now - this.lastLogineTimestamp).TotalMinutes >= 1.0d)
            {
                FF14PluginHelper.RefreshPlayer();
                playerRefreshed = true;

                RefreshPartyList();
                partyRefreshed = true;
            }

            if (logInfoQueue.IsEmpty)
            {
                return EMPTY_STRING_LIST;
            }

            var list = new List<string>(logInfoQueue.Count);
            var partyChanged = false;
            var jobChanged = false;
            var summoned = false;
            var zoneChanged = false;

            LogLineEventArgs logInfo;
            while (logInfoQueue.TryDequeue(out logInfo))
            {
                string logLine = logInfo.logLine.Trim();

                // FFXIVでの使用？
                if (!Settings.Default.UseOtherThanFFXIV)
                {
                    // ジョブに変化あり？
                    if (!jobChanged)
                    {
                        if (IsJobChanged(logLine))
                        {
                            jobChanged = true;
                            if (!playerRefreshed)
                            {
                                FF14PluginHelper.RefreshPlayer();
                                playerRefreshed = true;
                            }

                            if (!partyRefreshed)
                            {
                                RefreshPartyList();
                                partyRefreshed = true;
                            }
                        }
                    }

                    // パーティに変化あり
                    if (!partyChanged)
                    {
                        if (IsPartyChanged(logLine))
                        {
                            partyChanged = true;
                        }
                    }

                    if (!(summoned && zoneChanged))
                    {
                        // ペットIDのCacheを更新する
                        var player = FF14PluginHelper.GetPlayer();
                        if (player != null)
                        {
                            var jobName = Job.GetJobName(player.Job);
                            if (player.AsJob().IsSummoner())
                            {
                                if (logLine.Contains(player.Name + "の「サモン") ||
                                    logLine.Contains("You cast Summon"))
                                {
                                    summoned = true;
                                }

                                if (petIdCheckedZone != ActGlobals.oFormActMain.CurrentZone)
                                {
                                    zoneChanged = true;
                                }
                            }
                        }
                    }
                }

                list.Add(logLine);

                // ログファイルに出力する
                this.AppendLogFile(logLine);
            }

            if (partyChanged)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    RefreshPartyList();
                });
            }

            if (summoned)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    RefreshPetID();
                });
            }

            if (zoneChanged)
            {
                var oldSource = petIdRefreshTaskCancelTokenSource;
                if (oldSource != null)
                {
                    lock (oldSource)
                    {
                        if (!oldSource.IsCancellationRequested)
                        {
                            try
                            {
                                oldSource.Cancel();
                            }
                            catch { }
                        }
                    }
                }

                var newSource = petIdRefreshTaskCancelTokenSource = new CancellationTokenSource();
                var token = newSource.Token;
                var count = 0;

                Task.Run(async () =>
                {
                    while (petIdCheckedZone != ActGlobals.oFormActMain.CurrentZone)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(15));

                        RefreshPetID();
                        count++;

                        if (count >= 6)
                        {
                            return;
                        }
                    }
                }, token);
            }

            // ログのタイムスタンプを記録する
            this.lastLogineTimestamp = DateTime.Now;

            return list;
        }

        private static bool IsJobChanged(string logLine)
        {
            return logLine.Contains("にチェンジした。") || logLine.Contains("You change to ");
        }

        private static bool IsPartyChanged(string logLine)
        {
            return PARTY_CHANGED_WORDS.AsParallel()
                .Any(words => words.Any(word => logLine.Contains(word)));
        }

        #endregion

        #region private 生ログのファイル書き出し機能

        private bool SaveLogEnabled =>
            Settings.Default.SaveLogEnabled && !string.IsNullOrWhiteSpace(Settings.Default.SaveLogFile);

        /// <summary>
        /// ログを追記する
        /// </summary>
        /// <param name="logLine">追記するログ</param>
        private void AppendLogFile(string logLine)
        {
            if (SaveLogEnabled)
            {
                lock (this.fileOutputLogBuffer)
                {
                    this.fileOutputLogBuffer.AppendLine(logLine);

                    if (this.fileOutputLogBuffer.Length >= (5 * 1024))
                    {
                        this.FlushLogFile();
                    }
                }
            }
        }

        /// <summary>
        /// ログファイルをフラッシュする
        /// </summary>
        private void FlushLogFile()
        {
            if (SaveLogEnabled)
            {
                lock (this.fileOutputLogBuffer)
                {
                    if (this.fileOutputLogBuffer.Length >= (5 * 1024))
                    {
                        File.AppendAllText(
                            Settings.Default.SaveLogFile,
                            this.fileOutputLogBuffer.ToString(),
                            new UTF8Encoding(false));

                        this.fileOutputLogBuffer.Clear();
                    }
                }
            }
        }

        #endregion

        #region public static キーワードの代名詞を実際の値に置換

        /// <summary>
        /// マッチングキーワードを生成する
        /// </summary>
        /// <param name="keyword">元のキーワード</param>
        /// <returns>生成したキーワード</returns>
        public static string MakeKeyword(string keyword)
        {
            if (keyword == null)
            {
                return "";
            }

            keyword = keyword.Trim();

            // FFXIV以外での使用？
            if (Settings.Default.UseOtherThanFFXIV)
            {
                // カスタムプレースホルダを置換する
                keyword = ReplaceCustomPlaceholders(keyword);
                return keyword;
            }

            if (keyword.Length == 0 || !keyword.Contains("<") || !keyword.Contains(">"))
            {
                return keyword;
            }

            var player = FF14PluginHelper.GetPlayer();
            if (player != null)
            {
                keyword = keyword.Replace("<me>", player.Name.Trim());
            }

            if (enabledPartyMemberPlaceHolder && PartyList.Any())
            {
                foreach (var t in PartyList.Zip(PARTY_PLACEHOLDERS,
                    (name, placeholder) => Tuple.Create(placeholder, name)))
                {
                    keyword = keyword.Replace(t.Item1, t.Item2);
                }
            }

            if (!string.IsNullOrWhiteSpace(currentPetId))
            {
                keyword = keyword.Replace("<petid>", currentPetId);
            }

            // ジョブ名プレースホルダを置換する
            // ex. <PLD>, <PLD1> ...
            foreach (var replacement in PlaceholderToJobNameDictionaly)
            {
                keyword = keyword.Replace(replacement.Key, replacement.Value);
            }

            keyword = ReplaceCustomPlaceholders(keyword);

            return keyword;
        }

        #endregion

        #region public static 状態更新指示

        /// <summary>
        /// パーティリストを更新する
        /// </summary>
        public static void RefreshPartyList()
        {
            // プレイヤー情報を取得する
            var player = FF14PluginHelper.GetPlayer();
            if (player == null)
            {
                return;
            }

            if (enabledPartyMemberPlaceHolder)
            {
#if DEBUG
                Debug.WriteLine("PT: Refresh");
#endif
                // PTメンバの名前を記録しておく
                var combatants = FF14PluginHelper.GetCombatantListParty();

                // FF14内部のPTメンバ自動ソート順で並び替える
                var sorted =
                    from x in combatants
                    join y in Job.JobList on
                        x.Job equals y.JobId
                    where
                    x.ID != player.ID
                    orderby
                    y.Role,
                    x.Job,
                    x.ID descending
                    select
                    x.Name.Trim();

                partyList = new List<string>(sorted);

                // パーティメンバが空だったら自分を補完しておく
                if (!combatants.Any())
                {
                    combatants.Add(player);
                }

                var newList = new Dictionary<string, string>();

                // ジョブ名によるプレースホルダを登録する
                foreach (var job in Job.JobList)
                {
                    // このジョブに該当するパーティメンバを抽出する
                    var combatantsByJob = (
                        from x in combatants
                        where
                        x.Job == job.JobId
                        orderby
                        x.ID == player.ID ? 0 : 1,
                        x.ID descending
                        select
                        x).ToArray();

                    if (!combatantsByJob.Any())
                    {
                        continue;
                    }

                    // <JOBn>形式を置換する
                    // ex. <PLD1> → Taro Paladin
                    // ex. <PLD2> → Jiro Paladin
                    for (int i = 0; i < combatantsByJob.Length; i++)
                    {
                        var placeholder = string.Format(
                            "<{0}{1}>",
                            job.JobName,
                            i + 1);

                        newList.Add(placeholder.ToUpper(), combatantsByJob[i].Name);
                    }

                    // <JOB>形式を置換する
                    // ただし、この場合は正規表現のグループ形式とする
                    // また、グループ名にはジョブの略称を設定する
                    // ex. <PLD> → (?<PLDs>Taro Paladin|Jiro Paladin)
                    var names = string.Join("|", combatantsByJob.Select(x => x.Name).ToArray());
                    var oldValue = string.Format("<{0}>", job.JobName);
                    var newValue = string.Format(
                        "(?<{0}s>{1})",
                        job.JobName.ToUpper(),
                        names);

                    newList.Add(oldValue.ToUpper(), newValue);
                }

                placeholderToJobNameDictionaly = newList;
            }
            else
            {
                partyList = EMPTY_STRING_LIST;
                placeholderToJobNameDictionaly = EMPTY_STRING_PAIR_MAP;
            }

            // 置換後のマッチングキーワードを消去する
            SpellTimerTable.ClearReplacedKeywords();
            OnePointTelopTable.Default.ClearReplacedKeywords();

            // スペルタイマーの再描画を行う
            SpellTimerTable.ClearUpdateFlags();

            // モニタタブの情報を無効にする
            SpecialSpellTimerPlugin.ConfigPanel.InvalidatePlaceholders();
        }

        /// <summary>
        /// ペットIDを更新する
        /// </summary>
        public static void RefreshPetID()
        {
            lock (lockPetidObject)
            {
                if (petIdCheckedZone == ActGlobals.oFormActMain.CurrentZone)
                {
                    return;
                }

                // Combatantリストを取得する
                var combatants = FF14PluginHelper.GetCombatantList();

                if (combatants != null && combatants.Count > 0)
                {
                    var pet = (
                        from x in combatants
                        where
                        x.OwnerID == combatants[0].ID &&
                        (
                            x.Name.Contains("フェアリー・") ||
                            x.Name.Contains("・エギ") ||
                            x.Name.Contains("カーバンクル・")
                        )
                        select
                        x).FirstOrDefault();

                    if (pet != null)
                    {
                        currentPetId = Convert.ToString((long)((ulong)pet.ID), 16).ToUpper();
                        petIdCheckedZone = ActGlobals.oFormActMain.CurrentZone;

                        // 置換後のマッチングキーワードを消去する
                        SpellTimerTable.ClearReplacedKeywords();
                        OnePointTelopTable.Default.ClearReplacedKeywords();
                    }
                }
            }
        }

        #endregion

        #region public static カスタムプレースホルダー

        /// <summary>
        /// カスタム代名詞による置換文字列のセット
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> customPlaceholders =
            new ConcurrentDictionary<string, string>();

        /// <summary>
        /// カスタムプレースホルダを置換する
        /// ex. <C1>, <C2> <focus> <ターゲット>...
        /// </summary>
        private static string ReplaceCustomPlaceholders(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return keyword;
            }

            var ret = keyword;
            foreach (var p in customPlaceholders)
            {
                ret = ret.Replace(p.Key, p.Value);
            }

            return ret;
        }

        /// <summary>
        /// カスタム代名詞辞書のキーを作る
        /// </summary>
        private static string ToCustomPlaceholderKey(string name)
        {
            return "<" + name + ">";
        }

        /// <summary>
        /// カスタムプレースホルダーに追加する
        /// <param name="name">追加するプレースホルダーの名称</param>
        /// <param name="value">置換する文字列</param>
        /// </summary>
        public static void SetCustomPlaceholder(string name, string value)
        {
            customPlaceholders.AddOrUpdate(ToCustomPlaceholderKey(name), value, (key, oldValue) => value);

            // 置換後のマッチングキーワードを消去する
            SpellTimerTable.ClearReplacedKeywords();
            OnePointTelopTable.Default.ClearReplacedKeywords();
        }

        /// <summary>
        /// カスタムプレースホルダーを削除する
        /// <param name="name">削除するプレースホルダーの名称</param>
        /// </summary>
        public static void ClearCustomPlaceholder(string name)
        {
            string beforeValue;
            customPlaceholders.TryRemove(ToCustomPlaceholderKey(name), out beforeValue);

            // 置換後のマッチングキーワードを消去する
            SpellTimerTable.ClearReplacedKeywords();
            OnePointTelopTable.Default.ClearReplacedKeywords();
        }

        /// <summary>
        /// カスタムプレースホルダーを全て削除する
        /// </summary>
        public static void ClearCustomPlaceholderAll()
        {
            customPlaceholders.Clear();

            // 置換後のマッチングキーワードを消去する
            SpellTimerTable.ClearReplacedKeywords();
            OnePointTelopTable.Default.ClearReplacedKeywords();
        }

        #endregion
    }
}
