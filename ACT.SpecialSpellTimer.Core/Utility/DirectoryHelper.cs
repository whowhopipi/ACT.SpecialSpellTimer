using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ACT.SpecialSpellTimer.Utility
{
    public static class DirectoryHelper
    {
        private static Dictionary<string, string> subDirectories = new Dictionary<string, string>();

        public static string FindSubDirectory(
            string subDirectoryName)
        {
            lock (subDirectories)
            {
                if (subDirectories.ContainsKey(subDirectoryName))
                {
                    return subDirectories[subDirectoryName];
                }
            }

            var parentDirs = new string[]
            {
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                PluginCore.Instance.Location
            };

            foreach (var parentDir in parentDirs)
            {
                var dir = Path.Combine(parentDir, subDirectoryName);
                if (Directory.Exists(dir))
                {
                    lock (subDirectories)
                    {
                        subDirectories[subDirectoryName] = dir;
                    }

                    return dir;
                }
            }

            return string.Empty;
        }
    }
}
