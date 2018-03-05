using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Threading;
using FFXIV.Framework.Common;
using FFXIV.Framework.WPF.Views;

namespace ACT.SpecialSpellTimer.RaidTimeline.Views
{
    /// <summary>
    /// TimelineNoticeOverlay.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineNoticeOverlay :
        Window,
        IOverlay,
        INotifyPropertyChanged
    {
        #region Design View

        private static TimelineNoticeOverlay designOverlay;

        private static IList<TimelineVisualNoticeModel> dummyNoticeList;

        private static IList<TimelineVisualNoticeModel> BindingDummyNoticeList => dummyNoticeList;

        public static void ShowDesignOverlay(
            TimelineStyle testStyle = null)
        {
            if (designOverlay == null)
            {
                dummyNoticeList = TimelineVisualNoticeModel.CreateDummyNotices(testStyle);

                designOverlay = CreateDesignOverlay();
            }

            designOverlay.Show();
            designOverlay.OverlayVisible = true;

            foreach (var notice in dummyNoticeList)
            {
                designOverlay.AddNotice(notice, true);
            }
        }

        public static void HideDesignOverlay()
        {
            if (designOverlay != null)
            {
                designOverlay.OverlayVisible = false;
                designOverlay.Hide();
                designOverlay.Close();
                designOverlay = null;
            }
        }

        private static TimelineNoticeOverlay CreateDesignOverlay()
        {
            var overlay = new TimelineNoticeOverlay();
            return overlay;
        }

        #endregion Design View

        #region View

        public static TimelineNoticeOverlay NoticeView { get; private set; }

        public static void ShowNotice()
        {
            if (!TimelineSettings.Instance.Enabled ||
                !TimelineSettings.Instance.OverlayVisible)
            {
                return;
            }

            WPFHelper.Invoke(() =>
            {
                if (NoticeView == null)
                {
                    NoticeView = new TimelineNoticeOverlay();
                    NoticeView.Show();
                }

                ChangeClickthrough(TimelineSettings.Instance.Clickthrough);

                NoticeView.OverlayVisible = true;
            });
        }

        public static void ChangeClickthrough(
            bool isClickthrough)
        {
            if (NoticeView != null)
            {
                NoticeView.IsClickthrough = isClickthrough;
            }
        }

        public static void CloseNotice()
        {
            WPFHelper.Invoke(() =>
            {
                if (NoticeView != null)
                {
                    NoticeView.DataContext = null;
                    NoticeView.Close();
                    NoticeView = null;
                }
            });
        }

        #endregion View

        public TimelineNoticeOverlay()
        {
            this.InitializeComponent();
            this.LoadResourcesDictionary();

            this.ToNonActive();

            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            this.Opacity = 0;
            this.Topmost = false;

            this.Loaded += (x, y) =>
            {
                this.IsClickthrough = this.Config.Clickthrough;
                this.StartZOrderCorrector();
            };

            this.Closed += (x, y) =>
            {
                this.StopZOrderCorrector();
            };

            this.SetupNoticesSource();
        }

        private readonly ObservableCollection<TimelineVisualNoticeModel> noticeList =
            new ObservableCollection<TimelineVisualNoticeModel>();

        public void AddNotice(
            TimelineVisualNoticeModel notice,
            bool dummyMode = false)
        {
            notice.StartNotice(
                (toRemove) => this.noticeList.Remove(toRemove),
                dummyMode);
            this.noticeList.Add(notice);
        }

        public void ClearNotice()
        {
            this.noticeList.Clear();
        }

        private CollectionViewSource noticesSource;

        public ICollectionView NoticeList => this.noticesSource?.View;

        public TimelineSettings Config => TimelineSettings.Instance;

        private void SetupNoticesSource()
        {
            this.noticesSource = new CollectionViewSource()
            {
                Source = this.noticeList,
                IsLiveFilteringRequested = true,
                IsLiveSortingRequested = true,
            };

            this.noticesSource.Filter += (x, y) =>
            {
                y.Accepted = (y.Item as TimelineVisualNoticeModel).IsVisible;
            };

            this.noticesSource.LiveFilteringProperties.Add(nameof(TimelineVisualNoticeModel.IsVisible));

            this.noticesSource.SortDescriptions.AddRange(new[]
            {
                new SortDescription()
                {
                    PropertyName = nameof(TimelineVisualNoticeModel.DurationToDisplay),
                    Direction = ListSortDirection.Ascending,
                }
            });

            this.RaisePropertyChanged(nameof(this.NoticeList));
        }

        #region Resources Dictionary

        private void LoadResourcesDictionary()
        {
            const string Direcotry = @"Resources\Styles";
            const string Resources = @"TimelineNoticeOverlayResources.xaml";
            var file = System.IO.Path.Combine(DirectoryHelper.FindSubDirectory(Direcotry), Resources);
            if (File.Exists(file))
            {
                this.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri(file, UriKind.Absolute)
                });
            }
        }

        #endregion Resources Dictionary

        #region IOverlay

        private bool overlayVisible;

        public bool OverlayVisible
        {
            get => this.overlayVisible;
            set => this.SetOverlayVisible(ref this.overlayVisible, value, this.Config.OverlayOpacity);
        }

        private bool? isClickthrough = null;

        public bool IsClickthrough
        {
            get => this.isClickthrough ?? false;
            set
            {
                if (this.isClickthrough != value)
                {
                    this.isClickthrough = value;

                    if (this.isClickthrough.Value)
                    {
                        this.ToTransparent();
                        this.ResizeMode = ResizeMode.NoResize;
                    }
                    else
                    {
                        this.ToNotTransparent();
                        this.ResizeMode = ResizeMode.CanResizeWithGrip;
                    }
                }
            }
        }

        #endregion IOverlay

        #region ZOrder Corrector

        private DispatcherTimer zorderCorrector;

        private void StartZOrderCorrector()
        {
            this.zorderCorrector = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromSeconds(1),
            };

            this.zorderCorrector.Tick += (x, y) =>
            {
                if (this.Visibility == Visibility.Visible)
                {
                    if (!this.IsOverlaysGameWindow())
                    {
                        this.EnsureTopMost();
                    }
                }
            };

            this.zorderCorrector.Start();
        }

        private void StopZOrderCorrector()
        {
            if (this.zorderCorrector != null)
            {
                this.zorderCorrector.Stop();
                this.zorderCorrector = null;
            }
        }

        public IntPtr Handle => new WindowInteropHelper(this).Handle;

        private bool IsOverlaysGameWindow()
        {
            var xivHandle = GetGameWindowHandle();
            var handle = this.Handle;

            while (handle != IntPtr.Zero)
            {
                // Overlayウィンドウよりも前面側にFF14のウィンドウがあった
                if (handle == xivHandle)
                {
                    return false;
                }

                handle = NativeMethods.GetWindow(handle, NativeMethods.GW_HWNDPREV);
            }

            // 前面側にOverlayが存在する、もしくはFF14が起動していない
            return true;
        }

        private void EnsureTopMost()
        {
            NativeMethods.SetWindowPos(
                this.Handle,
                NativeMethods.HWND_TOPMOST,
                0, 0, 0, 0,
                NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOACTIVATE);
        }

        private static object xivProcLocker = new object();
        private static Process xivProc;
        private static DateTime lastTry;
        private static TimeSpan tryInterval = new TimeSpan(0, 0, 15);

        private static IntPtr GetGameWindowHandle()
        {
            lock (xivProcLocker)
            {
                try
                {
                    // プロセスがすでに終了してるならプロセス情報をクリア
                    if (xivProc != null && xivProc.HasExited)
                    {
                        xivProc = null;
                    }

                    // プロセス情報がなく、tryIntervalよりも時間が経っているときは新たに取得を試みる
                    if (xivProc == null && DateTime.Now - lastTry > tryInterval)
                    {
                        xivProc = Process.GetProcessesByName("ffxiv").FirstOrDefault();
                        if (xivProc == null)
                        {
                            xivProc = Process.GetProcessesByName("ffxiv_dx11").FirstOrDefault();
                        }

                        lastTry = DateTime.Now;
                    }

                    if (xivProc != null)
                    {
                        return xivProc.MainWindowHandle;
                    }
                }
                catch (System.ComponentModel.Win32Exception) { }

                return IntPtr.Zero;
            }
        }

        #endregion ZOrder Corrector

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
    }
}
