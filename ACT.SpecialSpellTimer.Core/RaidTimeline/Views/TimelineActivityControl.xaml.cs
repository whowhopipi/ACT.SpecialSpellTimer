using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ACT.SpecialSpellTimer.RaidTimeline.Views
{
    /// <summary>
    /// TimelineActivityControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineActivityControl :
        UserControl,
        INotifyPropertyChanged
    {
#if DEBUG

        public static readonly TimelineActivityModel DummyActivity = new TimelineActivityModel()
        {
            Name = "Dummy",
            Text = "ダミーアクティビティ",
            Time = TimeSpan.FromSeconds(10.1),
            StyleModel = TimelineStyle.SuperDefaultStyle,
        };

#endif

        public TimelineActivityControl()
        {
#if false
#if DEBUG
            if (WPFHelper.IsDesignMode &&
                this.Activity == null)
            {
                this.Activity = DummyActivity;
            }
#endif
#endif
            this.InitializeComponent();
            this.LoadResourcesDictionary();
        }

        public TimelineActivityModel Activity
        {
            get => this.DataContext as TimelineActivityModel;
            set => this.DataContext = value;
        }

        #region Resources Dictionary

        private void LoadResourcesDictionary()
        {
            const string Resources = @"Resources\Styles\TimelineActivityControlResources.xaml";

            var file = Path.Combine(PluginCore.Instance?.Location, Resources);
            if (File.Exists(file))
            {
                this.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri(file, UriKind.Absolute)
                });
            }
        }

        #endregion Resources Dictionary

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
