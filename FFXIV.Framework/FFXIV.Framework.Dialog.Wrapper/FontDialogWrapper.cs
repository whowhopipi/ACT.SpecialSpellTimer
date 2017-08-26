using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;

namespace FFXIV.Framework.Dialog
{
    public class FontDialogWrapper
    {
        public static string BaseDirectory { get; set; } = string.Empty;

        private static string WPFDialogProccess => Path.Combine(
            string.IsNullOrEmpty(BaseDirectory) ?
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :
            BaseDirectory,
            @"FFXIV.Framework.Dialog.exe");

        public static FontDialogResult ShowDialog(
            FontInfo font = null)
        {
            var pi = new ProcessStartInfo(WPFDialogProccess);
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            var result = new FontDialogResult()
            {
                Font = font,
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

                result = FontDialogResult.FromString(stdout);
            }
            finally
            {
                p.Dispose();
            }

            return result;
        }

        public static Task<FontDialogResult> ShowDialogAsync(
            FontInfo font = null)
        {
            return Task.Run(() =>
            {
                return FontDialogWrapper.ShowDialog(font);
            });
        }
    }
}
