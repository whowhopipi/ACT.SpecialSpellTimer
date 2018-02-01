using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Models;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.WPF.Views;

namespace ACT.SpecialSpellTimer.Views
{
    /// <summary>
    /// SpellTimerList Window
    /// </summary>
    public partial class SpellTimerListWindow :
        Window,
        IOverlay,
        INotifyPropertyChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpellTimerListWindow(
            SpellPanel config)
        {
            this.Config = config;
            this.Config.PanelWindow = this;

            this.InitializeComponent();

            this.MouseLeftButtonDown += (x, y) => this.DragMove();
            this.Closed += (x, y) =>
            {
                if (this.SpellControls != null)
                {
                    this.SpellControls.Clear();
                }

                if (this.Config != null)
                {
                    this.Config.PanelWindow = null;
                    this.Config = null;
                }
            };
        }

        private bool overlayVisible;

        public bool OverlayVisible
        {
            get => this.overlayVisible;
            set => this.SetOverlayVisible(ref this.overlayVisible, value, Settings.Default.OpacityToView);
        }

        public SpellPanel Config { get; set; }

        public Dictionary<long, SpellTimerControl> SpellControls { get; private set; } = new Dictionary<long, SpellTimerControl>();

        public Spell[] Spells { get; set; }

        private SolidColorBrush backgroundBrush;

        public SolidColorBrush BackgroundBrush
        {
            get => this.backgroundBrush;
            set => this.SetProperty(ref this.backgroundBrush, value);
        }

        /// <summary>
        /// SpellTimerの描画をRefreshする
        /// </summary>
        public void RefreshSpellTimer()
        {
            // 表示するものがなければ何もしない
            if (this.Spells == null)
            {
                this.HideOverlay();
                return;
            }

            // 表示対象だけに絞る
            var spells =
                from x in this.Spells
                where
                x.ProgressBarVisible
                select
                x;

            // タイムアップしたものを除外する
            if ((Settings.Default.TimeOfHideSpell > 0.0d) &&
                this.Config.SortOrder != SpellOrders.Fixed)
            {
                spells =
                    from x in spells
                    where
                    x.DontHide ||
                    x.IsDesignMode ||
                    (DateTime.Now - x.CompleteScheduledTime).TotalSeconds <= Settings.Default.TimeOfHideSpell
                    select
                    x;
            }

            if (!spells.Any())
            {
                this.HideOverlay();
                return;
            }

            // ソートする
            switch (this.Config.SortOrder)
            {
                case SpellOrders.None:
                case SpellOrders.SortPriority:
                case SpellOrders.Fixed:
                    spells =
                        from x in spells
                        orderby
                        x.DisplayNo,
                        x.ID
                        select
                        x;
                    break;

                case SpellOrders.SortRecastTimeASC:
                    spells =
                        from x in spells
                        orderby
                        x.CompleteScheduledTime,
                        x.DisplayNo,
                        x.ID
                        select
                        x;
                    break;

                case SpellOrders.SortRecastTimeDESC:
                    spells =
                        from x in spells
                        orderby
                        x.CompleteScheduledTime descending,
                        x.DisplayNo,
                        x.ID
                        select
                        x;
                    break;

                case SpellOrders.SortMatchTime:
                    spells =
                        from x in spells
                        orderby
                        x.MatchDateTime == DateTime.MinValue ?
                            DateTime.MaxValue :
                            x.MatchDateTime,
                        x.DisplayNo,
                        x.ID
                        select
                        x;
                    break;
            }

            // 背景色を設定する
            if (spells.Count() > 0)
            {
                var s = spells.FirstOrDefault();
                if (s != null)
                {
                    var c = s.BackgroundColor.FromHTMLWPF();
                    var backGroundColor = Color.FromArgb(
                        (byte)s.BackgroundAlpha,
                        c.R,
                        c.G,
                        c.B);

                    this.BackgroundBrush = this.GetBrush(backGroundColor);
                }
            }

            // 水平レイアウト時のマージンを調整する
            var m = this.BaseGrid.Margin;
            m.Bottom = this.Config.Horizontal ? 0 : 6;
            this.BaseGrid.Margin = m;

            // スペルタイマコントロールのリストを生成する
            var displayList = new List<SpellTimerControl>();
            var timeupList = new List<SpellTimerControl>();
            foreach (var spell in spells)
            {
                SpellTimerControl c;
                if (this.SpellControls.ContainsKey(spell.ID))
                {
                    c = this.SpellControls[spell.ID];
                }
                else
                {
                    c = new SpellTimerControl()
                    {
                        Spell = spell,
                        Visibility = Visibility.Collapsed,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0, 0, 0, 0),
                        Progress = 1.0d,
                        RecastTime = 0d,
                    };

                    this.SpellControls.Add(spell.ID, c);
                    this.BaseGrid.Children.Add(c);

                    c.SetValue(Grid.ColumnProperty, 0);
                    c.SetValue(Grid.RowProperty, 0);
                }

                // Designモードならば必ず再描画する
                if (spell.IsDesignMode)
                {
                    if (spell.MatchDateTime == DateTime.MinValue)
                    {
                        c.Update();
                        c.StartBarAnimation();
                    }
                    else
                    {
                        if ((DateTime.Now - spell.CompleteScheduledTime).TotalSeconds > 1.0d)
                        {
                            spell.MatchDateTime = DateTime.MinValue;
                        }
                    }
                }

                // 一度もログにマッチしていない時はバーを初期化する
                if (spell.MatchDateTime == DateTime.MinValue &&
                    !spell.UpdateDone)
                {
                    c.Progress = 1.0d;
                    c.RecastTime = 0;
                    c.Update();
                    c.StartBarAnimation();

                    spell.UpdateDone = true;
                }
                else
                {
                    c.RecastTime = (spell.CompleteScheduledTime - DateTime.Now).TotalSeconds;
                    if (c.RecastTime < 0)
                    {
                        c.RecastTime = 0;
                    }

                    var totalRecastTime = (spell.CompleteScheduledTime - spell.MatchDateTime).TotalSeconds;
                    c.Progress = totalRecastTime != 0 ?
                        (totalRecastTime - c.RecastTime) / totalRecastTime :
                        1.0d;
                    if (c.Progress > 1.0d)
                    {
                        c.Progress = 1.0d;
                    }

                    if (!spell.UpdateDone)
                    {
                        c.Update();
                        c.StartBarAnimation();

                        spell.UpdateDone = true;
                    }
                }

                c.Refresh();
                displayList.Add(c);

                if ((Settings.Default.TimeOfHideSpell > 0.0d) &&
                    this.Config.FixedPositionSpell)
                {
                    if (!spell.IsDesignMode)
                    {
                        if (!spell.DontHide &&
                            (DateTime.Now - spell.CompleteScheduledTime).TotalSeconds > Settings.Default.TimeOfHideSpell)
                        {
                            timeupList.Add(c);
                        }
                    }
                }
            }

            // 今回表示しないスペルを隠す
            foreach (var c in this.SpellControls)
            {
                if (!spells.Any(x => x.ID == c.Key))
                {
                    c.Value.Visibility = Visibility.Collapsed;
                }
            }

            // 行・列の個数がスペル表示数より小さい場合に拡張する
            // また不要な行・列を削除する
            if (this.Config.Horizontal)
            {
                if (this.BaseGrid.RowDefinitions.Count > 1)
                {
                    this.BaseGrid.RowDefinitions.RemoveRange(1, this.BaseGrid.RowDefinitions.Count - 1);
                }

                for (int i = 0; i < (displayList.Count - this.BaseGrid.ColumnDefinitions.Count); i++)
                {
                    var column = new ColumnDefinition();
                    column.Width = GridLength.Auto;
                    this.BaseGrid.ColumnDefinitions.Add(column);
                }
            }
            else
            {
                if (this.BaseGrid.ColumnDefinitions.Count > 1)
                {
                    this.BaseGrid.ColumnDefinitions.RemoveRange(1, this.BaseGrid.ColumnDefinitions.Count - 1);
                }

                for (int i = 0; i < (displayList.Count - this.BaseGrid.RowDefinitions.Count); i++)
                {
                    var row = new RowDefinition();
                    row.Height = GridLength.Auto;
                    this.BaseGrid.RowDefinitions.Add(row);
                }
            }

            // スペルの表示順とマージンを設定する
            var index = 0;
            foreach (var displaySpell in displayList)
            {
                var margin = displaySpell.Margin;
                if (index != 0)
                {
                    margin.Left = this.Config.Horizontal ? this.Config.Margin : 0;
                    margin.Top = this.Config.Horizontal ? 0 : this.Config.Margin;
                }
                else
                {
                    margin.Left = 0;
                    margin.Top = 0;
                }

                displaySpell.Margin = margin;
                displaySpell.VerticalAlignment = this.Config.Horizontal ? VerticalAlignment.Bottom : VerticalAlignment.Top;

                displaySpell.SetValue(Grid.RowProperty, this.Config.Horizontal ? 0 : index);
                displaySpell.SetValue(Grid.ColumnProperty, this.Config.Horizontal ? index : 0);
                displaySpell.Visibility = Visibility.Visible;

                index++;
            }

            // タイムアップしたものは非表示とする
            foreach (var c in timeupList)
            {
                c.Visibility = Visibility.Hidden;
            }

            if (spells.Count() > 0)
            {
                this.ShowOverlay();
            }
            else
            {
                this.HideOverlay();
            }
        }

        #region フォーカスを奪わない対策

        private const int GWL_EXSTYLE = -20;

        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion フォーカスを奪わない対策

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
