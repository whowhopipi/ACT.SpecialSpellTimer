using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.RaidTimeline.Views;
using ACT.SpecialSpellTimer.Sound;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Bridge;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    public partial class TimelineController :
        BindableBase
    {
        public TimelineController(
            TimelineModel model)
        {
            this.Model = model;
        }

        public TimelineOverlay View
        {
            get;
            set;
        } = null;

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
            lock (this)
            {
                var itemsToRemove = this.ActivityLine.Where(condition).ToList();

                foreach (var itemToRemove in itemsToRemove)
                {
                    this.ActivityLine.Remove(itemToRemove);
                }

                return itemsToRemove.Count;
            }
        }

        public void ClearActivity()
        {
            lock (this)
            {
                this.ActivityLine.Clear();
            }
        }

        public bool CallActivity(
            TimelineActivityModel currentActivity)
        {
            var name = currentActivity.CallTarget;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            lock (this)
            {
                var currentIndex = this.ActivityLine.IndexOf(currentActivity);

                // 対象のサブルーチンを取得する
                var targetSub = this.Model.Subroutines.FirstOrDefault(x =>
                    x.Enabled ?? true &&
                    string.Equals(x.Name == name, StringComparison.OrdinalIgnoreCase));

                if (targetSub == null)
                {
                    return false;
                }

                // サブルーチン配下のActivityを取得する
                var acts = targetSub.Activities
                    .Where(x => x.Enabled ?? true)
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
            TimelineActivityModel currentActivity)
        {
            var name = currentActivity.GoToDestination;

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
                        item.IsDone = false;
                        item.IsNotified = false;
                    }

                    this.CurrentTime = targetAct.Time;

                    return true;
                }

                // サブルーチンに飛ぶ
                var targetSub = this.Model.Subroutines.FirstOrDefault(x =>
                    x.Enabled ?? true &&
                    string.Equals(x.Name == name, StringComparison.OrdinalIgnoreCase));

                if (targetSub == null)
                {
                    return false;
                }

                // サブルーチン配下のActivityを取得する
                var acts = targetSub.Activities
                    .Where(x => x.Enabled ?? true)
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

        public bool IsAvailable =>
            string.Equals(
                ActGlobals.oFormActMain.CurrentZone,
                this.Model.Zone,
                StringComparison.OrdinalIgnoreCase);

        private TimeSpan currentTime = TimeSpan.Zero;

        /// <summary>
        /// 現在の経過時間
        /// </summary>
        private TimeSpan CurrentTime
        {
            get => this.currentTime;
            set => this.SetProperty(ref this.currentTime, value);
        }

        /// <summary>
        /// 前回の判定時刻
        /// </summary>
        private DateTime PreviouseDetectTime
        {
            get;
            set;
        }

        public void LoadActivity()
        {
            if (!this.IsAvailable)
            {
                return;
            }

            this.CurrentTime = TimeSpan.Zero;
            this.ClearActivity();

            int seq = 1;
            foreach (var src in this.Model.Activities
                .Where(x => x.Enabled ?? true))
            {
                var act = src.Clone();
                act.Seq = seq++;
                act.IsDone = false;
                act.IsNotified = false;
                this.AddActivity(act);
            }
        }

        private DispatcherTimer TimelineTimer
        {
            get;
            set;
        } = null;

        public void Start()
        {
            lock (this)
            {
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
            }
        }

        private void TimelineTimer_Tick(
            object sender,
            EventArgs e)
        {
            lock (this)
            {
                var now = DateTime.Now;
                this.CurrentTime += now - this.PreviouseDetectTime;
                this.PreviouseDetectTime = now;

                this.RefreshActivityLine();
            }
        }

        private void RefreshActivityLine()
        {
            if (this.CurrentTime == TimeSpan.Zero)
            {
                return;
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
            var currentActivity = (
                from x in this.ActivityLine
                where
                !x.IsDone &&
                x.Time <= this.CurrentTime
                orderby
                x.Seq descending
                select
                x).FirstOrDefault();

            if (currentActivity == null)
            {
                return;
            }

            foreach (var act in this.ActivityLine
                .Where(x =>
                    !x.IsDone &&
                    x.Seq <= currentActivity.Seq))
            {
                act.IsDone = true;
            }

            // jumpを判定する
            if (!this.CallActivity(currentActivity))
            {
                this.GoToActivity(currentActivity);
            }
        }

        public const string TimelineNoticeLog =
            "00:0038:Notice from TL. text={0}, notice={1}, offset={2:N1}";

        private void NotifyActivity(
            TimelineActivityModel act)
        {
            act.IsNotified = true;

            var offset = this.CurrentTime - act.Time;
            var log = string.Format(
                TimelineNoticeLog,
                act.Text,
                act.Notice,
                offset.TotalSeconds);

            ActGlobals.oFormActMain.ParseRawLogLine(false, DateTime.Now, log);

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
    }
}
