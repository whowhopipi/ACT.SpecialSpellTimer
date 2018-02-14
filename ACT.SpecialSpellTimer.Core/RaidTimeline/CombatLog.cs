using System;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    /// <summary>
    /// 戦闘ログの種類
    /// </summary>
    public enum CombatLogType
    {
        CombatStart = 0,
        CombatEnd = 1,
        CastStart = 2,
        Action = 3,
        Added = 4,
        HPRate = 5,
        Dialog = 6,
    }

    /// <summary>
    /// 戦闘ログ
    /// </summary>
    public class CombatLog
    {
        public string Activity { get; set; } = string.Empty;
        public string Actor { get; set; } = string.Empty;
        public double CastTime { get; set; }
        public decimal HPRate { get; set; }
        public long ID { get; set; }
        public bool IsOrigin { get; set; }
        public CombatLogType LogType { get; set; }
        public string LogTypeName { get; set; } = string.Empty;
        public string Raw { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
        public double TimeStampElapted { get; set; }
    }
}
