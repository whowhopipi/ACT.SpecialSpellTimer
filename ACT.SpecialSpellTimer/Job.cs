namespace ACT.SpecialSpellTimer
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// ジョブ
    /// </summary>
    public class Job
    {
        /// <summary>
        /// ジョブリスト
        /// </summary>
        private static readonly IReadOnlyList<Job> _jobList = new List<Job> {
            new Job(JobIds.GLD, JobRoles.Tank),
            new Job(JobIds.PUG, JobRoles.MeleeDPS),
            new Job(JobIds.MRD, JobRoles.Tank),
            new Job(JobIds.LNC, JobRoles.MeleeDPS),
            new Job(JobIds.ARC, JobRoles.RangerDPS),
            new Job(JobIds.CNJ, JobRoles.Healer),
            new Job(JobIds.THM, JobRoles.CasterDPS),
            new Job(JobIds.CRP, JobRoles.Crafter),
            new Job(JobIds.BSM, JobRoles.Crafter),
            new Job(JobIds.ARM, JobRoles.Crafter),
            new Job(JobIds.GSM, JobRoles.Crafter),
            new Job(JobIds.LTW, JobRoles.Crafter),
            new Job(JobIds.WVR, JobRoles.Crafter),
            new Job(JobIds.ALC, JobRoles.Crafter),
            new Job(JobIds.CUL, JobRoles.Crafter),
            new Job(JobIds.MIN, JobRoles.Gatherer),
            new Job(JobIds.BOT, JobRoles.Gatherer),
            new Job(JobIds.FSH, JobRoles.Gatherer),
            new Job(JobIds.PLD, JobRoles.Tank),
            new Job(JobIds.MNK, JobRoles.MeleeDPS),
            new Job(JobIds.WAR, JobRoles.Tank),
            new Job(JobIds.DRG, JobRoles.MeleeDPS),
            new Job(JobIds.BRD, JobRoles.RangerDPS),
            new Job(JobIds.WHM, JobRoles.Healer),
            new Job(JobIds.BLM, JobRoles.CasterDPS),
            new Job(JobIds.ACN, JobRoles.CasterDPS),
            new Job(JobIds.SMN, JobRoles.CasterDPS),
            new Job(JobIds.SCH, JobRoles.Healer),
            new Job(JobIds.ROG, JobRoles.MeleeDPS),
            new Job(JobIds.NIN, JobRoles.MeleeDPS),
            new Job(JobIds.MCH, JobRoles.RangerDPS),
            new Job(JobIds.DRK, JobRoles.Tank),
            new Job(JobIds.AST, JobRoles.Healer),
        };

        /// <summary>
        /// ジョブ辞書
        /// </summary>
        private static readonly IReadOnlyDictionary<int, Job> _jobDictionary =
            _jobList.ToDictionary(job => job.JobId, job => job);

        /// <summary>
        /// ジョブの一覧
        /// </summary>
        public static IReadOnlyList<Job> JobList => _jobList;

        /// <summary>
        /// ジョブIDをキーに持つ辞書
        /// </summary>
        public static IReadOnlyDictionary<int, Job> JobDictionary => _jobDictionary;

        /// <summary>
        /// JobId
        /// </summary>
        public int JobId { get; }

        /// <summary>
        /// JobName
        /// </summary>
        public string JobName { get; }

        /// <summary>
        /// ロール
        /// </summary>
        public JobRoles Role { get; }

        public static Job FromId(int jobId)
        {
            return JobDictionary[jobId];
        }

        /// <summary>
        /// ジョブIDからジョブ名を取得する
        /// </summary>
        /// <param name="jobID">ジョブID</param>
        /// <returns>ジョブ名</returns>
        public static string GetJobName(int jobID)
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
