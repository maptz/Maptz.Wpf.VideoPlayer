using System;
using System.Globalization;
using System.Windows.Data;
namespace Maptz.Subtitler.Wpf.Engine.Converters
{

    public class MsToTimeCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long l = (long)value;
            TimeCode offset = (TimeCode)parameter;
            var ret = offset.Add(TimeCode.FromSeconds((double)l / 1000.0, offset.FrameRate));
            return ret.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}