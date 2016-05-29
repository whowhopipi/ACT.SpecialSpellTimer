namespace ACT.SpecialSpellTimer
{
    using System.Collections.Generic;

    /// <summary>
    /// ジョブ
    /// </summary>
    public class Job
    {
        /// <summary>
        /// ジョブリスト
        /// </summary>
        private static readonly IReadOnlyList<Job> _jobList;

        /// <summary>
        /// ジョブ辞書
        /// </summary>
        private static readonly IReadOnlyDictionary<int, Job> _jobDictionary;

        /// <summary>
        /// JobId
        /// </summary>
        public int JobId { get; private set; }

        /// <summary>
        /// JobName
        /// </summary>
        public string JobName { get; private set; }

        /// <summary>
        /// ロール
        /// </summary>
        public JobRoles Role { get; private set; }

        static Job()
        {
            var list = new List<Job>();
            list.Add(new Job(JobIds.GLD, JobRoles.Tank));
            list.Add(new Job(JobIds.PUG, JobRoles.MeleeDPS));
            list.Add(new Job(JobIds.MRD, JobRoles.Tank));
            list.Add(new Job(JobIds.LNC, JobRoles.MeleeDPS));
            list.Add(new Job(JobIds.ARC, JobRoles.RangerDPS));
            list.Add(new Job(JobIds.CNJ, JobRoles.Healer));
            list.Add(new Job(JobIds.THM, JobRoles.CasterDPS));
            list.Add(new Job(JobIds.CRP, JobRoles.Crafter));
            list.Add(new Job(JobIds.BSM, JobRoles.Crafter));
            list.Add(new Job(JobIds.ARM, JobRoles.Crafter));
            list.Add(new Job(JobIds.GSM, JobRoles.Crafter));
            list.Add(new Job(JobIds.LTW, JobRoles.Crafter));
            list.Add(new Job(JobIds.WVR, JobRoles.Crafter));
            list.Add(new Job(JobIds.ALC, JobRoles.Crafter));
            list.Add(new Job(JobIds.CUL, JobRoles.Crafter));
            list.Add(new Job(JobIds.MIN, JobRoles.Gatherer));
            list.Add(new Job(JobIds.BOT, JobRoles.Gatherer));
            list.Add(new Job(JobIds.FSH, JobRoles.Gatherer));
            list.Add(new Job(JobIds.PLD, JobRoles.Tank));
            list.Add(new Job(JobIds.MNK, JobRoles.MeleeDPS));
            list.Add(new Job(JobIds.WAR, JobRoles.Tank));
            list.Add(new Job(JobIds.DRG, JobRoles.MeleeDPS));
            list.Add(new Job(JobIds.BRD, JobRoles.RangerDPS));
            list.Add(new Job(JobIds.WHM, JobRoles.Healer));
            list.Add(new Job(JobIds.BLM, JobRoles.CasterDPS));
            list.Add(new Job(JobIds.ACN, JobRoles.CasterDPS));
            list.Add(new Job(JobIds.SMN, JobRoles.CasterDPS));
            list.Add(new Job(JobIds.SCH, JobRoles.Healer));
            list.Add(new Job(JobIds.ROG, JobRoles.MeleeDPS));
            list.Add(new Job(JobIds.NIN, JobRoles.MeleeDPS));
            list.Add(new Job(JobIds.MCH, JobRoles.RangerDPS));
            list.Add(new Job(JobIds.DRK, JobRoles.Tank));
            list.Add(new Job(JobIds.AST, JobRoles.Healer));
            _jobList = list;

            var dict = new Dictionary<int, Job>();
            foreach (var job in JobList)
            {
                dict.Add(job.JobId, job);
            }
            _jobDictionary = dict;
        }

        /// <summary>
        /// ジョブの一覧
        /// </summary>
        public static IReadOnlyList<Job> JobList
        {
            get
            {
                return _jobList;
            }
        }

        /// <summary>
        /// ジョブIDをキーに持つ辞書
        /// </summary>
        public static IReadOnlyDictionary<int, Job> JobDictionary
        {
            get
            {
                return _jobDictionary;
            }
        }

        public static Job FromId(int jobId)
        {
            return JobDictionary[jobId];
        }

        /// <summary>
        /// ジョブIDからジョブ名を取得する
        /// </summary>
        /// <param name="jobID">ジョブID</param>
        /// <returns>ジョブ名</returns>
        public static string GetJobName(
            int jobID)
        {
            if (JobDictionary.ContainsKey(jobID))
            {
                return JobDictionary[jobID].JobName;
            }
            else
            {
                return string.Empty;
            }
        }

        private Job(JobIds id, JobRoles role)
        {
            this.JobId = (int)id;
            this.JobName = System.Enum.GetName(typeof(JobIds), id);
            this.Role = role;
        }

        public bool IsSummoner()
        {
            const int ARC = (int)JobIds.ARC;
            const int SCH = (int)JobIds.SCH;
            const int SMN = (int)JobIds.SMN;
            return JobId == ARC || JobId == SCH || JobId == SMN;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return Utility.Translate.Get(this.JobName);
        }
    }

    public enum JobRoles
    {
        Tank = 10,
        Healer = 20,
        DPS = 30,
        MeleeDPS = 31,
        RangerDPS = 32,
        CasterDPS = 33,
        Crafter = 40,
        Gatherer = 50,
    }

    public enum JobIds
    {
        GLD = 1,
        PUG = 2,
        MRD = 3,
        LNC = 4,
        ARC = 5,
        CNJ = 6,
        THM = 7,
        CRP = 8,
        BSM = 9,
        ARM = 10,
        GSM = 11,
        LTW = 12,
        WVR = 13,
        ALC = 14,
        CUL = 15,
        MIN = 16,
        BOT = 17,
        FSH = 18,
        PLD = 19,
        MNK = 20,
        WAR = 21,
        DRG = 22,
        BRD = 23,
        WHM = 24,
        BLM = 25,
        ACN = 26,
        SMN = 27,
        SCH = 28,
        ROG = 29,
        NIN = 30,
        MCH = 31,
        DRK = 32,
        AST = 33,
    }
}
