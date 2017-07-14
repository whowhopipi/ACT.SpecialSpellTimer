using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer.FFXIVHelper
{
    public class FFXIV
    {
        #region Singleton

        private static FFXIV instance;

        private FFXIV()
        {
        }

        public static FFXIV Instance => (instance ?? (instance = new FFXIV()));

        #endregion Singleton

        private IReadOnlyList<Zone> zoneList;

        private IReadOnlyDictionary<uint, Combatant> combatantDictionary;
        private IReadOnlyList<Combatant> combatantList;
        private object combatantListLock = new object();

        private List<uint> currentPartyIDList = new List<uint>();
        private object currentPartyIDListLock = new object();

        /// <summary>
        /// FFXIV_ACT_Plugin
        /// </summary>
        private dynamic plugin;

        /// <summary>
        /// FFXIV_ACT_Plugin.MemoryScanSettings
        /// </summary>
        private dynamic pluginConfig;

        /// <summary>
        /// FFXIV_ACT_Plugin.Memory.Memory
        /// </summary>
        private dynamic pluginMemory;

        /// <summary>
        /// FFXIV_ACT_Plugin.Memory.ScanCombatants
        /// </summary>
        private dynamic pluginScancombat;

        /// <summary>
        /// ACTプラグイン型のプラグインオブジェクトのインスタンス
        /// </summary>
        private IActPluginV1 ActPlugin => (IActPluginV1)this.plugin;

        public IReadOnlyList<Zone> ZoneList => this.zoneList;

#if false
        // とりあえずはリストを直接外部に後悔しないことにする
        public IReadOnlyDictionary<uint, Combatant> CombatantDictionary => this.combatantDictionary;
        public IReadOnlyList<Combatant> CombatantList => this.combatantList;
        public object CombatantListLock => this.combatantListLock;

        public IReadOnlyCollection<uint> CurrentPartyIDList => this.currentPartyIDList;
        public object CurrentPartyIDListLock => this.currentPartyIDListLock;
#endif

        public bool IsAvalable
        {
            get
            {
                if (!ActGlobals.oFormActMain.Visible ||
                    this.plugin == null ||
                    this.Process == null ||
                    this.pluginScancombat == null)
                {
                    return false;
                }

                return true;
            }
        }

        public Process Process => (Process)this.pluginConfig?.Process;

        /// <summary>
        /// ACTプラグインアセンブリ
        /// </summary>
        private Assembly FFXIVPluginAssembly => this.ActPlugin?.GetType()?.Assembly;

        #region Start/End

        private Task scanFFXIVTask;
        private bool scanFFXIVTaskRunning;
        private Task task;
        private bool taskRunning;

        public void End()
        {
            if (this.task != null)
            {
                this.taskRunning = false;
                this.task.Wait();
                this.task.Dispose();
                this.task = null;
            }

            if (this.scanFFXIVTask != null)
            {
                this.scanFFXIVTaskRunning = false;
                this.scanFFXIVTask.Wait();
                this.scanFFXIVTask.Dispose();
                this.scanFFXIVTask = null;
            }
        }

        public void Start()
        {
            this.task = new Task(() =>
            {
                while (this.taskRunning)
                {
                    try
                    {
                        this.Attach();
                        this.GetZoneList();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("attach ffxiv plugin error:", ex);
                        Thread.Sleep(5000);
                    }

                    Thread.Sleep(5000);
                }
            });

            this.taskRunning = true;
            this.task.Start();

            this.scanFFXIVTask = new Task(() =>
            {
                while (this.scanFFXIVTaskRunning)
                {
                    try
                    {
                        this.RefreshCombatantList();
                        this.RefreshCurrentPartyIDList();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("scan ffxiv error:", ex);
                        Thread.Sleep(5000);
                    }

                    Thread.Sleep((int)Settings.Default.LogPollSleepInterval);
                }
            });

            this.scanFFXIVTaskRunning = true;
            this.scanFFXIVTask.Start();
        }

        #endregion Start/End

        public IReadOnlyList<Combatant> GetCombatantList()
        {
            lock (this.combatantListLock)
            {
                return new List<Combatant>(this.combatantList);
            }
        }

        public Combatant GetPlayer()
        {
            lock (this.combatantListLock)
            {
                return this.combatantList.FirstOrDefault();
            }
        }

        public IReadOnlyList<Combatant> GetPartyList()
        {
            var combatants = this.GetCombatantList();
            var paryIDs = default(List<uint>);

            lock (this.currentPartyIDList)
            {
                paryIDs = new List<uint>(this.currentPartyIDList);
            }

            var q =
                from x in combatants
                where
                paryIDs.Any(y => y == x.ID)
                select
                x;

            return new List<Combatant>(q);
        }

        public void RefreshCombatantList()
        {
            if (!this.IsAvalable)
            {
                return;
            }

            var newList = new List<Combatant>();
            var newDictionary = new Dictionary<uint, Combatant>();

            dynamic list = this.pluginScancombat.GetCombatantList();
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

                newList.Add(combatant);
                newDictionary.Add(combatant.ID, combatant);
            }

            lock (this.combatantListLock)
            {
                this.combatantList = newList;
                this.combatantDictionary = newDictionary;
            }
        }

        public void RefreshCurrentPartyIDList()
        {
            if (!this.IsAvalable)
            {
                return;
            }

            var partyList = pluginScancombat.GetCurrentPartyList(
                out int partyCount) as List<uint>;

            lock (this.currentPartyIDList)
            {
                this.currentPartyIDList = partyList;
            }
        }

        private void Attach()
        {
            if (!ActGlobals.oFormActMain.Visible)
            {
                return;
            }

            this.AttachPlugin();
            this.AttachScanMemory();
        }

        private void AttachPlugin()
        {
            if (this.plugin != null)
            {
                return;
            }

            foreach (var item in ActGlobals.oFormActMain.ActPlugins)
            {
                if (item.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                    item.lblPluginStatus.Text.ToUpper() == "FFXIV Plugin Started.".ToUpper())
                {
                    this.plugin = item.pluginObj;
                    break;
                }
            }
        }

        private void AttachScanMemory()
        {
            if (this.plugin == null)
            {
                return;
            }

            FieldInfo fi;

            if (this.pluginMemory == null)
            {
                fi = this.plugin.GetType().GetField(
                    "_Memory",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                this.pluginMemory = fi.GetValue(this.plugin);
            }

            if (this.pluginMemory == null)
            {
                return;
            }

            if (this.pluginConfig == null)
            {
                fi = this.pluginMemory.GetType().GetField(
                    "_config",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                this.pluginConfig = fi.GetValue(this.pluginMemory);
            }

            if (this.pluginConfig == null)
            {
                return;
            }

            if (this.pluginScancombat == null)
            {
                fi = this.pluginConfig.GetType().GetField(
                    "ScanCombatants",
                    BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
                this.pluginScancombat = fi.GetValue(this.pluginConfig);
            }
        }

        public int GetCurrentZoneID()
        {
            var currentZoneName = ActGlobals.oFormActMain.CurrentZone;
            if (string.IsNullOrEmpty(currentZoneName) ||
                currentZoneName == "Unknown Zone")
            {
                return 0;
            }

            if (this.zoneList == null ||
                this.zoneList.Count < 1)
            {
                return 0;
            }

            var foundZone = zoneList.AsParallel()
                .FirstOrDefault(zone =>
                    string.Equals(
                        zone.Name, currentZoneName,
                        StringComparison.OrdinalIgnoreCase));
            return foundZone != null ? foundZone.ID : 0;
        }

        private void GetZoneList()
        {
            if (this.zoneList != null &&
                this.zoneList.Count > 0)
            {
                return;
            }

            var newList = new List<Zone>();

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

            newList = newList.OrderBy(x => x.ID).ToList();

            this.zoneList = newList;
        }
    }
}