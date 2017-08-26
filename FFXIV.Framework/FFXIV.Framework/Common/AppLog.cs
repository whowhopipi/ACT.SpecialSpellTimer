using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets.Wrappers;

namespace FFXIV.Framework.Common
{
    public static class AppLog
    {
        public const string ChatLoggerName = "ChatLogger";
        public const string DefaultLoggerName = "DefaultLogger";

        public static readonly object locker = new object();
        public static readonly List<AppLogEntry> logBuffer = new List<AppLogEntry>();

        public static event EventHandler AppendedLog;

        /// <summary>
        /// ChatLog用のLogger
        /// </summary>
        public static Logger ChatLogger => LogManager.GetLogger(ChatLoggerName);

        /// <summary>
        /// 通常Logger
        /// </summary>
        public static Logger DefaultLogger => LogManager.GetLogger(DefaultLoggerName);

        public static StringBuilder Log
        {
            get
            {
                var sb = new StringBuilder();

                lock (AppLog.locker)
                {
                    sb.Append(
                        AppLog.logBuffer
                        .Aggregate<AppLogEntry, string>(
                            string.Empty, (x, y) =>
                            $"{x.ToString()}\n{y.ToString()}"));
                }

                return sb;
            }
        }

        public static void AppendLog(
            string dateTime,
            string level,
            string message)
        {
            DateTime d;
            DateTime.TryParse(dateTime, out d);

            var entry = new AppLogEntry()
            {
                DateTime = d,
                Level = level,
                Message = message,
            };

            lock (AppLog.locker)
            {
                AppLog.logBuffer.Add(entry);
            }

            AppLog.OnAppendedLog(new EventArgs());
        }

        /// <summary>
        /// すべてのBufferingTargetをFlushする
        /// </summary>
        public static void FlushAll()
        {
            LogManager.Configuration.AllTargets
                .OfType<BufferingTargetWrapper>()
                .ToList()
                .ForEach(b => b.Flush(c =>
                {
                }));
        }

        public static void LoadConfiguration(
            string fileName)
        {
            if (File.Exists(fileName))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(fileName, true);
                return;
            }

            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (!string.IsNullOrEmpty(fileName))
            {
                var file = Path.Combine(dir, Path.GetFileName(fileName));
                if (File.Exists(file))
                {
                    LogManager.Configuration = new XmlLoggingConfiguration(file, true);
                }
            }
        }

        private static void OnAppendedLog(
            EventArgs e)
        {
            AppLog.AppendedLog?.Invoke(null, e);
        }
    }
}
