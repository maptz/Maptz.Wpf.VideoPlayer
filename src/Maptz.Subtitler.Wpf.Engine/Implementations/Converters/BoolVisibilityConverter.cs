using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Maptz.Subtitler.Wpf.Engine.Converters
{

    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bl = (bool)value;
            return bl ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}