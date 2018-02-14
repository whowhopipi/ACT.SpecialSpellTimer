using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ACT.SpecialSpellTimer.RaidTimeline;
using ACT.SpecialSpellTimer.resources;
using Advanced_Combat_Tracker;
using FFXIV.Framework.Globalization;
using Prism.Commands;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// TimelineAnalyzerView.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineAnalyzerView :
        UserControl,
        ILocalizable,
        INotifyPropertyChanged
    {
        public TimelineAnalyzerView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);

            this.CombatLogs.CollectionChanged += (x, y) =>
            {
                this.RaisePropertyChanged(nameof(this.Zone));
            };
        }

        private CollectionViewSource combatLogSource = new CollectionViewSource()
        {
            Source = CombatAnalyzer.Instance.CurrentCombatLogList,
            IsLiveFilteringRequested = true,
            IsLiveSortingRequested = true,
            IsLiveGroupingRequested = true,
        };

        public Settings RootConfig => Settings.Default;

        public ICollectionView CombatLogs => this.combatLogSource.View;

        public string Zone => this.CombatLogs.Cast<CombatLog>().FirstOrDefault()?.Zone;

        private void AutoCombatLogAnalyze_Checked(
            object sender,
            RoutedEventArgs e)
        {
            CombatAnalyzer.Instance.Start();
        }

        private void AutoCombatLogAnalyzex_Unchecked(
            object sender,
            RoutedEventArgs e)
        {
            CombatAnalyzer.Instance.End();
        }

        private System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog()
        {
            Description = "ログの保存先を選択してください。",
            ShowNewFolderButton = true,
        };

        private ICommand browseLogDirectoryCommand;

        public ICommand BrowseLogDirectoryCommand =>
            this.browseLogDirectoryCommand ?? (this.browseLogDirectoryCommand = new DelegateCommand(() =>
            {
                this.dialog.SelectedPath = this.RootConfig.CombatLogSaveDirectory;
                if (this.dialog.ShowDialog(ActGlobals.oFormActMain) ==
                    System.Windows.Forms.DialogResult.OK)
                {
                    this.RootConfig.CombatLogSaveDirectory = this.dialog.SelectedPath;
                }
            }));

        private ICommand openLogCommand;

        public ICommand OpenLogCommand =>
            this.openLogCommand ?? (this.openLogCommand = new DelegateCommand(() =>
            {
                var dir = this.RootConfig.CombatLogSaveDirectory;
                if (Directory.Exists(dir))
                {
                    Process.Start(dir);
                }
            }));

        private System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog()
        {
            RestoreDirectory = true,
            Filter = "Spreadsheet Files|*.xlsx|All Files|*.*",
            FilterIndex = 0,
            DefaultExt = ".xlsx",
            SupportMultiDottedExtensions = true,
        };

        private ICommand saveToSpreadsheetCommand;

        public ICommand SaveToSpreadsheetCommand =>
            this.saveToSpreadsheetCommand ?? (this.saveToSpreadsheetCommand = new DelegateCommand(async () =>
            {
                var logs = this.CombatLogs.Cast<CombatLog>()?.ToList();
                if (logs == null ||
                    !logs.Any())
                {
                    return;
                }

                this.saveFileDialog.FileName =
                    $"{DateTime.Now.ToString("yyyy-MM-dd")}.{logs.First().Zone}.CombatLog.xlsx";

                if (this.saveFileDialog.ShowDialog(ActGlobals.oFormActMain)
                    == System.Windows.Forms.DialogResult.OK)
                {
                    var file = this.saveFileDialog.FileName;
                    this.saveFileDialog.FileName = Path.GetFileName(file);

                    try
                    {
                        await Task.Run(() => CombatAnalyzer.Instance.SaveToSpreadsheet(file, logs));

                        ModernMessageBox.ShowDialog(
                            $"CombatLog Saved.\n\n\"{Path.GetFileName(file)}\"",
                            "Timeline Analyzer");
                    }
                    catch (Exception ex)
                    {
                        ModernMessageBox.ShowDialog(
                            $"Save CombatLog Error.",
                            "Timeline Analyzer",
                            MessageBoxButton.OK,
                            ex);
                    }
                }
            }));

        #region TextBox Utility

        private void TextBoxSelect(
            object sender,
            MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void TextBoxOnGotFocus(
            object sender,
            RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        #endregion TextBox Utility

        #region ILocalizebale

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        #endregion ILocalizebale

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
