using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ACT.SpecialSpellTimer.Config;
using ACT.SpecialSpellTimer.Models;
using FFXIV.Framework.WPF.Views;

namespace ACT.SpecialSpellTimer.Views
{
    public interface ISpellPanelWindow :
        IOverlay
    {
        bool IsClickthrough { get; set; }

        SpellPanel Panel { get; }

        IList<Spell> Spells { get; set; }

        void Refresh();
    }

    public static class SpellPanelWindowExtensions
    {
        public static Window ToWindow(this ISpellPanelWindow s) => s as Window;
    }

    /// <summary>
    /// AdvancedSpellPanelWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AdvancedSpellPanelWindow :
        Window,
        ISpellPanelWindow,
        INotifyPropertyChanged
    {
        public static ISpellPanelWindow GetWindow(
            SpellPanel panel)
        {
            if (panel.EnabledAdvancedLayout)
            {
                return new AdvancedSpellPanelWindow(panel);
            }
            else
            {
                return new SpellPanelWindow(panel);
            }
        }

        public AdvancedSpellPanelWindow() :
            this(SpellPanel.GeneralPanel)
        {
        }

        public AdvancedSpellPanelWindow(
            SpellPanel panel)
        {
            this.Panel = panel;
            this.Panel.PanelWindow = this;

            this.InitializeComponent();
            this.ToNonActive();

            this.MouseLeftButtonDown += (x, y) => this.DragMove();

            for (int r = 0; r < this.GuidRulerGrid.RowDefinitions.Count; r++)
            {
                for (int c = 0; c < this.GuidRulerGrid.ColumnDefinitions.Count; c++)
                {
                    var rect = new Rectangle()
                    {
                        Stroke = Brushes.LemonChiffon,
                        StrokeThickness = 0.2,
                    };

                    Grid.SetRow(rect, r);
                    Grid.SetColumn(rect, c);
                    this.GuidRulerGrid.Children.Insert(0, rect);
                }
            }

            this.Closed += (x, y) =>
            {
                this.ActiveSpells.Clear();

                if (this.Panel != null)
                {
                    this.Panel.PanelWindow = null;
                    this.Panel = null;
                }
            };

            this.ActiveSpells.CollectionChanged += this.ActiveSpells_CollectionChanged;

            this.Opacity = 0;
            this.Topmost = false;
        }

        private bool overlayVisible;

        public bool OverlayVisible
        {
            get => this.overlayVisible;
            set => this.SetOverlayVisible(ref this.overlayVisible, value, Settings.Default.OpacityToView);
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

        private SpellPanel panel;

        public SpellPanel Panel
        {
            get => this.panel;
            private set => this.SetProperty(ref this.panel, value);
        }

        private IList<Spell> spells;

        public IList<Spell> Spells
        {
            get => this.spells;
            set => this.SetProperty(ref this.spells, value);
        }

        public ObservableCollection<Spell> ActiveSpells
        {
            get;
            private set;
        } = new ObservableCollection<Spell>();

        private List<SpellControl> spellControls = new List<SpellControl>();

        public void Refresh()
        {
            // 表示するものがなければ何もしない
            if (this.Spells == null)
            {
                this.ActiveSpells.Clear();
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
            if (Settings.Default.TimeOfHideSpell > 0.0d)
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
                this.ActiveSpells.Clear();
                this.HideOverlay();
                return;
            }

            // 有効なスペルリストを入れ替える
            var toAdd = spells.Where(x => !this.ActiveSpells.Any(y => y.Guid == x.Guid));
            var toRemove = this.ActiveSpells.Where(x => !spells.Any(y => y.Guid == x.Guid)).ToList();

            this.ActiveSpells.AddRange(toAdd);
            foreach (var spell in toRemove)
            {
                this.ActiveSpells.Remove(spell);
            }

            // 表示を更新する
            this.RefreshRender();

            if (this.ActiveSpells.Any())
            {
                this.ShowOverlay();
            }
        }

        private void ActiveSpells_CollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Spell spell in e.NewItems)
                {
                    var control = new SpellControl(spell);

                    control.SetBinding(Canvas.LeftProperty, new Binding(nameof(Spell.Left)));
                    control.SetBinding(Canvas.TopProperty, new Binding(nameof(Spell.Top)));
                    control.SetBinding(Canvas.ZIndexProperty, new Binding(nameof(Spell.DisplayNo)));

                    spell.UpdateDone = false;

                    this.SpellsCanvas.Children.Add(control);
                    this.spellControls.Add(control);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Spell spell in e.OldItems)
                {
                    var control = this.spellControls.FirstOrDefault(x =>
                        (x.DataContext as Spell).Guid == spell.Guid);

                    if (control != null)
                    {
                        this.SpellsCanvas.Children.Remove(control);
                        this.spellControls.Remove(control);
                    }
                }
            }
        }

        public void RefreshRender()
        {
            foreach (var control in this.spellControls)
            {
                var spell = control.Spell;

                // Designモードならば必ず再描画する
                if (spell.IsDesignMode)
                {
                    if (spell.MatchDateTime == DateTime.MinValue)
                    {
                        control.Update();
                        control.StartBarAnimation();
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
                    control.Progress = 1.0d;
                    control.RecastTime = 0;
                    control.Update();
                    control.StartBarAnimation();

                    spell.UpdateDone = true;
                }
                else
                {
                    control.RecastTime = (spell.CompleteScheduledTime - DateTime.Now).TotalSeconds;
                    if (control.RecastTime < 0)
                    {
                        control.RecastTime = 0;
                    }

                    var totalRecastTime = (spell.CompleteScheduledTime - spell.MatchDateTime).TotalSeconds;
                    control.Progress = totalRecastTime != 0 ?
                        (totalRecastTime - control.RecastTime) / totalRecastTime :
                        1.0d;
                    if (control.Progress > 1.0d)
                    {
                        control.Progress = 1.0d;
                    }

                    if (!spell.UpdateDone)
                    {
                        control.Update();
                        control.StartBarAnimation();

                        spell.UpdateDone = true;
                    }
                }

                control.Refresh();
            }
        }

        #region Drag & Drop

        private bool isDrag;
        private Point dragOffset;

        private void DragOnMouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            var el = sender as UIElement;
            if (el != null)
            {
                this.isDrag = true;
                this.dragOffset = e.GetPosition(el);
                el.CaptureMouse();
            }
        }

        private void DragOnMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (this.isDrag)
            {
                var el = sender as UIElement;
                el.ReleaseMouseCapture();
                this.isDrag = false;
            }
        }

        private void DragOnMouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (this.isDrag)
            {
                var pt = Mouse.GetPosition(this.SpellsCanvas);
                var el = sender as UIElement;
                Canvas.SetLeft(el, pt.X - this.dragOffset.X);
                Canvas.SetTop(el, pt.Y - this.dragOffset.Y);
            }
        }

        #endregion Drag & Drop

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
