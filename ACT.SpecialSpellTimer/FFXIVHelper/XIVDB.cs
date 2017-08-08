using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ACT.SpecialSpellTimer.FFXIVHelper
{
    public class XIVDB
    {
        #region Singleton

        private static XIVDB instance = new XIVDB();

        public static XIVDB Instance => instance;

        #endregion Singleton

        #region Resources Files

        public string AreaFile => Path.Combine(
            this.ResourcesDirectory + @"\xivdb",
            @"Instance.csv");

        public string PlacenameFile => Path.Combine(
                    this.ResourcesDirectory + @"\xivdb",
            @"Placename.csv");

        #endregion Resources Files

        private readonly List<Area> areaList = new List<Area>();
        private readonly List<Placename> placenameList = new List<Placename>();

        public IReadOnlyList<Area> AreaList => this.areaList;
        public IReadOnlyList<Placename> PlacenameList => this.placenameList;

        public string ResourcesDirectory
        {
            get
            {
                // ACTのパスを取得する
                var asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    var actDirectory = Path.GetDirectoryName(asm.Location);
                    var resourcesUnderAct = Path.Combine(actDirectory, @"resources");

                    if (Directory.Exists(resourcesUnderAct))
                    {
                        return resourcesUnderAct;
                    }
                }

                // 自身の場所を取得する
                var selfDirectory = SpecialSpellTimerPlugin.Location ?? string.Empty;
                var resourcesUnderThis = Path.Combine(selfDirectory, @"resources");

                if (Directory.Exists(resourcesUnderThis))
                {
                    return resourcesUnderThis;
                }

                return string.Empty;
            }
        }

        public void Load()
        {
            this.LoadArea();
            this.LoadPlacename();
        }

        private void LoadArea()
        {
            if (!File.Exists(this.AreaFile))
            {
                return;
            }

            this.areaList.Clear();

            using (var sr = new StreamReader(this.AreaFile, new UTF8Encoding(false)))
            {
                // ヘッダを飛ばす
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    var values = line.Split(',');
                    if (values.Length >= 3)
                    {
                        var entry = new Area()
                        {
                            ID = int.Parse(values[0]),
                            NameEn = values[1],
                            Name = values[2]
                        };

                        this.areaList.Add(entry);
                    }
                }
            }
        }

        private void LoadPlacename()
        {
            if (!File.Exists(this.PlacenameFile))
            {
                return;
            }

            this.placenameList.Clear();

            using (var sr = new StreamReader(this.PlacenameFile, new UTF8Encoding(false)))
            {
                // ヘッダを飛ばす
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    var values = line.Split(',');
                    if (values.Length >= 3)
                    {
                        var entry = new Placename()
                        {
                            ID = int.Parse(values[0]),
                            NameEn = values[1],
                            Name = values[2]
                        };

                        this.placenameList.Add(entry);
                    }
                }
            }
        }

        #region Sub classes

        /// <summary>
        /// Area ただしXIVDB上の呼称では「Instance」Area
        /// </summary>
        public class Area
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string NameEn { get; set; }
        }

        public class Placename
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string NameEn { get; set; }
        }

        #endregion Sub classes
    }
}
