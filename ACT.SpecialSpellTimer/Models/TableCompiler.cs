using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer.Models
{
    public class TableCompiler
    {
        #region Singleton

        private static TableCompiler instance = new TableCompiler();

        public static TableCompiler Instance => instance;

        #endregion Singleton

        #region Worker

        private const int WorkerInterval = 5;
        private Thread worker;

        private bool workerRunning;

        #endregion Worker

        #region Begin / End

        public void Begin()
        {
            this.workerRunning = true;

            this.worker = new Thread(this.DoWork);
            this.worker.Start();
        }

        public void End()
        {
            this.workerRunning = false;

            if (this.worker != null)
            {
                this.worker.Join(TimeSpan.FromSeconds(WorkerInterval * 2));
                if (this.worker.IsAlive)
                {
                    this.worker.Abort();
                }

                this.worker = null;
            }
        }

        #endregion Begin / End

        private List<Combatant> partyList = new List<Combatant>();
        private Combatant player = new Combatant();

        private List<SpellTimer> spellList = new List<SpellTimer>();
        private object spellListLocker = new object();
        private List<OnePointTelop> tickerList = new List<OnePointTelop>();
        private object tickerListLocker = new object();

        public List<SpellTimer> SpellList
        {
            get
            {
                lock (this.spellListLocker)
                {
                    return new List<SpellTimer>(this.spellList);
                }
            }
        }

        public List<OnePointTelop> TickerList
        {
            get
            {
                lock (this.tickerListLocker)
                {
                    return new List<OnePointTelop>(this.tickerList);
                }
            }
        }

        public void AddInstanceSpell(
            SpellTimer instancedSpell)
        {
            lock (this.spellListLocker)
            {
                this.spellList.Add(instancedSpell);
            }
        }

        public void CompileSpells()
        {
            var currentZoneID = FFXIV.Instance.GetCurrentZoneID();

            bool filter(SpellTimer spell)
            {
                var enabledByJob = false;
                var enabledByZone = false;

                // ジョブフィルタをかける
                if (this.player == null ||
                    string.IsNullOrEmpty(spell.JobFilter))
                {
                    enabledByJob = true;
                }
                else
                {
                    var jobs = spell.JobFilter.Split(',');
                    if (jobs.Any(x => x == this.player.Job.ToString()))
                    {
                        enabledByJob = true;
                    }
                }

                // ゾーンフィルタをかける
                if (currentZoneID == 0 ||
                    string.IsNullOrEmpty(spell.ZoneFilter))
                {
                    enabledByZone = true;
                }
                else
                {
                    var zoneIDs = spell.ZoneFilter.Split(',');
                    if (zoneIDs.Any(x => x == currentZoneID.ToString()))
                    {
                        enabledByZone = true;
                    }
                }

                return enabledByJob && enabledByZone;
            }

            // 元のリストの複製を得る
            var sourceList = new List<SpellTimer>(SpellTimerTable.Table);

            var q =
                from x in sourceList
                where
                x.Enabled &&
                filter(x)
                orderby
                x.DisplayNo
                select
                x;

            // コンパイル済みの正規表現をセットする
            foreach (var spell in q.AsParallel())
            {
                spell.KeywordReplaced = this.GetMatchingKeyword(spell.KeywordReplaced, spell.Keyword);
                spell.KeywordForExtendReplaced1 = this.GetMatchingKeyword(spell.KeywordForExtendReplaced1, spell.KeywordForExtend1);
                spell.KeywordForExtendReplaced2 = this.GetMatchingKeyword(spell.KeywordForExtendReplaced2, spell.KeywordForExtend2);

                if (!spell.RegexEnabled)
                {
                    spell.RegexPattern = string.Empty;
                    spell.Regex = null;
                    spell.RegexForExtendPattern1 = string.Empty;
                    spell.RegexForExtend1 = null;
                    spell.RegexForExtendPattern2 = string.Empty;
                    spell.RegexForExtend2 = null;
                    continue;
                }

                var r1 = this.GetRegex(spell.Regex, spell.RegexPattern, spell.KeywordReplaced);
                var r2 = this.GetRegex(spell.RegexForExtend1, spell.RegexForExtendPattern1, spell.KeywordForExtendReplaced1);
                var r3 = this.GetRegex(spell.RegexForExtend2, spell.RegexForExtendPattern2, spell.KeywordForExtendReplaced2);

                spell.Regex = r1.newRegex;
                spell.RegexPattern = r1.newPattern;
                spell.RegexForExtend1 = r2.newRegex;
                spell.RegexForExtendPattern1 = r2.newPattern;
                spell.RegexForExtend2 = r3.newRegex;
                spell.KeywordForExtendReplaced2 = r3.newPattern;
            }

            lock (this.spellListLocker)
            {
                this.spellList = new List<SpellTimer>(q);
            }
        }

        public void CompileTickers()
        {
            var currentZoneID = FFXIV.Instance.GetCurrentZoneID();

            bool filter(OnePointTelop spell)
            {
                var enabledByJob = false;
                var enabledByZone = false;

                // ジョブフィルタをかける
                if (this.player == null ||
                    string.IsNullOrWhiteSpace(spell.JobFilter))
                {
                    enabledByJob = true;
                }
                else
                {
                    var jobs = spell.JobFilter.Split(',');
                    if (jobs.Any(x => x == this.player.Job.ToString()))
                    {
                        enabledByJob = true;
                    }
                }

                // ゾーンフィルタをかける
                if (currentZoneID == 0 ||
                    string.IsNullOrWhiteSpace(spell.ZoneFilter))
                {
                    enabledByZone = true;
                }
                else
                {
                    var zoneIDs = spell.ZoneFilter.Split(',');
                    if (zoneIDs.Any(x => x == currentZoneID.ToString()))
                    {
                        enabledByZone = true;
                    }
                }

                return enabledByJob && enabledByZone;
            }

            // 元のリストの複製を得る
            var sourceList = new List<OnePointTelop>(OnePointTelopTable.Default.Table);

            var q =
                from x in sourceList
                where
                x.Enabled &&
                filter(x)
                orderby
                x.MatchDateTime ascending
                select
                x;

            // コンパイル済みの正規表現をセットする
            foreach (var spell in q.AsParallel())
            {
                spell.KeywordReplaced = this.GetMatchingKeyword(spell.KeywordReplaced, spell.Keyword);
                spell.KeywordToHideReplaced = this.GetMatchingKeyword(spell.KeywordToHideReplaced, spell.KeywordToHide);

                if (!spell.RegexEnabled)
                {
                    spell.RegexPattern = string.Empty;
                    spell.Regex = null;
                    spell.RegexPatternToHide = string.Empty;
                    spell.RegexToHide = null;
                    continue;
                }

                var r1 = this.GetRegex(spell.Regex, spell.RegexPattern, spell.KeywordReplaced);
                var r2 = this.GetRegex(spell.RegexToHide, spell.RegexPatternToHide, spell.KeywordToHideReplaced);

                spell.Regex = r1.newRegex;
                spell.RegexPattern = r1.newPattern;
                spell.RegexToHide = r2.newRegex;
                spell.RegexPatternToHide = r2.newPattern;
            }

            lock (this.tickerListLocker)
            {
                this.tickerList = new List<OnePointTelop>(q);
            }
        }

        public void RecompileSpells()
        {
            var rawTable = new List<SpellTimer>(SpellTimerTable.Table);
            foreach (var spell in rawTable.AsParallel())
            {
                spell.KeywordReplaced = string.Empty;
                spell.KeywordForExtendReplaced1 = string.Empty;
                spell.KeywordForExtendReplaced2 = string.Empty;
            }

            this.CompileSpells();

            // スペルタイマの描画済みフラグを落とす
            SpellTimerTable.ClearUpdateFlags();
        }

        public void RecompileTickers()
        {
            var rawTable = new List<OnePointTelop>(OnePointTelopTable.Default.Table);
            foreach (var spell in rawTable.AsParallel())
            {
                spell.KeywordReplaced = string.Empty;
                spell.KeywordToHideReplaced = string.Empty;
            }

            this.CompileTickers();
        }

        private void DoWork()
        {
            while (this.workerRunning)
            {
                try
                {
                    this.RefreshCombatants();

                    var isPlayerChanged = this.IsPlayerChanged();
                    var isPartyChanged = this.IsPartyChanged();
                    var isZoneChanged = this.IsZoneChanged();

                    if (isPlayerChanged)
                    {
                        this.RefreshPlayerPlacceholder();
                    }

                    if (isPartyChanged)
                    {
                        this.RefreshPartyPlaceholders();
                        this.RefreshPetPlaceholder();
                    }

                    if (isZoneChanged)
                    {
                        this.RefreshPetPlaceholder();
                    }

                    if (isPlayerChanged ||
                        isPartyChanged ||
                        isZoneChanged)
                    {
                        this.CompileSpells();
                        this.CompileTickers();
                    }
                }
                catch (ThreadAbortException)
                {
                    this.workerRunning = false;
                }
                catch (Exception ex)
                {
                    Logger.Write("table compiler error:", ex);
                }

                Thread.Sleep(TimeSpan.FromSeconds(WorkerInterval));
            }
        }

        private string GetMatchingKeyword(
            string destinationKeyword,
            string sourceKeyword,
            bool forceUpdate = false)
        {
            if (string.IsNullOrEmpty(sourceKeyword))
            {
                return string.Empty;
            }

            if (!sourceKeyword.Contains("<") ||
                !sourceKeyword.Contains(">"))
            {
                return sourceKeyword;
            }

            var placeholders = this.PlaceholderList;

            string replace(string text)
            {
                var r = text;

                foreach (var p in placeholders)
                {
                    r = r.Replace(p.Placeholder, p.ReplaceString);
                }

                return r;
            }

            if (forceUpdate ||
                string.IsNullOrEmpty(destinationKeyword))
            {
                var newKeyword = string.Empty;
                newKeyword = replace(sourceKeyword);
                newKeyword = DQXUtility.MakeKeyword(sourceKeyword);

                return newKeyword;
            }

            return destinationKeyword;
        }

        private (Regex newRegex, string newPattern) GetRegex(
            Regex destinationRegex,
            string destinationPattern,
            string sourcePattern)
        {
            (Regex newRegex, string newPattern) r = (destinationRegex, destinationPattern);

            if (!string.IsNullOrEmpty(sourcePattern))
            {
                var newPattern = sourcePattern.ToRegexPattern();

                if (destinationRegex == null ||
                    destinationPattern != newPattern)
                {
                    r.newRegex = newPattern.ToRegex();
                    r.newPattern = newPattern;
                }
            }

            return r;
        }

        private void RefreshCombatants()
        {
            var player = FFXIV.Instance.GetPlayer();
            if (player != null)
            {
                this.player = player;
            }

            var party = FFXIV.Instance.GetPartyList();
            if (party != null)
            {
                var newList = new List<Combatant>(party);

                if (newList.Count < 1 &&
                    !string.IsNullOrEmpty(this.player.Name))
                {
                    newList.Add(this.player);
                }

                this.partyList = newList;
            }
        }

        #region 条件の変更を判定するメソッド群

        private IReadOnlyList<Combatant> previousParty = new List<Combatant>();
        private Combatant previousPlayer = new Combatant();
        private string previousZoneName = string.Empty;

        public bool IsPartyChanged()
        {
            var r = false;

            var party = this.partyList
                .Where(x => x.MobType == MobType.Player)
                .ToList();

            if (this.previousParty.Count !=
                party.Count)
            {
                r = true;
            }
            else
            {
                // 前のパーティと名前が一致するか検証する
                var count = party
                    .Where(x =>
                        this.previousParty.Any(y =>
                            y.Name == x.Name))
                    .Count();

                if (party.Count != count)
                {
                    r = true;
                }
            }

            this.previousParty = party;

            return r;
        }

        public bool IsPlayerChanged()
        {
            var r = false;

            if (this.previousPlayer.Name != this.player.Name ||
                this.previousPlayer.Job != this.player.Job)
            {
                r = true;
            }

            this.previousPlayer = this.player;

            return r;
        }

        public bool IsZoneChanged()
        {
            var r = false;

            var zoneName = ActGlobals.oFormActMain.CurrentZone;
            if (zoneName != null)
            {
                if (this.previousZoneName != zoneName)
                {
                    r = true;
                }

                this.previousZoneName = zoneName;
            }

            return r;
        }

        #endregion 条件の変更を判定するメソッド群

        #region プレースホルダに関するメソッド群

        private List<(string Placeholder, string ReplaceString, PlaceholderTypes Type)> placeholderList =
            new List<(string Placeholder, string ReplaceString, PlaceholderTypes Type)>();

        public IReadOnlyList<(string Placeholder, string ReplaceString, PlaceholderTypes Type)> PlaceholderList
        {
            get
            {
                lock (this.PlaceholderListSyncRoot)
                {
                    return
                        new List<(string Placeholder, string ReplaceString, PlaceholderTypes Type)>(
                            this.placeholderList);
                }
            }
        }

        private object PlaceholderListSyncRoot =>
            ((ICollection)this.placeholderList)?.SyncRoot;

        public void RefreshPartyPlaceholders()
        {
            if (!Settings.Default.EnabledPartyMemberPlaceholder)
            {
                return;
            }

            var newList =
                new List<(string Placeholder, string ReplaceString, PlaceholderTypes Type)>();

            // FF14内部のPTメンバ自動ソート順で並び替える
            var partyListSorted =
                from x in this.partyList
                join y in Job.Instance.JobList on
                    x.Job equals y.JobId
                where
                x.ID != this.player.ID
                orderby
                y.Role,
                x.Job,
                x.ID descending
                select
                x;

            // 通常のPTメンバ代名詞 <2>～<8> を登録する
            var index = 2;
            foreach (var combatant in partyListSorted)
            {
                newList.Add((
                    $"<{index}>",
                    combatant.Name,
                    PlaceholderTypes.Party));

                index++;
            }

            // ジョブ名によるプレースホルダを登録する
            foreach (var job in Job.Instance.JobList)
            {
                // このジョブに該当するパーティメンバを抽出する
                var combatantsByJob = (
                    from x in this.partyList
                    where
                    x.Job == job.JobId
                    orderby
                    x.ID == this.player.ID ? 0 : 1,
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

                    newList.Add((
                        placeholder.ToUpper(),
                        combatantsByJob[i].Name,
                        PlaceholderTypes.Party));
                }

                // <JOB>形式を置換する ただし、この場合は正規表現のグループ形式とする
                // また、グループ名にはジョブの略称を設定する
                // ex. <PLD> → (?<PLDs>Taro Paladin|Jiro Paladin)
                var names = string.Join("|", combatantsByJob.Select(x => x.Name).ToArray());
                var oldValue = $"<{job.JobName}>";
                var newValue = $"(?<{job.JobName.ToUpper()}s>{names})";

                newList.Add((
                    oldValue.ToUpper(),
                    newValue,
                    PlaceholderTypes.Party));
            }

            // ロールによるプレースホルダを登録する
            // ex. <TANK>   -> (?<TANKs>Taro Paladin|Jiro Paladin)
            // ex. <HEALER> -> (?<HEALERs>Taro Paladin|Jiro Paladin)
            // ex. <DPS>    -> (?<DPSs>Taro Paladin|Jiro Paladin)
            // ex. <MELEE>  -> (?<MELEEs>Taro Paladin|Jiro Paladin)
            // ex. <RANGE>  -> (?<RANGEs>Taro Paladin|Jiro Paladin)
            // ex. <MAGIC>  -> (?<MAGICs>Taro Paladin|Jiro Paladin)
            var partyListByRole = FFXIV.Instance.GetPatryListByRole();
            foreach (var role in partyListByRole)
            {
                var names = string.Join("|", role.Combatants.Select(x => x.Name).ToArray());
                var oldValue = $"<{role.RoleLabel}>";
                var newValue = $"(?<{role.RoleLabel}s>{names})";

                newList.Add((
                    oldValue.ToUpper(),
                    newValue,
                    PlaceholderTypes.Party));
            }

            lock (this.PlaceholderListSyncRoot)
            {
                this.placeholderList.RemoveAll(x => x.Type == PlaceholderTypes.Party);
                this.placeholderList.AddRange(newList);
            }
        }

        public void RefreshPetPlaceholder()
        {
            if (!Settings.Default.EnabledPartyMemberPlaceholder)
            {
                return;
            }

            var playerJob = this.player.AsJob();
            if (playerJob != null ||
                !playerJob.IsSummoner())
            {
                return;
            }

            void refreshPetID()
            {
                // 3秒毎に30秒間判定させる
                const int Interval = 3;
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var combatants = FFXIV.Instance.GetCombatantList();
                        if (combatants == null)
                        {
                            continue;
                        }

                        var pet = (
                            from x in combatants
                            where
                            x.OwnerID == this.player.ID &&
                            (
                                x.Name.Contains("フェアリー・") ||
                                x.Name.Contains("・エギ") ||
                                x.Name.Contains("カーバンクル・")
                            )
                            select
                            x).FirstOrDefault();

                        if (pet != null)
                        {
                            lock (this.PlaceholderListSyncRoot)
                            {
                                this.placeholderList.RemoveAll(x => x.Type == PlaceholderTypes.Pet);
                                this.placeholderList.Add((
                                    "<petid>",
                                    Convert.ToString((long)((ulong)pet.ID), 16).ToUpper(),
                                    PlaceholderTypes.Pet));
                            }

                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(Interval));
                }
            }

            Task.Run(() => refreshPetID())
                .ContinueWith((task) =>
                {
                    this.RecompileSpells();
                    this.RecompileTickers();
                });
        }

        public void RefreshPlayerPlacceholder()
        {
            if (string.IsNullOrEmpty(this.player.Name))
            {
                return;
            }

            lock (this.PlaceholderListSyncRoot)
            {
                this.placeholderList.RemoveAll(x => x.Type == PlaceholderTypes.Me);
                this.placeholderList.Add((
                    "<me>",
                    this.player.Name,
                    PlaceholderTypes.Me));
            }
        }

        #endregion プレースホルダに関するメソッド群

        #region カスタムプレースホルダに関するメソッド群

        /// <summary>
        /// カスタムプレースホルダーを削除する
        /// <param name="name">削除するプレースホルダーの名称</param>
        /// </summary>
        public void ClearCustomPlaceholder(string name)
        {
            lock (this.PlaceholderListSyncRoot)
            {
                this.placeholderList.RemoveAll(x =>
                    x.Placeholder == $"<{name}>" &&
                    x.Type == PlaceholderTypes.Custom);
            }

            this.RecompileSpells();
            this.RecompileTickers();
        }

        /// <summary>
        /// カスタムプレースホルダーを全て削除する
        /// </summary>
        public void ClearCustomPlaceholderAll()
        {
            lock (this.PlaceholderListSyncRoot)
            {
                this.placeholderList.RemoveAll(x =>
                    x.Type == PlaceholderTypes.Custom);
            }

            this.RecompileSpells();
            this.RecompileTickers();
        }

        /// <summary>
        /// カスタムプレースホルダーに追加する
        /// </summary>
        /// <param name="name">追加するプレースホルダーの名称</param>
        /// <paramname="value">置換する文字列</param>
        public void SetCustomPlaceholder(string name, string value)
        {
            lock (this.PlaceholderListSyncRoot)
            {
                this.placeholderList.Add((
                    $"<{name}>",
                    value,
                    PlaceholderTypes.Custom));
            }

            this.RecompileSpells();
            this.RecompileTickers();
        }

        #endregion カスタムプレースホルダに関するメソッド群
    }
}
