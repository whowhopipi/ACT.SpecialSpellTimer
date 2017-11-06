using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ACT.SpecialSpellTimer.Config;

namespace ACT.SpecialSpellTimer
{
    public class ChatLogWorker
    {
        #region Singleton

        private static ChatLogWorker instance = new ChatLogWorker();

        public static ChatLogWorker Instance => instance;

        #endregion Singleton

        private readonly double FlushInterval = 10000;
        private readonly Encoding UTF8Encoding = new UTF8Encoding(false);
        private readonly StringBuilder LogBuffer = new StringBuilder(128 * 5120);

        private System.Timers.Timer worker;

        private string OutputDirectory => Settings.Default.SaveLogDirectory;

        private bool OutputEnabled =>
            Settings.Default.SaveLogEnabled &&
            !string.IsNullOrEmpty(this.OutputFile);

        private string OutputFile =>
            !string.IsNullOrEmpty(OutputDirectory) ?
            Path.Combine(
                this.OutputDirectory,
                $@"CombatLog.{DateTime.Now.ToString("yyyy-MM-dd")}.log") :
            string.Empty;

        public void AppendLines(
            List<string> logLineList)
        {
            try
            {
                if (!this.OutputEnabled)
                {
                    return;
                }

                lock (this.LogBuffer)
                {
                    var time = DateTime.Now.ToString("HH:mm:ss.fff");
                    foreach (var line in logLineList)
                    {
                        this.LogBuffer.AppendLine($"[{time}] {line}");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Begin()
        {
            lock (this.LogBuffer)
            {
                this.LogBuffer.Clear();
            }

            this.worker = new System.Timers.Timer();
            this.worker.AutoReset = true;
            this.worker.Interval = FlushInterval;
            this.worker.Elapsed += (s, e) => this.Flush();

            this.worker.Start();
        }

        public void End()
        {
            this.worker?.Stop();
            this.worker?.Dispose();
            this.worker = null;
            this.Flush();
        }

        public void Flush()
        {
            try
            {
                if (!this.OutputEnabled)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(this.OutputDirectory))
                {
                    if (!Directory.Exists(this.OutputDirectory))
                    {
                        Directory.CreateDirectory(this.OutputDirectory);
                    }
                }

                lock (this.LogBuffer)
                {
                    if (this.LogBuffer.Length <= 0)
                    {
                        return;
                    }

                    File.AppendAllText(
                        this.OutputFile,
                        this.LogBuffer.ToString(),
                        this.UTF8Encoding);

                    this.LogBuffer.Clear();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
