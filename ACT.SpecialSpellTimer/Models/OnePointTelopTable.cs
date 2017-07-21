using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using ACT.SpecialSpellTimer.Sound;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// ワンポイントテレロップ設定テーブル
    /// </summary>
    public class OnePointTelopTable
    {
        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static OnePointTelopTable instance = new OnePointTelopTable();

        /// <summary>
        /// データテーブル
        /// </summary>
        private readonly List<OnePointTelop> table = new List<OnePointTelop>();

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
        public static OnePointTelopTable Default => instance;

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
        /// テーブルの編集中？
        /// </summary>
        public bool IsEditingTable { get; set; }

        /// <summary>
        /// 生のテーブル
        /// </summary>
        public List<OnePointTelop> Table => this.table;

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
            foreach (var row in TableCompiler.Instance.TickerList)
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
