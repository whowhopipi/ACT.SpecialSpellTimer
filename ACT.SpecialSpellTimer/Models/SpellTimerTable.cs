using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Sound;
using ACT.SpecialSpellTimer.Utility;

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

        private static readonly object lockObject = new object();

        /// <summary>
        /// インスタンス化されたスペルの辞書 key : スペルの表示名
        /// </summary>
        private volatile ConcurrentDictionary<string, SpellTimer> instanceSpells = new ConcurrentDictionary<string, SpellTimer>();

        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        private volatile List<SpellTimer> table = new List<SpellTimer>();

        public SpellTimerTable()
        {
            this.Load();
        }

        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public string DefaultFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.Spells.xml");

        /// <summary>
        /// テーブルの編集中？
        /// </summary>
        public bool IsEditingTable { get; set; }

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
                    Path.GetFileNameWithoutExtension(file) + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".bak");

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
                (x) =>
                {
                    var ns = new SpellTimer();

                    ns.SpellTitleReplaced = x;

                    ns.guid = Guid.NewGuid();
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
                    ns.FontFamily = sourceSpell.FontFamily;
                    ns.FontSize = sourceSpell.FontSize;
                    ns.FontStyle = sourceSpell.FontStyle;
                    ns.FontColor = sourceSpell.FontColor;
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
                    ns.OverlapRecastTime = sourceSpell.OverlapRecastTime;
                    ns.ReduceIconBrightness = sourceSpell.ReduceIconBrightness;
                    ns.RegexEnabled = sourceSpell.RegexEnabled;
                    ns.JobFilter = sourceSpell.JobFilter;
                    ns.ZoneFilter = sourceSpell.ZoneFilter;
                    ns.TimersMustRunningForStart = sourceSpell.TimersMustRunningForStart;
                    ns.TimersMustStoppingForStart = sourceSpell.TimersMustStoppingForStart;
                    ns.Enabled = sourceSpell.Enabled;

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

                    ns.ToInstance = false;
                    ns.IsInstance = true;

                    return ns;
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
                    instance.ID = Table.Max(y => y.ID) + 1;
                    this.table.Add(instance);
                    TableCompiler.Instance.AddInstanceSpell(instance);
                }
            }

            return instance;
        }

        /// <summary>
        /// 指定されたGuidを持つSpellTimerを取得する
        /// </summary>
        /// <param name="guid">Guid</param>
        public SpellTimer GetSpellTimerByGuid(Guid guid)
        {
            return this.table.Where(x => x.guid == guid).FirstOrDefault();
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
                if (isClear)
                {
                    this.table.Clear();
                }
#if false
                // 旧フォーマットを置換する
                var content = File.ReadAllText(file, new UTF8Encoding(false)).Replace(
                    "DocumentElement",
                    "ArrayOfSpellTimer");
                File.WriteAllText(file, content, new UTF8Encoding(false));
#endif
                using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                {
                    try
                    {
                        if (sr.BaseStream.Length > 0)
                        {
                            var xs = new XmlSerializer(table.GetType());
                            var data = xs.Deserialize(sr) as List<SpellTimer>;
                            this.table.AddRange(data);
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
        /// インスタンス化されたスペルをすべて削除する
        /// </summary>
        public void RemoveAllInstanceSpells()
        {
            this.instanceSpells.Clear();

            lock (lockObject)
            {
                var collection = table.Where(x => x.IsInstance);
                foreach (var item in collection)
                {
                    this.table.Remove(item);
                }
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
        /// 保存する
        /// </summary>
        public void Save()
        {
            this.Save(this.DefaultFile);
        }

        /// <summary>
        /// 保存する
        /// </summary>
        /// <param name="file">ファイルパス</param>
        public void Save(
            string file)
        {
            if (this.table == null)
            {
                return;
            }

            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var work = new List<SpellTimer>(
                this.table.Where(x => !x.IsInstance));

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

                    SpellTimer o;
                    this.instanceSpells.TryRemove(instance.SpellTitleReplaced, out o);

                    // スペルコレクション本体から除去する
                    lock (lockObject)
                    {
                        this.table.Remove(instance);
                    }

                    instance.Dispose();
                }
            }
        }
    }
}
