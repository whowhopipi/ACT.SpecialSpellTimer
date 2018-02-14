using System;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    /// <summary>
    /// 戦闘ログの種類
    /// </summary>
    public enum CombatLogType
    {
        Unknown = 0,
        CombatStart,
        CombatEnd,
        CastStart,
        Action,
        Added,
        HPRate,
        Dialog,
    }

    public static class CombatLogTypeExtensions
    {
        public static string ToText(
            this CombatLogType t)
            => new[]
            {
                "UNKNOWN",
                "Combat Start",
                "Combat End",
                "Starts Using",
                "Action",
                "Added",
                "HP Rate",
                "Dialog",
            }[(int)t];
    }

    /// <summary>
    /// 戦闘ログ
    /// </summary>
    public class CombatLog
    {
        /// <summary>
        /// 一意な連番
        /// </summary>
        public long ID { get; set; } = 0;

        /// <summary>
        /// 起点？
        /// </summary>
        public bool IsOrigin { get; set; }

        /// <summary>
        /// ログのタイムスタンプ
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 経過時間
        /// </summary>
        public TimeSpan TimeStampElapted { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// 経過時間
        /// </summary>
        public string TimeStampElaptedString => this.TimeStampElapted.ToTLString();

        /// <summary>
        /// ログの種類
        /// </summary>
        public CombatLogType LogType { get; set; } = CombatLogType.Unknown;

        /// <summary>
        /// ログの種類
        /// </summary>
        public string LogTypeName => this.LogType.ToText();

        /// <summary>
        /// 発生したActivity
        /// </summary>
        public string Activity { get; set; } = string.Empty;

        /// <summary>
        /// Actor
        /// </summary>
        public string Actor { get; set; } = string.Empty;

        /// <summary>
        /// Actorの残HP率
        /// </summary>
        public decimal HPRate { get; set; }

        /// <summary>
        /// Actorの残HP率
        /// </summary>
        public string HPRateText =>
            this.HPRate != 0 ?
            this.HPRate.ToString("P1") :
            string.Empty;

        /// <summary>
        /// ナマのログ
        /// </summary>
        public string Raw { get; set; } = string.Empty;

        /// <summary>
        /// ナマのログからタイムスタンプを除去した部分
        /// </summary>
        public string RawWithoutTimestamp => this.Raw.Substring(15);
    }
}
