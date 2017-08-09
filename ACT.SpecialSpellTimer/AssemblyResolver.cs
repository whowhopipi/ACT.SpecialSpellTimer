using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
    public class AssemblyResolver
    {
        #region Singleton

        private static AssemblyResolver instance = new AssemblyResolver();

        public static AssemblyResolver Instance => instance;

        #endregion Singleton

        private static readonly Regex AssemblyNameParser = new Regex(
            @"(?<name>.+?), Version=(?<version>.+?), Culture=(?<culture>.+?), PublicKeyToken=(?<pubkey>.+)",
            RegexOptions.Compiled);

        public List<string> Directories { get; private set; } = new List<string>();

        public void Initialize(
            IActPluginV1 plugin)
        {
            var pluginDirectory = ActGlobals.oFormActMain?.PluginGetSelfData(plugin)?.pluginFile.DirectoryName;
            if (!string.IsNullOrEmpty(pluginDirectory))
            {
                this.Directories.Add(pluginDirectory);
            }

            this.Directories.Add(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Advanced Combat Tracker\Plugins"));

            AppDomain.CurrentDomain.AssemblyResolve
                += this.CustomAssemblyResolve;
        }

        private Assembly CustomAssemblyResolve(object sender, ResolveEventArgs e)
        {
            // Directories プロパティで指定されたディレクトリを基準にアセンブリを検索する
            foreach (var directory in this.Directories)
            {
                var asmPath = string.Empty;
                var match = AssemblyNameParser.Match(e.Name);
                if (match.Success)
                {
                    var asmFileName = match.Groups["name"].Value + ".dll";
                    if (match.Groups["culture"].Value == "neutral")
                    {
                        asmPath = Path.Combine(directory, asmFileName);
                    }
                    else
                    {
                        asmPath = Path.Combine(directory, match.Groups["culture"].Value, asmFileName);
                    }
                }
                else
                {
                    asmPath = Path.Combine(directory, e.Name + ".dll");
                }

                if (File.Exists(asmPath))
                {
                    var asm = Assembly.LoadFile(asmPath);
                    return asm;
                }
            }

            return null;
        }
    }
}
