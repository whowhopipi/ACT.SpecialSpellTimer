using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Dialog;
using FFXIV.Framework.Globalization;
using Prism.Commands;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// OptionsVisualView.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionsVisualView :
        UserControl,
        ILocalizable
    {
        public OptionsVisualView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        public Settings Config => Settings.Default;

        private ICommand CreateChangeColorCommand(
            Func<Color> getCurrentColor,
            Action<Color> changeColorAction)
            => new DelegateCommand(() =>
            {
                var result = ColorDialogWrapper.ShowDialog(getCurrentColor(), true);
                if (result.Result)
                {
                    changeColorAction.Invoke(result.Color);
                }
            });

        private ICommand changeProgressBarBackgroundColorCommand;

        public ICommand ChangeProgressBarBackgroundColorCommand =>
            this.changeProgressBarBackgroundColorCommand ?? (this.changeProgressBarBackgroundColorCommand = this.CreateChangeColorCommand(
                () => this.Config.BarDefaultBackgroundColor,
                (color) => this.Config.BarDefaultBackgroundColor = color));
    }
}
