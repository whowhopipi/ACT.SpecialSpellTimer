using System.Windows;
using System.Windows.Controls;
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

        private void ShowContent(
            object model)
        {
            var content = default(UIElement);

            switch (model)
            {
                case SpellPanel panel:
                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.DarkViolet);
                    this.ContentGrid.Visibility = Visibility.Visible;

                    content = new SpellPanelConfigView()
                    {
                        DataContext = new SpellPanelConfigViewModel(model as SpellPanel)
                    };

                    break;

                case Spell spell:
                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.MediumBlue);
                    this.ContentGrid.Visibility = Visibility.Visible;

                    content = new SpellConfigView()
                    {
                        DataContext = new SpellConfigViewModel(model as Spell)
                    };

                    break;

                case Ticker ticker:
                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.OliveDrab);
                    this.ContentGrid.Visibility = Visibility.Visible;

                    content = new SpellPanelConfigView()
                    {
                        DataContext = new TickerConfigViewModel(model as Ticker)
                    };

                    break;

                default:
                    this.ContentBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    this.ContentGrid.Visibility = Visibility.Hidden;
                    break;
            }

            if (content != null)
            {
                this.ContentGrid.Children.Clear();
                this.ContentGrid.Children.Add(content);
            }
        }
    }
}
