using Maptz.QuickVideoPlayer;
using Maptz.Subtitler.App;
using Maptz.Subtitler.Wpf.Controls;
using System.Windows;
using System.Windows.Media;
namespace Maptz.Subtitler.Wpf.Engine.Icons
{

    public class XamlIconSource : IIconSource
    {
        public XamlIconSource(string path)
        {
            this.Path = path;
        }

        public string Path { get; }
        public FrameworkElement GetIconElement()
        {
            return new VectorIcon
            {
                Geometry = Geometry.Parse(this.Path)
            };
        }
    }
}