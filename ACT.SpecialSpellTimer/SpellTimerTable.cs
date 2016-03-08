namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// SpellTimerテーブル
    /// </summary>
    public static class SpellTimerTable
    {
        private static object lockObject = new object();

        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        private static List<SpellTimer> table;

        private static SpellTimer[] enabledTable;

        private static DateTime enabledTableTimeStamp;

        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        public static List<SpellTimer> Table
        {
            get
            {
                lock (lockObject)
                {
                    if (table == null)
                    {
                        table = new List<SpellTimer>();
                        Load();
                    }

                    return table;
                }
            }
        }

        /// <summary>
        /// 有効なSpellTimerデータテーブル
        /// </summary>
        public static SpellTimer[] EnabledTable
        {
            get
            {
                lock (lockObject)
                {
                    if (enabledTable == null ||
                        (DateTime.Now - enabledTableTimeStamp).TotalSeconds >= 5.0d)
                    {
                        enabledTableTimeStamp = DateTime.Now;
                        enabledTable = EnabledTableCore;
                    }

                    return enabledTable;
                }
            }
        }

        /// <summary>
        /// 有効なSpellTimerデータテーブル
        /// </summary>
        private static SpellTimer[] EnabledTableCore
        {
            get
            {
                var spells =
                    from x in Table
                    where
                    x.Enabled
                    orderby
                    x.DisplayNo
                    select
                    x;

                var player = FF14PluginHelper.GetPlayer();
                var currentZoneID = FF14PluginHelper.GetCurrentZoneID();

                var spellsFilteredJob = new List<SpellTimer>();
                foreach (var spell in spells)
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
                        spellsFilteredJob.Add(spell);
                    }
                }

                // コンパイル済みの正規表現をセットする
                foreach (var spell in spellsFilteredJob)
                {
                    if (string.IsNullOrWhiteSpace(spell.KeywordReplaced))
                    {
                        spell.KeywordReplaced = LogBuffer.MakeKeyword(spell.Keyword);
                    }

                    if (string.IsNullOrWhiteSpace(spell.KeywordForExtendReplaced1))
                    {
                        spell.KeywordForExtendReplaced1 = LogBuffer.MakeKeyword(spell.KeywordForExtend1);
                    }

                    if (string.IsNullOrWhiteSpace(spell.KeywordForExtendReplaced2))
                    {
                        spell.KeywordForExtendReplaced2 = LogBuffer.MakeKeyword(spell.KeywordForExtend2);
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
                    var pattern = !string.IsNullOrWhiteSpace(spell.KeywordReplaced) ?
                        ".*" + spell.KeywordReplaced + ".*" :
                        string.Empty;

                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        if (spell.Regex == null ||
                            spell.RegexPattern != pattern)
                        {
                            spell.RegexPattern = pattern;
                            spell.Regex = new Regex(
                                pattern,
                                RegexOptions.Compiled);
                        }
                    }
                    else
                    {
                        spell.RegexPattern = string.Empty;
                        spell.Regex = null;
                    }

                    // 延長するためのマッチングキーワードの正規表現を生成する1
                    pattern = !string.IsNullOrWhiteSpace(spell.KeywordForExtendReplaced1) ?
                        ".*" + spell.KeywordForExtendReplaced1 + ".*" :
                        string.Empty;

                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        if (spell.RegexForExtend1 == null ||
                            spell.RegexForExtendPattern1 != pattern)
                        {
                            spell.RegexForExtendPattern1 = pattern;
                            spell.RegexForExtend1 = new Regex(
                                pattern,
                                RegexOptions.Compiled);
                        }
                    }
                    else
                    {
                        spell.RegexForExtendPattern1 = string.Empty;
                        spell.RegexForExtend1 = null;
                    }

                    // 延長するためのマッチングキーワードの正規表現を生成する2
                    pattern = !string.IsNullOrWhiteSpace(spell.KeywordForExtendReplaced2) ?
                        ".*" + spell.KeywordForExtendReplaced2 + ".*" :
                        string.Empty;

                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        if (spell.RegexForExtend2 == null ||
                            spell.RegexForExtendPattern2 != pattern)
                        {
                            spell.RegexForExtendPattern2 = pattern;
                            spell.RegexForExtend2 = new Regex(
                                pattern,
                                RegexOptions.Compiled);
                        }
                    }
                    else
                    {
                        spell.RegexForExtendPattern2 = string.Empty;
                        spell.RegexForExtend2 = null;
                    }
                }

                return spellsFilteredJob.ToArray();
            }
        }

        /// <summary>
        /// 定義のインスタンス（表示用のコピー）を生成する
        /// </summary>
        /// <param name="element">インスタンス化する定義</param>
        /// <returns>生成されたインスタンス</returns>
        public static SpellTimer CreateInstanceByElement(
            SpellTimer element)
        {
            var instance = new SpellTimer();

            instance.ID = Table.Max(x => x.ID) + 1;
            instance.guid = Guid.NewGuid();
            instance.Panel = element.Panel;
            instance.SpellTitle = element.SpellTitle;
            instance.SpellIcon = element.SpellIcon;
            instance.SpellIconSize = element.SpellIconSize;
            instance.Keyword = element.Keyword;
            instance.KeywordForExtend1 = element.KeywordForExtend1;
            instance.KeywordForExtend2 = element.KeywordForExtend2;
            instance.RecastTime = element.RecastTime;
            instance.RecastTimeExtending1 = element.RecastTimeExtending1;
            instance.RecastTimeExtending2 = element.RecastTimeExtending2;
            instance.ExtendBeyondOriginalRecastTime = element.ExtendBeyondOriginalRecastTime;
            instance.UpperLimitOfExtension = element.UpperLimitOfExtension;
            instance.RepeatEnabled = element.RepeatEnabled;
            instance.ProgressBarVisible = element.ProgressBarVisible;
            instance.MatchSound = element.MatchSound;
            instance.MatchTextToSpeak = element.MatchTextToSpeak;
            instance.OverSound = element.OverSound;
            instance.OverTextToSpeak = element.OverTextToSpeak;
            instance.OverTime = element.OverTime;
            instance.BeforeSound = element.BeforeSound;
            instance.BeforeTextToSpeak = element.BeforeTextToSpeak;
            instance.BeforeTime = element.BeforeTime;
            instance.TimeupSound = element.TimeupSound;
            instance.TimeupTextToSpeak = element.TimeupTextToSpeak;
            instance.MatchDateTime = element.MatchDateTime;
            instance.TimeupHide = element.TimeupHide;
            instance.IsReverse = element.IsReverse;
            instance.Font = element.Font;
            instance.FontFamily = element.FontFamily;
            instance.FontSize = element.FontSize;
            instance.FontStyle = element.FontStyle;
            instance.FontColor = element.FontColor;
            instance.FontOutlineColor = element.FontOutlineColor;
            instance.BarColor = element.BarColor;
            instance.BarOutlineColor = element.BarOutlineColor;
            instance.BarWidth = element.BarWidth;
            instance.BarHeight = element.BarHeight;
            instance.BackgroundColor = element.BackgroundColor;
            instance.BackgroundAlpha = element.BackgroundAlpha;
            instance.DontHide = element.DontHide;
            instance.HideSpellName = element.HideSpellName;
            instance.OverlapRecastTime = element.OverlapRecastTime;
            instance.ReduceIconBrightness = element.ReduceIconBrightness;
            instance.RegexEnabled = element.RegexEnabled;
            instance.JobFilter = element.JobFilter;
            instance.ZoneFilter = element.ZoneFilter;
            instance.TimersMustRunningForStart = element.TimersMustRunningForStart;
            instance.TimersMustStoppingForStart = element.TimersMustStoppingForStart;
            instance.Enabled = element.Enabled;

            instance.MatchedLog = element.MatchedLog;
            instance.Regex = element.Regex;
            instance.RegexPattern = element.RegexPattern;
            instance.KeywordReplaced = element.KeywordReplaced;
            instance.RegexForExtend1 = element.RegexForExtend1;
            instance.RegexForExtendPattern1 = element.RegexForExtendPattern1;
            instance.KeywordForExtendReplaced1 = element.KeywordForExtendReplaced1;
            instance.RegexForExtend2 = element.RegexForExtend2;
            instance.RegexForExtendPattern2 = element.RegexForExtendPattern2;
            instance.KeywordForExtendReplaced2 = element.KeywordForExtendReplaced2;

            instance.ToInstance = false;
            instance.IsInstance = true;

            lock (lockObject)
            {
                table.Add(instance);

                var array = new SpellTimer[enabledTable.Length + 1];
                Array.Copy(enabledTable, array, enabledTable.Length);
                array[enabledTable.Length] = instance;
                enabledTable = array;
            }

            return instance;
        }

        /// <summary>
        /// 指定したスペルをコレクションから除去する
        /// </summary>
        /// <param name="spell">除去するスペル</param>
        public static void RemoveSpell(
            SpellTimer spell)
        {
            lock (lockObject)
            {
                table.Remove(spell);
            }
        }

        /// <summary>
        /// インスタンス化されたスペルをすべて削除する
        /// </summary>
        public static void RemoveAllInstanceSpells()
        {
            lock (lockObject)
            {
                var collection = table.Where(x => x.IsInstance);
                foreach (var item in collection)
                {
                    table.Remove(item);
                }
            }
        }

        /// <summary>
        /// 置換後のキーワードをクリアする
        /// </summary>
        public static void ClearReplacedKeywords()
        {
            foreach (var item in Table)
            {
                item.KeywordReplaced = string.Empty;
                item.KeywordForExtendReplaced1 = string.Empty;
                item.KeywordForExtendReplaced2 = string.Empty;
            }

            // 有効SpellTimerのキャッシュを無効にする
            enabledTableTimeStamp = DateTime.MinValue;
        }

        /// <summary>
        /// スペルの描画済みフラグをクリアする
        /// </summary>
        public static void ClearUpdateFlags()
        {
            foreach (var item in Table)
            {
                item.UpdateDone = false;
            }
        }

        /// <summary>
        /// 指定されたGuidを持つSpellTimerを取得する
        /// </summary>
        /// <param name="guid">Guid</param>
        public static SpellTimer GetSpellTimerByGuid(Guid guid)
        {
            return table.Where(x => x.guid == guid).FirstOrDefault();
        }

        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public static string DefaultFile
        {
            get
            {
                var r = string.Empty;

                r = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"anoyetta\ACT\ACT.SpecialSpellTimer.Spells.xml");

                return r;
            }
        }

        /// <summary>
        /// カウントをリセットする
        /// </summary>
        public static void Reset()
        {
            var id = 0L;
            foreach (var row in Table)
            {
                id++;
                row.ID = id;
                if (row.guid == Guid.Empty)
                {
                    row.guid = Guid.NewGuid();
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

                row.MatchSound = !string.IsNullOrWhiteSpace(row.MatchSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.MatchSound)) :
                    string.Empty;
                row.OverSound = !string.IsNullOrWhiteSpace(row.OverSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.OverSound)) :
                    string.Empty;
                row.BeforeSound = !string.IsNullOrWhiteSpace(row.BeforeSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.BeforeSound)) :
                    string.Empty;
                row.TimeupSound = !string.IsNullOrWhiteSpace(row.TimeupSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.TimeupSound)) :
                    string.Empty;

                if (row.BarWidth == 0 && row.BarHeight == 0)
                {
                    row.BarWidth = Settings.Default.ProgressBarSize.Width;
                    row.BarHeight = Settings.Default.ProgressBarSize.Height;
                }

                if (string.IsNullOrWhiteSpace(row.FontFamily))
                {
                    row.FontFamily = Settings.Default.Font.Name;
                    row.FontSize = Settings.Default.Font.Size;
                    row.FontStyle = (int)Settings.Default.Font.Style;
                }

                if (string.IsNullOrWhiteSpace(row.BackgroundColor))
                {
                    row.BackgroundColor = Color.Transparent.ToHTML();
                }

                if (row.Font == null ||
                    row.Font.Family == null ||
                    string.IsNullOrWhiteSpace(row.Font.Family.Source))
                {
                    var style = (FontStyle)row.FontStyle;

                    row.Font = new FontInfo()
                    {
                        FamilyName = row.FontFamily,
                        Size = row.FontSize / 72.0d * 96.0d,
                        Style = System.Windows.FontStyles.Normal,
                        Weight = System.Windows.FontWeights.Normal,
                        Stretch = System.Windows.FontStretches.Normal
                    };

                    if ((style & FontStyle.Italic) != 0)
                    {
                        row.Font.Style = System.Windows.FontStyles.Italic;
                    }

                    if ((style & FontStyle.Bold) != 0)
                    {
                        row.Font.Weight = System.Windows.FontWeights.Bold;
                    }
                }
            }
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        public static void Load()
        {
            Load(DefaultFile, true);
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        /// <param name="file">ファイルパス</param>
        /// <param name="isClear">消去してからロードする？</param>
        public static void Load(
            string file,
            bool isClear)
        {
            if (File.Exists(file))
            {
                if (isClear)
                {
                    Table.Clear();
                }

                // 旧フォーマットを置換する
                var content = File.ReadAllText(file, new UTF8Encoding(false)).Replace(
                    "DocumentElement",
                    "ArrayOfSpellTimer");
                File.WriteAllText(file, content, new UTF8Encoding(false));

                using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                {
                    try
                    {
                        if (sr.BaseStream.Length > 0)
                        {
                            var xs = new XmlSerializer(table.GetType());
                            var data = xs.Deserialize(sr) as List<SpellTimer>;
                            table.AddRange(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Translate.Get("LoadXMLError"), ex);
                    }
                }

                Reset();
            }
        }

        /// <summary>
        /// 保存する
        /// </summary>
        public static void Save()
        {
            Save(DefaultFile);
        }

        /// <summary>
        /// 保存する
        /// </summary>
        /// <param name="file">ファイルパス</param>

        public static void Save(
            string file)
        {
            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var work = new List<SpellTimer>(
                table.Where(x => !x.IsInstance));

            foreach (var item in work)
            {
                item.MatchSound = !string.IsNullOrWhiteSpace(item.MatchSound) ?
                    Path.GetFileName(item.MatchSound) :
                    string.Empty;
                item.OverSound = !string.IsNullOrWhiteSpace(item.OverSound) ?
                    Path.GetFileName(item.OverSound) :
                    string.Empty;
                item.BeforeSound = !string.IsNullOrWhiteSpace(item.BeforeSound) ?
                    Path.GetFileName(item.BeforeSound) :
                    string.Empty;
                item.TimeupSound = !string.IsNullOrWhiteSpace(item.TimeupSound) ?
                    Path.GetFileName(item.TimeupSound) :
                    string.Empty;

                if (item.Font != null &&
                    item.Font.Family != null &&
                    !string.IsNullOrWhiteSpace(item.Font.Family.Source))
                {
                    item.FontFamily = string.Empty;
                    item.FontSize = 1;
                    item.FontStyle = 0;
                }
            }

            using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
            {
                var xs = new XmlSerializer(work.GetType());
                xs.Serialize(sw, work);
            }

            foreach (var item in work)
            {
                item.MatchSound = !string.IsNullOrWhiteSpace(item.MatchSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.MatchSound)) :
                    string.Empty;
                item.OverSound = !string.IsNullOrWhiteSpace(item.OverSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.OverSound)) :
                    string.Empty;
                item.BeforeSound = !string.IsNullOrWhiteSpace(item.BeforeSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.BeforeSound)) :
                    string.Empty;
                item.TimeupSound = !string.IsNullOrWhiteSpace(item.TimeupSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.TimeupSound)) :
                    string.Empty;
            }
        }

        /// <summary>
        /// テーブルファイルをバックアップする
        /// </summary>
        public static void Backup()
        {
            var file = DefaultFile;

            if (File.Exists(file))
            {
                var backupFile = Path.Combine(
                    Path.GetDirectoryName(file),
                    Path.GetFileNameWithoutExtension(file) + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".bak");

                File.Copy(
                    file,
                    backupFile,
                    true);

                // 古いバックアップを消す
                foreach (var bak in Directory.GetFiles(Path.GetDirectoryName(file), "*.bak"))
                {
                    var timeStamp = File.GetCreationTime(bak);
                    if ((DateTime.Now - timeStamp).TotalDays >= 3.0d)
                    {
                        File.Delete(bak);
                    }
                }
            }
        }
    }

    /// <summary>
    /// スペルタイマ
    /// </summary>
    [Serializable]
    public class SpellTimer
    {
        public SpellTimer()
        {
            this.guid = Guid.Empty;
            this.Panel = string.Empty;
            this.SpellTitle = string.Empty;
            this.SpellIcon = string.Empty;
            this.Keyword = string.Empty;
            this.KeywordForExtend1 = string.Empty;
            this.KeywordForExtend2 = string.Empty;
            this.MatchSound = string.Empty;
            this.MatchTextToSpeak = string.Empty;
            this.OverSound = string.Empty;
            this.OverTextToSpeak = string.Empty;
            this.TimeupSound = string.Empty;
            this.TimeupTextToSpeak = string.Empty;
            this.FontColor = string.Empty;
            this.FontOutlineColor = string.Empty;
            this.BarColor = string.Empty;
            this.BarOutlineColor = string.Empty;
            this.BackgroundColor = string.Empty;
            this.JobFilter = string.Empty;
            this.SpellTitleReplaced = string.Empty;
            this.MatchedLog = string.Empty;
            this.RegexPattern = string.Empty;
            this.JobFilter = string.Empty;
            this.ZoneFilter = string.Empty;
            this.TimersMustRunningForStart = new Guid[0];
            this.TimersMustStoppingForStart = new Guid[0];
            this.Font = new FontInfo();
            this.KeywordReplaced = string.Empty;
            this.KeywordForExtendReplaced1 = string.Empty;
            this.KeywordForExtendReplaced2 = string.Empty;
        }

        public long ID { get; set; }
        public Guid guid { get; set; }
        public long DisplayNo { get; set; }
        public string Panel { get; set; }
        public string SpellTitle { get; set; }
        public string SpellIcon { get; set; }
        public int SpellIconSize { get; set; }
        public string Keyword { get; set; }
        public string KeywordForExtend1 { get; set; }
        public string KeywordForExtend2 { get; set; }
        public long RecastTime { get; set; }
        public long RecastTimeExtending1 { get; set; }
        public long RecastTimeExtending2 { get; set; }
        public bool ExtendBeyondOriginalRecastTime { get; set; }
        public long UpperLimitOfExtension { get; set; }
        public bool RepeatEnabled { get; set; }
        public bool ProgressBarVisible { get; set; }
        public string MatchSound { get; set; }
        public string MatchTextToSpeak { get; set; }
        public string OverSound { get; set; }
        public string OverTextToSpeak { get; set; }
        public long OverTime { get; set; }
        public string BeforeSound { get; set; }
        public string BeforeTextToSpeak { get; set; }
        public long BeforeTime { get; set; }
        public string TimeupSound { get; set; }
        public string TimeupTextToSpeak { get; set; }
        public DateTime MatchDateTime { get; set; }
        public bool TimeupHide { get; set; }
        public bool IsReverse { get; set; }
        public FontInfo Font { get; set; }
        public string FontFamily { get; set; }
        public float FontSize { get; set; }
        public int FontStyle { get; set; }
        public string FontColor { get; set; }
        public string FontOutlineColor { get; set; }
        public string BarColor { get; set; }
        public string BarOutlineColor { get; set; }
        public int BarWidth { get; set; }
        public int BarHeight { get; set; }
        public string BackgroundColor { get; set; }
        public int BackgroundAlpha { get; set; }
        public bool DontHide { get; set; }
        public bool HideSpellName { get; set; }
        public bool OverlapRecastTime { get; set; }
        public bool ReduceIconBrightness { get; set; }
        public bool RegexEnabled { get; set; }
        public string JobFilter { get; set; }
        public string ZoneFilter { get; set; }
        public Guid[] TimersMustRunningForStart { get; set; }
        public Guid[] TimersMustStoppingForStart { get; set; }

        /// <summary>インスタンス化する</summary>
        /// <remarks>表示テキストが異なる条件でマッチングした場合に当該スペルの新しいインスタンスを生成する</remarks>
        public bool ToInstance { get; set; }

        public bool Enabled { get; set; }

        [XmlIgnore]
        public DateTime CompleteScheduledTime { get; set; }
        [XmlIgnore]
        public volatile bool UpdateDone;
        [XmlIgnore]
        public bool OverDone { get; set; }
        [XmlIgnore]
        public bool BeforeDone { get; set; }
        [XmlIgnore]
        public bool TimeupDone { get; set; }
        [XmlIgnore]
        public string SpellTitleReplaced { get; set; }
        [XmlIgnore]
        public string MatchedLog { get; set; }
        [XmlIgnore]
        public Regex Regex { get; set; }
        [XmlIgnore]
        public string RegexPattern { get; set; }
        [XmlIgnore]
        public string KeywordReplaced { get; set; }
        [XmlIgnore]
        public Regex RegexForExtend1 { get; set; }
        [XmlIgnore]
        public string RegexForExtendPattern1 { get; set; }
        [XmlIgnore]
        public string KeywordForExtendReplaced1 { get; set; }
        [XmlIgnore]
        public Regex RegexForExtend2 { get; set; }
        [XmlIgnore]
        public string RegexForExtendPattern2 { get; set; }
        [XmlIgnore]
        public string KeywordForExtendReplaced2 { get; set; }

        /// <summary>インスタンス化されたスペルか？</summary>
        [XmlIgnore]
        public bool IsInstance { get; set; }
    }
}
