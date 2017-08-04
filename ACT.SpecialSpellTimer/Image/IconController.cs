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

        public string IconDirectory
        {
            get
            {
                // ACTのパスを取得する
                var asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    var actDirectory = Path.GetDirectoryName(asm.Location);
                    var resourcesUnderAct = Path.Combine(actDirectory, @"resources\icon");

                    if (Directory.Exists(resourcesUnderAct))
                    {
                        return resourcesUnderAct;
                    }
                }

                // 自身の場所を取得する
                var selfDirectory = SpecialSpellTimerPlugin.Location ?? string.Empty;
                var resourcesUnderThis = Path.Combine(selfDirectory, @"resources\icon");

                if (Directory.Exists(resourcesUnderThis))
                {
                    return resourcesUnderThis;
                }

                return string.Empty;
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

            if (Directory.Exists(this.IconDirectory))
            {
                list.AddRange(EmulateIcon(this.IconDirectory, ""));
            }

            return list.ToArray();
        }

        public IconFile GetIconFile(String relativePath)
        {
            if (relativePath == string.Empty ||
                relativePath == null)
            {
                return null;
            }

            var iconPath = Path.Combine(this.IconDirectory, relativePath);
            if (File.Exists(iconPath))
            {
                return new IconFile()
                {
                    FullPath = iconPath.ToString(),
                    RelativePath = relativePath
                };
            }

            return null;
        }

        private IconFile[] EmulateIcon(String target, String prefix)
        {
            var list = new List<IconFile>();

            foreach (var dir in Directory.GetDirectories(target))
            {
                list.AddRange(EmulateIcon(dir, prefix + Path.GetFileName(dir) + Path.DirectorySeparatorChar));
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
