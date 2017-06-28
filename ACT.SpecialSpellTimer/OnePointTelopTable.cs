namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Models;
    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// ワンポイントテレロップ設定テーブル
    /// </summary>
    public class OnePointTelopTable
    {
        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static OnePointTelopTable instance;

        /// <summary>
        /// データテーブル
        /// </summary>
        private readonly List<OnePointTelop> table = new List<OnePointTelop>();

        private volatile OnePointTelop[] enabledTable;

        private DateTime enabledTableTimeStamp;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OnePointTelopTable()
        {
            this.Load();
        }

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static OnePointTelopTable Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new OnePointTelopTable();
                }

                return instance;
            }
        }

        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public string DefaultFile
        {
            get
            {
                var r = string.Empty;

                r = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"anoyetta\ACT\ACT.SpecialSpellTimer.Telops.xml");

                return r;
            }
        }

        /// <summary>
        /// 有効なエントリのリスト
        /// </summary>
        public IReadOnlyList<OnePointTelop> EnabledTable
        {
            get
            {
                var now = DateTime.Now;

                if (!this.IsEditingTable)
                {
                    if (this.enabledTable == null ||
                        (now - this.enabledTableTimeStamp).TotalSeconds >= 5.0d)
                    {
                        this.enabledTableTimeStamp = now;
                        this.enabledTable = EnabledTableCore;
                    }
                }

                return this.enabledTable;
            }
        }

        /// <summary>
        /// テーブルの編集中？
        /// </summary>
        public bool IsEditingTable { get; set; }

        /// <summary>
        /// 生のテーブル
        /// </summary>
        public List<OnePointTelop> Table => this.table;

        /// <summary>
        /// 有効なエントリのリスト
        /// </summary>
        private OnePointTelop[] EnabledTableCore
        {
            get
            {
                var spells =
                    from x in this.table
                    where
                    x.Enabled
                    orderby
                    x.MatchDateTime ascending
                    select
                    x;

                var player = FF14PluginHelper.GetPlayer();
                var currentZoneID = FF14PluginHelper.GetCurrentZoneID();

                var spellsFilteredJob = new List<OnePointTelop>();
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

                return spellsFilteredJob.ToArray();
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

        /// <summary>
        /// 置換後のキーワードをクリアする
        /// </summary>
        public void ClearReplacedKeywords()
        {
            foreach (var item in this.Table)
            {
                item.KeywordReplaced = string.Empty;
                item.KeywordToHideReplaced = string.Empty;
            }

            // 有効SpellTimerのキャッシュを無効にする
            enabledTableTimeStamp = DateTime.MinValue;
        }

        /// <summary>
        /// 指定されたGuidを持つOnePointTelopを取得する
        /// </summary>
        /// <param name="guid">Guid</param>
        public OnePointTelop GetOnePointTelopByGuid(Guid guid)
        {
            return table.Where(x => x.guid == guid).FirstOrDefault();
        }

        /// <summary>
        /// Load
        /// </summary>
        public void Load()
        {
            this.Load(this.DefaultFile, true);
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="file">ファイル</param>
        /// <param name="isClear">クリアしてから取り込むか？</param>
        public void Load(
            string file,
            bool isClear)
        {
            if (File.Exists(file))
            {
                if (isClear)
                {
                    this.table.Clear();
                }

                // 旧フォーマットを置換する
                var content = File.ReadAllText(file, new UTF8Encoding(false)).Replace(
                    "DocumentElement",
                    "ArrayOfOnePointTelop");
                File.WriteAllText(file, content, new UTF8Encoding(false));

                using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                {
                    try
                    {
                        if (sr.BaseStream.Length > 0)
                        {
                            var xs = new XmlSerializer(table.GetType());
                            var data = xs.Deserialize(sr) as List<OnePointTelop>;
                            table.AddRange(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Translate.Get("LoadXMLError"), ex);
                    }
                }

                this.Reset();
            }
        }

        /// <summary>
        /// マッチ状態をリセットする
        /// </summary>
        public void Reset()
        {
            var id = 0L;
            foreach (var row in this.table)
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
                row.RegexToHide = null;
                row.RegexPatternToHide = string.Empty;

                row.MatchSound = !string.IsNullOrWhiteSpace(row.MatchSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.MatchSound)) :
                    string.Empty;
                row.DelaySound = !string.IsNullOrWhiteSpace(row.DelaySound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.DelaySound)) :
                    string.Empty;

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
        /// カウントをリセットする
        /// </summary>
        public void ResetCount()
        {
            foreach (var row in this.EnabledTable)
            {
                row.MatchDateTime = DateTime.MinValue;
                row.Delayed = false;
                row.ForceHide = false;

                row.StartDelayedSoundTimer();
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save()
        {
            this.Save(this.DefaultFile);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="file">ファイル</param>
        public void Save(
            string file)
        {
            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (var item in table)
            {
                item.MatchSound = !string.IsNullOrWhiteSpace(item.MatchSound) ?
                    Path.GetFileName(item.MatchSound) :
                    string.Empty;
                item.DelaySound = !string.IsNullOrWhiteSpace(item.DelaySound) ?
                    Path.GetFileName(item.DelaySound) :
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
                var xs = new XmlSerializer(table.GetType());
                xs.Serialize(sw, table);
            }

            foreach (var item in table)
            {
                item.MatchSound = !string.IsNullOrWhiteSpace(item.MatchSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.MatchSound)) :
                    string.Empty;
                item.DelaySound = !string.IsNullOrWhiteSpace(item.DelaySound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.DelaySound)) :
                    string.Empty;
            }
        }
    }
}