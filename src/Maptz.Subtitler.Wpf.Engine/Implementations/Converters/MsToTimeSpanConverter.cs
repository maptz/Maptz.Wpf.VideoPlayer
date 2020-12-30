using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Tiles;
using System;
using System.Globalization;
using System.Windows.Data;
namespace Maptz.Subtitler.Wpf.Engine.Converters
{
    public class MsToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var l = (long)value;
            //var frp = UVWavHelpers.FullRangeProjection;
            //var ret = (double)l / (double)frp.Width;
            //return ret;
            var ret = TimeSpan.FromMilliseconds(l);
            return ret.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}