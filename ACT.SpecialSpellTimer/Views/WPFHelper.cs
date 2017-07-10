using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ACT.SpecialSpellTimer.Views
{
    internal class WPFHelper
    {
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