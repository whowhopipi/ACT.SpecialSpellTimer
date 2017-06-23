namespace ACT.SpecialSpellTimer.Sound
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// Soundコントローラ
    /// </summary>
    public class SoundController
    {
        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static SoundController instance;

        /// <summary>
        /// ゆっくりをチェックしたタイムスタンプ
        /// </summary>
        private DateTime checkedYukkuriTimeStamp = DateTime.MinValue;

        /// <summary>
        /// ゆっくりが有効かどうか？
        /// </summary>
        private bool enabledYukkuri;

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static SoundController Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundController();
                }

                return instance;
            }
        }

        /// <summary>
        /// ゆっくりが有効かどうか？
        /// </summary>
        public bool EnabledYukkuri
        {
            get
            {
                if ((DateTime.Now - this.checkedYukkuriTimeStamp).TotalSeconds >= 10d)
                {
                    if (ActGlobals.oFormActMain.Visible)
                    {
                        this.enabledYukkuri = ActGlobals.oFormActMain.ActPlugins
                            .Where(x =>
                                x.pluginFile.Name.ToUpper().Contains("ACT.TTSYukkuri".ToUpper()) &&
                                x.lblPluginStatus.Text.ToUpper() == "Plugin Started".ToUpper())
                            .Any();

                        this.checkedYukkuriTimeStamp = DateTime.Now;
                    }
                }

                return this.enabledYukkuri;
            }
        }

        public string WaveDirectory
        {
            get
            {
                // ACTのパスを取得する
                var asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    var actDirectory = Path.GetDirectoryName(asm.Location);
                    var resourcesUnderAct = Path.Combine(actDirectory, @"resources\wav");

                    if (Directory.Exists(resourcesUnderAct))
                    {
                        return resourcesUnderAct;
                    }
                }

                // 自身の場所を取得する
                var selfDirectory = SpecialSpellTimerPlugin.Location ?? string.Empty;
                var resourcesUnderThis = Path.Combine(selfDirectory, @"resources\wav");

                if (Directory.Exists(resourcesUnderThis))
                {
                    return resourcesUnderThis;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Waveファイルを列挙する
        /// </summary>
        /// <returns>
        /// Waveファイルのコレクション</returns>
        public WaveFile[] EnumlateWave()
        {
            var list = new List<WaveFile>();

            // 未選択用のダミーをセットしておく
            list.Add(new WaveFile()
            {
                FullPath = string.Empty
            });

            if (Directory.Exists(this.WaveDirectory))
            {
                foreach (var wave in Directory.GetFiles(this.WaveDirectory, "*.wav")
                    .OrderBy(x => x)
                    .ToArray())
                {
                    list.Add(new WaveFile()
                    {
                        FullPath = wave
                    });
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 再生する
        /// </summary>
        /// <param name="source">
        /// 再生する対象</param>
        public void Play(
            string source)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    return;
                }

                if (this.EnabledYukkuri)
                {
                    Task.Run(() => ActGlobals.oFormActMain.TTS(source));
                }
                else
                {
                    Task.Run(() =>
                    {
                        // wav？
                        if (source.EndsWith(".wav"))
                        {
                            // ファイルが存在する？
                            if (File.Exists(source))
                            {
                                ActGlobals.oFormActMain.PlaySound(source);
                            }
                        }
                        else
                        {
                            ActGlobals.oFormActMain.TTS(source);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Translate.Get("SoundError"), ex);
            }
        }

        /// <summary>
        /// Waveファイル
        /// </summary>
        public class WaveFile
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