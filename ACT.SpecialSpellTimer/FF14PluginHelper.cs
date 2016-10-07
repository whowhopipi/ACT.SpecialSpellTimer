namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;

    public static partial class FF14PluginHelper
    {
        private static object plugin;
        private static object pluginMemory;
        private static dynamic pluginConfig;
        private static dynamic pluginScancombat;
        private static volatile IReadOnlyList<Zone> zoneList;

        private static readonly List<Combatant> EmptyCombatants = new List<Combatant>();
        private static object combatantsRaw;
        private static List<Combatant> combatants = new List<Combatant>();
        private static Task scanCombatTask;
        private static bool scanCombatTaskRunning;
        private static object combatantsLock = new object();

        private static void Initialize()
        {
            // FFXIV以外で使用する？
            if (Settings.Default.UseOtherThanFFXIV)
            {
                // 何もしない
                return;
            }

            if (!ActGlobals.oFormActMain.Visible)
            {
                return;
            }

            if (plugin == null)
            {
                foreach (var item in ActGlobals.oFormActMain.ActPlugins)
                {
                    if (item.pluginFile.Name.ToUpper() == "FFXIV_ACT_Plugin.dll".ToUpper() &&
                        item.lblPluginStatus.Text.ToUpper() == "FFXIV Plugin Started.".ToUpper())
                    {
                        plugin = item.pluginObj;

                        Logger.Write("FFXIV_ACT_Plugin.dll found. and started.");
                        break;
                    }
                }
            }

            if (plugin != null)
            {
                FieldInfo fi;

                if (pluginMemory == null)
                {
                    fi = plugin.GetType().GetField("_Memory", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                    pluginMemory = fi.GetValue(plugin);
                }

                if (pluginMemory == null)
                {
                    return;
                }

                if (pluginConfig == null)
                {
                    fi = pluginMemory.GetType().GetField("_config", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                    pluginConfig = fi.GetValue(pluginMemory);
                }

                if (pluginConfig == null)
                {
                    return;
                }

                if (pluginScancombat == null)
                {
                    fi = pluginConfig.GetType().GetField("ScanCombatants", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                    pluginScancombat = fi.GetValue(pluginConfig);
                }
            }
        }

        public static Process FFXIVProcess =>
            (Process)pluginConfig?.Process;

        public static List<Combatant> Combatants
        {
            get { lock (combatantsLock) { return combatants ?? EmptyCombatants; } }
        }

        public static void Run()
        {
            // FFXIV以外で使用する？
            if (Settings.Default.UseOtherThanFFXIV)
            {
                return;
            }

            if (scanCombatTaskRunning)
            {
                return;
            }

            scanCombatTaskRunning = true;

            scanCombatTask = new Task(() =>
            {
                while (scanCombatTaskRunning)
                {
                    Thread.Sleep(50);

                    try
                    {
                        Initialize();

                        if (pluginConfig == null ||
                            pluginScancombat == null)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        RefreshCombatants();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(
                            "Catch exception at ScanCombatTask.\n" +
                            ex.ToString());
                    }
                }
            });

            scanCombatTask.Start();
        }

        public static void Stop()
        {
            scanCombatTaskRunning = false;

            if (scanCombatTask != null)
            {
                scanCombatTask.Wait();
            }
        }

        private static void RefreshCombatants()
        {
            if (pluginScancombat == null)
            {
                return;
            }

            // Combatantsの参照を取得していなければ取得する
            if (combatantsRaw == null)
            {
                var fi = (pluginScancombat as object).GetType().GetField(
                    "_Combatants",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);

                combatantsRaw = fi.GetValue(pluginScancombat);
            }

            // Combatantsを事前バインド型に詰め替える
            lock (combatantsLock)
            {
                combatants.Clear();

                var list = combatantsRaw as IReadOnlyList<dynamic>;
                foreach (var item in list)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var c = new Combatant();

                    c.ID = (uint)item.ID;
                    c.OwnerID = (uint)item.OwnerID;
                    c.Job = (int)item.Job;
                    c.Name = (string)item.Name;
                    c.type = (byte)item.type;
                    c.Level = (int)item.Level;
                    c.CurrentHP = (int)item.CurrentHP;
                    c.MaxHP = (int)item.MaxHP;
                    c.CurrentMP = (int)item.CurrentMP;
                    c.MaxMP = (int)item.MaxMP;
                    c.CurrentTP = (int)item.CurrentTP;

                    combatants.Add(c);
                }
            }
        }

#if false
        public static List<Combatant> GetCombatantList()
        {
            Initialize();

            var result = new List<Combatant>();

            if (plugin == null ||
                FFXIVProcess == null ||
                pluginScancombat == null)
            {
                return result;
            }

            if (combatantsRaw == null)
            {
                var fi = (pluginScancombat as object).GetType().GetField(
                    "_Combatants",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);

                combatantsRaw = fi.GetValue(pluginScancombat);
            }

            var list = combatantsRaw as List<dynamic>;
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
                combatant.Name = (string)item.Name;
                combatant.type = (byte)item.type;
                combatant.Level = (int)item.Level;
                combatant.CurrentHP = (int)item.CurrentHP;
                combatant.MaxHP = (int)item.MaxHP;
                combatant.CurrentMP = (int)item.CurrentMP;
                combatant.MaxMP = (int)item.MaxMP;
                combatant.CurrentTP = (int)item.CurrentTP;

                result.Add(combatant);
            }

            return result;
        }
#endif

        public static List<uint> GetCurrentPartyList(
            out int partyCount)
        {
            var partyList = new List<uint>();
            partyCount = 0;

            if (pluginScancombat == null ||
                FFXIVProcess == null)
            {
                return partyList;
            }

            partyList = pluginScancombat.GetCurrentPartyList(
                out partyCount) as List<uint>;

            return partyList;
        }

        public static IReadOnlyList<Zone> GetZoneList()
        {
            var list = zoneList;
            if (list != null && list.Count() > 0)
            {
                return list;
            }

            if (plugin == null)
            {
                return Enumerable.Empty<Zone>().ToList();
            }

            var newList = new List<Zone>();

            var asm = plugin.GetType().Assembly;

            using (var st = asm.GetManifestResourceStream("FFXIV_ACT_Plugin.Resources.ZoneList_EN.txt"))
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
                            newList.Add(new Zone()
                            {
                                ID = int.Parse(values[0]),
                                Name = values[1].Trim()
                            });
                        }
                    }
                }
            }

            using (var st = asm.GetManifestResourceStream("FFXIV_ACT_Plugin.Resources.ZoneList_Custom.txt"))
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
                            newList.Add(new Zone()
                            {
                                ID = int.Parse(values[0]),
                                Name = values[1].Trim()
                            });
                        }
                    }
                }
            }

            newList = newList.OrderBy(x => x.ID).ToList();
            zoneList = newList;
            return newList;
        }

        public static int GetCurrentZoneID()
        {
            var currentZoneName = ActGlobals.oFormActMain.CurrentZone;
            if (string.IsNullOrEmpty(currentZoneName) ||
                currentZoneName == "Unknown Zone")
            {
                return 0;
            }

            var zoneList = GetZoneList();

            if (zoneList == null ||
                zoneList.Count < 1)
            {
                return 0;
            }

            var foundZone = zoneList.AsParallel().FirstOrDefault(zone =>
                string.Equals(zone.Name, currentZoneName, StringComparison.OrdinalIgnoreCase));
            return foundZone != null ? foundZone.ID : 0;
        }
    }

    public class Combatant
    {
        public uint ID;
        public uint OwnerID;
        public int Order;
        public byte type;
        public int Job;
        public int Level;
        public string Name;
        public int CurrentHP;
        public int MaxHP;
        public int CurrentMP;
        public int MaxMP;
        public int CurrentTP;
        public int MaxTP;
        public int CurrentCP;
        public int MaxCP;
        public int CurrentGP;
        public int MaxGP;
        public bool IsCasting;
        public int CastBuffID;
        public uint CastTargetID;
        public float CastDurationCurrent;
        public float CastDurationMax;
        public float PosX;
        public float PosY;
        public float PosZ;

        public MobType MobType => (MobType)this.type;

        public float GetHorizontalDistance(Combatant target) =>
            (float)Math.Sqrt(
                Math.Pow(this.PosX - target.PosX, 2) +
                Math.Pow(this.PosY - target.PosY, 2));

        public float GetDistance(Combatant target) =>
            (float)Math.Sqrt(
                Math.Pow(this.PosX - target.PosX, 2) +
                Math.Pow(this.PosY - target.PosY, 2) +
                Math.Pow(this.PosZ - target.PosZ, 2));

        public SpecialSpellTimer.Job AsJob()
        {
            return SpecialSpellTimer.Job.FromId(Job);
        }
    }

    public class Zone
    {
        public int ID;
        public string Name;

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Type of the entity
    /// </summary>
    public enum MobType : byte
    {
        Unknown = 0,
        Player = 0x01,
        Mob = 0x02,
        NPC = 0x03,
        Type4 = 0x04,
        Aetheryte = 0x05,
        Gathering = 0x06,
        Type7 = 0x07,
        Type8 = 0x08,
        Minion = 0x09
    }
}
