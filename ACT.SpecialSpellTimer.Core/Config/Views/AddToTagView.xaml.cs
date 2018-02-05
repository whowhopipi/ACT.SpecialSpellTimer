using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// AddToTagView.xaml の相互作用ロジック
    /// </summary>
    public partial class AddToTagView :
        Window,
        ILocalizable,
        INotifyPropertyChanged
    {
        public AddToTagView() : this(new Tag()
        {
            Name = "DUMMY TAG"
        })
        {
        }

        public AddToTagView(
            Tag targetTag)
        {
            this.TargetTag = targetTag;

            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);

            // ウィンドウのスタート位置を決める
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            this.PreviewKeyUp += (x, y) =>
            {
                if (y.Key == Key.Escape)
                {
                    this.Close();
                }
            };

            this.CloseButton.Click += (x, y) =>
            {
                this.Close();
            };

            this.ApplyButton.Click += this.ApplyButton_Click;
        }

        private void ApplyButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            var items = new List<ItemTags>();

            foreach (var dest in this.TargetsSpells.Where(x => x.ToCopy))
            {
                var panel = dest.Item as SpellPanel;

                if (!TagTable.Instance.ItemTags.Any(x =>
                    x.ItemID == panel.ID &&
                    x.TagID == this.TargetTag.ID))
                {
                    items.Add(new ItemTags(panel.ID, this.TargetTag.ID));
                }
            }

            foreach (var dest in this.TargetsTickers.Where(x => x.ToCopy))
            {
                var ticker = dest.Item as Ticker;

                if (!TagTable.Instance.ItemTags.Any(x =>
                    x.ItemID == ticker.Guid &&
                    x.TagID == this.TargetTag.ID))
                {
                    items.Add(new ItemTags(ticker.Guid, this.TargetTag.ID));
                }
            }

            TagTable.Instance.ItemTags.AddRange(items);
            TagTable.Instance.Save();

            this.Close();
        }

        public Tag TargetTag { get; set; }

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        private CopyConfigView.Dest[] targetsSpells;
        private CopyConfigView.Dest[] targetsTickers;

        public CopyConfigView.Dest[] TargetsSpells
        {
            get
            {
                if (this.targetsSpells == null)
                {
                    var dests = new List<CopyConfigView.Dest>();

                    dests.AddRange(
                        from x in SpellPanelTable.Instance.Table
                        orderby
                        x.PanelName
                        select new CopyConfigView.Dest()
                        {
                            Item = x
                        });

                    this.targetsSpells = dests.ToArray();
                }

                return this.targetsSpells;
            }
        }

        public CopyConfigView.Dest[] TargetsTickers
        {
            get
            {
                if (this.targetsTickers == null)
                {
                    var dests = new List<CopyConfigView.Dest>();

                    dests.AddRange(
                        from x in TickerTable.Instance.Table
                        orderby
                        x.Title,
                        x.ID
                        select new CopyConfigView.Dest()
                        {
                            Item = x
                        });

                    this.targetsTickers = dests.ToArray();
                }

                return this.targetsTickers;
            }
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
    }
}
