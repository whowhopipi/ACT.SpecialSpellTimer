using System.Windows;
using System.Windows.Media;
using FFXIV.Framework.Common;
using FFXIV.Framework.Dialog.Views;

namespace FFXIV.Framework.Dialog
{
    /// <summary>
    /// DebugWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow :
        Window
    {
        private Color color;
        private FontInfo font;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void OpenColorDialogButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            var result = App.ShowColorDialog(
                this.color != null ?
                $"{this.color.ToString()}" :
                null);
            if (result)
            {
                this.color = ColorDialog.Color;
            }
        }

        private void OpenFontDialogButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            var result = App.ShowFontDialog(
                this.font != null ?
                $"{this.font.ToString()}" :
                null);
            if (result)
            {
                this.font = FontDialog.Font;
            }
        }
    }
}
