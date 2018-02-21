using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.RaidTimeline.Views;
using ACT.SpecialSpellTimer.Sound;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Bridge;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    public enum TimelineStatus
    {
        Unloaded = 0,
        Loading,
        Loaded,
        Runnning
    }

    public static class TimelineStatusEx
    {
        public static string ToText(
            this TimelineStatus s)
            => new[]
            {
                string.Empty,
                "Loading...",
                "Standby",
                "Running",
            }[(int)s];
    }

    public partial class TimelineController :
        BindableBase
    {
        #region Logger

        private NLog.Logger AppLogger => FFXIV.Framework.Common.AppLog.DefaultLogger;

        #endregion Logger

        private static readonly object Locker = new object();

        /// <summary>
        /// 現在のController
        /// </summary>
        public static TimelineController CurrentController
        {
            get;
            private set;
        }

        public TimelineController(
            TimelineModel model)
        {
            this.Model = model;
        }

        public TimelineModel Model
        {
            get;
            private set;
        } = null;

        public ObservableCollection<TimelineActivityModel> ActivityLine
        {
            get;
            private set;
        } = new ObservableCollection<TimelineActivityModel>();

        public bool IsAvailable =>
            string.Equals(
                ActGlobals.oFormActMain.CurrentZone,
                this.Model.Zone,
                StringComparison.OrdinalIgnoreCase) &&
            Settings.Default.FFXIVLocale == this.Model.Locale;

        private TimelineStatus status = TimelineStatus.Unloaded;

        public TimelineStatus Status
        {
            get => this.status;
            set
            {
                if (this.SetProperty(ref status, value))
                {
                    this.RaisePropertyChanged(nameof(this.StatusText));
                }
            }
        }

        public string StatusText => this.status.ToText();

        private TimeSpan currentTime = TimeSpan.Zero;

        /// <summary>
        /// 現在の経過時間
        /// </summary>
        public TimeSpan CurrentTime
        {
            get => this.currentTime;
            private set
            {
                if (this.SetProperty(ref this.currentTime, value))
                {
                    this.RaisePropertyChanged(nameof(this.CurrentTimeText));
                }
            }
        }

        public string CurrentTimeText => this.CurrentTime.ToTLString();

        /// <summary>
        /// 前回の判定時刻
        /// </summary>
        private DateTime PreviouseDetectTime
        {
            get;
            set;
        }

        public void Load()
        {
            lock (Locker)
            {
                this.Status = TimelineStatus.Loading;

                CurrentController = this;

                this.LoadActivityLine();
                TimelineOverlay.ShowTimeline(this.Model);

                this.LogWorker = new Thread(this.DetectLogLoop)
                {
                    IsBackground = true
                };

                this.isLogWorkerRunning = true;
                this.LogWorker.Start();

                this.logInfoQueue = new ConcurrentQueue<LogLineEventArgs>();
                ActGlobals.oFormActMain.OnLogLineRead -= this.OnLogLineRead;
                ActGlobals.oFormActMain.OnLogLineRead += this.OnLogLineRead;

                this.Status = TimelineStatus.Loaded;
                this.AppLogger.Trace($"[TL] Timeline loaded. name={this.Model.Name}");
            }
        }

        public void Unload()
        {
            lock (Locker)
            {
                this.isLogWorkerRunning = false;

                TimelineOverlay.CloseTimeline();

                this.CurrentTime = TimeSpan.Zero;
                this.ClearActivity();

                if (this.LogWorker != null)
                {
                    if (this.LogWorker.IsAlive)
                    {
                        this.LogWorker.Join(TimeSpan.FromSeconds(0.2));
                    }

                    if (this.LogWorker.IsAlive)
                    {
                        this.LogWorker.Abort();
                    }

                    this.LogWorker = null;
                }

                ActGlobals.oFormActMain.OnLogLineRead -= this.OnLogLineRead;
                this.logInfoQueue = null;

                CurrentController = null;

                this.Status = TimelineStatus.Unloaded;
                this.AppLogger.Trace($"[TL] Timeline unloaded. name={this.Model.Name}");
            }
        }

        private void LoadActivityLine()
        {
            this.CurrentTime = TimeSpan.Zero;
            TimelineActivityModel.CurrentTime = TimeSpan.Zero;
            this.ClearActivity();

            var defaultStyle = TimelineSettings.Instance.DefaultStyle;
            var acts = new List<TimelineActivityModel>();

            int seq = 1;
            foreach (var src in this.Model.Activities
                .Where(x => x.Enabled.GetValueOrDefault()))
            {
                var act = src.Clone();
                act.Init(seq++);
                act.RefreshProgress();
                setStyle(act);

                acts.Add(act);
            }

            foreach (var sub in this.Model.Subroutines
                .Where(x => x.Enabled.GetValueOrDefault()))
            {
                foreach (var act in sub.Activities
                    .Where(x => x.Enabled.GetValueOrDefault()))
                {
                    setStyle(act);
                }
            }

            this.AddRangeActivity(acts);

            void setStyle(TimelineActivityModel act)
            {
                if (string.IsNullOrEmpty(act.Style))
                {
                    act.StyleModel = defaultStyle;
                    return;
                }

                act.StyleModel = TimelineSettings.Instance.Styles
                    .FirstOrDefault(x => string.Equals(
                        x.Name == act.Style,
                        StringComparison.OrdinalIgnoreCase)) ??
                    defaultStyle;
            }
        }

        #region Activityライン捌き

        public void AddActivity(
            TimelineActivityModel activity)
        {
            lock (this)
            {
                this.ActivityLine.Add(activity);
            }
        }

        public void AddRangeActivity(
            IEnumerable<TimelineActivityModel> activities)
        {
            lock (this)
            {
                this.ActivityLine.AddRange(activities);
            }
        }

        public void RemoveActivity(
            TimelineActivityModel activity)
        {
            lock (this)
            {
                this.ActivityLine.Remove(activity);
            }
        }

        public int RemoveAllActivity(
            Func<TimelineActivityModel, bool> condition)
        {
            var count = 0;

            lock (this)
            {
                var itemsToRemove = this.ActivityLine.Where(condition).ToList();

                foreach (var itemToRemove in itemsToRemove)
                {
                    this.ActivityLine.Remove(itemToRemove);
                }

                count = itemsToRemove.Count;
            }

            return count;
        }

        public void ClearActivity()
        {
            lock (this)
            {
                this.ActivityLine.Clear();
            }
        }

        public bool CallActivity(
            TimelineActivityModel currentActivity,
            string destination = null)
        {
            var name = string.IsNullOrEmpty(destination) ?
                currentActivity.GoToDestination :
                destination;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            lock (this)
            {
                var currentIndex = this.ActivityLine.IndexOf(currentActivity);

                // 対象のサブルーチンを取得する
                var targetSub = this.Model.Subroutines.FirstOrDefault(x =>
                    x.Enabled.GetValueOrDefault() &&
                    string.Equals(x.Name == name, StringComparison.OrdinalIgnoreCase));

                if (targetSub == null)
                {
                    return false;
                }

                // サブルーチン配下のActivityを取得する
                var acts = targetSub.Activities
                    .Where(x => x.Enabled.GetValueOrDefault())
                    .Select(x => x.Clone());

                if (!acts.Any())
                {
                    return false;
                }

                // 差し込まれる次のシーケンスを取得する
                var nextSeq = currentActivity.Seq + 1;

                // 差し込まれる後のActivityのシーケンスを振り直す
                var seq = nextSeq + acts.Count();
                var time = acts.Max(x => x.Time) + this.CurrentTime;
                foreach (var item in this.ActivityLine.Where(x =>
                    x.Seq > currentActivity.Seq))
                {
                    item.Seq = seq++;
                    item.Time += time;
                }

                // 差し込むActivityにシーケンスをふる
                var i = currentIndex + 1;
                foreach (var act in acts)
                {
                    act.Seq = nextSeq++;
                    act.Time += this.CurrentTime;
                    this.ActivityLine.Insert(i++, act);
                }

                return true;
            }
        }

        public bool GoToActivity(
            TimelineActivityModel currentActivity,
            string destination = null)
        {
            var name = string.IsNullOrEmpty(destination) ?
                currentActivity.GoToDestination :
                destination;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            lock (this)
            {
                var currentIndex = this.ActivityLine.IndexOf(currentActivity);

                // 対象のActivityを探す
                var targetAct = this.ActivityLine.FirstOrDefault(x =>
                    string.Equals(x.Name == name, StringComparison.OrdinalIgnoreCase));

                if (targetAct != null)
                {
                    foreach (var item in this.ActivityLine.Where(x =>
                        x.IsDone &&
                        x.Seq >= targetAct.Seq))
                    {
                        item.Init();
                    }

                    this.CurrentTime = targetAct.Time;

                    return true;
                }

                // サブルーチンに飛ぶ
                var targetSub = this.Model.Subroutines.FirstOrDefault(x =>
                    x.Enabled.GetValueOrDefault() &&
                    string.Equals(x.Name == name, StringComparison.OrdinalIgnoreCase));

                if (targetSub == null)
                {
                    return false;
                }

                // サブルーチン配下のActivityを取得する
                var acts = targetSub.Activities
                    .Where(x => x.Enabled.GetValueOrDefault())
                    .Select(x => x.Clone());

                if (!acts.Any())
                {
                    return false;
                }

                // 差し込まれる次のシーケンスを取得する
                var nextSeq = currentActivity.Seq + 1;

                // 後のActivityを削除する
                this.RemoveAllActivity(x => x.Seq > currentActivity.Seq);

                // 差し込むActivityにシーケンスをふる
                var i = currentIndex + 1;
                foreach (var act in acts)
                {
                    act.Seq = nextSeq++;
                    act.Time += this.CurrentTime;
                    this.ActivityLine.Insert(i++, act);
                }

                return true;
            }
        }

        #endregion Activityライン捌き

        #region Log 関係のスレッド

        private ConcurrentQueue<LogLineEventArgs> logInfoQueue;
        private volatile bool isLogWorkerRunning = false;

        private Thread LogWorker
        {
            get;
            set;
        } = null;

        public void EnqueueLog(
            LogLineEventArgs logInfo)
        {
            if (!this.isLogWorkerRunning)
            {
                return;
            }

            this.logInfoQueue?.Enqueue(logInfo);
        }

        private void OnLogLineRead(
            bool isImport,
            LogLineEventArgs logInfo)
        {
            try
            {
                if (!this.isLogWorkerRunning)
                {
                    return;
                }

                // 18文字以下のログは読み捨てる
                // なぜならば、タイムスタンプ＋ログタイプのみのログだから
                if (logInfo.logLine.Length <= 18)
                {
                    return;
                }

                this.logInfoQueue?.Enqueue(logInfo);
            }
            catch (Exception ex)
            {
                this.AppLogger.Error(
                    ex,
                    $"[TL] Error OnLoglineRead. name={this.Model.Name}, zone={this.Model.Zone}, file={this.Model.File}");
            }
        }

        private void DetectLogLoop()
        {
            while (this.isLogWorkerRunning)
            {
                var isExistsLog = false;

                try
                {
                    if (this.logInfoQueue == null ||
                        this.logInfoQueue.IsEmpty)
                    {
                        continue;
                    }

                    var logs = this.GetLogs();
                    if (!logs.Any())
                    {
                        continue;
                    }

                    isExistsLog = true;
                    this.DetectLogs(logs);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    this.AppLogger.Error(
                        ex,
                        $"[TL] Error DetectLog. name={this.Model.Name}, zone={this.Model.Zone}, file={this.Model.File}");
                }
                finally
                {
                    if (isExistsLog)
                    {
                        Thread.Yield();
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(Settings.Default.LogPollSleepInterval));
                    }
                }
            }
        }

        private IReadOnlyList<XIVLog> GetLogs()
        {
            var list = new List<XIVLog>(this.logInfoQueue.Count);

            if (this.logInfoQueue != null)
            {
                while (this.logInfoQueue.TryDequeue(out LogLineEventArgs logInfo))
                {
                    var logLine = logInfo.logLine;

                    // エフェクトに付与されるツールチップ文字を除去する
                    // 4文字分のツールチップ文字を除去する
                    int index;
                    if ((index = logLine.IndexOf(
                        LogBuffer.TooltipSuffix,
                        0,
                        StringComparison.Ordinal)) > -1)
                    {
                        logLine = logLine.Remove(index - 1, 4);
                    }

                    // 残ったReplacementCharを除去する
                    logLine = logLine.Replace(LogBuffer.TooltipReplacementChar, string.Empty);

                    list.Add(new XIVLog(logLine));
                }
            }

            return list;
        }

        private void DetectLogs(
            IReadOnlyList<XIVLog> logs)
        {
            var detectors = default(List<TimelineBase>);

            lock (this)
            {
                detectors = (
                    from x in this.Model.Triggers
                    where
                    x.Enabled.GetValueOrDefault() &&
                    !string.IsNullOrEmpty(x.SyncKeyword) &&
                    x.SynqRegex != null
                    select
                    x).Cast<TimelineBase>().ToList();

                var acts =
                    from x in this.ActivityLine
                    where
                    x.Enabled.GetValueOrDefault() &&
                    !string.IsNullOrEmpty(x.SyncKeyword) &&
                    x.SynqRegex != null &&
                    this.CurrentTime >= x.Time + TimeSpan.FromSeconds(x.SyncOffsetStart.Value) &&
                    this.CurrentTime <= x.Time + TimeSpan.FromSeconds(x.SyncOffsetEnd.Value) &&
                    !x.IsSynced
                    select
                    x;

                detectors.AddRange(acts);
            }

            // 開始・終了判定のキーワードを取得する
            var keywords = CombatAnalyzer.Keywords.Where(x =>
                x.Category == KewordTypes.Start ||
                x.Category == KewordTypes.End);

            logs.AsParallel().ForAll(xivlog =>
            {
                // 開始・終了を判定する
                var key = (
                    from x in keywords
                    where
                    xivlog.Log.ContainsIgnoreCase(x.Keyword)
                    select
                    x).FirstOrDefault();

                if (key != null)
                {
                    switch (key.Category)
                    {
                        case KewordTypes.Start:
                            WPFHelper.BeginInvoke(() => this.StartActivityLine());
                            break;

                        case KewordTypes.End:
                            WPFHelper.BeginInvoke(() => this.EndActivityLine());
                            break;
                    }
                }

                // アクティビティ・トリガとマッチングする
                detectors.AsParallel().ForAll(detector =>
                {
                    switch (detector)
                    {
                        case TimelineActivityModel act:
                            detectActivity(xivlog, act);
                            break;

                        case TimelineTriggerModel tri:
                            detectTrigger(xivlog, tri);
                            break;
                    }
                });
            });

            bool detectActivity(
                XIVLog xivlog,
                TimelineActivityModel act)
            {
                var match = act.SynqRegex.Match(xivlog.Log);
                if (!match.Success)
                {
                    return false;
                }

                WPFHelper.BeginInvoke(() =>
                {
                    lock (this)
                    {
                        foreach (var item in this.ActivityLine.Where(x =>
                            x.IsDone &&
                            x.Seq >= act.Seq))
                        {
                            item.Init();
                        }

                        this.CurrentTime = act.Time;
                    }
                });

                return true;
            }

            bool detectTrigger(
                XIVLog xivlog,
                TimelineTriggerModel tri)
            {
                var match = tri.SynqRegex.Match(xivlog.Log);
                if (!match.Success)
                {
                    return false;
                }

                tri.MatchedCounter++;

                if (tri.SyncCount.Value != 0)
                {
                    if (tri.SyncCount.Value != tri.MatchedCounter)
                    {
                        return false;
                    }
                }

                WPFHelper.BeginInvoke(() =>
                {
                    lock (this)
                    {
                        this.NotifyTrigger(tri);

                        var active = (
                            from x in this.ActivityLine
                            where
                            x.IsActive &&
                            !x.IsDone &&
                            x.Time <= this.CurrentTime
                            orderby
                            x.Seq descending
                            select
                            x).FirstOrDefault();

                        if (active != null)
                        {
                            // jumpを判定する
                            if (!this.CallActivity(active, tri.CallTarget))
                            {
                                this.GoToActivity(active, tri.GoToDestination);
                            }
                        }
                    }
                });

                return true;
            }
        }

        #endregion Log 関係のスレッド

        #region 時間進行関係のスレッド

        private DispatcherTimer TimelineTimer
        {
            get;
            set;
        } = null;

        private volatile bool isRunning = false;

        public void StartActivityLine()
        {
            lock (this)
            {
                if (this.isRunning)
                {
                    return;
                }

                this.CurrentTime = TimeSpan.Zero;

                if (this.TimelineTimer == null)
                {
                    this.TimelineTimer = new DispatcherTimer(DispatcherPriority.Background)
                    {
                        Interval = TimeSpan.FromSeconds(0.05),
                    };

                    this.TimelineTimer.Tick += this.TimelineTimer_Tick;
                }

                this.PreviouseDetectTime = DateTime.Now;
                this.TimelineTimer.Start();

                this.isRunning = true;
                this.Status = TimelineStatus.Runnning;
                this.AppLogger.Trace($"[TL] Timeline started. name={this.Model.Name}");
            }
        }

        public void EndActivityLine()
        {
            lock (this)
            {
                if (!this.isRunning)
                {
                    return;
                }

                this.TimelineTimer.Stop();
                this.LoadActivityLine();

                this.isRunning = false;
                this.Status = TimelineStatus.Loaded;
                this.AppLogger.Trace($"[TL] Timeline stoped. name={this.Model.Name}");
            }
        }

        private void TimelineTimer_Tick(
            object sender,
            EventArgs e)
        {
            try
            {
                lock (this)
                {
                    var now = DateTime.Now;
                    this.CurrentTime += now - this.PreviouseDetectTime;
                    this.PreviouseDetectTime = now;

                    this.RefreshActivityLine();
                }
            }
            catch (Exception ex)
            {
                this.AppLogger.Error(
                    ex,
                    $"[TL] Error Timeline ticker. name={this.Model.Name}, zone={this.Model.Zone}, file={this.Model.File}");
            }
        }

        private void RefreshActivityLine()
        {
            if (this.CurrentTime == TimeSpan.Zero)
            {
                return;
            }

            // 現在の時間を更新する
            TimelineActivityModel.CurrentTime = this.CurrentTime;

            // 表示するものだけ進捗状況を更新する
            var toShow =
                from x in this.ActivityLine
                where
                x.Enabled.GetValueOrDefault() &&
                !x.IsDone &&
                !string.IsNullOrEmpty(x.Text)
                select
                x;

            var count = 0;
            foreach (var act in toShow)
            {
                if (count >= TimelineSettings.Instance.ShowActivitiesCount * 2)
                {
                    break;
                }

                act.RefreshProgress();
                count++;
            }

            // 通知を判定する
            var toNotify =
                from x in this.ActivityLine
                where
                !x.IsNotified &&
                x.Time + TimeSpan.FromSeconds(x.NoticeOffset.Value) <= this.CurrentTime
                select
                x;

            foreach (var act in toNotify)
            {
                this.NotifyActivity(act);
            }

            // 表示を終了させる
            var toDoneTop = (
                from x in this.ActivityLine
                where
                !x.IsDone &&
                x.Time <= this.CurrentTime - TimeSpan.FromSeconds(1)
                orderby
                x.Seq descending
                select
                x).FirstOrDefault();

            if (toDoneTop != null)
            {
                foreach (var act in this.ActivityLine
                    .Where(x =>
                        !x.IsDone &&
                        x.Seq <= toDoneTop.Seq))
                {
                    act.IsDone = true;
                }
            }

            // Activeなアクティビティを決める
            var active = (
                from x in this.ActivityLine
                where
                !x.IsActive &&
                !x.IsDone &&
                x.Time <= this.CurrentTime
                orderby
                x.Seq descending
                select
                x).FirstOrDefault();

            if (active != null)
            {
                active.IsActive = true;

                // jumpを判定する
                if (!this.CallActivity(active))
                {
                    this.GoToActivity(active);
                }
            }
        }

        #endregion 時間進行関係のスレッド

        #region 通知に関するメソッド

        private void NotifyActivity(
            TimelineActivityModel act)
        {
            act.IsNotified = true;

            if (string.IsNullOrEmpty(act.Name) &&
                string.IsNullOrEmpty(act.Text) &&
                string.IsNullOrEmpty(act.Notice))
            {
                return;
            }

            var now = DateTime.Now;
            var offset = this.CurrentTime - act.Time;
            var log =
                $"[{now.ToString("HH:mm:ss.fff")}] 00:0038:Notice from TL. " +
                $"name={act.Name}, text={act.Text}, notice={act.Notice}, offset={offset.TotalSeconds:N1}";

            ActGlobals.oFormActMain.ParseRawLogLine(false, now, log);

            if (string.IsNullOrEmpty(act.Notice))
            {
                return;
            }

            var notice = act.Notice;

            var isWave =
                notice.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                notice.EndsWith(".wave", StringComparison.OrdinalIgnoreCase);

            if (isWave)
            {
                notice = Path.Combine(
                    SoundController.Instance.WaveDirectory,
                    notice);
            }

            switch (act.NoticeDevice.Value)
            {
                case NoticeDevices.Both:
                    SoundController.Instance.Play(notice);
                    break;

                case NoticeDevices.Main:
                    PlayBridge.Instance.PlayMainDeviceDelegate?.Invoke(notice);
                    break;

                case NoticeDevices.Sub:
                    PlayBridge.Instance.PlaySubDeviceDelegate?.Invoke(notice);
                    break;
            }
        }

        private void NotifyTrigger(
            TimelineTriggerModel tri)
        {
            if (string.IsNullOrEmpty(tri.Name) &&
                string.IsNullOrEmpty(tri.Text) &&
                string.IsNullOrEmpty(tri.Notice))
            {
                return;
            }

            var now = DateTime.Now;
            var log =
                $"[{now.ToString("HH:mm:ss.fff")}] 00:0038:Notice from TL. " +
                $"name={tri.Name}, text={tri.Text}, notice={tri.Notice}";

            ActGlobals.oFormActMain.ParseRawLogLine(false, now, log);

            if (string.IsNullOrEmpty(tri.Notice))
            {
                return;
            }

            var notice = tri.Notice;

            var isWave =
                notice.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                notice.EndsWith(".wave", StringComparison.OrdinalIgnoreCase);

            if (isWave)
            {
                notice = Path.Combine(
                    SoundController.Instance.WaveDirectory,
                    notice);
            }

            switch (tri.NoticeDevice.Value)
            {
                case NoticeDevices.Both:
                    SoundController.Instance.Play(notice);
                    break;

                case NoticeDevices.Main:
                    PlayBridge.Instance.PlayMainDeviceDelegate?.Invoke(notice);
                    break;

                case NoticeDevices.Sub:
                    PlayBridge.Instance.PlaySubDeviceDelegate?.Invoke(notice);
                    break;
            }
        }

        #endregion 通知に関するメソッド
    }
}
