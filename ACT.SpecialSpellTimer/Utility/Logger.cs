namespace ACT.SpecialSpellTimer.Utility
{
    using System;
    using System.IO;

    /// <summary>
    /// ログ
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="text">書き込む内容</param>
        public static void Write(
            string text)
        {
            try
            {
                var textToWrite =
                    "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " +
                    text;

                if (SpecialSpellTimerPlugin.ConfigPanel != null)
                {
                    SpecialSpellTimerPlugin.ConfigPanel.AppendLog(textToWrite);
                }

                var logFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"anoyetta\ACT\ACT.SpecialSpellTimer.log");

                File.AppendText(textToWrite + Environment.NewLine);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// ログを書き込む
        /// </summary>
        /// <param name="text">書き込む内容</param>
        /// <param name="ex">例外情報</param>
        public static void Write(
            string text,
            Exception ex)
        {
            Write(text + Environment.NewLine + ex.ToString());
        }
    }
}
