namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using ACT.SpecialSpellTimer.Config;
    using ACT.SpecialSpellTimer.FFXIVHelper;
    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// ログのバッファ
    /// </summary>
    public class LogBuffer : IDisposable
    {
        #region private static fields

        /// <summary>
        /// 空の文字列リスト
        /// </summary>
        private static readonly IReadOnlyList<string> EMPTY_STRING_LIST =
            new List<string>();

        /// <summary>
        /// 空の(文字列 -&gt; 文字列)マップ
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> EMPTY_STRING_PAIR_MAP =
            new Dictionary<string, string>();

        /// <summary>
        /// ペットID更新ロックオブジェクト
        /// </summary>
        private static readonly object lockPetidObject = new object();

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

        /// <summary> パーティ名プレースホルダーのキャッシュ "<2>" ～ "<8>" </summary>
        private static readonly IReadOnlyList<string> PARTY_PLACEHOLDERS =
            Enumerable.Range(2, 7).Select(ordinal => "<" + ordinal + ">").ToList();

        /// <summary>
        /// ツールチップ文字除去するための正規表現
        /// </summary>
        private static readonly Regex TooltipCharsRegex =
            new Regex(@".\u0001\u0001\uFFFD", RegexOptions.Compiled);

        /// <summary>
        /// ペットのID
        /// </summary>
        private static volatile string currentPetId = string.Empty;

        /// <summary>
        /// 現在のパーティメンバー名リストのキャッシュ
        /// </summary>
        private static volatile IReadOnlyList<string> partyList = EMPTY_STRING_LIST;

        /// <summary>
        /// ペットのIDを取得したゾーン
        /// </summary>
        private static volatile string petIdCheckedZone = string.Empty;

        /// <summary>
        /// ジョブ名プレースホルダー -&gt; プレイヤー名
        /// </summary>
        private static volatile IReadOnlyDictionary<string, string> placeholderToJobNameDictionaly = EMPTY_STRING_PAIR_MAP;

        /// <summary>
        /// パーティメンバの代名詞が有効か？
        /// </summary>
        private static bool EnabledPartyMemberPlaceHolder => Settings.Default.EnabledPartyMemberPlaceholder;

        #endregion private static fields

        #region public static properties

        /// <summary>
        /// パーティメンバー名
        /// </summary>
        public static IReadOnlyList<string> PartyList => partyList;

        /// <summary>
        /// ジョブ代名詞による置換文字列セットのリスト
        /// </summary>
        public static IReadOnlyDictionary<string, string> PlaceholderToJobNameDictionaly => placeholderToJobNameDictionaly;

        #endregion public static properties

        #region private fields

        /// <summary>
        /// ログファイル出力用のバッファ
        /// </summary>
        private readonly StringBuilder fileOutputLogBuffer = new StringBuilder();

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

        /// <summary>
        /// 現在走っているゾーン/ペットID情報更新タスクのキャンセルトークンソース
        /// </summary>
        private volatile CancellationTokenSource petIdRefreshTaskCancelTokenSource;

        #endregion private fields

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

            LogLineEventArgs ignore;
            while (logInfoQueue.TryDequeue(out ignore)) ;

            this.FlushLogFile();

            partyList = EMPTY_STRING_LIST;
            placeholderToJobNameDictionaly = EMPTY_STRING_PAIR_MAP;
            currentPetId = string.Empty;
            petIdCheckedZone = string.Empty;

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
            if (isImport)
            {
                return;
            }

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
#if !DEBUG
            if (isImport)
            {
                return;
            }
#endif
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

        /// <summary>
        /// ログ行を返す
        /// </summary>
        /// <returns>ログ行の配列</returns>
        public IReadOnlyList<string> GetLogLines()
        {
            var partyRefreshed = false;

            if (logInfoQueue.IsEmpty)
            {
                return EMPTY_STRING_LIST;
            }

            var list = new List<string>(logInfoQueue.Count);
            var partyChanged = false;
            var jobChanged = false;
            var summoned = false;
            var zoneChanged = false;
            var partyChangedAtDQX = false;

            LogLineEventArgs logInfo;
            while (logInfoQueue.TryDequeue(out logInfo))
            {
                var logLine = logInfo.logLine.Trim();

                // エフェクトに付与されるツールチップ文字を除去する
                if (Settings.Default.RemoveTooltipSymbols)
                {
                    logLine = TooltipCharsRegex.Replace(logLine, string.Empty);
                }

                // FFXIVでの使用？
                if (!Settings.Default.UseOtherThanFFXIV)
                {
                    // ジョブに変化あり？
                    if (!jobChanged)
                    {
                        if (IsJobChanged(logLine))
                        {
                            jobChanged = true;
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
                        var player = FFXIV.Instance.GetPlayer();
                        if (player != null)
                        {
                            var job = player.AsJob();
                            if (job != null)
                            {
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
                }

                // パーティに変化があるか？（対DQX）
                var r = DQXUtility.IsPartyChanged(logLine);
                if (!partyChangedAtDQX)
                {
                    partyChangedAtDQX = r;
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

            if (partyChangedAtDQX)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    DQXUtility.RefeshKeywords();
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

        /// <summary>
        /// バッファーにログがない場合 true
        /// </summary>
        /// <returns>バッファーにログがない場合 true</returns>
        public bool NonEmpty()
        {
            return !logInfoQueue.IsEmpty;
        }

        /// <summary>
        /// ジョブが変更されたか？
        /// </summary>
        /// <param name="logLine">ログ行</param>
        /// <returns>bool</returns>
        private static bool IsJobChanged(string logLine)
        {
            return
                logLine.Contains("にチェンジした。") ||
                logLine.Contains("You change to ");
        }

        /// <summary>
        /// パーティが変更されたか？
        /// </summary>
        /// <param name="logLine">ログ行</param>
        /// <returns>bool</returns>
        private static bool IsPartyChanged(string logLine)
        {
            return PARTY_CHANGED_WORDS.AsParallel()
                .Any(words => words.Any(word => logLine.Contains(word)));
        }

        #endregion ログ処理

        #region private 生ログのファイル書き出し機能

        private bool SaveLogEnabled =>
            Settings.Default.SaveLogEnabled &&
            !string.IsNullOrWhiteSpace(Settings.Default.SaveLogFile);

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

        #endregion private 生ログのファイル書き出し機能

        #region public static キーワードの代名詞を実際の値に置換

        /// <summary>
        /// マッチングキーワードを生成する
        /// </summary>
        /// <param name="keyword">元のキーワード</param>
        /// <returns>生成したキーワード</returns>
        public static string MakeKeyword(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return string.Empty;
            }

            keyword = keyword.Trim();

            if (keyword.Length == 0 ||
                !keyword.Contains("<") ||
                !keyword.Contains(">"))
            {
                return keyword;
            }

            // カスタムプレスホルダを置換する
            keyword = ReplaceCustomPlaceholders(keyword);

            // 対DQX用のキーワードを置換する
            keyword = DQXUtility.MakeKeyword(keyword);

            // FFXIV以外での使用？
            if (Settings.Default.UseOtherThanFFXIV)
            {
                return keyword;
            }

            var player = FFXIV.Instance.GetPlayer();
            if (player != null)
            {
                keyword = keyword.Replace("<me>", player.Name.Trim());
            }

            if (EnabledPartyMemberPlaceHolder && PartyList.Any())
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

            return keyword;
        }

        #endregion public static キーワードの代名詞を実際の値に置換

        #region public static 状態更新指示

        /// <summary>
        /// パーティリストを更新する
        /// </summary>
        public static void RefreshPartyList()
        {
            // プレイヤー情報を取得する
            var player = FFXIV.Instance.GetPlayer();
            if (player == null)
            {
                return;
            }

            if (EnabledPartyMemberPlaceHolder)
            {
#if DEBUG
                Debug.WriteLine("PT: Refresh");
#endif
                // PTメンバの名前を記録しておく
                var combatants = new List<Combatant>(FFXIV.Instance.GetPartyList());

                // FF14内部のPTメンバ自動ソート順で並び替える
                var sorted =
                    from x in combatants
                    join y in Job.Instance.JobList on
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
                foreach (var job in Job.Instance.JobList)
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

                    // <JOB>形式を置換する ただし、この場合は正規表現のグループ形式とする
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
                var combatants = FFXIV.Instance.GetCombatantList();

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

        #endregion public static 状態更新指示

        #region public static カスタムプレースホルダー

        /// <summary>
        /// カスタム代名詞による置換文字列のセット
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> customPlaceholders =
            new ConcurrentDictionary<string, string>();

        /// <summary>
        /// カスタムプレースホルダーを削除する
        /// <param name="name">削除するプレースホルダーの名称</param>
        /// </summary>
        public static void ClearCustomPlaceholder(string name)
        {
            string beforeValue;
            customPlaceholders.TryRemove(
                ToCustomPlaceholderKey(name),
                out beforeValue);

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

        /// <summary>
        /// カスタムプレースホルダーに追加する
        /// </summary>
        /// <param name="name">追加するプレースホルダーの名称</param>
        /// <paramname="value">置換する文字列</param>
        public static void SetCustomPlaceholder(string name, string value)
        {
            customPlaceholders.AddOrUpdate(
                ToCustomPlaceholderKey(name),
                value,
                (key, oldValue) => value);

            // 置換後のマッチングキーワードを消去する
            SpellTimerTable.ClearReplacedKeywords();
            OnePointTelopTable.Default.ClearReplacedKeywords();
        }

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

        #endregion public static カスタムプレースホルダー
    }
}