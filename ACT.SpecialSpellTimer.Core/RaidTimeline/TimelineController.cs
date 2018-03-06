using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.RaidTimeline.Views;
using ACT.SpecialSpellTimer.Sound;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Bridge;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.FFXIVHelper;
using Prism.Mvvm;
using static ACT.SpecialSpellTimer.Models.TableCompiler;

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

        public static string ToIndicator(
            this TimelineStatus s)
            => new[]
            {
                string.Empty,
                "Ｒ",
                "⬛",
                "▶",
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
        /// タイムラインから発生するログのSymbol
        /// </summary>
        private const string TLSymbol = "[TL]";

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
                    this.RaisePropertyChanged(nameof(this.StatusIndicator));
                }
            }
        }

        public string StatusText => this.status.ToText();

        public string StatusIndicator => this.status.ToIndicator();

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
                    this.CurrentTimeText = this.currentTime.ToTLString();
                }
            }
        }

        private string currentTimeText = "00:00";

        public string CurrentTimeText
        {
            get => this.currentTimeText;
            set => this.SetProperty(ref this.currentTimeText, value);
        }

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
                TimelineNoticeOverlay.ShowNotice();

                this.LogWorker = new Thread(this.DetectLogLoop)
                {
                    IsBackground = true
                };

                this.isLogWorkerRunning = true;
                this.LogWorker.Start();

                this.logInfoQueue = new ConcurrentQueue<LogLineEventArgs>();
                ActGlobals.oFormActMain.OnLogLineRead -= this.OnLogLineRead;
                ActGlobals.oFormActMain.OnLogLineRead += this.OnLogLineRead;

                this.StartNotifyWorker();

                this.Status = TimelineStatus.Loaded;
                this.AppLogger.Trace($"[TL] Timeline loaded. name={this.Model.TimelineName}");
            }
        }

        public void Unload()
        {
            lock (Locker)
            {
                this.isLogWorkerRunning = false;

                TimelineOverlay.CloseTimeline();
                TimelineNoticeOverlay.CloseNotice();

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

                this.StopNotifyWorker();

                CurrentController = null;

                this.Status = TimelineStatus.Unloaded;
                this.AppLogger.Trace($"[TL] Timeline unloaded. name={this.Model.TimelineName}");
            }
        }

        private void LoadActivityLine()
        {
            this.CurrentTime = TimeSpan.Zero;
            TimelineActivityModel.CurrentTime = TimeSpan.Zero;
            this.ClearActivity();

            var defaultStyle = TimelineSettings.Instance.DefaultStyle;
            var defaultNoticeStyle = TimelineSettings.Instance.DefaultNoticeStyle;

            // <HOGE>を[HOGE]に置き換えたプレースホルダリストを生成する
            var placeholders = TableCompiler.Instance.PlaceholderList
                .Select(x =>
                    new PlaceholderContainer(
                        x.Placeholder
                            .Replace("<", "[")
                            .Replace(">", "]"),
                        x.ReplaceString,
                        x.Type))
                .ToArray();

            // 全体の初期化を行う
            this.Model.Walk((element) =>
            {
                // トリガのマッチカウンタを初期化する
                if (element is TimelineTriggerModel tri)
                {
                    tri.MatchedCounter = 0;
                }

                // アクティビティにスタイルを設定する
                setStyle(element);

                // sync用の正規表現にプレースホルダをセットしてコンパイルし直す
                setRegex(element, placeholders);
            });

            var acts = new List<TimelineActivityModel>();
            int seq = 1;

            // entryポイントの指定がある？
            var entry = string.IsNullOrEmpty(this.Model.Entry) ?
                null :
                this.Model.Subroutines.FirstOrDefault(x =>
                    x.Enabled.GetValueOrDefault() &&
                    string.Equals(
                        x.Name,
                        this.Model.Entry,
                        StringComparison.OrdinalIgnoreCase));

            // entryサブルーチンをロードする
            var srcs = entry?.Activities ?? this.Model.Activities;
            foreach (var src in srcs
                .Where(x => x.Enabled.GetValueOrDefault()))
            {
                var act = src.Clone();
                act.Init(seq++);
                act.RefreshProgress();
                acts.Add(act);
            }

            // 一括して登録する
            this.AddRangeActivity(acts);

            // 表示設定を更新しておく
            this.RefreshActivityLineVisibility();

            // スタイルを適用する
            void setStyle(TimelineBase element)
            {
                if (element is TimelineActivityModel act)
                {
                    if (string.IsNullOrEmpty(act.Style))
                    {
                        act.StyleModel = defaultStyle;
                        return;
                    }

                    act.StyleModel = TimelineSettings.Instance.Styles
                        .FirstOrDefault(x => string.Equals(
                            x.Name,
                            act.Style,
                            StringComparison.OrdinalIgnoreCase)) ??
                        defaultStyle;
                }

                if (element is TimelineVisualNoticeModel notice)
                {
                    if (string.IsNullOrEmpty(notice.Style))
                    {
                        notice.StyleModel = defaultNoticeStyle;
                        return;
                    }

                    notice.StyleModel = TimelineSettings.Instance.Styles
                        .FirstOrDefault(x => string.Equals(
                            x.Name,
                            notice.Style,
                            StringComparison.OrdinalIgnoreCase)) ??
                        defaultNoticeStyle;
                }
            }

            // 正規表現をセットする
            void setRegex(
                TimelineBase element,
                IList<PlaceholderContainer> placeholderList)
            {
                if (!(element is ISynchronizable sync))
                {
                    return;
                }

                var replacedKeyword = sync.SyncKeyword;

                if (!string.IsNullOrEmpty(replacedKeyword))
                {
                    foreach (var ph in placeholderList)
                    {
                        replacedKeyword = replacedKeyword.Replace(
                            ph.Placeholder,
                            ph.ReplaceString);
                    }
                }

                sync.SyncKeywordReplaced = replacedKeyword;
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
                currentActivity?.CallTarget ?? string.Empty :
                destination;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            lock (this)
            {
                var currentIndex = 0;
                var currnetSeq = 1;

                if (currentActivity != null)
                {
                    currentIndex = this.ActivityLine.IndexOf(currentActivity);
                    currnetSeq = currentActivity.Seq;
                }

                // 対象のサブルーチンを取得する
                var targetSub = this.Model.Subroutines.FirstOrDefault(x =>
                    x.Enabled.GetValueOrDefault() &&
                    string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

                if (targetSub == null)
                {
                    return false;
                }

                // サブルーチン中のトリガのカウンタを初期化する
                foreach (var tri in targetSub.Triggers.Where(x =>
                    x.Enabled.GetValueOrDefault()))
                {
                    tri.MatchedCounter = 0;
                }

                // サブルーチン配下のActivityを取得する
                var acts = targetSub.Activities
                    .Where(x => x.Enabled.GetValueOrDefault())
                    .Select(x => x.Clone());

                if (!acts.Any())
                {
                    return false;
                }

                try
                {
                    this.Model.StopLive();

                    // 差し込まれる次のシーケンスを取得する
                    var nextSeq = currnetSeq + 1;

                    // 差し込まれる後のActivityのシーケンスを振り直す
                    var seq = nextSeq + acts.Count();
                    foreach (var item in this.ActivityLine.Where(x =>
                        x.Seq > currnetSeq))
                    {
                        item.Seq = seq++;
                    }

                    // 差し込むActivityにシーケンスをふる
                    var toInsert = new List<TimelineActivityModel>();
                    foreach (var act in acts)
                    {
                        act.Init(nextSeq++);
                        act.Time += this.CurrentTime;
                        toInsert.Add(act);
                    }

                    this.ActivityLine.AddRange(toInsert);
                }
                finally
                {
                    this.Model.ResumeLive();
                }

                return true;
            }
        }

        public bool GoToActivity(
            TimelineActivityModel currentActivity,
            string destination = null)
        {
            var name = string.IsNullOrEmpty(destination) ?
                currentActivity?.GoToDestination ?? string.Empty :
                destination;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            lock (this)
            {
                var currentIndex = this.ActivityLine.IndexOf(currentActivity);
                var currnetSeq = 1;

                if (currentActivity != null)
                {
                    currentIndex = this.ActivityLine.IndexOf(currentActivity);
                    currnetSeq = currentActivity.Seq;
                }

                // 対象のActivityを探す
                var targetAct = this.ActivityLine.FirstOrDefault(x =>
                    string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

                if (targetAct != null)
                {
                    try
                    {
                        this.Model.StopLive();

                        // ジャンプ後のアクティビティを最初期化する
                        foreach (var item in this.ActivityLine.Where(x =>
                            x.IsDone &&
                            x.Seq >= targetAct.Seq))
                        {
                            item.Init();
                        }

                        this.CurrentTime = targetAct.Time;
                    }
                    finally
                    {
                        this.Model.ResumeLive();
                    }

                    return true;
                }

                // サブルーチンに飛ぶ
                var targetSub = this.Model.Subroutines.FirstOrDefault(x =>
                    x.Enabled.GetValueOrDefault() &&
                    string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

                if (targetSub == null)
                {
                    return false;
                }

                // サブルーチン中のトリガのカウンタを初期化する
                foreach (var tri in targetSub.Triggers.Where(x =>
                    x.Enabled.GetValueOrDefault()))
                {
                    tri.MatchedCounter = 0;
                }

                // サブルーチン配下のActivityを取得する
                var acts = targetSub.Activities
                    .Where(x => x.Enabled.GetValueOrDefault())
                    .Select(x => x.Clone());

                if (!acts.Any())
                {
                    return false;
                }

                try
                {
                    this.Model.StopLive();

                    // 差し込まれる次のシーケンスを取得する
                    var nextSeq = currnetSeq + 1;

                    // 後のActivityを削除する
                    this.RemoveAllActivity(x => x.Seq > currnetSeq);

                    // 差し込むActivityにシーケンスをふる
                    var toInsert = new List<TimelineActivityModel>();
                    foreach (var act in acts)
                    {
                        act.Init(nextSeq++);
                        act.Time += this.CurrentTime;
                        toInsert.Add(act);
                    }

                    this.ActivityLine.AddRange(toInsert);
                }
                finally
                {
                    this.Model.ResumeLive();
                }

                return true;
            }
        }

        private void LoadSubs(
            TimelineTriggerModel parent)
        {
            lock (this)
            {
                foreach (var item in parent.LoadStatements
                    .Where(x => x.Enabled.GetValueOrDefault()))
                {
                    this.LoadSub(item);
                    Thread.Yield();
                }
            }
        }

        private void LoadSub(
            TimelineLoadModel load)
        {
            if (string.IsNullOrEmpty(load.Target))
            {
                return;
            }

            var sub = this.Model.Subroutines.FirstOrDefault(x =>
                x.Enabled.GetValueOrDefault() &&
                string.Equals(
                    x.Name,
                    load.Target,
                    StringComparison.OrdinalIgnoreCase));

            if (sub == null)
            {
                return;
            }

            // サブルーチン中のトリガのカウンタを初期化する
            foreach (var tri in sub.Triggers.Where(x =>
                x.Enabled.GetValueOrDefault()))
            {
                tri.MatchedCounter = 0;
            }

            var acts = sub.Activities
                .Where(x => x.Enabled.GetValueOrDefault())
                .Select(x => x.Clone());

            if (!acts.Any())
            {
                return;
            }

            try
            {
                this.Model.StopLive();

                // truncateする？
                if (load.IsTruncate)
                {
                    this.ActivityLine.Clear();
                }

                // 最後のアクティビティを取得する
                var last = this.ActivityLine
                    .OrderBy(x => x.Seq)
                    .LastOrDefault();

                var nextSeq = 1;
                var originTime = this.CurrentTime;
                if (last != null)
                {
                    nextSeq = last.Seq + 1;

                    if (last.Time > originTime)
                    {
                        originTime = last.Time;
                    }
                }

                // 差し込むActivityにシーケンスをふる
                var toInsert = new List<TimelineActivityModel>();
                foreach (var act in acts)
                {
                    act.Init(nextSeq++);
                    act.Time += originTime;
                    toInsert.Add(act);
                }

                this.ActivityLine.AddRange(toInsert);
            }
            finally
            {
                this.Model.ResumeLive();
            }
        }

        #endregion Activityライン捌き

        #region Log 関係のスレッド

        private ConcurrentQueue<LogLineEventArgs> logInfoQueue;
        private volatile bool isLogWorkerRunning = false;

        /// <summary>
        /// 明らかにTLの判定外とするログキーワード
        /// </summary>
        public static readonly string[] IgnoreLogKeywords = new[]
        {
            "] 15:",    // ダメージかアクションの生ログ
            "] 16:",    // エフェクトの生ログ
            "] 17:",    // Cancel
            "] 18:",    // DoT/HoT Tick
            "] 19:",    // defeated
            "] 0D:",    // HP Rate
        };

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
                if (!TimelineSettings.Instance.Enabled)
                {
                    return;
                }

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
                    $"[TL] Error OnLoglineRead. name={this.Model.TimelineName}, zone={this.Model.Zone}, file={this.Model.File}");
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

                    if (!TimelineSettings.Instance.Enabled)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
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
                        $"[TL] Error DetectLog. name={this.Model.TimelineName}, zone={this.Model.Zone}, file={this.Model.File}");
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

        private long no = 0L;

        private IReadOnlyList<XIVLog> GetLogs()
        {
            var list = new List<XIVLog>(this.logInfoQueue.Count);

            if (this.logInfoQueue != null)
            {
                var prelog = new string[3];
                var prelogIndex = 0;

                while (this.logInfoQueue.TryDequeue(out LogLineEventArgs logInfo))
                {
                    var logLine = logInfo.logLine;

                    // 直前とまったく同じログはスキップする
                    if (prelog[0] == logLine ||
                        prelog[1] == logLine ||
                        prelog[2] == logLine)
                    {
                        continue;
                    }

                    prelog[prelogIndex++] = logLine;
                    if (prelogIndex >= 3)
                    {
                        prelogIndex = 0;
                    }

                    // 無効キーワードが含まれていればスキップする
                    if (IgnoreLogKeywords.Any(x => logLine.Contains(x)))
                    {
                        continue;
                    }

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

                    list.Add(new XIVLog(logLine)
                    {
                        No = this.no++
                    });
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
                // Globalトリガを登録する
                detectors = (
                    from x in this.Model.Triggers
                    where
                    x.Enabled.GetValueOrDefault() &&
                    !string.IsNullOrEmpty(x.SyncKeyword) &&
                    x.SynqRegex != null
                    select
                    x).Cast<TimelineBase>().ToList();

                // 判定期間中のアクティビティを登録する
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

                // カレントサブルーチンのトリガを登録する
                if (this.ActiveActivity != null)
                {
                    var sub = this.ActiveActivity.Parent as TimelineSubroutineModel;
                    if (sub != null)
                    {
                        var triggers =
                            from x in sub.Triggers
                            where
                            x.Enabled.GetValueOrDefault() &&
                            !string.IsNullOrEmpty(x.SyncKeyword) &&
                            x.SynqRegex != null
                            select
                            x;

                        detectors.AddRange(triggers);
                    }
                }
            }

            // 開始・終了判定のキーワードを取得する
            var keywords = CombatAnalyzer.Keywords.Where(x =>
                x.Category == KewordTypes.TimelineStart ||
                x.Category == KewordTypes.End);

            // [TL] キーワードを含まないログに対して判定する
            logs.AsParallel().Where(x => !x.Log.Contains(TLSymbol)).ForAll(xivlog =>
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
                        case KewordTypes.TimelineStart:
                            if (this.Model.StartTriggerRegex == null)
                            {
                                WPFHelper.BeginInvoke(() =>
                                {
                                    Thread.Sleep(TimeSpan.FromSeconds(4.8));
                                    this.StartActivityLine();
                                });
                            }
                            break;

                        case KewordTypes.End:
                            WPFHelper.BeginInvoke(() => this.EndActivityLine());
                            break;
                    }
                }

                // StartTriggerがある？
                if (!this.isRunning)
                {
                    if (this.Model.StartTriggerRegex != null)
                    {
                        var match = this.Model.StartTriggerRegex.Match(xivlog.Log);
                        if (match.Success)
                        {
                            WPFHelper.BeginInvoke(() => this.StartActivityLine());
                        }
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
                var match = default(Match);

                lock (act)
                {
                    if (act.IsSynced)
                    {
                        return false;
                    }

                    match = act.SynqRegex.Match(xivlog.Log);
                    if (!match.Success)
                    {
                        return false;
                    }

                    act.IsSynced = true;
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

                        // ログを発生させる
                        raiseLog(act);
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

                tri.TextReplaced = match.Result(tri.Text ?? string.Empty);
                tri.NoticeReplaced = match.Result(tri.Notice ?? string.Empty);

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
                        this.notifyQueue.Enqueue(tri);

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

                        try
                        {
                            this.Model.StopLive();

                            // jumpを判定する
                            if (!this.CallActivity(active, tri.CallTarget))
                            {
                                if (!this.GoToActivity(active, tri.GoToDestination))
                                {
                                    this.LoadSubs(tri);
                                }
                            }

                            // ログを発生させる
                            raiseLog(tri);
                        }
                        finally
                        {
                            this.Model.ResumeLive();
                        }
                    }
                });

                return true;
            }

            void raiseLog(
                TimelineBase element)
            {
                var now = DateTime.Now;
                var log = string.Empty;

                var sub = element.Parent as TimelineSubroutineModel;

                switch (element)
                {
                    case TimelineActivityModel act:
                        log =
                            $"[{now.ToString("HH:mm:ss.fff")}] 00:0038:{TLSymbol} Synced to activity. " +
                            $"name={act.Name}, sub={sub?.Name}";
                        break;

                    case TimelineTriggerModel tri:
                        log =
                            $"[{now.ToString("HH:mm:ss.fff")}] 00:0038:{TLSymbol} Synced to trigger. " +
                            $"name={tri.Name}, sync-count={tri.MatchedCounter}, sub={sub?.Name}";
                        break;

                    default:
                        return;
                }

                ActGlobals.oFormActMain.ParseRawLogLine(false, now, log);
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

        public bool IsRunning => this.isRunning;

        public TimelineActivityModel ActiveActivity
        {
            get;
            private set;
        }

        public void StartActivityLine()
        {
            if (!TimelineSettings.Instance.Enabled)
            {
                return;
            }

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
                this.AppLogger.Trace($"{TLSymbol} Timeline started. name={this.Model.TimelineName}");
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

                TimelineNoticeOverlay.NoticeView?.ClearNotice();

                this.isRunning = false;
                this.Status = TimelineStatus.Loaded;
                this.AppLogger.Trace($"{TLSymbol} Timeline stoped. name={this.Model.TimelineName}");
            }
        }

        private void TimelineTimer_Tick(
            object sender,
            EventArgs e)
        {
            try
            {
                if (!TimelineSettings.Instance.Enabled)
                {
                    return;
                }

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
                    $"[TL] Error Timeline ticker. name={this.Model.TimelineName}, zone={this.Model.Zone}, file={this.Model.File}");
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
                this.notifyQueue.Enqueue(act);
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
                // アクティブなアクティビティを設定する
                this.ActiveActivity = active;
                active.IsActive = true;

                // jumpを判定する
                if (!this.CallActivity(active))
                {
                    this.GoToActivity(active);
                }
            }

            // 表示を更新する
            this.RefreshActivityLineVisibility();
        }

        public void RefreshActivityLineVisibility()
        {
            var maxTime = this.CurrentTime.Add(TimeSpan.FromSeconds(
                TimelineSettings.Instance.ShowActivitiesTime));

            var toShow =
                from x in this.ActivityLine
                where
                x.Enabled.GetValueOrDefault() &&
                !string.IsNullOrEmpty(x.Text)
                orderby
                x.Seq ascending
                select
                x;

            var count = 0;
            foreach (var x in toShow)
            {
                if (count < TimelineSettings.Instance.ShowActivitiesCount &&
                    !x.IsDone &&
                    x.Time <= maxTime)
                {
                    if (count == 0)
                    {
                        this.Model.SubName = (x.Parent as TimelineSubroutineModel)?.Name ?? string.Empty;

                        x.IsTop = true;
                        x.Opacity = 1.0d;
                        x.Scale = TimelineSettings.Instance.NearestActivityScale;
                    }
                    else
                    {
                        x.IsTop = false;
                        x.Opacity = TimelineSettings.Instance.NextActivityBrightness;
                        x.Scale = 1.0d;
                    }

                    x.IsVisible = true;
                    x.RefreshProgress();
                    count++;
                }
                else
                {
                    x.IsVisible = false;
                }
            }
        }

        #endregion 時間進行関係のスレッド

        #region 通知に関するメソッド

        private static readonly object NoticeLocker = new object();

        private readonly ConcurrentQueue<TimelineBase> notifyQueue = new ConcurrentQueue<TimelineBase>();

        private ThreadWorker notifyWorker;

        private void StartNotifyWorker()
        {
            if (this.notifyWorker == null)
            {
                this.notifyWorker = new ThreadWorker(
                    () =>
                    {
                        while (this.notifyQueue.TryDequeue(out TimelineBase element))
                        {
                            switch (element)
                            {
                                case TimelineActivityModel act:
                                    this.NotifyActivity(act);
                                    break;

                                case TimelineTriggerModel tri:
                                    this.NotifyTrigger(tri);
                                    break;
                            }

                            Thread.Yield();
                        }
                    },
                    50,
                    "Timeline Notify Worker");
            }

            this.notifyWorker.Run();
        }

        public void StopNotifyWorker()
        {
            if (this.notifyWorker != null)
            {
                this.notifyWorker.Abort(50);
                while (this.notifyQueue.TryDequeue(out TimelineBase q)) ;
                this.notifyWorker = null;
            }
        }

        private void NotifyActivity(
            TimelineActivityModel act)
        {
            if (string.IsNullOrEmpty(act.Name) &&
                string.IsNullOrEmpty(act.Text) &&
                string.IsNullOrEmpty(act.Notice))
            {
                return;
            }

            lock (act)
            {
                if (act.IsNotified)
                {
                    return;
                }

                act.IsNotified = true;
            }

            var now = DateTime.Now;
            var offset = this.CurrentTime - act.Time;
            var log =
                $"[{now.ToString("HH:mm:ss.fff")}] 00:0038:{TLSymbol} Notice from TL. " +
                $"name={act.Name}, text={act.TextReplaced}, notice={act.NoticeReplaced}, offset={offset.TotalSeconds:N1}";

            var notice = act.NoticeReplaced;
            if (string.Equals(notice, "auto", StringComparison.OrdinalIgnoreCase))
            {
                notice = !string.IsNullOrEmpty(act.TextReplaced) ?
                    act.TextReplaced :
                    act.Name;

                if (offset.TotalSeconds <= -1.0)
                {
                    var ofsetText = (offset.TotalSeconds * -1).ToString("N0");
                    notice += $" まで、あと{ofsetText}秒";
                }

                if (!string.IsNullOrEmpty(notice))
                {
                    notice += "。";
                }
            }

            RaiseLog(log);
            NotifySound(notice, act.NoticeDevice.GetValueOrDefault());
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
                $"[{now.ToString("HH:mm:ss.fff")}] 00:0038:{TLSymbol} Notice from TL. " +
                $"name={tri.Name}, text={tri.Text}, notice={tri.Notice}";

            var notice = tri.Notice;
            if (string.Equals(notice, "auto", StringComparison.OrdinalIgnoreCase))
            {
                notice = !string.IsNullOrEmpty(tri.Text) ?
                    tri.Text :
                    tri.Name;

                if (!string.IsNullOrEmpty(notice))
                {
                    notice += "。";
                }
            }

            RaiseLog(log);
            NotifySound(notice, tri.NoticeDevice.GetValueOrDefault());

            var vnotices = tri.VisualNoticeStatements
                .Where(x => x.Enabled.GetValueOrDefault())
                .Select(x => x.Clone());

            if (!vnotices.Any())
            {
                return;
            }

            WPFHelper.BeginInvoke(() =>
            {
                foreach (var v in vnotices)
                {
                    switch (v.Text)
                    {
                        case TimelineVisualNoticeModel.ParentTextPlaceholder:
                            v.TextToDisplay = tri.TextReplaced;
                            break;

                        case TimelineVisualNoticeModel.ParentNoticePlaceholder:
                            v.TextToDisplay = tri.NoticeReplaced;
                            break;

                        default:
                            v.TextToDisplay = v.Text;
                            break;
                    }

                    if (string.IsNullOrEmpty(v.TextToDisplay))
                    {
                        continue;
                    }

                    // PC名をルールに従って置換する
                    v.TextToDisplay = FFXIVPlugin.Instance.ReplacePartyMemberName(
                        v.TextToDisplay,
                        Settings.Default.PCNameInitialOnDisplayStyle);

                    TimelineNoticeOverlay.NoticeView?.AddNotice(v);
                }
            });
        }

        private static string lastRaisedLog = string.Empty;
        private static DateTime lastRaisedLogTimestamp = DateTime.MinValue;

        private static void RaiseLog(
            string log)
        {
            if (string.IsNullOrEmpty(log))
            {
                return;
            }

            lock (NoticeLocker)
            {
                if (lastRaisedLog == log)
                {
                    if ((DateTime.Now - lastRaisedLogTimestamp).TotalSeconds <= 0.1)
                    {
                        return;
                    }
                }

                lastRaisedLog = log;
                lastRaisedLogTimestamp = DateTime.Now;
            }

            ActGlobals.oFormActMain.BeginInvoke((MethodInvoker)delegate
            {
                ActGlobals.oFormActMain.ParseRawLogLine(false, DateTime.Now, log);
            });
        }

        private static string lastNotice = string.Empty;
        private static DateTime lastNoticeTimestamp = DateTime.MinValue;

        private static void NotifySound(
            string notice,
            NoticeDevices device)
        {
            if (string.IsNullOrEmpty(notice))
            {
                return;
            }

            lock (NoticeLocker)
            {
                if (lastNotice == notice)
                {
                    if ((DateTime.Now - lastNoticeTimestamp).TotalSeconds <= 0.1)
                    {
                        return;
                    }
                }

                lastNotice = notice;
                lastNoticeTimestamp = DateTime.Now;
            }

            var isWave =
                notice.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                notice.EndsWith(".wave", StringComparison.OrdinalIgnoreCase);

            if (isWave)
            {
                notice = Path.Combine(
                    SoundController.Instance.WaveDirectory,
                    notice);
            }
            else
            {
                notice = TTSDictionary.Instance.ReplaceWordsTTS(notice);
            }

            switch (device)
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
