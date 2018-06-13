using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Config;
using FFXIV.Framework.Common;
using FFXIV.Framework.FFXIVHelper;
using FFXIV.Framework.WPF.Views;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.Views
{
    /// <summary>
    /// POSView.xaml の相互作用ロジック
    /// </summary>
    public partial class POSView :
        Window,
        IOverlay
    {
        private static POSView instance;

        public static POSView Instance => instance;

        public static void ShowPOS()
        {
            instance = new POSView()
            {
                OverlayVisible = Settings.Default.POSViewVisible,
            };

            instance.Show();
        }

        public static void ClosePOS()
        {
            if (instance != null)
            {
                instance.Close();
                instance = null;
            }
        }

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);

        public POSView()
        {
            this.InitializeComponent();

            this.DataContext = new POSViewModel();

            this.Opacity = 0;
            this.ToNonActive();

            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            this.Loaded += this.POSView_Loaded;
            this.Closed += this.POSView_Closed;
#if !DEBUG
            this.XText.Text = string.Empty;
            this.YText.Text = string.Empty;
            this.ZText.Text = string.Empty;
#endif
            this.BaseGrid.Visibility = Visibility.Hidden;
        }

        public POSViewModel ViewModel => this.DataContext as POSViewModel;

        private bool overlayVisible;
        private bool? clickTranceparent;

        public bool OverlayVisible
        {
            get => this.overlayVisible;
            set => this.SetOverlayVisible(ref this.overlayVisible, value, Settings.Default.Opacity);
        }

        public bool ClickTransparent
        {
            get => this.clickTranceparent ?? false;
            set
            {
                if (this.clickTranceparent != value)
                {
                    this.clickTranceparent = value;

                    if (this.clickTranceparent.Value)
                    {
                        this.ToTransparent();
                    }
                    else
                    {
                        this.ToNotTransparent();
                    }
                }
            }
        }

        private void POSView_Loaded(object sender, RoutedEventArgs e)
        {
            this.timer.Interval = TimeSpan.FromSeconds(1.0);
            this.timer.Tick += this.Timer_Tick;
            this.timer.Start();

            this.StartZOrderCorrector();
        }

        private void POSView_Closed(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.timer = null;

            this.StopZOrderCorrector();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!PluginMainWorker.Instance.IsFFXIVActive &&
                Settings.Default.HideWhenNotActive)
            {
                this.BaseGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                var player = FFXIVPlugin.Instance.GetPlayer();
                if (player == null)
                {
                    this.BaseGrid.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.XText.Text = player.PosXMap.ToString("N2");
                    this.YText.Text = player.PosYMap.ToString("N2");
                    this.ZText.Text = player.PosZMap.ToString("N2");

                    this.BaseGrid.Visibility = Visibility.Visible;
                }
            }

            // ついでにクリック透過を切り替える
            this.ClickTransparent = Settings.Default.ClickThroughEnabled;
        }

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
                if (this.OverlayVisible)
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
    }

    public class POSViewModel :
        BindableBase
    {
        public Settings Config => Settings.Default;
    }
}
