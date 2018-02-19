using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.RaidTimeline;
using ACT.SpecialSpellTimer.resources;
using ACT.SpecialSpellTimer.Utility;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Common;
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
                this.LoadTimelineModels();
                this.SetupZoneChanger();
            }
        }

        public TimelineSettings TimelineConfig => TimelineSettings.Instance;

        public string TimelineDirectory => DirectoryHelper.FindSubDirectory(@"resources\timeline");

        private ObservableCollection<TimelineModel> timelineModels = new ObservableCollection<TimelineModel>();

        public ObservableCollection<TimelineModel> TimelineModels => this.timelineModels;

        public bool LoadTimelineModels()
        {
            var result = true;

            var dir = this.TimelineDirectory;
            if (!Directory.Exists(dir))
            {
                return result;
            }

            var list = new List<TimelineModel>();
            foreach (var file in Directory.GetFiles(dir, "*.xml"))
            {
                try
                {
                    var tl = TimelineModel.Load(file);
                    list.Add(tl);
                }
                catch (Exception ex)
                {
                    this.AppLogger.Error(
                        ex,
                        $"[TL] Load error. file={file}");

                    result = false;
                }
            }

            WPFHelper.Invoke(() =>
            {
                lock (this)
                {
                    foreach (var tl in this.TimelineModels)
                    {
                        if (tl.IsActive)
                        {
                            tl.IsActive = false;
                        }
                    }

                    this.TimelineModels.Clear();
                    this.TimelineModels.AddRange(list.OrderBy(x => x.FileName));
                    this.currentZoneName = string.Empty;
                }
            });

            return result;
        }

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
                await Task.Run(() => this.LoadTimelineModels());
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
                    style = TimelineStyle.DefaultStyle.Clone();
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
            lock (this)
            {
                if (this.currentZoneName != ActGlobals.oFormActMain.CurrentZone)
                {
                    this.currentZoneName = ActGlobals.oFormActMain.CurrentZone;

                    foreach (var tl in this.TimelineModels.Where(x => x.IsActive))
                    {
                        tl.IsActive = false;
                    }

                    var nextTimeline = this.TimelineModels.FirstOrDefault(x => x.Controller.IsAvailable);
                    if (nextTimeline != null)
                    {
                        nextTimeline.IsActive = true;
                    }
                }
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
