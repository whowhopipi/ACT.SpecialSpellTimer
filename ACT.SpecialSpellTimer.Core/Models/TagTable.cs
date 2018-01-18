using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Utility;
using FFXIV.Framework.Common;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Models
{
    public class TagTable :
        BindableBase
    {
        #region Singleton

        private static TagTable instance = new TagTable();

        public static TagTable Instance => instance;

        #endregion Singleton

        private ObservableCollection<Tag> table = new ObservableCollection<Tag>();

        public ObservableCollection<Tag> Table => this.table;

        public string DefaultFile =>
            Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer.Tags.xml");

        public void Add(
            Tag tag)
            => this.Table.Add(tag);

        public Tag AddNew(
            Tag parent)
        {
            var tag = new Tag();

            if (parent != null)
            {
                tag.ParentID = parent.ID;
            }

            this.Add(tag);

            return tag;
        }

        public Tag AddNew(
            Guid parentID)
            => this.AddNew(this.table.FirstOrDefault(x => x.ID == parentID));

        public Tag AddNew() => this.AddNew(null);

        public void Remove(
            Tag tag)
        {
            foreach (var child in tag.Children)
            {
                this.Remove(child);
            }

            this.table.Remove(tag);
        }

        public void Load()
        {
            var file = this.DefaultFile;

            if (!File.Exists(file))
            {
                return;
            }

            using (var sr = new StreamReader(file, new UTF8Encoding(false)))
            {
                try
                {
                    if (sr.BaseStream.Length > 0)
                    {
                        var xs = new XmlSerializer(table.GetType());
                        var data = xs.Deserialize(sr) as IList<Tag>;

                        this.table.Clear();
                        foreach (var item in data)
                        {
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

        public void Save()
        {
            var file = this.DefaultFile;

            FileHelper.CreateDirectory(file);

            using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
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

                FileHelper.CreateDirectory(backupFile);

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
