using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Subtitler.App;
using Maptz.Subtitler.App.Projects;
using Maptz.Tiles;
using System;
using System.Globalization;
using System.Windows.Data;
namespace Maptz.Subtitler.Wpf.Engine.Converters
{

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