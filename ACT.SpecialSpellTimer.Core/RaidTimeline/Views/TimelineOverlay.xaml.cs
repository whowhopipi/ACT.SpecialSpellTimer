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
        public TimelineOverlay(
            TimelineModel model)
        {
            this.DataContext = model;
            model.Controller.View = this;

            this.InitializeComponent();

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

        public TimelineSettings Config => TimelineSettings.Instance;

        public TimelineModel Model => this.DataContext as TimelineModel;

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
