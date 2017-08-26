using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer
{
    public class AssemblyResolver
    {
        private const string PluginName = "ACT.SpecialSpellTimer";

        #region Singleton

        private static AssemblyResolver instance = new AssemblyResolver();

        public static AssemblyResolver Instance => instance;

        #endregion Singleton

        public List<string> Directories { get; private set; } = new List<string>();

        public void Initialize()
        {
            var pluginDirectory = ActGlobals.oFormActMain?.ActPlugins
                .FirstOrDefault(x => x.pluginFile.Name.ToUpper().Contains(PluginName.ToUpper()))?
                .pluginFile.DirectoryName;
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
            Assembly tryLoadAssembly(
                string directory,
                string extension)
            {
                var asm = new AssemblyName(e.Name);

                var asmPath = Path.Combine(directory, asm.Name + extension);
                if (File.Exists(asmPath))
                {
                    return Assembly.LoadFrom(asmPath);
                }

                return null;
            }

            // Directories プロパティで指定されたディレクトリを基準にアセンブリを検索する
            foreach (var directory in this.Directories)
            {
                var asm = tryLoadAssembly(directory, ".dll");
                if (asm != null)
                {
                    return asm;
                }
            }

            return null;
        }
    }
}
