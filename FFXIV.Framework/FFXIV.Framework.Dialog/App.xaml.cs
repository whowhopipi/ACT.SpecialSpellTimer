using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using FFXIV.Framework.Dialog.Views;
using FFXIV.Framework.Extensions;
using FirstFloor.ModernUI.Windows.Controls;

namespace FFXIV.Framework.Dialog
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App :
        Application
    {
        public App()
        {
            Application.Current.ShutdownMode =
                ShutdownMode.OnLastWindowClose;

            this.Startup += this.App_Startup;
            this.Exit += this.App_Exit;
            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            var m = string.Empty;
            m += "予期しない例外が発生しました。アプリケーションを終了します。" + Environment.NewLine;
            m += Environment.NewLine;
            m += e.Exception.ToString();

            ModernDialog.ShowMessage(
                m,
                "Unhandled Exception",
                MessageBoxButton.OK);

            try
            {
                Application.Current.Shutdown();
            }
            catch
            {
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Trace.WriteLine("App_Exit begin.");
            Trace.WriteLine("App_Exit end.");
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Trace.WriteLine("App_Startup begin.");

                var arguments = e.Args.Aggregate((x, y) => x + y);

                // カラーダイアログを表示する
                if (arguments.Contains(ColorDialogResult.SymbolKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    App.ShowColorDialog(arguments);
                    return;
                }

                // フォントダイアログを表示する
                if (arguments.Contains(FontDialogResult.SymbolKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    App.ShowFontDialog(arguments);
                    return;
                }

                // デバッグモード
                var window = new MainWindow();
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Show();
            }
            finally
            {
                Trace.WriteLine("App_Startup end.");
            }
        }

        public static bool ShowColorDialog(
            string arguments)
        {
            var result = new ColorDialogResult();

            try
            {
                if (!string.IsNullOrEmpty(arguments))
                {
                    result = ColorDialogResult.FromString(arguments);
                    ColorDialog.Color = result.Color;
                    ColorDialog.IgnoreAlpha = result.IgnoreAlpha;
                }

                result.Result = ColorDialog.ShowDialog() ?? false;
                result.Color = ColorDialog.Color;

                Console.WriteLine(result.ToString());
            }
            catch (Exception ex)
            {
                var m = string.Empty;
                m += "予期しない例外が発生しました。Dialogを終了します。" + Environment.NewLine;
                m += Environment.NewLine;
                m += $"Arguments: {arguments}";
                m += ex.ToString();

                ModernDialog.ShowMessage(
                    m,
                    ex.Message,
                    MessageBoxButton.OK);
            }

            return result.Result;
        }

        public static bool ShowFontDialog(
            string arguments)
        {
            var result = new FontDialogResult();

            try
            {
                if (!string.IsNullOrEmpty(arguments))
                {
                    result = FontDialogResult.FromString(arguments);
                    FontDialog.Font = result.Font;
                }

                result.Result = FontDialog.ShowDialog() ?? false;
                result.Font = FontDialog.Font;

                Console.WriteLine(result.ToString());
            }
            catch (Exception ex)
            {
                var m = string.Empty;
                m += "予期しない例外が発生しました。Dialogを終了します。" + Environment.NewLine;
                m += Environment.NewLine;
                m += $"Arguments: {arguments}";
                m += ex.ToString();

                ModernDialog.ShowMessage(
                    m,
                    ex.Message,
                    MessageBoxButton.OK);
            }

            return result.Result;
        }
    }
}
