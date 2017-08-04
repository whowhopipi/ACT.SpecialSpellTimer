using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer.Sound
{
    /// <summary>
    /// Soundコントローラ
    /// </summary>
    public class SoundController
    {
        #region Singleton

        private static SoundController instance = new SoundController();

        public static SoundController Instance => instance;

        #endregion Singleton

        /// <summary>
        /// ゆっくりが有効かどうか？
        /// </summary>
        private volatile bool enabledYukkuri = false;

        private Task existYukkuriThread;
        private volatile bool existYukkuriThreadRunning = false;

        #region Begin / End

        public void Begin()
        {
            this.existYukkuriThreadRunning = true;
            this.existYukkuriThread = new Task(() =>
            {
                while (this.existYukkuriThreadRunning)
                {
                    try
                    {
                        if (ActGlobals.oFormActMain != null &&
                            ActGlobals.oFormActMain.Visible &&
                            ActGlobals.oFormActMain.ActPlugins != null)
                        {
                            this.enabledYukkuri = ActGlobals.oFormActMain.ActPlugins
                                .Where(x =>
                                    x.pluginFile.Name.ToUpper().Contains("ACT.TTSYukkuri".ToUpper()) &&
                                    x.lblPluginStatus.Text.ToUpper() == "Plugin Started".ToUpper())
                                .Any();
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        this.existYukkuriThreadRunning = false;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("soundcontroller backgound thread error:", ex);
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            });

            this.existYukkuriThread.Start();
        }

        public void End()
        {
            this.existYukkuriThreadRunning = false;

            if (this.existYukkuriThread != null)
            {
                this.existYukkuriThread.Wait();
                if (this.existYukkuriThread.IsCanceled)
                {
                    this.existYukkuriThread.Dispose();
                }

                this.existYukkuriThread = null;
            }
        }

        #endregion Begin / End

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

                if (this.enabledYukkuri)
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
            public string Name =>
                !string.IsNullOrWhiteSpace(this.FullPath) ?
                Path.GetFileName(this.FullPath) :
                string.Empty;

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
