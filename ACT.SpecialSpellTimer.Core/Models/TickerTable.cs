using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using ACT.SpecialSpellTimer.Utility;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// ワンポイントテレロップ設定テーブル
    /// </summary>
    public class TickerTable
    {
        #region Singleton

        private static TickerTable instance = new TickerTable();

        public static TickerTable Instance => instance;

        #endregion Singleton

        /// <summary>
        /// データテーブル
        /// </summary>
        private volatile ObservableCollection<Ticker> table = new ObservableCollection<Ticker>();

        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public string DefaultFile =>
            Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.Telops.xml");

        /// <summary>
        /// 生のテーブル
        /// </summary>
        public ObservableCollection<Ticker> Table => this.table;

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
        /// 指定されたGuidを持つOnePointTelopを取得する
        /// </summary>
        /// <param name="guid">Guid</param>
        public Ticker GetOnePointTelopByGuid(Guid guid)
        {
            return table
                .AsParallel()
                .Where(x => x.Guid == guid)
                .FirstOrDefault();
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
                using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                {
                    if (sr.BaseStream.Length > 0)
                    {
                        var xs = new XmlSerializer(table.GetType());
                        var data = xs.Deserialize(sr) as IList<Ticker>;

                        if (isClear)
                        {
                            this.table.Clear();
                        }

                        foreach (var item in data)
                        {
                            this.table.Add(item);
                        }
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
                if (row.Guid == Guid.Empty)
                {
                    row.Guid = Guid.NewGuid();
                }

                row.MatchDateTime = DateTime.MinValue;
                row.Regex = null;
                row.RegexPattern = string.Empty;
                row.RegexToHide = null;
                row.RegexPatternToHide = string.Empty;

                if (string.IsNullOrWhiteSpace(row.BackgroundColor))
                {
                    row.BackgroundColor = Color.Transparent.ToHTML();
                }

                // NaNを潰す
                row.Top = double.IsNaN(row.Top) ? 0 : row.Top;
                row.Left = double.IsNaN(row.Top) ? 0 : row.Left;
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
        public void Save(
            bool force = false)
        {
            this.Save(this.DefaultFile, force);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="file">ファイル</param>
        public void Save(
            string file,
            bool force)
        {
            if (!force)
            {
                if (this.table.Count <= 0)
                {
                    return;
                }
            }

            this.Save(file, this.table);
        }

        public void Save(
            string file,
            IList<Ticker> list)
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
    }
}
