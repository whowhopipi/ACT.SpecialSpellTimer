using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// InformationsView.xaml の相互作用ロジック
    /// </summary>
    public partial class InformationsView :
        UserControl,
        ILocalizable,
        INotifyPropertyChanged
    {
        public InformationsView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);

            this.timer.Interval = TimeSpan.FromSeconds(10);
            this.timer.Tick += (x, y) =>
            {
                if (this.IsLoaded)
                {
                    this.RefreshPlaceholderList();
                    this.RefreshTriggerList();
                }
            };

            this.timer.Start();
        }

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        public Settings Config => Settings.Default;

        public ObservableCollection<TableCompiler.PlaceholderContainer> PlaceholderList
        {
            get;
            private set;
        } = new ObservableCollection<TableCompiler.PlaceholderContainer>();

        public ObservableCollection<TriggerContainer> TriggerList
        {
            get;
            private set;
        } = new ObservableCollection<TriggerContainer>();

        public long ActiveTriggerCount { get; private set; } = 0;

        private void RefreshPlaceholderList()
        {
            this.PlaceholderList.Clear();
            this.PlaceholderList.AddRange(
                from x in TableCompiler.Instance.PlaceholderList
                orderby
                x.Type,
                x.Placeholder
                select
                x);
        }

        private void RefreshTriggerList()
        {
            this.TriggerList.Clear();

            var source = TableCompiler.Instance.TriggerList;

            var i = 1;
            var query =
                from x in source
                where
                (x as Ticker) != null ||
                (x as Spell).IsInstance == false
                orderby
                x.ItemType
                select new TriggerContainer()
                {
                    Trigger = x,
                    No = i++,
                };

            this.TriggerList.AddRange(query);

            this.ActiveTriggerCount = source.LongCount();
            this.RaisePropertyChanged(nameof(this.ActiveTriggerCount));
        }

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

        public class TriggerContainer
        {
            public ITrigger Trigger { get; set; }

            public long No { get; set; }

            public string TriggerType => this.Trigger?.ItemType.ToString();

            public string Name
            {
                get
                {
                    switch (this.Trigger)
                    {
                        case Spell s:
                            return s.SpellTitle;

                        case Ticker t:
                            return t.Title;

                        default:
                            return string.Empty;
                    }
                }
            }

            public string Pattern
            {
                get
                {
                    switch (this.Trigger)
                    {
                        case Spell s:
                            return
                                !string.IsNullOrEmpty(s.KeywordReplaced) ?
                                s.KeywordReplaced :
                                s.Keyword;

                        case Ticker t:
                            return
                                !string.IsNullOrEmpty(t.KeywordReplaced) ?
                                t.KeywordReplaced :
                                t.Keyword;

                        default:
                            return string.Empty;
                    }
                }
            }

            public bool UseRegex
            {
                get
                {
                    switch (this.Trigger)
                    {
                        case Spell s:
                            return s.RegexEnabled;

                        case Ticker t:
                            return t.RegexEnabled;

                        default:
                            return false;
                    }
                }
            }
        }
    }
}
