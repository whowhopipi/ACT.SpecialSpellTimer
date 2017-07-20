using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ACT.SpecialSpellTimer.FFXIVHelper;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer.Models
{
    public class TableCompiler
    {
        #region Singleton

        private static TableCompiler instance;

        public static TableCompiler Instance => (instance ?? (instance = new TableCompiler()));

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
            var newList = new List<SpellTimer>();

            // 元のリストの複製を得る
            var sourceList = new List<SpellTimer>(SpellTimerTable.Table);

            var spells =
                from x in sourceList
                where
                x.Enabled
                orderby
                x.DisplayNo
                select
                x;

            var player = FFXIV.Instance.GetPlayer();
            var currentZoneID = FFXIV.Instance.GetCurrentZoneID();

            foreach (var spell in spells.AsParallel())
            {
                var enabledByJob = false;
                var enabledByZone = false;

                // ジョブフィルタをかける
                if (player == null ||
                    string.IsNullOrEmpty(spell.JobFilter))
                {
                    enabledByJob = true;
                }
                else
                {
                    var jobs = spell.JobFilter.Split(',');
                    if (jobs.Any(x => x == player.Job.ToString()))
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

                if (enabledByJob && enabledByZone)
                {
                    newList.Add(spell);
                }
            }

            // コンパイル済みの正規表現をセットする
            foreach (var spell in newList.AsParallel())
            {
                if (string.IsNullOrEmpty(spell.KeywordReplaced))
                {
                    spell.KeywordReplaced = string.IsNullOrEmpty(spell.Keyword)
                        ? string.Empty : LogBuffer.MakeKeyword(spell.Keyword);
                }

                if (string.IsNullOrEmpty(spell.KeywordForExtendReplaced1))
                {
                    spell.KeywordForExtendReplaced1 = string.IsNullOrEmpty(spell.KeywordForExtend1)
                        ? string.Empty : LogBuffer.MakeKeyword(spell.KeywordForExtend1);
                }

                if (string.IsNullOrEmpty(spell.KeywordForExtendReplaced2))
                {
                    spell.KeywordForExtendReplaced2 = string.IsNullOrEmpty(spell.KeywordForExtend2)
                        ? string.Empty : LogBuffer.MakeKeyword(spell.KeywordForExtend2);
                }

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

                // マッチングキーワードの正規表現を生成する
                var pattern = spell.KeywordReplaced.ToRegexPattern();
                if (!string.IsNullOrEmpty(pattern))
                {
                    if (spell.Regex == null ||
                        spell.RegexPattern != pattern)
                    {
                        spell.RegexPattern = pattern;
                        spell.Regex = pattern.ToRegex();
                    }
                }
                else
                {
                    spell.RegexPattern = string.Empty;
                    spell.Regex = null;
                }

                // 延長するためのマッチングキーワードの正規表現を生成する1
                pattern = spell.KeywordForExtendReplaced1.ToRegexPattern();
                if (!string.IsNullOrEmpty(pattern))
                {
                    if (spell.RegexForExtend1 == null ||
                        spell.RegexForExtendPattern1 != pattern)
                    {
                        spell.RegexForExtendPattern1 = pattern;
                        spell.RegexForExtend1 = pattern.ToRegex();
                    }
                }
                else
                {
                    spell.RegexForExtendPattern1 = string.Empty;
                    spell.RegexForExtend1 = null;
                }

                // 延長するためのマッチングキーワードの正規表現を生成する2
                pattern = spell.KeywordForExtendReplaced2.ToRegexPattern();
                if (!string.IsNullOrEmpty(pattern))
                {
                    if (spell.RegexForExtend2 == null ||
                        spell.RegexForExtendPattern2 != pattern)
                    {
                        spell.RegexForExtendPattern2 = pattern;
                        spell.RegexForExtend2 = pattern.ToRegex();
                    }
                }
                else
                {
                    spell.RegexForExtendPattern2 = string.Empty;
                    spell.RegexForExtend2 = null;
                }
            }

            lock (this.spellList)
            {
                this.spellList = newList;
            }
        }

        public void CompileTickers()
        {
            var newList = new List<OnePointTelop>();

            // 元のリストの複製を得る
            var sourceList = new List<OnePointTelop>(OnePointTelopTable.Default.Table);

            var spells =
                from x in sourceList
                where
                x.Enabled
                orderby
                x.MatchDateTime ascending
                select
                x;

            var player = FFXIV.Instance.GetPlayer();
            var currentZoneID = FFXIV.Instance.GetCurrentZoneID();

            foreach (var spell in spells.AsParallel())
            {
                var enabledByJob = false;
                var enabledByZone = false;

                // ジョブフィルタをかける
                if (player == null ||
                    string.IsNullOrWhiteSpace(spell.JobFilter))
                {
                    enabledByJob = true;
                }
                else
                {
                    var jobs = spell.JobFilter.Split(',');
                    if (jobs.Any(x => x == player.Job.ToString()))
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

                if (enabledByJob && enabledByZone)
                {
                    newList.Add(spell);
                }
            }

            // コンパイル済みの正規表現をセットする
            foreach (var spell in newList.AsParallel())
            {
                if (string.IsNullOrWhiteSpace(spell.KeywordReplaced))
                {
                    spell.KeywordReplaced = LogBuffer.MakeKeyword(spell.Keyword);
                }

                if (string.IsNullOrWhiteSpace(spell.KeywordToHideReplaced))
                {
                    spell.KeywordToHideReplaced = LogBuffer.MakeKeyword(spell.KeywordToHide);
                }

                if (!spell.RegexEnabled)
                {
                    spell.RegexPattern = string.Empty;
                    spell.Regex = null;
                    spell.RegexPatternToHide = string.Empty;
                    spell.RegexToHide = null;
                    continue;
                }

                // 表示用の正規表現を設定する
                var pattern = spell.KeywordReplaced.ToRegexPattern();
                if (!string.IsNullOrWhiteSpace(pattern))
                {
                    if (spell.Regex == null ||
                        spell.RegexPattern != pattern)
                    {
                        spell.RegexPattern = pattern;
                        spell.Regex = pattern.ToRegex();
                    }
                }
                else
                {
                    spell.RegexPattern = string.Empty;
                    spell.Regex = null;
                }

                // 非表示用の正規表現を設定する
                var patternToHide = spell.KeywordToHideReplaced.ToRegexPattern();
                if (!string.IsNullOrWhiteSpace(patternToHide))
                {
                    if (spell.RegexToHide == null ||
                        spell.RegexPatternToHide != patternToHide)
                    {
                        spell.RegexPatternToHide = patternToHide;
                        spell.RegexToHide = patternToHide.ToRegex();
                    }
                }
                else
                {
                    spell.RegexPatternToHide = string.Empty;
                    spell.RegexToHide = null;
                }
            }

            lock (this.tickerListLocker)
            {
                this.tickerList = newList;
            }
        }

        private void DoWork()
        {
            while (this.workerRunning)
            {
                try
                {
                    this.CompileSpells();
                    this.CompileTickers();
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
    }
}