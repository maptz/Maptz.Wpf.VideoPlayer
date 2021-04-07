using System;
using System.Globalization;
using System.Windows.Data;
namespace Maptz.Subtitler.Wpf.Engine.Converters
{

    public class TimeCodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tc = (TimeCode)value;
            return tc.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}