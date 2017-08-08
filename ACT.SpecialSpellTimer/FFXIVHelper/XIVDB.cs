using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public string SkillFile => Path.Combine(
            this.ResourcesDirectory + @"\xivdb",
            @"Action.csv");

        #endregion Resources Files

        #region Resources Lists

        private readonly List<Area> areaList = new List<Area>();
        private readonly List<Placename> placenameList = new List<Placename>();
        private readonly Dictionary<int, Skill> skillList = new Dictionary<int, Skill>();

        public IReadOnlyList<Area> AreaList => this.areaList;
        public IReadOnlyList<Placename> PlacenameList => this.placenameList;
        public IReadOnlyDictionary<int, Skill> SkillList => this.skillList;

        #endregion Resources Lists

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

        public Skill FindSkill(
            int ID)
        {
            if (this.skillList.ContainsKey(ID))
            {
                return this.skillList[ID];
            }

            return null;
        }

        public Skill FindSkill(
            string ID)
        {
            try
            {
                var IDAsInt = Convert.ToInt32(ID, 16);
                return this.FindSkill(IDAsInt);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Load()
        {
            Task.WaitAll(
                Task.Run(() => this.LoadArea()),
                Task.Run(() => this.LoadPlacename()),
                Task.Run(() => this.LoadSkill()));
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

        private void LoadSkill()
        {
            if (!File.Exists(this.SkillFile))
            {
                return;
            }

            this.skillList.Clear();

            using (var sr = new StreamReader(this.SkillFile, new UTF8Encoding(false)))
            {
                // ヘッダを飛ばす
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    var values = line.Split(',');
                    if (values.Length >= 5)
                    {
                        var entry = new Skill()
                        {
                            ID = int.Parse(values[0]),
                            Name = values[4]
                        };

                        this.skillList[entry.ID] = entry;
                    }
                }
            }
        }

        #region Sub classes

        /// <summary>
        /// Area ただしXIVDB上の呼称では「Instance」
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

        /// <summary>
        /// Skill ただしXIVDB上の呼称では「Action」
        /// </summary>
        public class Skill
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        #endregion Sub classes
    }
}
