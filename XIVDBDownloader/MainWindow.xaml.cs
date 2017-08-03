using MahApps.Metro.Controls;

using XIVDBDownloader.ViewModels;

namespace XIVDBDownloader
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow :
        MetroWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.ViewModel.View = this;
        }

        /// <summary>ViewModel</summary>
        public MainWindowViewModel ViewModel => (MainWindowViewModel)this.DataContext;
    }
}
