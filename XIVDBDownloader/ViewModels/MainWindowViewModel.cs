using System.IO;
using System.Windows.Input;
using Prism.Mvvm;
using XIVDBDownloader.Constants;

namespace XIVDBDownloader.ViewModels
{
    public class MainWindowViewModel :
        BindableBase
    {
        #region View

        public MainWindow View { get; set; }

        #endregion View

        #region Properties

        private DataModels dataModel = DataModels.Action;
        private bool isEnabledDownload = true;
        private Language language = Language.JA;
        private string messages = string.Empty;
#if DEBUG
        private string saveDirectory = Path.GetFullPath(@".\resources\xivdb");
#else
        private string saveDirectory = Path.GetFullPath(@"..\..\resources\xivdb");
#endif

        public DataModels DataModel
        {
            get => this.dataModel;
            set => this.SetProperty(ref this.dataModel, value);
        }

        public bool IsEnabledDownload
        {
            get => this.isEnabledDownload;
            set => this.SetProperty(ref this.isEnabledDownload, value);
        }

        public Language Language
        {
            get => this.language;
            set => this.SetProperty(ref this.language, value);
        }

        public string Messages
        {
            get => this.messages;
            set
            {
                if (this.SetProperty(ref this.messages, value))
                {
                    this.View.MessagesScrollViewer.ScrollToEnd();
                }
            }
        }

        public string SaveDirectory
        {
            get => this.saveDirectory;
            set => this.SetProperty(ref this.saveDirectory, value);
        }

        #endregion Properties

        #region Commands

        private ICommand downloadCommand;

        public ICommand DownloadCommand =>
            this.downloadCommand ?? (this.downloadCommand = new DownloadCommand(this));

        #endregion Commands
    }
}
