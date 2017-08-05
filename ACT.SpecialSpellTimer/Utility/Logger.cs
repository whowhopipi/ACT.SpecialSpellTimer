using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ACT.SpecialSpellTimer.Utility
{
    /// <summary>
    /// ログ
    /// </summary>
    public static class Logger
    {
        private static readonly TimeSpan dueTime = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan period = TimeSpan.FromSeconds(5);
        private static readonly StringBuilder buffer = new StringBuilder();
        private static Timer flushTimer;

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

            lock (buffer)
            {
                buffer.AppendLine(log);
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

            lock (buffer)
            {
                buffer.AppendLine(log);
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

            lock (buffer)
            {
                buffer.AppendLine(log);
            }
        }

        #region Controll metods

        public static void Begin()
        {
            lock (buffer)
            {
                buffer.Clear();

                if (flushTimer != null)
                {
                    flushTimer.Dispose();
                }

                flushTimer = new Timer(ignoreState => Flush(), null, dueTime, period);
            }
        }

        public static void End()
        {
            try
            {
                lock (buffer)
                {
                    if (flushTimer != null)
                    {
                        flushTimer.Dispose();
                    }

                    flushTimer = null;

                    Flush();
                }
            }
            catch (Exception)
            {
            }
        }

        private static void Flush()
        {
            lock (buffer)
            {
                try
                {
                    var directoryName = Path.GetDirectoryName(LogFile);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    File.AppendAllText(
                        LogFile,
                        buffer.ToString(),
                        new UTF8Encoding(false));

                    if (ConfigPanelLog.Instance != null)
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
