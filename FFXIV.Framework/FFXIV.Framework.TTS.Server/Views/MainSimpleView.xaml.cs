using System;
using System.Threading.Tasks;
using System.Windows;
using FFXIV.Framework.Common;
using FFXIV.Framework.TTS.Server.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;

namespace FFXIV.Framework.TTS.Server.Views
{
    /// <summary>
    /// MainView.xaml の相互作用ロジック
    /// </summary>
    public partial class MainView :
        MetroWindow
    {
        #region Singleton

        private static MainView instance = new MainView();

        public static MainView Instance => instance;

        #endregion Singleton

        #region Logger

        private Logger logger = AppLog.DefaultLogger;

        #endregion Logger

        public MainView()
        {
            this.InitializeComponent();
            this.ViewModel.View = this;
            this.StateChanged += this.MainView_StateChanged;
        }

        public MainSimpleViewModel ViewModel => (MainSimpleViewModel)this.DataContext;

        public Task<MessageDialogResult> ShowMessageDialogAync(
            string title,
            string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return this.ShowMessageAsync(
                title,
                message,
                style);
        }

        #region Window state

        private WindowState previousWindowState;

        public void RestoreWindowState()
        {
            this.WindowState = this.previousWindowState;
        }

        private void MainView_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:
                case WindowState.Maximized:
                    this.ShowInTaskbar = true;
                    this.previousWindowState = this.WindowState;
                    break;

                case WindowState.Minimized:
                    this.ShowInTaskbar = false;
                    break;
            }
        }

        #endregion Window state
    }
}
