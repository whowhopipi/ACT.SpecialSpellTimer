using System.IO;
using System.Text;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    public class TimelineRazorModel
    {
        public string Zone { get; set; } = string.Empty;

        public TimelineRazorPlayer Player { get; set; }

        public TimelineRazorPlayer[] Party { get; set; }

        public string Include(
            string file)
        {
            if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(
                    TimelineManager.Instance.TimelineDirectory,
                    file);
            }

            if (File.Exists(file))
            {
                return File.ReadAllText(file, new UTF8Encoding(false)).TrimEnd('\r', '\n');
            }

            return string.Empty;
        }
    }

    public class TimelineRazorPlayer
    {
        public int Number { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string Job { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
