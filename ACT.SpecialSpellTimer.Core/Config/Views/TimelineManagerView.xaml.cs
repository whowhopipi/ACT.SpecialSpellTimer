using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.RaidTimeline;
using ACT.SpecialSpellTimer.RaidTimeline.Views;
using ACT.SpecialSpellTimer.resources;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Common;
using FFXIV.Framework.Dialog;
using FFXIV.Framework.Globalization;
using Prism.Commands;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TimelineManagerView.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineManagerView :
        UserControl,
        ILocalizable,
        INotifyPropertyChanged
    {
        #region Logger

        private NLog.Logger AppLogger => FFXIV.Framework.Common.AppLog.DefaultLogger;

        #endregion Logger

        public TimelineManagerView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);

            if (!WPFHelper.IsDesignMode)
            {
                TimelineManager.Instance.LoadTimelineModels();
                this.SetupZoneChanger();
            }
        }

        public string TimelineDirectory => TimelineManager.Instance.TimelineDirectory;

        public ObservableCollection<TimelineModel> TimelineModels => TimelineManager.Instance.TimelineModels;

        public TimelineSettings TimelineConfig => TimelineSettings.Instance;

        private ICommand openTimelineFolderCommand;

        public ICommand OpenTimelineFolderCommand =>
            this.openTimelineFolderCommand ?? (this.openTimelineFolderCommand = new DelegateCommand(() =>
            {
                if (Directory.Exists(this.TimelineDirectory))
                {
                    Process.Start(this.TimelineDirectory);
                }
            }));

        private ICommand reloadTimelineFolderCommand;

        public ICommand ReloadTimelineFolderCommand =>
            this.reloadTimelineFolderCommand ?? (this.reloadTimelineFolderCommand = new DelegateCommand(async () =>
            {
                await Task.Run(() => TimelineManager.Instance.LoadTimelineModels());

                lock (this)
                {
                    this.currentZoneName = string.Empty;
                }
            }));

        private ICommand startTimelineCommand;

        public ICommand StartTimelineCommand =>
            this.startTimelineCommand ?? (this.startTimelineCommand = new DelegateCommand<Button>((button) =>
            {
                if (button == null)
                {
                    return;
                }

                var activeTL = TimelineManager.Instance.TimelineModels.FirstOrDefault(x => x.IsActive);
                if (activeTL == null)
                {
                    return;
                }

                var toStart = button.Content.ToString() == "Start";

                if (toStart)
                {
                    activeTL.Controller.StartActivityLine();
                    button.Content = "Stop";
                }
                else
                {
                    activeTL.Controller.EndActivityLine();
                    button.Content = "Start";
                }
            }));

        private ICommand testTimelineCommand;

        public ICommand TestTimelineCommand =>
            this.testTimelineCommand ?? (this.testTimelineCommand = new DelegateCommand(() =>
            {
                this.TestTimeline();
            }));

        private System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()
        {
            RestoreDirectory = true,
            Filter = "CombatLog Files|*.log|All Files|*.*",
            FilterIndex = 0,
            DefaultExt = ".log",
            SupportMultiDottedExtensions = true,
        };

        /// <summary>
        /// タイムラインをテストする
        /// </summary>
        private void TestTimeline()
        {
            var result = this.openFileDialog.ShowDialog(ActGlobals.oFormActMain);
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var view = new TimelineTesterView(this.openFileDialog.FileName);
            view.Show();
        }

        private ICommand showDummyOverlayCommand;

        public ICommand ShowDummyOverlayCommand =>
            this.showDummyOverlayCommand ?? (this.showDummyOverlayCommand = new DelegateCommand<bool?>((isChecked) =>
            {
                if (!isChecked.HasValue)
                {
                    return;
                }

                if (isChecked.Value)
                {
                    TimelineOverlay.ShowDesignOverlay();
                }
                else
                {
                    TimelineOverlay.HideDesignOverlay();
                }
            }));

        private ICommand CreateChangeColorCommand(
            Func<Color> getCurrentColor,
            Action<Color> changeColorAction)
            => new DelegateCommand(() =>
            {
                var result = ColorDialogWrapper.ShowDialog(getCurrentColor(), false);
                if (result.Result)
                {
                    changeColorAction.Invoke(result.Color);
                }
            });

        private ICommand changeBackgroundColorCommand;

        public ICommand ChangeBackgroundColorCommand =>
            this.changeBackgroundColorCommand ?? (this.changeBackgroundColorCommand = this.CreateChangeColorCommand(
                () => this.TimelineConfig.BackgroundColor,
                (color) => this.TimelineConfig.BackgroundColor = color));

        private ICommand addStyleCommand;

        public ICommand AddStyleCommand =>
            this.addStyleCommand ?? (this.addStyleCommand = new DelegateCommand(() =>
            {
                var style = default(TimelineStyle);

                if (this.StyleListView.SelectedItem != null)
                {
                    style = (this.StyleListView.SelectedItem as TimelineStyle).Clone();
                }
                else
                {
                    style = TimelineStyle.SuperDefaultStyle.Clone();
                }

                style.Name = "New Style";
                TimelineSettings.Instance.Styles.Add(style);
                this.StyleListView.SelectedItem = style;
            }));

        private ICommand deleteStyleCommand;

        public ICommand DeleteStyleCommand =>
            this.deleteStyleCommand ?? (this.deleteStyleCommand = new DelegateCommand(() =>
            {
                if (this.StyleListView.SelectedItem != null)
                {
                    var style = this.StyleListView.SelectedItem as TimelineStyle;

                    if (TimelineSettings.Instance.Styles.Count > 1)
                    {
                        TimelineSettings.Instance.Styles.Remove(style);
                        this.StyleListView.SelectedItem = TimelineSettings.Instance.Styles.First();
                    }
                }
            }));

        private void TopActivityStyle_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            TimelineController.CurrentController?.Model?.RefreshTopActivityStyle();
            TimelineModel.DummyTimeline.RefreshTopActivityStyle();
        }

        #region Zone Changer

        private string currentZoneName = string.Empty;
        private DispatcherTimer timer = null;

        private void SetupZoneChanger()
        {
            this.timer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromSeconds(1),
            };

            this.timer.Tick += this.ZoneChanger_Tick;
            this.timer.Start();
        }

        private void ZoneChanger_Tick(object sender, EventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (this.currentZoneName != ActGlobals.oFormActMain.CurrentZone)
                    {
                        this.currentZoneName = ActGlobals.oFormActMain.CurrentZone;

                        var tls = TimelineManager.Instance.TimelineModels.ToArray();

                        foreach (var tl in tls.Where(x => x.IsActive))
                        {
                            tl.Controller.Unload();
                            tl.IsActive = false;
                        }

                        var nextTimeline = tls.FirstOrDefault(x => x.Controller.IsAvailable);
                        if (nextTimeline != null)
                        {
                            nextTimeline.Controller.Load();
                            nextTimeline.IsActive = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AppLogger.Error(
                    ex,
                    $"[TL] Auto loading error. zone={ActGlobals.oFormActMain.CurrentZone}");
            }
        }

        #endregion Zone Changer

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
    }
}
