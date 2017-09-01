using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FFXIV.Framework.Converters
{
    public class ThicknessToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = System.Convert.ToDouble(value);
            return new Thickness(v / 2, v / 2, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
