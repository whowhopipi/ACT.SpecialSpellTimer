namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// ジョブID
    /// </summary>
    public enum JobIds
    {
        Unknown = 0,
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
        SAM = 34,
        RDM = 35,
    }

    /// <summary>
    /// ロール
    /// </summary>
    public enum JobRoles
    {
        Unknown = 0,
        Tank = 10,
        Healer = 20,
        DPS = 30,
        MeleeDPS = 31,
        RangeDPS = 32,
        MagicDPS = 33,
        Crafter = 40,
        Gatherer = 50,
    }

    /// <summary>
    /// ジョブ
    /// </summary>
    public class Job
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        private static Job instance;

        /// <summary>
        /// ジョブリスト
        /// </summary>
        private static Job[] jobList = new Job[]
        {
            new Job(JobIds.GLD, JobRoles.Tank),
            new Job(JobIds.PUG, JobRoles.MeleeDPS),
            new Job(JobIds.MRD, JobRoles.Tank),
            new Job(JobIds.LNC, JobRoles.MeleeDPS),
            new Job(JobIds.ARC, JobRoles.RangeDPS),
            new Job(JobIds.CNJ, JobRoles.Healer),
            new Job(JobIds.THM, JobRoles.MagicDPS),
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
            new Job(JobIds.BRD, JobRoles.RangeDPS),
            new Job(JobIds.WHM, JobRoles.Healer),
            new Job(JobIds.BLM, JobRoles.MagicDPS),
            new Job(JobIds.ACN, JobRoles.MagicDPS),
            new Job(JobIds.SMN, JobRoles.MagicDPS),
            new Job(JobIds.SCH, JobRoles.Healer),
            new Job(JobIds.ROG, JobRoles.MeleeDPS),
            new Job(JobIds.NIN, JobRoles.MeleeDPS),
            new Job(JobIds.MCH, JobRoles.RangeDPS),
            new Job(JobIds.DRK, JobRoles.Tank),
            new Job(JobIds.AST, JobRoles.Healer),
            new Job(JobIds.SAM, JobRoles.MeleeDPS),
            new Job(JobIds.RDM, JobRoles.MagicDPS),
        };

        /// <summary>
        /// ジョブIDによる辞書
        /// </summary>
        private IReadOnlyDictionary<int, Job> jobDictionary;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Job()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ジョブID</param>
        /// <param name="role">ロール</param>
        private Job(JobIds id, JobRoles role)
        {
            this.JobId = (int)id;
            this.JobName = Enum.GetName(typeof(JobIds), id);
            this.Role = role;
        }

        /// <summary>
        /// インスタンス
        /// </summary>
        public static Job Instance => (instance ?? (instance = new Job()));

        /// <summary>
        /// ジョブIDをキーに持つ辞書
        /// </summary>
        public IReadOnlyDictionary<int, Job> JobDictionary
            => (this.jobDictionary ??
            (this.jobDictionary = jobList.ToDictionary(x => x.JobId, x => x)));

        /// <summary>
        /// JobId
        /// </summary>
        public int JobId { get; }

        /// <summary>
        /// ジョブの一覧
        /// </summary>
        public IReadOnlyList<Job> JobList => jobList;

        /// <summary>
        /// JobName
        /// </summary>
        public string JobName { get; }

        /// <summary>
        /// ロール
        /// </summary>
        public JobRoles Role { get; }

        /// <summary>
        /// IDからジョブを取得する
        /// </summary>
        /// <param name="jobId">ジョブID</param>
        /// <returns>ジョブ</returns>
        public Job FromId(int jobId)
        {
            if (this.JobDictionary.ContainsKey(jobId))
            {
                return this.JobDictionary[jobId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ジョブIDからジョブ名を取得する
        /// </summary>
        /// <param name="jobID">ジョブID</param>
        /// <returns>ジョブ名</returns>
        public string GetJobName(int jobID)
        {
            if (this.JobDictionary.ContainsKey(jobID))
            {
                return this.JobDictionary[jobID].JobName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 当該ジョブがサモナーか？
        /// </summary>
        /// <returns>bool</returns>
        public bool IsSummoner()
        {
            const int ARC = (int)JobIds.ARC;
            const int SCH = (int)JobIds.SCH;
            const int SMN = (int)JobIds.SMN;

            return
                this.JobId == ARC ||
                this.JobId == SCH ||
                this.JobId == SMN;
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
}