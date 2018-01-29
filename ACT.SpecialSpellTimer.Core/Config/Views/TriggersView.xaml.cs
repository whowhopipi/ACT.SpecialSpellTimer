using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private void ShowContent(
            object model)
        {
            switch (model)
            {
                case SpellPanel panel:
                    if (this.spellPanelViewModel == null)
                    {
                        this.spellPanelViewModel = new SpellPanelConfigViewModel(model as SpellPanel);
                        this.SpellPanelView.DataContext = this.spellPanelViewModel;
                    }
                    else
                    {
                        this.spellPanelViewModel.Model = model as SpellPanel;
                    }

                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.DarkViolet);
                    this.SpellPanelView.Visibility = Visibility.Visible;
                    this.SpellView.Visibility = Visibility.Collapsed;
                    this.TickerView.Visibility = Visibility.Collapsed;
                    this.TagView.Visibility = Visibility.Collapsed;
                    break;

                case Spell spell:
                    if (this.spellViewModel == null)
                    {
                        this.spellViewModel = new SpellConfigViewModel(model as Spell);
                        this.SpellView.DataContext = this.spellViewModel;
                    }
                    else
                    {
                        this.spellViewModel.Model = model as Spell;
                    }

                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.MediumBlue);
                    this.SpellPanelView.Visibility = Visibility.Collapsed;
                    this.SpellView.Visibility = Visibility.Visible;
                    this.TickerView.Visibility = Visibility.Collapsed;
                    this.TagView.Visibility = Visibility.Collapsed;
                    break;

                case Ticker ticker:
                    if (this.tickerViewModel == null)
                    {
                        this.tickerViewModel = new TickerConfigViewModel(model as Ticker);
                        this.TickerView.DataContext = this.tickerViewModel;
                    }
                    else
                    {
                        this.tickerViewModel.Model = model as Ticker;
                    }

                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.OliveDrab);
                    this.SpellPanelView.Visibility = Visibility.Collapsed;
                    this.SpellView.Visibility = Visibility.Collapsed;
                    this.TickerView.Visibility = Visibility.Visible;
                    this.TagView.Visibility = Visibility.Collapsed;
                    break;

                default:
                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    this.SpellPanelView.Visibility = Visibility.Collapsed;
                    this.SpellView.Visibility = Visibility.Collapsed;
                    this.TickerView.Visibility = Visibility.Collapsed;
                    this.TagView.Visibility = Visibility.Collapsed;
                    break;
            }
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
