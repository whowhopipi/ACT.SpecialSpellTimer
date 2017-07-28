﻿namespace ACT.SpecialSpellTimer.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;

    using ACT.SpecialSpellTimer.Config;
    using ACT.SpecialSpellTimer.Models;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// SpellTimerList Window
    /// </summary>
    public partial class SpellTimerListWindow : Window
    {
        /// <summary>
        /// ドラッグ終了
        /// </summary>
        private Action<MouseEventArgs> DragOff;

        /// <summary>
        /// ドラッグ開始
        /// </summary>
        private Action<MouseEventArgs> DragOn;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpellTimerListWindow()
        {
#if DEBUG
            Debug.WriteLine("SpellList");
#endif
            this.InitializeComponent();

            this.SpellTimerControls = new Dictionary<long, SpellTimerControl>();

            this.Loaded += this.SpellTimerListWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();
            this.Closed += (s1, e1) =>
            {
                if (this.SpellTimerControls != null)
                {
                    this.SpellTimerControls.Clear();
                }
            };

            this.DragOn = new Action<MouseEventArgs>((mouse) =>
            {
                if (mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.IsDragging = true;
#if DEBUG
                    Debug.WriteLine("Drag On");
#endif
                }
            });

            this.DragOff = new Action<MouseEventArgs>((mouse) =>
            {
                if (mouse.LeftButton == MouseButtonState.Released)
                {
                    this.IsDragging = false;
#if DEBUG
                    Debug.WriteLine("Drag Off");
#endif
                }
            });

            this.MouseDown += (s1, e1) => this.DragOn(e1);
            this.MouseUp += (s1, e1) => this.DragOff(e1);
        }

        /// <summary>
        /// ドラッグ中か？
        /// </summary>
        public bool IsDragging { get; private set; }

        /// <summary>
        /// 水平レイアウトか？
        /// </summary>
        public bool IsHorizontal { get; set; }

        /// <summary>
        /// このPanelの名前
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// 扱うSpellTimer間のマージン
        /// </summary>
        public int SpellMargin { get; set; }

        /// <summary>
        /// SpellTimerを固定位置に表示するか？
        /// </summary>
        public bool SpellPositionFixed { get; set; }

        /// <summary>
        /// 扱っているスペルタイマコントロールのリスト
        /// </summary>
        public Dictionary<long, SpellTimerControl> SpellTimerControls { get; private set; }

        /// <summary>
        /// 扱うSpellTimerのリスト
        /// </summary>
        public SpellTimer[] SpellTimers { get; set; }

        /// <summary>背景色のBrush</summary>
        private SolidColorBrush BackgroundBrush { get; set; }

        /// <summary>
        /// SpellTimerの描画をRefreshする
        /// </summary>
        public void RefreshSpellTimer()
        {
            if (this.IsDragging)
            {
                return;
            }

            // 表示するものがなければ何もしない
            if (this.SpellTimers == null)
            {
                this.HideOverlay();
                return;
            }

            // 表示対象だけに絞る
            var spells =
                from x in this.SpellTimers
                where
                x.ProgressBarVisible
                select
                x;

            // タイムアップしたものを除外する
            if ((Settings.Default.TimeOfHideSpell > 0.0d) &&
                !this.SpellPositionFixed)
            {
                spells =
                    from x in spells
                    where
                    x.DontHide ||
                    (DateTime.Now - x.CompleteScheduledTime).TotalSeconds <= Settings.Default.TimeOfHideSpell
                    select
                    x;
            }

            if (!spells.Any())
            {
                this.HideOverlay();
                return;
            }

            // リキャストの近いもの順でソートする
            if (Settings.Default.AutoSortEnabled && !this.SpellPositionFixed)
            {
                // 昇順？
                if (!Settings.Default.AutoSortReverse)
                {
                    spells =
                        from x in spells
                        orderby
                        x.CompleteScheduledTime,
                        x.DisplayNo
                        select
                        x;
                }
                else
                {
                    spells =
                        from x in spells
                        orderby
                        x.CompleteScheduledTime descending,
                        x.DisplayNo
                        select
                        x;
                }
            }
            else
            {
                // 固定の表示順でソートする
                spells =
                    from x in spells
                    orderby
                    x.DisplayNo
                    select
                    x;
            }

            // Brushを生成する
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

            // 背景色を設定する
            var nowbackground = this.BaseColorRectangle.Fill as SolidColorBrush;
            if (nowbackground == null ||
                nowbackground.Color != this.BackgroundBrush.Color)
            {
                if (this.BackgroundBrush != null)
                {
                    this.BaseColorRectangle.Fill = this.BackgroundBrush;
                }
            }

            // 水平レイアウト時のマージンを調整する
            var m = this.BaseGrid.Margin;
            m.Bottom = this.IsHorizontal ? 0 : 6;
            this.BaseGrid.Margin = m;

            // スペルタイマコントロールのリストを生成する
            var displayList = new List<SpellTimerControl>();
            var timeupList = new List<SpellTimerControl>();
            foreach (var spell in spells)
            {
                SpellTimerControl c;
                if (this.SpellTimerControls.ContainsKey(spell.ID))
                {
                    c = this.SpellTimerControls[spell.ID];
                }
                else
                {
                    c = new SpellTimerControl();
                    this.SpellTimerControls.Add(spell.ID, c);

                    c.Visibility = Visibility.Collapsed;
                    c.MouseDown += (s, e) => this.DragOn(e);
                    c.MouseUp += (s, e) => this.DragOff(e);

                    c.HorizontalAlignment = HorizontalAlignment.Left;
                    c.VerticalAlignment = VerticalAlignment.Top;
                    c.Margin = new Thickness(0, 0, 0, 0);

                    this.BaseGrid.Children.Add(c);

                    c.SetValue(Grid.ColumnProperty, 0);
                    c.SetValue(Grid.RowProperty, 0);
                }

                c.SpellTitle = string.IsNullOrWhiteSpace(spell.SpellTitleReplaced) ?
                    spell.SpellTitle :
                    spell.SpellTitleReplaced;
                c.SpellIcon = spell.SpellIcon;
                c.SpellIconSize = spell.SpellIconSize;
                c.IsReverse = spell.IsReverse;
                c.HideSpellName = spell.HideSpellName;
                c.WarningTime = spell.WarningTime;
                c.ChangeFontColorsWhenWarning = spell.ChangeFontColorsWhenWarning;
                c.OverlapRecastTime = spell.OverlapRecastTime;
                c.ReduceIconBrightness = spell.ReduceIconBrightness;
                c.RecastTime = 0;
                c.Progress = 1.0d;

                c.BarWidth = spell.BarWidth;
                c.BarHeight = spell.BarHeight;
                c.FontInfo = spell.Font;
                c.FontColor = spell.FontColor;
                c.FontOutlineColor = spell.FontOutlineColor;
                c.WarningFontColor = spell.WarningFontColor;
                c.WarningFontOutlineColor = spell.WarningFontOutlineColor;
                c.BarColor = spell.BarColor;
                c.BarOutlineColor = spell.BarOutlineColor;

                // 一度もログにマッチしていない時はバーを初期化する
                if (spell.MatchDateTime == DateTime.MinValue &&
                    !spell.UpdateDone)
                {
                    c.Progress = 1.0;
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
                    this.SpellPositionFixed)
                {
                    if (!spell.DontHide &&
                        (DateTime.Now - spell.CompleteScheduledTime).TotalSeconds > Settings.Default.TimeOfHideSpell)
                    {
                        timeupList.Add(c);
                    }
                }
            }

            // 今回表示しないスペルを隠す
            foreach (var c in this.SpellTimerControls)
            {
                if (!spells.Any(x => x.ID == c.Key))
                {
                    c.Value.Visibility = Visibility.Collapsed;
                }
            }

            // 行・列の個数がスペル表示数より小さい場合に拡張する
            // また不要な行・列を削除する
            if (this.IsHorizontal)
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
                    margin.Left = this.IsHorizontal ? this.SpellMargin : 0;
                    margin.Top = this.IsHorizontal ? 0 : this.SpellMargin;
                }
                else
                {
                    margin.Left = 0;
                    margin.Top = 0;
                }

                displaySpell.Margin = margin;
                displaySpell.VerticalAlignment = this.IsHorizontal ? VerticalAlignment.Bottom : VerticalAlignment.Top;

                displaySpell.SetValue(Grid.RowProperty, this.IsHorizontal ? 0 : index);
                displaySpell.SetValue(Grid.ColumnProperty, this.IsHorizontal ? index : 0);
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
        }

        /// <summary>
        /// Loaded
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SpellTimerListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Panelの位置を復元する
            var setting = PanelSettings.Default.SettingsTable
                .Where(x => x.PanelName == this.PanelName)
                .FirstOrDefault();

            if (setting != null)
            {
                this.Left = setting.Left;
                this.Top = setting.Top;
                this.SpellMargin = setting.Margin;
                this.IsHorizontal = setting.Horizontal;
                this.SpellPositionFixed = setting.FixedPositionSpell;
            }

            this.RefreshSpellTimer();
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
    }
}