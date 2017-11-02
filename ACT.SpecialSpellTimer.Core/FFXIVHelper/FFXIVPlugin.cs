using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Common;
using FFXIV.Framework.FFXIVHelper;

namespace ACT.SpecialSpellTimer.FFXIVHelper
{
    public class FFXIVPlugin
    {
        #region Singleton

        private static FFXIVPlugin instance;

        public static FFXIVPlugin Instance => instance;

        public static void Initialize() => instance = new FFXIVPlugin();

        public static void Free() => instance = null;

        private FFXIVPlugin()
        {
        }

        #endregion Singleton

        /// <summary>
        /// FFXIV_ACT_Plugin
        /// </summary>
        private volatile dynamic plugin;

        /// <summary>
        /// FFXIV_ACT_Plugin.Parse.CombatantHistory
        /// </summary>
        private volatile dynamic pluginCombatantHistory;

        /// <summary>
        /// FFXIV_ACT_Plugin.MemoryScanSettings
        /// </summary>
        private volatile dynamic pluginConfig;

        /// <summary>
        /// FFXIV_ACT_Plugin.Parse.LogParse
        /// </summary>
        private volatile dynamic pluginLogParse;

        /// <summary>
        /// FFXIV_ACT_Plugin.Memory.Memory
        /// </summary>
        private volatile dynamic pluginMemory;

        /// <summary>
        /// FFXIV_ACT_Plugin.Memory.ScanCombatants
        /// </summary>
        private volatile dynamic pluginScancombat;

        /// <summary>
        /// ACTプラグイン型のプラグインオブジェクトのインスタンス
        /// </summary>
        private IActPluginV1 ActPlugin => (IActPluginV1)this.plugin;

        public bool IsAvalable
        {
            get
            {
                if (ActGlobals.oFormActMain == null ||
                    ActGlobals.oFormActMain.IsDisposed ||
                    !ActGlobals.oFormActMain.IsHandleCreated ||
                    !ActGlobals.oFormActMain.Visible ||
                    this.plugin == null ||
                    this.pluginConfig == null ||
                    this.pluginScancombat == null ||
                    this.pluginCombatantHistory == null ||
                    this.Process == null)
                {
                    return false;
                }

                return true;
            }
        }

        public Process Process => (Process)this.pluginConfig?.Process;

        public IReadOnlyList<Zone> ZoneList => this.zoneList;

        /// <summary>
        /// ACTプラグインアセンブリ
        /// </summary>
        private Assembly FFXIVPluginAssembly => this.ActPlugin?.GetType()?.Assembly;

        #region Combatants

        private readonly IReadOnlyList<Combatant> EmptyCombatantList = new List<Combatant>();

        private volatile IReadOnlyDictionary<uint, Combatant> combatantDictionary;
        private volatile IReadOnlyList<Combatant> combatantList;
        private object combatantListLock = new object();

        private volatile List<uint> currentPartyIDList = new List<uint>();
        private object currentPartyIDListLock = new object();

#if false
        // とりあえずはリストを直接外部に公開しないことにする
        public IReadOnlyDictionary<uint, Combatant> CombatantDictionary => this.combatantDictionary;
        public IReadOnlyList<Combatant> CombatantList => this.combatantList;
        public object CombatantListLock => this.combatantListLock;

        public IReadOnlyCollection<uint> CurrentPartyIDList => this.currentPartyIDList;
        public object CurrentPartyIDListLock => this.currentPartyIDListLock;
#endif

#if DEBUG

        private readonly IReadOnlyList<Combatant> DummyCombatants = new List<Combatant>()
        {
            new Combatant()
            {
                ID = 1,
                Name = "Me Taro",
                MaxHP = 30000,
                CurrentHP = 30000,
                MaxMP = 12000,
                CurrentMP = 12000,
                MaxTP = 3000,
                CurrentTP = 3000,
                Job = (int)JobIDs.PLD,
                type = (byte)MobType.Player,
            },

            new Combatant()
            {
                ID = 2,
                Name = "Warrior Jiro",
                MaxHP = 30000,
                CurrentHP = 30000,
                MaxMP = 12000,
                CurrentMP = 12000,
                MaxTP = 3000,
                CurrentTP = 3000,
                Job = (int)JobIDs.WAR,
                type = (byte)MobType.Player,
            },

            new Combatant()
            {
                ID = 3,
                Name = "White Hanako",
                MaxHP = 30000,
                CurrentHP = 30000,
                MaxMP = 12000,
                CurrentMP = 12000,
                MaxTP = 3000,
                CurrentTP = 3000,
                Job = (int)JobIDs.WHM,
                type = (byte)MobType.Player,
            },

            new Combatant()
            {
                ID = 4,
                Name = "Astro Himeko",
                MaxHP = 30000,
                CurrentHP = 30000,
                MaxMP = 12000,
                CurrentMP = 12000,
                MaxTP = 3000,
                CurrentTP = 3000,
                Job = (int)JobIDs.AST,
                type = (byte)MobType.Player,
            },
        };

        private readonly uint[] DummyPartyList = new uint[]
        {
            1, 2, 3, 4, 5, 6, 7, 8
        };

#endif

        #endregion Combatants

        #region Resources

        private volatile IReadOnlyDictionary<int, Buff> buffList = new Dictionary<int, Buff>();
        private volatile IReadOnlyDictionary<int, Skill> skillList = new Dictionary<int, Skill>();
        private volatile IReadOnlyList<Zone> zoneList = new List<Zone>();

        #endregion Resources

        #region Start/End

        private System.Timers.Timer attachFFXIVPluginWorker;
        private ThreadWorker scanFFXIVWorker;

        public void End()
        {
            this.scanFFXIVWorker?.Abort();

            this.attachFFXIVPluginWorker?.Stop();
            this.attachFFXIVPluginWorker.Dispose();
            this.attachFFXIVPluginWorker = null;
        }

        public void Start()
        {
            this.attachFFXIVPluginWorker = new System.Timers.Timer();
            this.attachFFXIVPluginWorker.AutoReset = true;
            this.attachFFXIVPluginWorker.Interval = 5000;
            this.attachFFXIVPluginWorker.Elapsed += (s, e) =>
            {
                this.Attach();
                this.LoadZoneList();
                this.LoadSkillToFFXIVPlugin();
            };

            this.scanFFXIVWorker = new ThreadWorker(() =>
            {
                if (!this.IsAvalable)
                {
                    Thread.Sleep(5000);
#if !DEBUG
                    return;
#endif
                }

                this.RefreshCombatantList();
                this.RefreshCurrentPartyIDList();
            },
            Settings.Default.LogPollSleepInterval,
            nameof(this.attachFFXIVPluginWorker));

            this.attachFFXIVPluginWorker.Start();
            this.scanFFXIVWorker.Run();

            // XIVDBをロードする
            XIVDB.Instance.Load();
        }

        #endregion Start/End

        #region Methods

        public IReadOnlyDictionary<uint, Combatant> GetCombatantDictionaly()
        {
            if (this.combatantDictionary == null)
            {
                return null;
            }

            lock (this.combatantListLock)
            {
                return new Dictionary<uint, Combatant>(
                    (Dictionary<uint, Combatant>)this.combatantDictionary);
            }
        }

        public IReadOnlyList<Combatant> GetCombatantList()
        {
            if (this.combatantList == null)
            {
                return this.EmptyCombatantList;
            }

            lock (this.combatantListLock)
            {
                return new List<Combatant>(this.combatantList);
            }
        }

        public int GetCurrentZoneID()
        {
            return this.pluginCombatantHistory?.CurrentZoneID ?? 0;
        }

        public IReadOnlyList<Combatant> GetPartyList()
        {
            var combatants = this.GetCombatantDictionaly();

            if (combatants == null ||
                combatants.Count < 1)
            {
                return this.EmptyCombatantList;
            }

            var partyIDs = default(List<uint>);

            lock (this.currentPartyIDListLock)
            {
                partyIDs = new List<uint>(this.currentPartyIDList);
            }

            var partyList = (
                from id in partyIDs
                where
                combatants.ContainsKey(id)
                select
                combatants[id]).ToList();

            return partyList;
        }

        /// <summary>
        /// パーティをロールで分類して取得する
        /// </summary>
        /// <returns>
        /// ロールで分類したパーティリスト</returns>
        public IReadOnlyList<CombatantsByRole> GetPatryListByRole()
        {
            var list = new List<CombatantsByRole>();

            var partyList = this.GetPartyList();

            var tanks = partyList
                .Where(x => x.AsJob().Role == Roles.Tank)
                .ToList();

            var dpses = partyList
                .Where(x =>
                    x.AsJob().Role == Roles.MeleeDPS ||
                    x.AsJob().Role == Roles.RangeDPS ||
                    x.AsJob().Role == Roles.MagicDPS)
                .ToList();

            var melees = partyList
                .Where(x => x.AsJob().Role == Roles.MeleeDPS)
                .ToList();

            var ranges = partyList
                .Where(x => x.AsJob().Role == Roles.RangeDPS)
                .ToList();

            var magics = partyList
                .Where(x => x.AsJob().Role == Roles.MagicDPS)
                .ToList();

            var healers = partyList
                .Where(x => x.AsJob().Role == Roles.Healer)
                .ToList();

            if (tanks.Any())
            {
                list.Add(new CombatantsByRole(
                    Roles.Tank,
                    "TANK",
                    tanks));
            }

            if (dpses.Any())
            {
                list.Add(new CombatantsByRole(
                    Roles.DPS,
                    "DPS",
                    dpses));
            }

            if (melees.Any())
            {
                list.Add(new CombatantsByRole(
                    Roles.MeleeDPS,
                    "MELEE",
                    melees));
            }

            if (ranges.Any())
            {
                list.Add(new CombatantsByRole(
                    Roles.RangeDPS,
                    "RANGE",
                    ranges));
            }

            if (magics.Any())
            {
                list.Add(new CombatantsByRole(
                    Roles.MagicDPS,
                    "MAGIC",
                    magics));
            }

            if (healers.Any())
            {
                list.Add(new CombatantsByRole(
                    Roles.Healer,
                    "HEALER",
                    healers));
            }

            return list;
        }

        public Combatant GetPlayer()
        {
            if (this.combatantList == null)
            {
                return null;
            }

            lock (this.combatantListLock)
            {
                return this.combatantList.FirstOrDefault();
            }
        }

        public void RefreshCombatantList()
        {
            if (!this.IsAvalable)
            {
#if DEBUG
                lock (this.combatantListLock)
                {
                    foreach (var entity in this.DummyCombatants)
                    {
                        entity.SetName(entity.Name);
                    }

                    this.combatantList = this.DummyCombatants;
                    this.combatantDictionary = this.DummyCombatants.ToDictionary(x => x.ID);
                }
#endif

                return;
            }

            dynamic list = this.pluginScancombat.GetCombatantList();
            var count = (int)list.Count;

            var newList = new List<Combatant>(count);
            var newDictionary = new Dictionary<uint, Combatant>(count);

            foreach (dynamic item in list.ToArray())
            {
                if (item == null)
                {
                    continue;
                }

                var combatant = new Combatant();

                combatant.ID = (uint)item.ID;
                combatant.OwnerID = (uint)item.OwnerID;
                combatant.Job = (int)item.Job;
                combatant.type = (byte)item.type;
                combatant.Level = (int)item.Level;
                combatant.CurrentHP = (int)item.CurrentHP;
                combatant.MaxHP = (int)item.MaxHP;
                combatant.CurrentMP = (int)item.CurrentMP;
                combatant.MaxMP = (int)item.MaxMP;
                combatant.CurrentTP = (int)item.CurrentTP;

                var name = (string)item.Name;

                // 名前を登録する
                // TYPEによって分岐するため先にTYPEを設定しておくこと
                combatant.SetName(name);

                newList.Add(combatant);
                newDictionary.Add(combatant.ID, combatant);
            }

            lock (this.combatantListLock)
            {
                this.combatantList = null;
                this.combatantDictionary = null;

                this.combatantList = newList;
                this.combatantDictionary = newDictionary;
            }
        }

        public void RefreshCurrentPartyIDList()
        {
            if (!this.IsAvalable)
            {
#if DEBUG
                lock (this.currentPartyIDListLock)
                {
                    this.currentPartyIDList = new List<uint>(this.DummyPartyList);
                }
#endif
                return;
            }

            var partyList = pluginScancombat.GetCurrentPartyList(
                out int partyCount) as List<uint>;

            lock (this.currentPartyIDListLock)
            {
                this.currentPartyIDList = partyList;
            }
        }

        /// <summary>
        /// 文中に含まれるパーティメンバの名前を設定した形式に置換する
        /// </summary>
        /// <param name="text">置換対象のテキスト</param>
        /// <returns>
        /// 置換後のテキスト</returns>
        public string ReplacePartyMemberName(
            string text)
        {
            var r = text;

            var party = this.GetPartyList();

            foreach (var pc in party)
            {
                if (string.IsNullOrEmpty(pc.Name) ||
                    string.IsNullOrEmpty(pc.NameFI) ||
                    string.IsNullOrEmpty(pc.NameIF) ||
                    string.IsNullOrEmpty(pc.NameII))
                {
                    continue;
                }

                switch (Settings.Default.PCNameInitialOnDisplayStyle)
                {
                    case NameStyles.FullName:
                        r = r.Replace(pc.NameFI, pc.Name);
                        r = r.Replace(pc.NameIF, pc.Name);
                        r = r.Replace(pc.NameII, pc.Name);
                        break;

                    case NameStyles.FullInitial:
                        r = r.Replace(pc.Name, pc.NameFI);
                        break;

                    case NameStyles.InitialFull:
                        r = r.Replace(pc.Name, pc.NameIF);
                        break;

                    case NameStyles.InitialInitial:
                        r = r.Replace(pc.Name, pc.NameII);
                        break;
                }
            }

            return r;
        }

        private void Attach()
        {
            this.AttachPlugin();
            this.AttachScanMemory();
        }

        private void AttachPlugin()
        {
            if (this.plugin != null ||
                ActGlobals.oFormActMain == null ||
                ActGlobals.oFormActMain.IsDisposed ||
                !ActGlobals.oFormActMain.IsHandleCreated ||
                !ActGlobals.oFormActMain.Visible)
            {
                return;
            }

            var ffxivPlugin = (
                from x in ActGlobals.oFormActMain.ActPlugins
                where
                x.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                x.lblPluginStatus.Text.ToUpper().Contains("FFXIV Plugin Started.".ToUpper())
                select
                x.pluginObj).FirstOrDefault();

            if (ffxivPlugin != null)
            {
                this.plugin = ffxivPlugin;
                Logger.Write("attached ffxiv plugin.");
            }
        }

        private void AttachScanMemory()
        {
            if (this.plugin == null)
            {
                return;
            }

            FieldInfo fi;

            if (this.pluginLogParse == null)
            {
                fi = this.plugin?.GetType().GetField(
                    "_LogParse",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                this.pluginLogParse = fi?.GetValue(this.plugin);
            }

            if (this.pluginCombatantHistory == null)
            {
                var settings = this.pluginLogParse?.Settings;
                if (settings != null)
                {
                    fi = settings?.GetType().GetField(
                        "CombatantHistory",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                    this.pluginCombatantHistory = fi?.GetValue(settings);
                }
            }

            if (this.pluginMemory == null)
            {
                fi = this.plugin?.GetType().GetField(
                    "_Memory",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                this.pluginMemory = fi?.GetValue(this.plugin);
            }

            if (this.pluginMemory == null)
            {
                return;
            }

            if (this.pluginConfig == null)
            {
                fi = this?.pluginMemory?.GetType().GetField(
                    "_config",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                this.pluginConfig = fi?.GetValue(this.pluginMemory);
            }

            if (this.pluginConfig == null)
            {
                return;
            }

            if (this.pluginScancombat == null)
            {
                fi = this.pluginConfig?.GetType().GetField(
                    "ScanCombatants",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                this.pluginScancombat = fi?.GetValue(this.pluginConfig);

                Logger.Write("attached ffxiv plugin ScanCombatants.");
            }
        }

        private void LoadBuffList()
        {
            if (this.buffList.Any())
            {
                return;
            }

            if (this.plugin == null)
            {
                return;
            }

            var asm = this.plugin.GetType().Assembly;

            var language = Settings.Default.Language;
            var resourcesName = $"FFXIV_ACT_Plugin.Resources.BuffList_{language.ToUpper()}.txt";

            using (var st = asm.GetManifestResourceStream(resourcesName))
            {
                if (st == null)
                {
                    return;
                }

                var newList = new Dictionary<int, Buff>();

                using (var sr = new StreamReader(st))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var values = line.Split('|');
                            if (values.Length >= 2)
                            {
                                var buff = new Buff()
                                {
                                    ID = int.Parse(values[0], NumberStyles.HexNumber),
                                    Name = values[1].Trim()
                                };

                                newList.Add(buff.ID, buff);
                            }
                        }
                    }
                }

                this.buffList = newList;
                Logger.Write("buff list loaded.");
            }
        }

        private void LoadSkillList()
        {
            if (this.skillList.Any())
            {
                return;
            }

            if (this.plugin == null)
            {
                return;
            }

            var asm = this.plugin.GetType().Assembly;

            var language = Settings.Default.Language;
            var resourcesName = $"FFXIV_ACT_Plugin.Resources.SkillList_{language.ToUpper()}.txt";

            using (var st = asm.GetManifestResourceStream(resourcesName))
            {
                if (st == null)
                {
                    return;
                }

                var newList = new Dictionary<int, Skill>();

                using (var sr = new StreamReader(st))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var values = line.Split('|');
                            if (values.Length >= 2)
                            {
                                var skill = new Skill()
                                {
                                    ID = int.Parse(values[0], NumberStyles.HexNumber),
                                    Name = values[1].Trim()
                                };

                                newList.Add(skill.ID, skill);
                            }
                        }
                    }
                }

                this.skillList = newList;
                Logger.Write("skill list loaded.");
            }
        }

        private void LoadZoneList()
        {
            if (this.zoneList.Any())
            {
                return;
            }

            if (this.plugin == null)
            {
                return;
            }

            var newList = new List<Zone>();

            var asm = this.plugin.GetType().Assembly;

            var language = "EN";
            var resourcesName = $"FFXIV_ACT_Plugin.Resources.ZoneList_{language.ToUpper()}.txt";

            using (var st = asm.GetManifestResourceStream(resourcesName))
            {
                if (st == null)
                {
                    return;
                }

                using (var sr = new StreamReader(st))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var values = line.Split('|');
                            if (values.Length >= 2)
                            {
                                var zone = new Zone()
                                {
                                    ID = int.Parse(values[0]),
                                    Name = values[1].Trim()
                                };

                                newList.Add(zone);
                            }
                        }
                    }
                }

                // ユーザで任意に追加したゾーンを読み込む
                this.LoadZoneListAdded(newList);

                // 新しいゾーンリストをセットする
                this.zoneList = newList;

                // ゾーンリストを翻訳する
                this.TranslateZoneList();

                Logger.Write("zone list loaded.");
            }
        }

        private void LoadZoneListAdded(List<Zone> baseZoneList)
        {
            try
            {
                var dir = XIVDB.Instance.ResourcesDirectory;
                var file = Path.Combine(dir, "Zones.csv");

                if (!File.Exists(file))
                {
                    return;
                }

                using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var values = line.Split(',');

                        if (values.Length >= 2)
                        {
                            var newZone = new Zone()
                            {
                                ID = int.Parse(values[0]),
                                Name = values[1].Trim(),
                                IsAddedByUser = true,
                            };

                            var oldZone = baseZoneList.FirstOrDefault(x =>
                                x.ID == newZone.ID);

                            if (oldZone != null)
                            {
                                oldZone.Name = newZone.Name;
                                oldZone.IsAddedByUser = true;
                            }
                            else
                            {
                                baseZoneList.Add(newZone);
                            }
                        }
                    }
                }

                Logger.Write($"Additional zone list loaded. {file}");
            }
            catch (Exception ex)
            {
                Logger.Write("error on load additional zone list.", ex);
            }
        }

        private void TranslateZoneList()
        {
            if (Settings.Default.Language != "JP")
            {
                return;
            }

            foreach (var zone in this.ZoneList)
            {
                var place = (
                    from x in XIVDB.Instance.PlacenameList.AsParallel()
                    where
                    string.Equals(x.NameEn, zone.Name, StringComparison.InvariantCultureIgnoreCase)
                    select
                    x).FirstOrDefault();

                if (place != null)
                {
                    zone.Name = place.Name;
                    zone.IDonDB = place.ID;
                }
                else
                {
                    var area = (
                        from x in XIVDB.Instance.AreaList.AsParallel()
                        where
                        string.Equals(x.NameEn, zone.Name, StringComparison.InvariantCultureIgnoreCase)
                        select
                        x).FirstOrDefault();

                    if (area != null)
                    {
                        zone.Name = area.Name;
                        zone.IDonDB = area.ID;
                    }
                }
            }
        }

        private volatile bool loadedSkillToFFXIVPlugin = false;

        /// <summary>
        /// FFXIVプラグインに足りないスキルをロードする
        /// </summary>
        public void LoadSkillToFFXIVPlugin()
        {
            if (!Settings.Default.ToComplementUnknownSkill)
            {
                return;
            }

            if (this.loadedSkillToFFXIVPlugin)
            {
                return;
            }

            if (this.plugin == null)
            {
                return;
            }

            var t = (this.plugin as object).GetType().Module.Assembly.GetType("FFXIV_ACT_Plugin.Resources.SkillList");
            var obj = t.GetField(
                "_instance",
                BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);

            var list = obj.GetType().GetField(
                "_SkillList",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj);

            var pluginSkillList = list as SortedDictionary<int, string>;

            foreach (var entry in XIVDB.Instance.SkillList)
            {
                if (!pluginSkillList.ContainsKey(entry.Key))
                {
                    pluginSkillList[entry.Key] = entry.Value.Name;
                }
            }

            if (XIVDB.Instance.SkillList.Any())
            {
                this.loadedSkillToFFXIVPlugin = true;
                Logger.Write("extra skill list loaded.");
            }
        }

        #endregion Methods

        #region Sub classes

        public class CombatantsByRole
        {
            public CombatantsByRole(
                Roles roleType,
                string roleLabel,
                IReadOnlyList<Combatant> combatants)
            {
                this.RoleType = roleType;
                this.RoleLabel = roleLabel;
                this.Combatants = combatants;
            }

            public IReadOnlyList<Combatant> Combatants { get; set; }
            public string RoleLabel { get; set; }
            public Roles RoleType { get; set; }
        }

        #endregion Sub classes
    }
}
