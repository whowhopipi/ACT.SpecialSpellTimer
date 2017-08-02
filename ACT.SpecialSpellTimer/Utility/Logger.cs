namespace ACT.SpecialSpellTimer.Utility
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// ログ
    /// </summary>
    public static class Logger
    {
        private static readonly TimeSpan dueTime = TimeSpan.FromSeconds(10);
        private static readonly object lockObject = new object();

        private static readonly TimeSpan period = TimeSpan.FromSeconds(5);
        private static volatile ConcurrentQueue<LogItem> buffer;
        private static volatile ConcurrentQueue<string> bufferForFile;
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
            if (buffer != null)
            {
                buffer.Enqueue(new LogItem(null, text, null));
            }
        }

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="text">書き込む内容</param>
        /// <param name="ex">例外情報</param>
        public static void Write(string text, Exception ex)
        {
            if (buffer != null)
            {
                buffer.Enqueue(new LogItem(ex, text, null));
            }
        }

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="format">複合書式設定文字列</param>
        /// <param name="args">0 個以上の書式設定対象オブジェクトを含んだオブジェクト配列。</param>
        public static void Write(string format, params object[] args)
        {
            if (buffer != null)
            {
                buffer.Enqueue(new LogItem(null, format, args));
            }
        }

        #region Controll metods

        public static void Begin()
        {
            lock (lockObject)
            {
                buffer = new ConcurrentQueue<LogItem>();
                bufferForFile = new ConcurrentQueue<string>();
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
                lock (lockObject)
                {
                    if (flushTimer != null)
                    {
                        flushTimer.Dispose();
                    }

                    flushTimer = null;

                    LogItem item;
                    while (buffer.TryDequeue(out item))
                    {
                        bufferForFile.Enqueue(item.ToString());
                    }

                    Flush();

                    buffer = null;
                    bufferForFile = null;
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Update()
        {
            if (SpecialSpellTimerPlugin.ConfigPanel == null)
            {
                return;
            }

            var buf = buffer;
            var fileBuf = bufferForFile;

            if (buf == null || fileBuf == null || buf.IsEmpty)
            {
                return;
            }

            LogItem item;
            while (buf.TryDequeue(out item))
            {
                var line = item.ToString();
                fileBuf.Enqueue(line);
                try
                {
                    ConfigPanelLog.Instance.AppendLog(line);
                }
                catch (Exception ex)
                {
                    if (!line.Contains("ConfigPanel.AppendLog failed."))
                    {
                        Write("ConfigPanel.AppendLog failed.", ex);
                    }
                }
            }
        }

        private static void Flush()
        {
            var fileBuf = bufferForFile;
            if (fileBuf == null || fileBuf.IsEmpty)
            {
                return;
            }

            lock (lockObject)
            {
                var directoryName = Path.GetDirectoryName(LogFile);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                var lines = new StringBuilder();
                string line;
                while (fileBuf.TryDequeue(out line))
                {
                    lines.AppendLine(line);
                }

                try
                {
                    File.AppendAllText(LogFile, lines.ToString(), new UTF8Encoding(false));
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion Controll metods

        #region LogItem

        private class LogItem
        {
            private object[] args;
            private Exception cause;
            private DateTime logTime;
            private string text;

            public LogItem(Exception cause, string text, object[] args)
            {
                logTime = DateTime.Now;
                this.cause = cause;
                this.text = text;
                this.args = args != null && args.Length == 0 ? null : args;
            }

            public override string ToString()
            {
                var log = "[" + logTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] "
                    + (args == null ? text : string.Format(text, args));
                return cause == null ? log : log + Environment.NewLine + cause.ToString();
            }
        }

        #endregion LogItem
    }
}