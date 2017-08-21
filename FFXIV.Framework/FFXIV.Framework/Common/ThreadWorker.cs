using System;
using System.Threading;
using NLog;

namespace FFXIV.Framework.Common
{
    public class ThreadWorker
    {
        #region Logger

        private Logger logger = AppLog.DefaultLogger;

        #endregion Logger

        private volatile bool isAbort;
        private Thread thread;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="doWorkAction">
        /// 定期的に実行するアクション</param>
        /// <param name="interval">
        /// インターバル。ミリ秒</param>
        public ThreadWorker(
            Action doWorkAction,
            double interval,
            string name = "")
        {
            this.DoWorkAction = doWorkAction;
            this.Interval = interval;
            this.Name = name;
        }

        public Action DoWorkAction { get; set; }

        public double Interval { get; set; }

        public string Name { get; set; }

        public static ThreadWorker Run(
            Action doWorkAction,
            double interval,
            string name = "")
        {
            var worker = new ThreadWorker(doWorkAction, interval, name);
            worker.Run();
            return worker;
        }

        public void Abort()
        {
            this.isAbort = true;

            if (this.thread != null)
            {
                this.thread.Join((int)this.Interval);
                if (this.thread.IsAlive)
                {
                    this.thread.Abort();
                }

                this.thread = null;
            }

            this.logger.Trace($"ThreadWorker - {this.Name} end.");
        }

        public void Run()
        {
            this.isAbort = false;

            this.thread = new Thread(this.DoWorkLoop);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        private void DoWorkLoop()
        {
            Thread.Sleep((int)this.Interval);
            this.logger.Trace($"ThreadWorker - {this.Name} start.");

            while (!this.isAbort)
            {
                try
                {
                    this.DoWorkAction?.Invoke();
                }
                catch (ThreadAbortException)
                {
                    this.isAbort = true;
                    this.logger.Trace($"ThreadWorker - {this.Name} abort.");
                    break;
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, $"ThreadWorker - {this.Name} error.");
                }

                if (this.isAbort)
                {
                    break;
                }

                Thread.Sleep((int)this.Interval);
            }
        }
    }
}
