using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
            }
        }

        private static TimelineOverlay CreateDesignOverlay()
        {
            var overlay = new TimelineOverlay();

            return overlay;
        }

        #endregion Design View

#if DEBUG

        public TimelineOverlay() :
            this(TimelineModel.DummyTimeline)
        {
        }

#else
        public TimelineOverlay() :
            this(new TimelineModel())
        {
        }
#endif

        public TimelineOverlay(
            TimelineModel model)
        {
            this.Model = model;
            this.Model.Controller.View = this;

            this.InitializeComponent();

            this.ToNonActive();
            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            this.Opacity = 0;
            this.Topmost = false;

            this.Closed += (x, y) =>
            {
                if (this.Model != null)
                {
                    this.Model.Controller.View = null;
                }
            };
        }

        private TimelineModel model;

        public TimelineModel Model
        {
            get => this.model;
            private set => this.SetProperty(ref this.model, value);
        }

        public TimelineSettings Config => TimelineSettings.Instance;

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
