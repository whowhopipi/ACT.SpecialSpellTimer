using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Utility;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// SpellTimerテーブル
    /// </summary>
    public class SpellTimerTable
    {
        #region Singleton

        private static SpellTimerTable instance = new SpellTimerTable();
        public static SpellTimerTable Instance => instance;

        #endregion Singleton

        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        private volatile List<SpellTimer> table = new List<SpellTimer>();

        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public string DefaultFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.Spells.xml");

        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        public List<SpellTimer> Table => this.table;

        /// <summary>
        /// カウントをリセットする
        /// </summary>
        public static void ResetCount()
        {
            foreach (var row in TableCompiler.Instance.SpellList)
            {
                row.MatchDateTime = DateTime.MinValue;
                row.UpdateDone = false;
                row.OverDone = false;
                row.BeforeDone = false;
                row.TimeupDone = false;
                row.CompleteScheduledTime = DateTime.MinValue;

                row.StartOverSoundTimer();
                row.StartBeforeSoundTimer();
                row.StartTimeupSoundTimer();
                row.StartGarbageInstanceTimer();
            }
        }

        /// <summary>
        /// テーブルファイルをバックアップする
        /// </summary>
        public void Backup()
        {
            var file = this.DefaultFile;

            if (File.Exists(file))
            {
                var backupFile = Path.Combine(
                    Path.Combine(Path.GetDirectoryName(file), "backup"),
                    Path.GetFileNameWithoutExtension(file) + "." + DateTime.Now.ToString("yyyy-MM-dd") + ".bak");

                if (!Directory.Exists(Path.GetDirectoryName(backupFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(backupFile));
                }

                File.Copy(
                    file,
                    backupFile,
                    true);

                // 古いバックアップを消す
                foreach (var bak in
                    Directory.GetFiles(Path.GetDirectoryName(backupFile), "*.bak"))
                {
                    var timeStamp = File.GetCreationTime(bak);
                    if ((DateTime.Now - timeStamp).TotalDays >= 3.0d)
                    {
                        File.Delete(bak);
                    }
                }
            }
        }

        /// <summary>
        /// スペルの描画済みフラグをクリアする
        /// </summary>
        public void ClearUpdateFlags()
        {
            foreach (var item in this.table)
            {
                item.UpdateDone = false;
            }
        }

        /// <summary>
        /// 指定されたGuidを持つSpellTimerを取得する
        /// </summary>
        /// <param name="guid">Guid</param>
        public SpellTimer GetSpellTimerByGuid(
            Guid guid)
        {
            return this.table
                .AsParallel()
                .Where(x => x.Guid == guid)
                .FirstOrDefault();
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        public void Load()
        {
            this.Load(this.DefaultFile, true);
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        /// <param name="file">ファイルパス</param>
        /// <param name="isClear">消去してからロードする？</param>
        public void Load(
            string file,
            bool isClear)
        {
            if (File.Exists(file))
            {
#if false
                // 旧フォーマットを置換する
                var content = File.ReadAllText(file, new UTF8Encoding(false)).Replace(
                    "DocumentElement",
                    "ArrayOfSpellTimer");
                File.WriteAllText(file, content, new UTF8Encoding(false));
#endif
                using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                {
                    if (sr.BaseStream.Length > 0)
                    {
                        var xs = new XmlSerializer(table.GetType());
                        var data = xs.Deserialize(sr) as List<SpellTimer>;

                        if (isClear)
                        {
                            this.table.Clear();
                        }

                        this.table.AddRange(data);
                    }
                }

                this.Reset();
            }
        }

        /// <summary>
        /// スペルテーブルを初期化する
        /// </summary>
        public void Reset()
        {
            var id = 0L;
            foreach (var row in this.table)
            {
                id++;
                row.ID = id;
                if (row.Guid == Guid.Empty)
                {
                    row.Guid = Guid.NewGuid();
                }

                row.MatchDateTime = DateTime.MinValue;
                row.Regex = null;
                row.RegexPattern = string.Empty;
                row.KeywordReplaced = string.Empty;
                row.RegexForExtend1 = null;
                row.RegexForExtendPattern1 = string.Empty;
                row.KeywordForExtendReplaced1 = string.Empty;
                row.RegexForExtend2 = null;
                row.RegexForExtendPattern2 = string.Empty;
                row.KeywordForExtendReplaced2 = string.Empty;

                if (row.BarWidth == 0 && row.BarHeight == 0)
                {
                    row.BarWidth = Settings.Default.ProgressBarSize.Width;
                    row.BarHeight = Settings.Default.ProgressBarSize.Height;
                }

                if (string.IsNullOrWhiteSpace(row.BackgroundColor))
                {
                    row.BackgroundColor = Color.Transparent.ToHTML();
                }
            }
        }

        /// <summary>
        /// 保存する
        /// </summary>
        public void Save(
            bool force = false)
        {
            this.Save(this.DefaultFile, force);
        }

        /// <summary>
        /// 保存する
        /// </summary>
        /// <param name="file">ファイルパス</param>
        public void Save(
            string file,
            bool force,
            string panelName = "")
        {
            if (this.table == null)
            {
                return;
            }

            if (!force)
            {
                if (this.table.Count <= 0)
                {
                    return;
                }
            }

            var work = this.table.Where(x =>
                !x.IsInstance &&
                (
                    string.IsNullOrEmpty(panelName) ||
                    x.Panel == panelName
                )).ToList();

            this.Save(file, work);
        }

        public void Save(
            string file,
            List<SpellTimer> list)
        {
            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
            {
                var xs = new XmlSerializer(list.GetType());
                xs.Serialize(sw, list);
            }
        }

        #region To Instance spells

        private static readonly object lockObject = new object();

        /// <summary>
        /// インスタンス化されたスペルの辞書 key : スペルの表示名
        /// </summary>
        private volatile ConcurrentDictionary<string, SpellTimer> instanceSpells =
            new ConcurrentDictionary<string, SpellTimer>();

        /// <summary>
        /// 同じスペル表示名のインスタンスを取得するか新たに作成する
        /// </summary>
        /// <param name="spellTitle">スペル表示名</param>
        /// <param name="sourceSpell">インスタンスの元となるスペル</param>
        /// <returns>インスタンススペル</returns>
        public SpellTimer GetOrAddInstance(
            string spellTitle,
            SpellTimer sourceSpell)
        {
            var instance = this.instanceSpells.GetOrAdd(
                spellTitle,
                (instanceSpellTitle) =>
                {
#if false
                    // Cloneだと不要なフィールドもコピーされてしまう
                    var ns = sourceSpell.Clone();

                    ns.SpellTitleReplaced = instanceSpellTitle;
                    ns.Guid = Guid.NewGuid();

                    ns.ToInstance = false;
                    ns.IsInstance = true;
                    ns.IsTemporaryDisplay = false;

                    return ns;
#else
                    var ns = new SpellTimer();

                    ns.SpellTitleReplaced = instanceSpellTitle;
                    ns.Guid = Guid.NewGuid();

                    ns.Panel = sourceSpell.Panel;
                    ns.SpellTitle = sourceSpell.SpellTitle;
                    ns.SpellIcon = sourceSpell.SpellIcon;
                    ns.SpellIconSize = sourceSpell.SpellIconSize;
                    ns.Keyword = sourceSpell.Keyword;
                    ns.KeywordForExtend1 = sourceSpell.KeywordForExtend1;
                    ns.KeywordForExtend2 = sourceSpell.KeywordForExtend2;
                    ns.RecastTime = sourceSpell.RecastTime;
                    ns.RecastTimeExtending1 = sourceSpell.RecastTimeExtending1;
                    ns.RecastTimeExtending2 = sourceSpell.RecastTimeExtending2;
                    ns.ExtendBeyondOriginalRecastTime = sourceSpell.ExtendBeyondOriginalRecastTime;
                    ns.UpperLimitOfExtension = sourceSpell.UpperLimitOfExtension;
                    ns.RepeatEnabled = sourceSpell.RepeatEnabled;
                    ns.ProgressBarVisible = sourceSpell.ProgressBarVisible;
                    ns.MatchSound = sourceSpell.MatchSound;
                    ns.MatchTextToSpeak = sourceSpell.MatchTextToSpeak;
                    ns.OverSound = sourceSpell.OverSound;
                    ns.OverTextToSpeak = sourceSpell.OverTextToSpeak;
                    ns.OverTime = sourceSpell.OverTime;
                    ns.BeforeSound = sourceSpell.BeforeSound;
                    ns.BeforeTextToSpeak = sourceSpell.BeforeTextToSpeak;
                    ns.BeforeTime = sourceSpell.BeforeTime;
                    ns.TimeupSound = sourceSpell.TimeupSound;
                    ns.TimeupTextToSpeak = sourceSpell.TimeupTextToSpeak;
                    ns.MatchDateTime = sourceSpell.MatchDateTime;
                    ns.TimeupHide = sourceSpell.TimeupHide;
                    ns.IsReverse = sourceSpell.IsReverse;
                    ns.Font = sourceSpell.Font;
                    ns.FontOutlineColor = sourceSpell.FontOutlineColor;
                    ns.WarningFontColor = sourceSpell.WarningFontColor;
                    ns.WarningFontOutlineColor = sourceSpell.WarningFontOutlineColor;
                    ns.BarColor = sourceSpell.BarColor;
                    ns.BarOutlineColor = sourceSpell.BarOutlineColor;
                    ns.BarWidth = sourceSpell.BarWidth;
                    ns.BarHeight = sourceSpell.BarHeight;
                    ns.BackgroundColor = sourceSpell.BackgroundColor;
                    ns.BackgroundAlpha = sourceSpell.BackgroundAlpha;
                    ns.DontHide = sourceSpell.DontHide;
                    ns.HideSpellName = sourceSpell.HideSpellName;
                    ns.WarningTime = sourceSpell.WarningTime;
                    ns.ChangeFontColorsWhenWarning = sourceSpell.ChangeFontColorsWhenWarning;
                    ns.BlinkTime = sourceSpell.BlinkTime;
                    ns.BlinkIcon = sourceSpell.BlinkIcon;
                    ns.BlinkBar = sourceSpell.BlinkBar;
                    ns.OverlapRecastTime = sourceSpell.OverlapRecastTime;
                    ns.ReduceIconBrightness = sourceSpell.ReduceIconBrightness;
                    ns.RegexEnabled = sourceSpell.RegexEnabled;
                    ns.JobFilter = sourceSpell.JobFilter;
                    ns.ZoneFilter = sourceSpell.ZoneFilter;
                    ns.TimersMustRunningForStart = sourceSpell.TimersMustRunningForStart;
                    ns.TimersMustStoppingForStart = sourceSpell.TimersMustStoppingForStart;
                    ns.Enabled = sourceSpell.Enabled;
                    ns.NotifyToDiscord = sourceSpell.NotifyToDiscord;
                    ns.NotifyToDiscordAtComplete = sourceSpell.NotifyToDiscordAtComplete;

                    ns.MatchedLog = sourceSpell.MatchedLog;
                    ns.Regex = sourceSpell.Regex;
                    ns.RegexPattern = sourceSpell.RegexPattern;
                    ns.KeywordReplaced = sourceSpell.KeywordReplaced;
                    ns.RegexForExtend1 = sourceSpell.RegexForExtend1;
                    ns.RegexForExtendPattern1 = sourceSpell.RegexForExtendPattern1;
                    ns.KeywordForExtendReplaced1 = sourceSpell.KeywordForExtendReplaced1;
                    ns.RegexForExtend2 = sourceSpell.RegexForExtend2;
                    ns.RegexForExtendPattern2 = sourceSpell.RegexForExtendPattern2;
                    ns.KeywordForExtendReplaced2 = sourceSpell.KeywordForExtendReplaced2;

                    ns.IsSequentialTTS = sourceSpell.IsSequentialTTS;
                    ns.PlayDelegate = sourceSpell.Play;
                    ns.ToInstance = false;
                    ns.IsInstance = true;
                    ns.IsTemporaryDisplay = false;

                    return ns;
#endif
                });

            lock (instance)
            {
                instance.CompleteScheduledTime = DateTime.MinValue;

                this.instanceSpells.TryAdd(
                    instance.SpellTitleReplaced,
                    instance);

                // スペルテーブル本体に登録する
                lock (lockObject)
                {
                    instance.ID = this.table.Max(y => y.ID) + 1;
                    this.table.Add(instance);
                    TableCompiler.Instance.AddSpell(instance);
                }
            }

            return instance;
        }

        /// <summary>
        /// インスタンス化されたスペルをすべて削除する
        /// </summary>
        public void RemoveInstanceSpellsAll()
        {
            this.instanceSpells.Clear();

            lock (lockObject)
            {
                this.table.RemoveAll(x => x.IsInstance);
            }

            TableCompiler.Instance.RemoveInstanceSpells();
        }

        /// <summary>
        /// instanceが不要になっていたらコレクションから除去する
        /// </summary>
        /// <param name="instance">インスタンス</param>
        public void TryRemoveInstance(
            SpellTimer instance)
        {
            var ttl = Settings.Default.TimeOfHideSpell + 30;

            lock (instance)
            {
                if (instance.CompleteScheduledTime != DateTime.MinValue &&
                    (DateTime.Now - instance.CompleteScheduledTime).TotalSeconds >= ttl)
                {
                    // ガーベージタイマを止める
                    instance.StopGarbageInstanceTimer();

                    if (!instance.IsInstance ||
                        instance.IsTemporaryDisplay)
                    {
                        return;
                    }

                    this.instanceSpells.TryRemove(instance.SpellTitleReplaced, out SpellTimer o);

                    // スペルコレクション本体から除去する
                    lock (lockObject)
                    {
                        this.table.Remove(instance);
                    }

                    // コンパイル済みリストから除去する
                    TableCompiler.Instance.RemoveSpell(instance);

                    instance.Dispose();
                }
            }
        }

        #endregion To Instance spells
    }
}
