using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.RaidTimeline;
using ACT.SpecialSpellTimer.resources;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.Globalization;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TimelineTesterView.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineTesterView :
        Window,
        ILocalizable,
        INotifyPropertyChanged
    {
        public TimelineTesterView(
            string logFile)
        {
            this.LogFile = logFile;

            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
            this.LoadConfigViewResources();

#if !DEBUG
            this.Topmost = true;
#endif

            // ウィンドウのスタート位置を決める
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            this.CloseButton.Click += (x, y) =>
            {
                this.testTimer.Stop();
                this.Close();
            };

            this.Loaded += this.TimelineTesterView_Loaded;
            this.testTimer.Tick += this.TestTimer_Tick;

            this.RunButton.Click += (x, y) =>
            {
                lock (this)
                {
                    this.testTimer.Stop();
                    this.isPause = false;

                    foreach (var log in this.Logs)
                    {
                        log.IsDone = false;
                    }

                    this.TestStartTime = DateTime.Now;
                    this.testTimer.Start();
                }
            };

            this.PauseButton.Click += (x, y) =>
            {
                this.isPause = !this.isPause;

                if (this.isPause)
                {
                    this.PauseButton.Content = "Resume";
                }
                else
                {
                    this.PauseButton.Content = "Pause";
                }
            };

            this.StopButton.Click += (x, y) =>
            {
                lock (this)
                {
                    this.testTimer.Stop();

                    foreach (var log in this.Logs)
                    {
                        log.IsDone = false;
                    }

                    this.TestStartTime = DateTime.MinValue;
                }
            };
        }

        public string LogFile
        {
            get;
            private set;
        }

        public ObservableCollection<TestLog> Logs
        {
            get;
            private set;
        } = new ObservableCollection<TestLog>();

        private DateTime testStartTime;
        private TimeSpan testTime;

        private DateTime TestStartTime
        {
            get => this.testStartTime;
            set
            {
                if (this.SetProperty(ref this.testStartTime, value))
                {
                    this.TestTime = TimeSpan.Zero;
                }
            }
        }

        private TimeSpan TestTime
        {
            get => this.testTime;
            set
            {
                if (this.SetProperty(ref this.testTime, value))
                {
                    this.RaisePropertyChanged(nameof(this.TestTimeText));
                }
            }
        }

        public string TestTimeText => this.TestTime.ToTLString();

        private DispatcherTimer testTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(0.1),
        };

        private async void TimelineTesterView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            if (!File.Exists(this.LogFile))
            {
                return;
            }

            var list = new List<TestLog>();

            await Task.Run(() =>
            {
                var seq = 1L;
                using (var sr = new StreamReader(this.LogFile, new UTF8Encoding(false)))
                {
                    while (!sr.EndOfStream)
                    {
                        var logline = sr.ReadLine();
                        var log = new TestLog(logline)
                        {
                            Seq = seq++
                        };

                        list.Add(log);
                    }
                }

                if (!list.Any())
                {
                    return;
                }

                var first = list.First();

                foreach (var log in list)
                {
                    log.Time = log.Timestamp - first.Timestamp;
                }
            });

            this.Logs.Clear();
            this.Logs.AddRange(list);
        }

        private volatile bool isPause = false;

        private void TestTimer_Tick(object sender, EventArgs e)
        {
            lock (this)
            {
                if (this.isPause)
                {
                    return;
                }

                this.TestTime = DateTime.Now - this.TestStartTime;

                var logs = (
                    from x in this.Logs
                    where
                    x.Time <= this.TestTime &&
                    !x.IsDone
                    orderby
                    x.Seq
                    select
                    x).ToArray();

                foreach (var log in logs)
                {
                    Thread.Yield();

                    log.IsDone = true;

                    var logInfo = new LogLineEventArgs(
                        $"[{DateTime.Now:HH:mm:ss.fff}] {log.Log}",
                        0,
                        DateTime.Now,
                        string.Empty,
                        true);

                    TimelineController.CurrentController?.EnqueueLog(logInfo);

                    this.TimelineTestListView.ScrollIntoView(log);
                }
            }
        }

        #region ILocalizebale

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        #endregion ILocalizebale

        #region INotifyPropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(
            [CallerMemberName]string propertyName = null)
        {
            this.PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            this.PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));

            return true;
        }

        #endregion INotifyPropertyChanged

        public class TestLog :
            BindableBase
        {
            private string logline;

            public TestLog(string logline)
            {
                this.logline = logline;

                this.Log = logline.Remove(0, 15);

                var timestampText = logline.Substring(0, 15).TrimEnd()
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                DateTime d;
                if (DateTime.TryParse(timestampText, out d))
                {
                    this.Timestamp = d;
                }
            }

            public long Seq { get; set; }

            private bool isDone;

            public bool IsDone
            {
                get => this.isDone;
                set
                {
                    if (this.SetProperty(ref this.isDone, value))
                    {
                        this.RaisePropertyChanged(nameof(this.DoneText));
                    }
                }
            }

            public string DoneText => this.IsDone ? "✔" : string.Empty;

            private DateTime timestamp;

            public DateTime Timestamp
            {
                get => this.timestamp;
                set => this.SetProperty(ref this.timestamp, value);
            }

            private TimeSpan time;

            public TimeSpan Time
            {
                get => this.time;
                set
                {
                    if (this.SetProperty(ref this.time, value))
                    {
                        this.RaisePropertyChanged(nameof(this.TimeText));
                    }
                }
            }

            public string TimeText => this.Time.ToTLString();

            private string log;

            public string Log
            {
                get => this.log;
                set => this.SetProperty(ref this.log, value);
            }

            public override string ToString() => $"[{this.TimeText}] {this.Log}";
        }
    }
}
