using System;
using System.Globalization;
using System.Windows.Data;

namespace FFXIV.Framework.Converters
{
    public class AddConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = System.Convert.ToDouble(value);
            var p = 0d;
            if (parameter != null)
            {
                p = System.Convert.ToDouble(parameter);
            }

            return v + p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
