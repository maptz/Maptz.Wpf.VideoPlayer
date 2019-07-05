using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Tiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unosquare.FFME;
using Unosquare.FFME.Common;
namespace Maptz.QuickVideoPlayer
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

    public class ViewMsToViewportConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as IViewMs;
            if (source == null) return null;
            var uspan = UVWavHelpers.GetUSpan(source.StartMs, source.EndMs);
            return new Viewport
            {
                Left = uspan.Value,
                Width = uspan.Width,
                Top = 0,
                Height = 1.0
            };

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as IViewport;
            var frp = UVWavHelpers.FullRangeProjection;
            var widthMs = (long)(source.Width * frp.Width);
            var start = (long)(source.Left * frp.Width);
            return new ViewMs
            {
                StartMs = start,
                EndMs = start + widthMs
            };

        }
    }
}