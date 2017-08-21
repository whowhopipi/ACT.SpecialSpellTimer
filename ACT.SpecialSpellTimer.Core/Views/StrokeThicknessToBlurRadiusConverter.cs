using System;
using System.Globalization;
using System.Windows.Data;

using ACT.SpecialSpellTimer.Config;
using FFXIV.Framework.Common;

namespace ACT.SpecialSpellTimer.Views
{
    public class StrokeThicknessToBlurRadiusConverter :
        IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            // アウトラインの太さを基準にして増幅する
            // 増幅率は一応設定可能とする
            var textBlurGain = 2.0d;
#if DEBUG
            if (WPFHelper.IsDesignMode)
            {
                return (double)value * textBlurGain;
            }
#endif
            textBlurGain = Settings.Default.TextBlurRate;

            return (double)value * textBlurGain;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
