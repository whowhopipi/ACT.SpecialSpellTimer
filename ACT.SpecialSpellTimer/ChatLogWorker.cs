using System;
using System.IO;
using System.Text;
using System.Threading;

using ACT.SpecialSpellTimer.Config;

namespace ACT.SpecialSpellTimer
{
    public class ChatLogWorker
    {
        #region Singleton

        private static ChatLogWorker instance = new ChatLogWorker();

        public static ChatLogWorker Instance => instance;

        #endregion Singleton

        private StringBuilder logBuffer = new StringBuilder();

        private Thread writeThread;
        private bool writeThreadRunning;

        private string OutputDirectory => Settings.Default.SaveLogDirectory;

        private string OutputFile { get; set; } = string.Empty;

        private bool OutputEnabled => Settings.Default.SaveLogEnabled && !string.IsNullOrEmpty(this.OutputFile);

        public void Begin()
        {
            if (!string.IsNullOrEmpty(this.OutputDirectory))
            {
                if (!Directory.Exists(this.OutputDirectory))
                {
                    Directory.CreateDirectory(this.OutputDirectory);
                }

                this.OutputFile = Path.Combine(
                    this.OutputDirectory,
                    $@"ACT.SpecialSpellTimer.Chatlog.{DateTime.Now.ToString("yyyy-MM-dd")}.log");
            }

            lock (this.logBuffer)
            {
                this.logBuffer.Clear();
            }

            this.writeThread = new Thread(() =>
            {
                while (this.writeThreadRunning)
                {
                    try
                    {
                        this.Flush();
                    }
                    catch (ThreadAbortException)
                    {
                        this.writeThreadRunning = false;
                        return;
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            });

            this.writeThreadRunning = true;
            this.writeThread.Start();
        }

        public void End()
        {
            this.writeThreadRunning = false;

            if (this.writeThread != null)
            {
                this.writeThread.Join(TimeSpan.FromSeconds(2));
                if (this.writeThread.IsAlive)
                {
                    this.writeThread.Abort();
                }

                this.writeThread = null;
            }

            this.Flush();
        }

        public void AppendLine(
            string text)
        {
            if (!this.OutputEnabled)
            {
                return;
            }

            try
            {
                lock (this.logBuffer)
                {
                    this.logBuffer.AppendLine(text);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Flush()
        {
            if (!this.OutputEnabled)
            {
                return;
            }

            try
            {
                lock (this.logBuffer)
                {
                    File.AppendAllText(this.OutputFile, this.logBuffer.ToString(), new UTF8Encoding(false));
                    this.logBuffer.Clear();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
