namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using ACT.SpecialSpellTimer.Properties;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// ログのバッファ
    /// </summary>
    public class LogBuffer : IDisposable
    {
        /// <summary>
        /// ペットID更新ロックオブジェクト
        /// </summary>
        private static object lockPetidObject = new object();

        /// <summary>
        /// パーティメンバの代名詞が有効か？
        /// </summary>
        private static bool enabledPartyMemberPlaceHolder = Settings.Default.EnabledPartyMemberPlaceholder;

        /// <summary>
        /// パーティメンバ
        /// </summary>
        private static List<string> ptmember;

        /// <summary>
        /// ジョブ代名詞による置換文字列セットのリスト
        /// </summary>
        private static List<KeyValuePair<string, string>> replacementsByJobs;

        /// <summary>
        /// カスタム代名詞による置換文字列のセット
        /// </summary>
        private static Dictionary<string, string> customPlaceholders = new Dictionary<string, string>();

        /// <summary>
        /// ペットのID
        /// </summary>
        private static string petid;

        /// <summary>
        /// ペットのIDを取得したゾーン
        /// </summary>
        private static string petidZone;

        /// <summary>
        /// 内部バッファ
        /// </summary>
        private List<string> buffer = new List<string>();

        /// <summary>
        /// 最後のログのタイムスタンプ
        /// </summary>
        private DateTime lastLogineTimestamp;

        /// <summary>
        /// ログファイル出力用のバッファ
        /// </summary>
        private StringBuilder logBuffer = new StringBuilder();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogBuffer()
        {
            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ActGlobals.oFormActMain.OnLogLineRead -= this.oFormActMain_OnLogLineRead;
            this.Clear();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// ログ行を返す
        /// </summary>
        /// <returns>
        /// ログ行の配列</returns>
        public string[] GetLogLines()
        {
            lock (this.buffer)
            {
                var logLines = this.buffer.ToArray();
                this.buffer.Clear();
                return logLines;
            }
        }

        /// <summary>
        /// バッファをクリアする
        /// </summary>
        public void Clear()
        {
            lock (this.buffer)
            {
                this.buffer.Clear();

                if (ptmember != null)
                {
                    ptmember.Clear();
                }

                // ログファイルをフラッシュする
                this.FlushLogFile();

#if DEBUG
                Debug.WriteLine("Logをクリアしました");
#endif
            }
        }

        /// <summary>
        /// ログを追記する
        /// </summary>
        /// <param name="logLine">追記するログ</param>
        public void AppendLogFile(
            string logLine)
        {
            if (Settings.Default.SaveLogEnabled &&
                !string.IsNullOrWhiteSpace(Settings.Default.SaveLogFile))
            {
                lock (this.logBuffer)
                {
                    this.logBuffer.AppendLine(logLine);

                    if (this.logBuffer.Length >= (5 * 1024))
                    {
                        this.FlushLogFile();
                    }
                }
            }
        }

        /// <summary>
        /// ログファイルをフラッシュする
        /// </summary>
        public void FlushLogFile()
        {
            if (Settings.Default.SaveLogEnabled &&
                !string.IsNullOrWhiteSpace(Settings.Default.SaveLogFile))
            {
                lock (this.logBuffer)
                {
                    File.AppendAllText(
                        Settings.Default.SaveLogFile,
                        this.logBuffer.ToString(),
                        new UTF8Encoding(false));

                    this.logBuffer.Clear();
                }
            }
        }

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

#if false
            Debug.WriteLine(logInfo.logLine);
#endif

            var logLine = logInfo.logLine.Trim();

            // 最後のログから1min間が空いた？
            if ((DateTime.Now - this.lastLogineTimestamp).TotalMinutes >= 1.0d)
            {
                FF14PluginHelper.RefreshPlayer();
                RefreshPTList();
            }

            // ジョブに変化あり？
            if (logLine.Contains("にチェンジした。") ||
                logLine.Contains("You change to "))
            {
                FF14PluginHelper.RefreshPlayer();
                RefreshPTList();
            }

            // パーティに変化あり？
            if (enabledPartyMemberPlaceHolder)
            {
                if (ptmember == null ||
                    replacementsByJobs == null ||
                    logLine.Contains("パーティを解散しました。") ||
                    logLine.Contains("がパーティに参加しました。") ||
                    logLine.Contains("がパーティから離脱しました。") ||
                    logLine.Contains("をパーティから離脱させました。") ||
                    logLine.Contains("の攻略を開始した。") ||
                    logLine.Contains("の攻略を終了した。") ||
                    (logLine.Contains("You join ") && logLine.Contains("'s party.")) ||
                    logLine.Contains("You left the party.") ||
                    logLine.Contains("You dissolve the party.") ||
                    logLine.Contains("The party has been disbanded.") ||
                    logLine.Contains("joins the party.") ||
                    logLine.Contains("has left the party.") ||
                    logLine.Contains("was removed from the party."))
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(5 * 1000);
                        RefreshPTList();
                    });
                }
            }

            // ペットIDのCacheを更新する
            var player = FF14PluginHelper.GetPlayer();
            if (player != null)
            {
                var jobName = Job.GetJobName(player.Job);
#if DEBUG
                Debug.WriteLine("JOB NAME!! " + jobName);
#endif
                if (player.AsJob().IsSummoner())
                {
                    if (logLine.Contains(player.Name + "の「サモン") ||
                        logLine.Contains("You cast Summon"))
                    {
                        Task.Run(() =>
                        {
                            Thread.Sleep(5 * 1000);
                            RefreshPetID();
                        });
                    }

                    if (petidZone != ActGlobals.oFormActMain.CurrentZone)
                    {
                        Task.Run(() =>
                        {
                            lock (lockPetidObject)
                            {
                                var count = 0;
                                while (petidZone != ActGlobals.oFormActMain.CurrentZone)
                                {
                                    Thread.Sleep(15 * 1000);
                                    RefreshPetID();
                                    count++;

                                    if (count >= 6)
                                    {
                                        petidZone = ActGlobals.oFormActMain.CurrentZone;
                                        break;
                                    }
                                }
                            }
                        });
                    }
                }
            }

            lock (this.buffer)
            {
                this.buffer.Add(logLine);

                // ログのタイムスタンプを記録する
                this.lastLogineTimestamp = DateTime.Now;
            }

            // ログファイルに出力する
            this.AppendLogFile(logLine);
        }

        /// <summary>
        /// マッチングキーワードを生成する
        /// </summary>
        /// <param name="keyword">元のキーワード</param>
        /// <returns>生成したキーワード</returns>
        public static string MakeKeyword(
            string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return keyword.Trim();
            }

            if (!keyword.Contains("<") ||
                !keyword.Contains(">"))
            {
                return keyword.Trim();
            }

            keyword = keyword.Trim();

            var player = FF14PluginHelper.GetPlayer();
            if (player != null)
            {
                keyword = keyword.Replace("<me>", player.Name.Trim());
            }

            if (enabledPartyMemberPlaceHolder)
            {
                if (ptmember != null)
                {
                    for (int i = 0; i < ptmember.Count; i++)
                    {
                        keyword = keyword.Replace(
                            "<" + (i + 2).ToString() + ">",
                            ptmember[i].Trim());
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(petid))
            {
                keyword = keyword.Replace("<petid>", petid);
            }

            // ジョブ名プレースホルダを置換する
            // ex. <PLD>, <PLD1> ...
            if (replacementsByJobs != null)
            {
                foreach (var replacement in replacementsByJobs)
                {
                    keyword = keyword.Replace(replacement.Key, replacement.Value);
                }
            }

            // カスタムプレースホルダを置換する
            // ex. <C1>, <C2> <focus> <ターゲット>...
            foreach (var p in customPlaceholders)
            {
                keyword = keyword.Replace("<" + p.Key + ">", p.Value);
            }

            return keyword;
        }

        /// <summary>
        /// PTメンバーリストを返す
        /// </summary>
        /// <returns>PTメンバーリスト</returns>
        public static string[] GetPTMember()
        {
            if (ptmember != null)
            {
                return ptmember.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// パーティリストを更新する
        /// </summary>
        public static void RefreshPTList()
        {
            if (ptmember == null)
            {
                ptmember = new List<string>();
            }
            else
            {
                ptmember.Clear();
            }

            if (replacementsByJobs == null)
            {
                replacementsByJobs = new List<KeyValuePair<string, string>>();
            }
            else
            {
                replacementsByJobs.Clear();
            }

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

                foreach (var name in sorted)
                {
                    ptmember.Add(name);
#if DEBUG
                    Debug.WriteLine("<-  " + name);
#endif
                }

                // パーティメンバが空だったら自分を補完しておく
                if (!combatants.Any())
                {
                    combatants.Add(player);
                }

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

                        replacementsByJobs.Add(new KeyValuePair<string, string>(placeholder.ToUpper(), combatantsByJob[i].Name));
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

                    replacementsByJobs.Add(new KeyValuePair<string, string>(oldValue.ToUpper(), newValue));
                }
            }

            // 置換後のマッチングキーワードを消去する
            SpellTimerTable.ClearReplacedKeywords();
            OnePointTelopTable.Default.ClearReplacedKeywords();

            // スペルタイマーの再描画を行う
            SpellTimerTable.ClearUpdateFlags();

            // モニタタブに出力する
            SpecialSpellTimerPlugin.ConfigPanel.RefreshPlaceholders(
                player.Name,
                ptmember,
                replacementsByJobs);
        }

        /// <summary>
        /// ペットIDを更新する
        /// </summary>
        public static void RefreshPetID()
        {
            // Combatantリストを取得する
            var combatant = FF14PluginHelper.GetCombatantList();

            if (combatant != null &&
                combatant.Count > 0)
            {
                var pet = (
                    from x in combatant
                    where
                    x.OwnerID == combatant[0].ID &&
                    (
                        x.Name.Contains("フェアリー・") ||
                        x.Name.Contains("・エギ") ||
                        x.Name.Contains("カーバンクル・")
                    )
                    select
                    x).FirstOrDefault();

                if (pet != null)
                {
                    petid = Convert.ToString((long)((ulong)pet.ID), 16).ToUpper();
                    petidZone = ActGlobals.oFormActMain.CurrentZone;

                    // 置換後のマッチングキーワードを消去する
                    SpellTimerTable.ClearReplacedKeywords();
                    OnePointTelopTable.Default.ClearReplacedKeywords();
                }
            }
        }

        /// <summary>
        /// カスタムプレースホルダーに追加する
        /// <param name="name">追加するプレースホルダーの名称</param>
        /// <param name="value">置換する文字列</param>
        /// </summary>
        public static void SetCustomPlaceholder(string name, string value)
        {
            customPlaceholders[name] = value;

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
            customPlaceholders.Remove(name);

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
    }
}
