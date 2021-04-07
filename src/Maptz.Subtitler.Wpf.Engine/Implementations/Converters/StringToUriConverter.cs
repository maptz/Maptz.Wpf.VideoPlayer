using System;
using System.Globalization;
using System.Windows.Data;
namespace Maptz.Subtitler.Wpf.Engine.Converters
{

    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (str != null) return new Uri(str);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}