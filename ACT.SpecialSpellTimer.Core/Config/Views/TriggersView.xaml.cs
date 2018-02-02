using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ACT.SpecialSpellTimer.Config.Models;
using ACT.SpecialSpellTimer.Config.ViewModels;
using ACT.SpecialSpellTimer.Models;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TriggersView.xaml の相互作用ロジック
    /// </summary>
    public partial class TriggersView : UserControl, ILocalizable
    {
        public TriggersView()
        {
            this.InitializeComponent();
            this.DataContext = new TriggersViewModel();
            this.SetLocale(Settings.Default.UILocale);

            this.TriggersTreeView.SelectedItemChanged += this.TriggersTreeViewOnSelectedItemChanged;

            this.TriggersTreeView.PreviewKeyUp += this.TriggersTreeViewOnPreviewKeyUp;
        }

        private void TriggersTreeViewOnPreviewKeyUp(
            object sender,
            KeyEventArgs e)
        {
            var item = (sender as TreeView)?.SelectedItem as TreeItemBase;
            if (item == null)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.F2:
                    item.RenameCommand.Execute(item);
                    break;

                case Key.Delete:
                    item.DeleteCommand.Execute(item);
                    break;
            }
        }

        public TriggersViewModel ViewModel => this.DataContext as TriggersViewModel;

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        private void TriggersTreeViewOnSelectedItemChanged(
            object sender,
            RoutedPropertyChangedEventArgs<object> e)
            => this.ShowContent(e.NewValue);

        private SpellPanelConfigViewModel spellPanelViewModel;
        private SpellConfigViewModel spellViewModel;
        private TickerConfigViewModel tickerViewModel;

        private object previousModel;

        private void ShowContent(
            object model)
        {
            if (this.previousModel != null)
            {
                if (this.previousModel is Spell s)
                {
                    s.IsRealtimeCompile = false;
                }

                if (this.previousModel is Ticker t)
                {
                    t.IsRealtimeCompile = false;
                }
            }

            switch (model)
            {
                case SpellPanel panel:
                    if (this.spellPanelViewModel == null)
                    {
                        this.spellPanelViewModel = new SpellPanelConfigViewModel(panel);
                        this.SpellPanelView.DataContext = this.spellPanelViewModel;
                    }
                    else
                    {
                        this.spellPanelViewModel.Model = panel;
                    }

                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.DarkViolet);
                    this.SpellPanelView.Visibility = Visibility.Visible;
                    this.SpellView.Visibility = Visibility.Collapsed;
                    this.TickerView.Visibility = Visibility.Collapsed;
                    break;

                case Spell spell:
                    spell.IsRealtimeCompile = true;
                    if (this.spellViewModel == null)
                    {
                        this.spellViewModel = new SpellConfigViewModel(spell);
                        this.SpellView.DataContext = this.spellViewModel;
                    }
                    else
                    {
                        this.spellViewModel.Model = spell;
                    }

                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.MediumBlue);
                    this.SpellPanelView.Visibility = Visibility.Collapsed;
                    this.SpellView.Visibility = Visibility.Visible;
                    this.TickerView.Visibility = Visibility.Collapsed;
                    break;

                case Ticker ticker:
                    ticker.IsRealtimeCompile = true;
                    if (this.tickerViewModel == null)
                    {
                        this.tickerViewModel = new TickerConfigViewModel(ticker);
                        this.TickerView.DataContext = this.tickerViewModel;
                    }
                    else
                    {
                        this.tickerViewModel.Model = ticker;
                    }

                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.OliveDrab);
                    this.SpellPanelView.Visibility = Visibility.Collapsed;
                    this.SpellView.Visibility = Visibility.Collapsed;
                    this.TickerView.Visibility = Visibility.Visible;
                    break;

                default:
                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    this.SpellPanelView.Visibility = Visibility.Collapsed;
                    this.SpellView.Visibility = Visibility.Collapsed;
                    this.TickerView.Visibility = Visibility.Collapsed;
                    break;
            }

            this.previousModel = model;
        }

        private void RenameTextBoxOnLostFocus(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is TextBox t)
            {
                if (t.Tag is Tag tag)
                {
                    tag.IsInEditMode = false;
                }
            }
        }

        private void RenameTextBoxOnKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Escape ||
                e.Key == Key.Enter)
            {
                if (sender is TextBox t)
                {
                    if (t.Tag is Tag tag)
                    {
                        tag.IsInEditMode = false;
                    }
                }
            }
        }

        private void RenameTextBoxOnIsVisibleChanged(
            object sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox t)
            {
                t.SelectAll();
                t.Focus();
            }
        }
    }
}
