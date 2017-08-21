using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace FFXIV.Framework.Common
{
    public class WPFHelper
    {
        public static DispatcherOperation BeginInvoke(
            Action action,
            DispatcherPriority priority = DispatcherPriority.Background)
        {
            return Application.Current?.Dispatcher.BeginInvoke(
                action,
                priority);
        }

        public static void Invoke(
            Action action,
            DispatcherPriority priority = DispatcherPriority.Background)
        {
            Application.Current?.Dispatcher.Invoke(
                action,
                priority);
        }

        private static int _IsDebugMode = -1;

        /// <summary>
        /// 現在のプロセスがデザインモードかどうか返します。
        /// </summary>
        public static bool IsDesignMode
        {
            get
            {
                if (_IsDebugMode == -1)
                {
                    if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    {
                        _IsDebugMode = 1;
                    }
                    else
                    {
                        using (var p = Process.GetCurrentProcess())
                        {
                            _IsDebugMode = (
                                p.ProcessName.Equals("DEVENV", StringComparison.OrdinalIgnoreCase) ||
                                p.ProcessName.Equals("XDesProc", StringComparison.OrdinalIgnoreCase)
                            ) ? 1 : 0;
                        }
                    }
                }

                return _IsDebugMode == 1;
            }
        }
    }
}
