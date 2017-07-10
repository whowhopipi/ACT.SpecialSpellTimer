using System.Windows;

using ACT.SpecialSpellTimer.Config;

namespace ACT.SpecialSpellTimer.Views
{
    public static class OutlineTextBlockExtensions
    {
        /// <summary>
        /// 自動計算したStrokeThicknessを設定する
        /// </summary>
        /// <param name="t">OutlineTextBlock</param>
        public static void SetAutoStrokeThickness(
            this OutlineTextBlock t)
        {
            // 基準の太さ
            var thickness = 1.0d;

            // フォントサイズを基準に補正をかける
            thickness *=
                t.FontSize / 11.0d;

            // ウェイトによる補正をかける
            thickness *= (
                t.FontWeight.ToOpenTypeWeight() /
                FontWeights.Normal.ToOpenTypeWeight())
                * 0.9d;

            // 設定によって増幅させる
            var textOutlineThicknessGain = 1.0d;
            if (!WPFHelper.IsDesignMode)
            {
                textOutlineThicknessGain = Settings.Default.TextOutlineThicknessRate;
            }

            t.StrokeThickness = thickness * textOutlineThicknessGain;
        }
    }
}