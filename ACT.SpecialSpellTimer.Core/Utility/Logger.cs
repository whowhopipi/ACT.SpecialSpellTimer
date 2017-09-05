using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using ACT.SpecialSpellTimer.Forms;

namespace ACT.SpecialSpellTimer.Utility
{
    /// <summary>
    /// ログ
    /// </summary>
    public static class Logger
    {
        private static readonly StringBuilder buffer = new StringBuilder();
        private static readonly double interval = 5000;
        private static readonly object lockObject = new object();
        private static System.Timers.Timer worker;

        private static string LogFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"anoyetta\ACT\ACT.SpecialSpellTimer." + DateTime.Now.ToString("yyyy-MM") + ".log");

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="text">書き込む内容</param>
        public static void Write(string text)
        {
            var log =
                $"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}] {text}";

            lock (lockObject)
            {
                buffer?.AppendLine(log);
            }
        }

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="text">書き込む内容</param>
        /// <param name="ex">例外情報</param>
        public static void Write(string text, Exception ex)
        {
            var log =
                $"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}] {text}" + Environment.NewLine +
                ex.ToString();

            lock (lockObject)
            {
                buffer?.AppendLine(log);
            }
        }

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="format">複合書式設定文字列</param>
        /// <param name="args">0 個以上の書式設定対象オブジェクトを含んだオブジェクト配列。</param>
        public static void Write(string format, params object[] args)
        {
            var text = string.Format(format, args);

            var log =
                $"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}] {text}";

            lock (lockObject)
            {
                buffer?.AppendLine(log);
            }
        }

        #region Controll metods

        public static void Begin()
        {
            lock (lockObject)
            {
                buffer?.Clear();
            }

            worker = new System.Timers.Timer();
            worker.Interval = interval;
            worker.AutoReset = true;
            worker.Elapsed += (s, e) =>
            {
                Flush();
            };

            worker.Start();
        }

        public static void End()
        {
            try
            {
                worker?.Stop();
                Flush();
            }
            catch (Exception)
            {
            }
        }

        private static void Flush()
        {
            var directoryName = Path.GetDirectoryName(LogFile);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            if (buffer == null ||
                buffer.Length <= 0)
            {
                return;
            }

            lock (lockObject)
            {
                try
                {
                    File.AppendAllText(
                        LogFile,
                        buffer.ToString(),
                        new UTF8Encoding(false));

                    if (ConfigPanelLog.Instance != null &&
                        !ConfigPanelLog.Instance.IsDisposed &&
                        ConfigPanelLog.Instance.IsHandleCreated)
                    {
                        ConfigPanelLog.Instance.AppendLog(buffer.ToString());
                    }

                    buffer.Clear();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion Controll metods
    }
}
