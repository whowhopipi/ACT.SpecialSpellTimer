using System;
using System.IO;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// Panel設定
    /// </summary>
    public class PanelSettings
    {
        /// <summary>
        /// 唯一のinstance
        /// </summary>
        private static PanelSettings instance = new PanelSettings();

        /// <summary>
        /// Panel設定データテーブル
        /// </summary>
        private volatile SpellTimerDataSet.PanelSettingsDataTable settingsTable = new SpellTimerDataSet.PanelSettingsDataTable();

        /// <summary>
        /// 唯一のinstance
        /// </summary>
        public static PanelSettings Instance => instance;

        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public string DefaultFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.Panels.xml");

        /// <summary>
        /// Panel設定データテーブル
        /// </summary>
        public SpellTimerDataSet.PanelSettingsDataTable SettingsTable => settingsTable;

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
        /// ロード
        /// </summary>
        public void Load()
        {
            if (File.Exists(this.DefaultFile))
            {
                this.settingsTable.Clear();

                try
                {
                    this.settingsTable.ReadXml(this.DefaultFile);
                }
                catch (Exception ex)
                {
                    Logger.Write(Translate.Get("LoadXMLError"), ex);
                }
            }
        }

        /// <summary>
        /// セーブ
        /// </summary>
        public void Save()
        {
            this.settingsTable.AcceptChanges();

            if (this.settingsTable == null ||
                this.settingsTable.Count < 1)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.DefaultFile);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            this.settingsTable.WriteXml(this.DefaultFile);
        }
    }
}
