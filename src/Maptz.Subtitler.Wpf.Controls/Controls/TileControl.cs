
using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.QuickVideoPlayer.Services;
using Maptz.Tiles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace Maptz.QuickVideoPlayer
{
    public class TileControl : Panel
    {

        protected override Size MeasureOverride(Size availableSize)
        {
            var baseVal = base.MeasureOverride(availableSize);

            this.Image.Measure(availableSize);
            baseVal.Width = Double.IsInfinity(baseVal.Width) ? 200.0 : baseVal.Width;
            baseVal.Height = Double.IsInfinity(baseVal.Height) ? 200.0 : baseVal.Height;
            return baseVal;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var arrane = base.ArrangeOverride(finalSize);
            this.Image.Width = finalSize.Width;
            this.Image.Height = finalSize.Height;
            this.Image.Arrange(new Rect(finalSize));
            
            return finalSize;
        }

        public IServiceProvider ServiceProvider { get; }
        public string TileKey { get; }
        public string FilePath { get; }

        public TileControl()
        {

            this.ServiceProvider = (Application.Current as IApp).ServiceProvider;
            this.TileKey = "00000000";
            this.FilePath = string.Empty;
            this.TileImageRepository = this.ServiceProvider.GetRequiredService<ITileImageRepository>();

            this.Image = new Image();
            this.Children.Add(this.Image);
        }

        public TileControl(IServiceProvider serviceProvider, string tileKey, string filePath)
        {
            this.ServiceProvider = serviceProvider;
            this.TileKey = tileKey;
            this.FilePath = filePath;
            this.TileImageRepository = this.ServiceProvider.GetService<ITileImageRepository>();

            this.Image = new Image();
            this.Image.Stretch = Stretch.Fill;
            this.Children.Add(this.Image);
        }

        public ITileImageRepository TileImageRepository { get; private set; }
        public Image Image { get; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.Loaded += async (s, exx) =>
            {
                var bitmap = await this.LoadImageAsync();
                this.Image.Source = bitmap;
                this.Image.Stretch = Stretch.Fill;
            };

            try
            {
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected override void OnRender(DrawingContext dc)
        {
            var margin = 0.000;
            var left = this.ActualWidth * margin;
            var width = this.ActualWidth * (1.0 - margin*2);

            var top = this.ActualHeight * margin;
            var height = this.ActualHeight * (1.0 - margin * 2);

            Rect rect = new Rect(new System.Windows.Point(left, top), new System.Windows.Size(width, height));
            var pen = new Pen(Brushes.Red, 0.4);
            //var pen = (Pen)null;
            //dc.DrawRectangle(System.Windows.Media.Brushes.Aqua, pen, rect);

            base.OnRender(dc);
        }

        private async Task<BitmapImage> LoadImageAsync()
        {
            var stream = await this.TileImageRepository.GetTileImageAsync(this.TileKey, this.FilePath);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            //bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
    }
}