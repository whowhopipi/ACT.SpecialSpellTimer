using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Utility;

namespace ACT.SpecialSpellTimer.Models
{
    /// <summary>
    /// Panel設定
    /// </summary>
    public class SpellPanelTable
    {
        #region Singleton

        private static SpellPanelTable instance = new SpellPanelTable();

        public static SpellPanelTable Instance => instance;

        #endregion Singleton

        private volatile ObservableCollection<SpellPanel> table = new ObservableCollection<SpellPanel>();

        public ObservableCollection<SpellPanel> Table => this.table;

        public string DefaultFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.Panels.xml");

        public void Load()
        {
            try
            {
                if (!File.Exists(this.DefaultFile))
                {
                    return;
                }

                // 旧形式を置換する
                var text = File.ReadAllText(
                    this.DefaultFile,
                    new UTF8Encoding(false));
                text = text.Replace("DocumentElement", "ArrayOfPanelSettings");
                File.WriteAllText(
                    this.DefaultFile,
                    text,
                    new UTF8Encoding(false));

                using (var sr = new StreamReader(this.DefaultFile, new UTF8Encoding(false)))
                {
                    try
                    {
                        if (sr.BaseStream.Length > 0)
                        {
                            var xs = new XmlSerializer(this.table.GetType());
                            var data = xs.Deserialize(sr) as IList<SpellPanel>;

                            this.table.Clear();
                            foreach (var item in data)
                            {
                                // 旧Generalパネルの名前を置換える
                                if (item.PanelName == "General")
                                {
                                    item.PanelName = SpellPanel.GeneralPanel.PanelName;
                                }

                                this.table.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Translate.Get("LoadXMLError"), ex);
                    }
                }
            }
            finally
            {
                // NaNを潰す
                foreach (var x in this.table)
                {
                    x.Top = double.IsNaN(x.Top) ? 0 : x.Top;
                    x.Left = double.IsNaN(x.Left) ? 0 : x.Left;
                    x.Margin = double.IsNaN(x.Margin) ? 0 : x.Margin;
                }

                // プリセットパネルを作る
                var generalPanel = this.Table.FirstOrDefault(x => x.PanelName == SpellPanel.GeneralPanel.PanelName);
                if (generalPanel == null)
                {
                    this.Table.Add(SpellPanel.GeneralPanel);
                }
                else
                {
                    SpellPanel.SetGeneralPanel(generalPanel);
                }
            }
        }

        public void Save()
        {
            if (this.table == null ||
                this.table.Count < 1)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.DefaultFile);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var sw = new StreamWriter(this.DefaultFile, false, new UTF8Encoding(false)))
            {
                var xs = new XmlSerializer(this.table.GetType());
                xs.Serialize(sw, this.table);
            }
        }

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
    }
}
