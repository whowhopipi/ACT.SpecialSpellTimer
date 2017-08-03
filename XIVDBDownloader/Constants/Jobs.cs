namespace XIVDBDownloader.Constants
{
    public enum JobIDs
    {
        Unknown = -1,
        ADV = 0,
        GLA = 1,
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

    public enum Roles
    {
        Unknown = 0,
        Tank = 113,
        Healer = 117,
        MeleeDPS = 114,
        RangeDPS = 115,
        MagicDPS = 116,
        Crafter = 210,
        Gatherer = 220,
    }

    public class Job
    {
        public JobIDs ID { get; set; }
        public string NameDE { get; set; }
        public string NameEN { get; set; }
        public string NameFR { get; set; }
        public string NameJA { get; set; }
        public Roles Role { get; set; }
    }

    public class Jobs
    {
        private static readonly Job[] jobs = new Job[]
        {
            new Job() {ID = JobIDs.Unknown, Role = Roles.Unknown, NameEN = "Unknown", NameJA = "Unknown", NameFR = "Unknown", NameDE = "Unknown" },
            new Job() {ID = JobIDs.ADV, Role = Roles.Unknown, NameEN = "Adventurer", NameJA = "冒険者", NameFR = "Aventurier", NameDE = "Abenteurer" },
            new Job() {ID = JobIDs.GLA, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.PUG, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "Faustk\u00e4mpfer" },
            new Job() {ID = JobIDs.MRD, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.LNC, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.ARC, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.CNJ, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.THM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.CRP, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.BSM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.ARM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.GSM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.LTW, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.WVR, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.ALC, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.CUL, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.MIN, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.BOT, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.FSH, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.PLD, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.MNK, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.WAR, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.DRG, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.BRD, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.WHM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.BLM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.ACN, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.SMN, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.SCH, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.ROG, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.NIN, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.MCH, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.DRK, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.AST, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.SAM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
            new Job() {ID = JobIDs.RDM, Role = Roles.Tank, NameEN = "", NameJA = "", NameFR = "", NameDE = "" },
        };
    }
}
