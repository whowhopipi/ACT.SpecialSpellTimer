namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;
    using System.Collections;

    /// <summary>
    /// Type of the entity
    /// </summary>
    public enum MobType : byte
    {
        Unknown = 0,
        Player = 0x01,
        Mob = 0x02,
        NPC = 0x03,
        Aetheryte = 0x05,
        Gathering = 0x06,
        Minion = 0x09
    }

    public static partial class FF14PluginHelper
    {
        private static object lockObject = new object();
        private static object plugin;
        private static dynamic pluginConfig;
        private static object pluginMemory;
        private static dynamic pluginScancombat;
        private static volatile IReadOnlyList<Zone> zoneList;

        public static Process GetFFXIVProcess
        {
            get
            {
                try
                {
                    Initialize();

                    if (pluginConfig == null)
                    {
                        return null;
                    }

                    var process = pluginConfig.Process;

                    return (Process)process;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static List<Combatant> GetCombatantList()
        {
            Initialize();

            var result = new List<Combatant>();

            if (plugin == null)
            {
                return result;
            }

            if (GetFFXIVProcess == null)
            {
                return result;
            }

            if (pluginScancombat == null)
            {
                return result;
            }

            dynamic list = pluginScancombat.GetCombatantList();
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

        public static List<uint> GetCurrentPartyList(
            out int partyCount)
        {
            Initialize();

            var partyList = new List<uint>();
            partyCount = 0;

            if (plugin == null)
            {
                return partyList;
            }

            if (GetFFXIVProcess == null)
            {
                return partyList;
            }

            if (pluginScancombat == null)
            {
                return partyList;
            }

            partyList = pluginScancombat.GetCurrentPartyList(
                out partyCount) as List<uint>;

            return partyList;
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

        public static IReadOnlyList<Zone> GetZoneList()
        {
            var list = zoneList;
            if (list != null && list.Count() > 0)
            {
                return list;
            }

            Initialize();

            if (plugin == null)
            {
                return Enumerable.Empty<Zone>().ToList();
            }

            var newList = new List<Zone>();

            // Zone
            {
                var t = plugin.GetType().Module.Assembly.GetType("FFXIV_ACT_Plugin.Resources.ZoneList");
                var obj = t.GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var s = t.GetField("_Zones", BindingFlags.NonPublic | BindingFlags.Instance).ReflectedType;
                IDictionary zonelist = (IDictionary)t.GetField("_Zones", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
                var listType = zonelist.GetType();
                var GetEnumeratorMethod = listType.GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.Instance);
                var ClearMethod = listType.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);
                var AddMethod = listType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
                var zoneType = AddMethod.GetParameters()[1].ParameterType;

                var idField = zoneType.GetField("id", BindingFlags.Public | BindingFlags.Instance);
                var nameField = zoneType.GetField("name", BindingFlags.Public | BindingFlags.Instance);

                foreach (DictionaryEntry entry in zonelist)
                {
                    var zone = entry.Value;
                    newList.Add(new Zone()
                    {
                        ID = (int)idField.GetValue(zone),
                        Name = (string)nameField.GetValue(zone)
                    });
                }
            }

            newList = newList.OrderBy(x => x.ID).ToList();
            zoneList = newList;
            return newList;
        }

        public static void Initialize()
        {
            // FFXIV以外で使用する？
            if (Settings.Default.UseOtherThanFFXIV)
            {
                // 何もしない
                return;
            }

            lock (lockObject)
            {
                if (!ActGlobals.oFormActMain.Visible)
                {
                    return;
                }

                if (plugin == null)
                {
                    foreach (var item in ActGlobals.oFormActMain.ActPlugins)
                    {
                        if (item.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                            item.lblPluginStatus.Text.ToUpper() == "FFXIV Plugin Started.".ToUpper())
                        {
                            plugin = item.pluginObj;

                            Logger.Write($"{item.pluginFile.Name} found. and started.");
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
        }
    }

    public class Combatant
    {
        public int CastBuffID;
        public float CastDurationCurrent;
        public float CastDurationMax;
        public uint CastTargetID;
        public int CurrentCP;
        public int CurrentGP;
        public int CurrentHP;
        public int CurrentMP;
        public int CurrentTP;
        public uint ID;
        public bool IsCasting;
        public int Job;
        public int Level;
        public int MaxCP;
        public int MaxGP;
        public int MaxHP;
        public int MaxMP;
        public int MaxTP;
        public string Name;
        public int Order;
        public uint OwnerID;
        public float PosX;
        public float PosY;
        public float PosZ;
        public byte type;
        public MobType MobType => (MobType)this.type;

        public SpecialSpellTimer.Job AsJob()
        {
            return SpecialSpellTimer.Job.FromId(Job);
        }

        public float GetDistance(Combatant target) =>
            (float)Math.Sqrt(
                Math.Pow(this.PosX - target.PosX, 2) +
                Math.Pow(this.PosY - target.PosY, 2) +
                Math.Pow(this.PosZ - target.PosZ, 2));

        public float GetHorizontalDistance(Combatant target) =>
                            (float)Math.Sqrt(
                Math.Pow(this.PosX - target.PosX, 2) +
                Math.Pow(this.PosY - target.PosY, 2));
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
}