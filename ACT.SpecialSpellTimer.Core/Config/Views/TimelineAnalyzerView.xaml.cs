using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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
        }

        public Settings RootConfig => Settings.Default;

        public ObservableCollection<CombatLog> CombatLogs => CombatAnalyzer.Instance.CurrentCombatLogList;

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
