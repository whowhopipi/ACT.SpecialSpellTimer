using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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

        #region Combatants

        private readonly IReadOnlyList<Combatant> EmptyCombatantList = new List<Combatant>();

        private IReadOnlyDictionary<uint, Combatant> combatantDictionary;
        private IReadOnlyList<Combatant> combatantList;
        private object combatantListLock = new object();

        private List<uint> currentPartyIDList = new List<uint>();
        private object currentPartyIDListLock = new object();

#if false
        // とりあえずはリストを直接外部に公開しないことにする
        public IReadOnlyDictionary<uint, Combatant> CombatantDictionary => this.combatantDictionary;
        public IReadOnlyList<Combatant> CombatantList => this.combatantList;
        public object CombatantListLock => this.combatantListLock;

        public IReadOnlyCollection<uint> CurrentPartyIDList => this.currentPartyIDList;
        public object CurrentPartyIDListLock => this.currentPartyIDListLock;
#endif

        #endregion Combatants

        #region Resources

        private IReadOnlyDictionary<int, Buff> buffList = new Dictionary<int, Buff>();
        private IReadOnlyDictionary<int, Skill> skillList = new Dictionary<int, Skill>();
        private IReadOnlyList<Zone> zoneList = new List<Zone>();

        #endregion Resources

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

        public IReadOnlyList<Zone> ZoneList => this.zoneList;

        /// <summary>
        /// ACTプラグイン型のプラグインオブジェクトのインスタンス
        /// </summary>
        private IActPluginV1 ActPlugin => (IActPluginV1)this.plugin;

        /// <summary>
        /// ACTプラグインアセンブリ
        /// </summary>
        private Assembly FFXIVPluginAssembly => this.ActPlugin?.GetType()?.Assembly;

        #region Start/End

        private double scanFFXIVDurationAvg;
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
                        this.LoadZoneList();
                        this.LoadSkillList();
                        this.LoadBuffList();
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
                    var interval = (int)Settings.Default.LogPollSleepInterval;

                    try
                    {
                        if (!this.IsAvalable)
                        {
                            Thread.Sleep(5000);
                            continue;
                        }

                        var sw = Stopwatch.StartNew();

                        // CombatantとパーティIDリストを更新する
                        this.RefreshCombatantList();
                        this.RefreshCurrentPartyIDList();

                        sw.Stop();
                        var duration = sw.ElapsedMilliseconds;

                        // 処理時間の平均値を算出する
                        this.scanFFXIVDurationAvg =
                            (this.scanFFXIVDurationAvg + duration) /
                            (this.scanFFXIVDurationAvg != 0 ? 2 : 1);

#if DEBUG
                        Debug.WriteLine($"Scan FFXIV duration {this.scanFFXIVDurationAvg:N1} ms");
#endif

                        // 待機時間の補正率を算出する
                        var correctionRate = 1.0d;
                        if (this.scanFFXIVDurationAvg != 0 &&
                            duration != 0)
                        {
                            correctionRate = duration / this.scanFFXIVDurationAvg;
                        }

                        // 待機時間を補正する
                        interval = (int)(interval * correctionRate);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("scan ffxiv error:", ex);
                        Thread.Sleep(5000);
                    }

                    Thread.Sleep(interval);
                }
            });

            this.scanFFXIVTaskRunning = true;
            this.scanFFXIVTask.Start();
        }

        #endregion Start/End

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

            lock (this.currentPartyIDListLock)
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
                    Logger.Write("attached ffxiv plugin.");
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

                this.zoneList = newList;
                Logger.Write("zone list loaded.");
            }
        }
    }
}