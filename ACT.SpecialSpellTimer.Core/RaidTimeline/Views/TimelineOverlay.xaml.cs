using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using FFXIV.Framework.Common;
using FFXIV.Framework.WPF.Views;

namespace ACT.SpecialSpellTimer.RaidTimeline.Views
{
    /// <summary>
    /// TimelineOverlay.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineOverlay :
        Window,
        IOverlay,
        INotifyPropertyChanged
    {
        #region Design View

        private static TimelineOverlay designOverlay;

        public static void ShowDesignOverlay()
        {
            if (designOverlay == null)
            {
                designOverlay = CreateDesignOverlay();
                designOverlay.Model = TimelineModel.DummyTimeline;
            }

            // 本番ビューを隠す
            if (TimelineView.OverlayVisible)
            {
                TimelineView.OverlayVisible = false;
            }

            designOverlay.Show();
            designOverlay.OverlayVisible = true;
        }

        public static void HideDesignOverlay()
        {
            if (designOverlay != null)
            {
                designOverlay.OverlayVisible = false;
                designOverlay.Hide();

                // 本番ビューを復帰させる
                if (TimelineSettings.Instance.OverlayVisible)
                {
                    if (!TimelineView.OverlayVisible &&
                        TimelineView.Model != null)
                    {
                        TimelineView.OverlayVisible = true;
                    }
                }
            }
        }

        private static TimelineOverlay CreateDesignOverlay()
        {
            var overlay = new TimelineOverlay();

            return overlay;
        }

        #endregion Design View

        #region View

        private static TimelineOverlay timelineView;

        private static TimelineOverlay TimelineView
        {
            get
            {
                if (timelineView == null)
                {
                    timelineView = new TimelineOverlay();
                    timelineView.Show();
                }

                return timelineView;
            }
        }

        public static void ShowTimeline(
            TimelineModel timelineModel)
        {
            WPFHelper.Invoke(() =>
            {
                if (TimelineSettings.Instance.OverlayVisible)
                {
                    TimelineView.Model = timelineModel;
                    TimelineView.OverlayVisible = true;

                    ChangeClickthrough(TimelineSettings.Instance.Clickthrough);
                }
            });
        }

        public static void ChangeClickthrough(
            bool isClickthrough)
        {
            if (timelineView != null)
            {
                timelineView.IsClickthrough = isClickthrough;
            }
        }

        public static void CloseTimeline()
        {
            WPFHelper.Invoke(() =>
            {
                if (TimelineOverlay.timelineView != null)
                {
                    TimelineOverlay.timelineView.Model = null;
                    TimelineOverlay.timelineView.DataContext = null;
                    TimelineOverlay.timelineView.Close();
                    TimelineOverlay.timelineView = null;
                }
            });
        }

        #endregion View

        public TimelineOverlay()
        {
            if (WPFHelper.IsDesignMode)
            {
                this.Model = TimelineModel.DummyTimeline;
            }

            this.InitializeComponent();
            this.LoadResourcesDictionary();

            this.ToNonActive();
            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            this.Opacity = 0;
            this.Topmost = false;
        }

        private TimelineModel model;

        public TimelineModel Model
        {
            get => this.model;
            set => this.SetProperty(ref this.model, value);
        }

        public TimelineSettings Config => TimelineSettings.Instance;

        #region Resources Dictionary

        private void LoadResourcesDictionary()
        {
            const string Resources = @"Resources\Styles\TimelineOverlayResources.xaml";

            var file = Path.Combine(PluginCore.Instance.Location, Resources);
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
                    }
                    else
                    {
                        this.ToNotTransparent();
                    }
                }
            }
        }

        #endregion IOverlay

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
