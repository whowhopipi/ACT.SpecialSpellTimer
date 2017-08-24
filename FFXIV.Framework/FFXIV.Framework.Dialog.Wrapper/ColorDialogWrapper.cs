using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using FFXIV.Framework.Extensions;

namespace FFXIV.Framework.Dialog
{
    public class ColorDialogWrapper
    {
        public static string BaseDirectory { get; set; } = string.Empty;

        private static string WPFDialogProccess => Path.Combine(
            string.IsNullOrEmpty(BaseDirectory) ?
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :
            BaseDirectory,
            @"FFXIV.Framework.Dialog.exe");

        public static ColorDialogResult ShowDialog(
            System.Drawing.Color? color = null,
            bool ignoreAlpha = false)
        {
            var wpfColor = color?.ToWPF();
            return ColorDialogWrapper.ShowDialog(wpfColor, ignoreAlpha);
        }

        public static ColorDialogResult ShowDialog(
            Color? color = null,
            bool ignoreAlpha = false)
        {
            var pi = new ProcessStartInfo(WPFDialogProccess);
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            var result = new ColorDialogResult()
            {
                Color = color.HasValue ? color.Value : Colors.Transparent,
                IgnoreAlpha = ignoreAlpha,
            };

            pi.Arguments = result.ToString().EscapeDoubleQuotes();
            Debug.WriteLine(pi.Arguments);

            var p = new Process()
            {
                StartInfo = pi,
            };

            var stdout = string.Empty;

            p.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    stdout = e.Data;
                }
            };

            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();

                result = ColorDialogResult.FromString(stdout);
            }
            finally
            {
                p.Dispose();
            }

            return result;
        }

        public static Task<ColorDialogResult> ShowDialogAsync(
            System.Drawing.Color? color = null,
            bool ignoreAlpha = false)
        {
            return Task.Run(() =>
            {
                return ColorDialogWrapper.ShowDialog(color, ignoreAlpha);
            });
        }

        public static Task<ColorDialogResult> ShowDialogAsync(
            Color? color = null,
            bool ignoreAlpha = false)
        {
            return Task.Run(() =>
            {
                return ColorDialogWrapper.ShowDialog(color, ignoreAlpha);
            });
        }
    }
}
