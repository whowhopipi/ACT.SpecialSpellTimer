using System;
using ACT.SpecialSpellTimer.Forms;

namespace ACT.SpecialSpellTimer.Utility
{
    /// <summary>
    /// ログ
    /// </summary>
    public static class Logger
    {
        private static NLog.Logger AppLogger => FFXIV.Framework.Common.AppLog.DefaultLogger;

        public static void Init()
        {
            FFXIV.Framework.Common.AppLog.AppendedLog -= OnAppendedLog;
            FFXIV.Framework.Common.AppLog.AppendedLog += OnAppendedLog;
        }

        public static void DeInit()
        {
            FFXIV.Framework.Common.AppLog.AppendedLog -= OnAppendedLog;
        }

        private static void OnAppendedLog(
            object sender,
            FFXIV.Framework.Common.AppendedLogEventArgs e)
        {
            ConfigPanelLog.Instance?.AppendLog(e.AppendedLogEntry.ToString());
        }

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="text">書き込む内容</param>
        public static void Write(string text)
        {
            AppLogger.Info(text);
        }

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="text">書き込む内容</param>
        /// <param name="ex">例外情報</param>
        public static void Write(string text, Exception ex)
        {
            AppLogger.Error(ex, text);
        }
    }
}
