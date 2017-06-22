namespace ACT.SpecialSpellTimer
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Windowの拡張メソッド
    /// </summary>
    public static partial class WindowExtension
    {
        /// <summary>
        /// Brush辞書
        /// </summary>
        private static Dictionary<string, SolidColorBrush> brushDictionary = new Dictionary<string, SolidColorBrush>();

        /// <summary>
        /// Brushを取得する
        /// </summary>
        /// <param name="x">Window</param>
        /// <param name="color">Brushの色</param>
        /// <returns>Brush</returns>
        public static SolidColorBrush GetBrush(
            this Window x,
            Color color)
        {
            return GetBrush(color);
        }

        /// <summary>
        /// Brushを取得する
        /// </summary>
        /// <param name="x">UserControl</param>
        /// <param name="color">Brushの色</param>
        /// <returns>Brush</returns>
        public static SolidColorBrush GetBrush(
            this UserControl x,
            Color color)
        {
            return GetBrush(color);
        }

        /// <summary>
        /// オーバーレイとして非表示にする
        /// </summary>
        /// <param name="x">Window</param>
        public static void HideOverlay(
            this Window x)
        {
            if (x.Opacity > 0d)
            {
                x.Opacity = 0d;
                x.Topmost = false;
            }
        }

        /// <summary>
        /// オーバーレイとして表示する
        /// </summary>
        /// <param name="x">Window</param>
        public static void ShowOverlay(
            this Window x)
        {
            if (x.Opacity <= 0d)
            {
                var targetOpacity = (100d - Settings.Default.Opacity) / 100d;
                x.Opacity = targetOpacity;
                x.Topmost = true;
            }
        }

        /// <summary>
        /// Brushを取得する
        /// </summary>
        /// <param name="color">Brushの色</param>
        /// <returns>Brush</returns>
        private static SolidColorBrush GetBrush(
            Color color)
        {
            if (!brushDictionary.ContainsKey(color.ToString()))
            {
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                brushDictionary[color.ToString()] = brush;
            }

            return brushDictionary[color.ToString()];
        }
    }
}