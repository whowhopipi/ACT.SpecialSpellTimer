using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ACT.SpecialSpellTimer.Image
{
    public class IconController
    {
        #region Singleton

        private static IconController instance = new IconController();

        public static IconController Instance => instance;

        #endregion Singleton

        public string[] IconDirectories
        {
            get
            {
                var dirs = new List<string>();

                var actDirectory = string.Empty;

                // ACTのパスを取得する
                var asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    actDirectory = Path.GetDirectoryName(asm.Location);

                    var dir1 = Path.Combine(actDirectory, @"resources\icon");
                    if (Directory.Exists(dir1))
                    {
                        dirs.Add(dir1);
                    }

                    var dir2 = Path.Combine(actDirectory, @"resources\xivdb\Action icons");
                    if (Directory.Exists(dir2))
                    {
                        dirs.Add(dir2);
                    }
                }

                // 自身の場所を取得する
                var selfDirectory = PluginCore.Instance?.Location ?? string.Empty;
                if (Path.GetFullPath(selfDirectory).ToLower() !=
                    Path.GetFullPath(actDirectory).ToLower())
                {
                    var dir3 = Path.Combine(selfDirectory, @"resources\icon");
                    if (Directory.Exists(dir3))
                    {
                        dirs.Add(dir3);
                    }

                    var dir4 = Path.Combine(selfDirectory, @"resources\xivdb\Action icons");
                    if (Directory.Exists(dir4))
                    {
                        dirs.Add(dir4);
                    }
                }

                return dirs.ToArray();
            }
        }

        /// <summary>
        /// Iconファイルを列挙する
        /// </summary>
        /// <returns>
        /// Iconファイルのコレクション</returns>
        public IconFile[] EnumlateIcon()
        {
            var list = new List<IconFile>();

            // 未選択用のダミーをセットしておく
            list.Add(new IconFile()
            {
                FullPath = string.Empty,
                RelativePath = string.Empty
            });

            foreach (var dir in this.IconDirectories)
            {
                if (Directory.Exists(dir))
                {
                    list.AddRange(this.EnumulateIcon(dir, string.Empty));
                }
            }

            return list.ToArray();
        }

        public IconFile GetIconFile(String relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }

            foreach (var dir in this.IconDirectories)
            {
                var iconPath = Path.Combine(dir, relativePath);
                if (File.Exists(iconPath))
                {
                    return new IconFile()
                    {
                        FullPath = iconPath.ToString(),
                        RelativePath = relativePath
                    };
                }
            }

            return null;
        }

        private IconFile[] EnumulateIcon(String target, String prefix)
        {
            var list = new List<IconFile>();

            foreach (var dir in Directory.GetDirectories(target))
            {
                list.AddRange(this.EnumulateIcon(dir, prefix + Path.GetFileName(dir) + Path.DirectorySeparatorChar));
            }

            foreach (var file in Directory.GetFiles(target, "*.png"))
            {
                list.Add(new IconFile()
                {
                    FullPath = file,
                    RelativePath = prefix + Path.GetFileName(file)
                });
            }

            return list.ToArray();
        }

        /// <summary>
        /// Iconファイル
        /// </summary>
        public class IconFile
        {
            public string Directory => 
                !string.IsNullOrEmpty(this.FullPath) ? 
                Path.GetDirectoryName(this.FullPath) :
                string.Empty;

            /// <summary>
            /// フルパス
            /// </summary>
            public string FullPath { get; set; }

            /// <summary>
            /// ファイル名
            /// </summary>
            public string Name
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(this.FullPath) ?
                        Path.GetFileName(this.FullPath) :
                        string.Empty;
                }
            }

            /// <summary>
            /// フルパス
            /// </summary>
            public string RelativePath { get; set; }

            /// <summary>
            /// ToString()
            /// </summary>
            /// <returns>一般化された文字列</returns>
            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
