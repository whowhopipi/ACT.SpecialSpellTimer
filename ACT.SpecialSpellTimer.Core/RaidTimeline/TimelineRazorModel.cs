using System;
using System.Linq;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    public class TimelineRazorModel
    {
        public DateTimeOffset LT => DateTimeOffset.Now;

        public EorzeaTime ET => this.LT.ToEorzeaTime();

        public string Zone { get; set; } = string.Empty;

        public string Locale { get; set; } = string.Empty;

        public TimelineRazorPlayer Player { get; set; }

        public TimelineRazorPlayer[] Party { get; set; }

        public bool InZone(
            string zone)
            => this.Zone.ContainsIgnoreCase(zone);
    }

    public class TimelineRazorPlayer
    {
        public int Number { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string Job { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool InJob(
            params string[] jobs)
        {
            if (jobs == null)
            {
                return false;
            }

            return jobs.Any(x => this.Job.ContainsIgnoreCase(x));
        }

        public bool InRole(
            params string[] roles)
        {
            if (roles == null)
            {
                return false;
            }

            return roles.Any(x => this.Role.ContainsIgnoreCase(x));
        }
    }
}
