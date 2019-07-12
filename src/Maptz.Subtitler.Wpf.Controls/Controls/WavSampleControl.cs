
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.QuickVideoPlayer.Services;
using Maptz.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Maptz.QuickVideoPlayer
{
    public class WavSampleControl : Panel
    {
        /* #region Private Static Methods */
        private static void OnEndMsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WavSampleControl).OnEndMsChanged((long)e.OldValue, (long)e.NewValue);
        }

        private static void OnFilePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WavSampleControl).OnFilePathChanged((string)e.OldValue, (string)e.NewValue);
        }

        private static void OnStartMsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WavSampleControl).OnStartMsChanged((long)e.OldValue, (long)e.NewValue);
        }

        /* #endregion Private Static Methods */
        /* #region Public Static Fields */
        // Using a DependencyProperty as the backing store for EndMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndMsProperty = DependencyProperty.Register("EndMs", typeof(long), typeof(WavSampleControl), new PropertyMetadata(1L, WavSampleControl.OnEndMsPropertyChanged));
        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(WavSampleControl), new PropertyMetadata(null, WavSampleControl.OnFilePathPropertyChanged));
        // Using a DependencyProperty as the backing store for StartMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartMsProperty = DependencyProperty.Register("StartMs", typeof(long), typeof(WavSampleControl), new PropertyMetadata(0L, WavSampleControl.OnStartMsPropertyChanged));
        /* #endregion Public Static Fields */
        /* #region Private Methods */
        private IViewport GetViewport()
        {
            var uspan = UVWavHelpers.GetUSpan(this.StartMs, this.EndMs);
            var viewport = new Viewport()
            {Left = uspan.Value, Width = uspan.Width, Top = 0, Height = uspan.Width};
            return viewport;
        }

        private void InvalidateTiles()
        {
            if (this.FilePath == null)
            {
                return;
            }

            var viewport = this.GetViewport();
            var tileProvider = new DefaultTileProvider();
            var levelOfDetail = QuadkeyHelpers.GetLevelOfDetail(viewport);
            //levelOfDetail++;
            
            var tiles = tileProvider.GetTiles(levelOfDetail, viewport);
            var tiles1d = tiles.Where(p => p.TileY() == 0);
            var currentKeys = this.TileControls.Keys.ToArray();
            /* #region Get new, old and existing keys */
            var newTileKeys = new List<string>();
            var oldTileKeys = new List<string>();
            var existingTileKeys = new List<string>();
            foreach (var tile in tiles1d)
            {
                if (currentKeys.Contains(tile.Key))
                {
                    existingTileKeys.Add(tile.Key);
                }
                else
                {
                    newTileKeys.Add(tile.Key);
                }
            }

            foreach (var existingKey in currentKeys)
            {
                if (!tiles.Any(p => p.Key == existingKey))
                {
                    oldTileKeys.Add(existingKey);
                }
            }

            /* #endregion*/
            foreach (var newTileKey in newTileKeys)
            {
                var tileControl = new TileControl(this.ServiceProvider, newTileKey, this.FilePath);
                this.TileControls.Add(newTileKey, tileControl);
                this.Children.Add(tileControl);
            //this.AddVisualChild(tileControl);
            }

            foreach (var oldTileKey in oldTileKeys)
            {
                var tileControl = this.TileControls[oldTileKey];
                this.TileControls.Remove(oldTileKey);
                this.Children.Remove(tileControl);
            //this.RemoveVisualChild(tileControl);
            }

            foreach (var existingTile in existingTileKeys)
            {
            }

            this.InvalidateArrange();
        }

        private void OnEndMsChanged(long oldValue, long newValue)
        {
            InvalidateTiles();
        }

        private void OnStartMsChanged(long oldValue, long newValue)
        {
            InvalidateTiles();
        }

        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override Size ArrangeOverride(Size finalSize)
        {
            var baseVal = base.ArrangeOverride(finalSize);
            //foreach(FrameworkElement child in this.Children)
            //{
            //    var imageRect = new Rect(0,0, 200,200);
            //    child.Arrange(imageRect);
            //}
            var viewport = this.GetViewport();
            var uspan = UVWavHelpers.GetUSpan(this.StartMs, this.EndMs);
            foreach (var kvp in this.TileControls)
            {
                QuadkeyHelpers.QuadKeyToTileXY(kvp.Key, out int tileX, out int tileY, out int ld);
                var tile = new Tile(tileX, tileY, ld);
                var tileUV = tile.Viewport();
                var imageLeft = finalSize.Width * (tileUV.Left - uspan.Value) / uspan.Width;
                var imageRight = finalSize.Width * ((tileUV.Left + tileUV.Width) - uspan.Value) / uspan.Width;
                var tileLeftMs = tileUV.Left * UVWavHelpers.FullRangeProjection.Width;
                var tileWidthMs = tileUV.Width * UVWavHelpers.FullRangeProjection.Width;
                var imageTop = 0.0;
                var imageHeight = finalSize.Height;
                //var imageLeft = 0.0;
                //var imageWidth = 100.0;
                //var imageTop = 0.0;
                //var imageHeight = 100.0;
                var imageRect = new Rect(imageLeft, imageTop, imageRight - imageLeft, imageHeight);
                //kvp.Value.SetValue(Canvas.LeftProperty, imageLeft);
                //kvp.Value.SetValue(Canvas.TopProperty, imageTop);
                //kvp.Value.Width = imageWidth;
                //kvp.Value.Height = imageHeight;
                kvp.Value.Arrange(imageRect);
            
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var baseVal = base.MeasureOverride(availableSize);
            foreach (var tc in this.TileControls)
            {
                tc.Value.Measure(availableSize);
            }

            return availableSize;
        }

        protected virtual void OnFilePathChanged(string oldValue, string newValue)
        {
            this.TileControls.Clear();
            this.InvalidateTiles();
        }

        /* #endregion Protected Methods */
        /* #region Public Fields */
        public IDictionary<string, TileControl> TileControls = new Dictionary<string, TileControl>();
        /* #endregion Public Fields */
        /* #region Public Properties */
        public long EndMs
        {
            get
            {
                return (long)GetValue(EndMsProperty);
            }

            set
            {
                SetValue(EndMsProperty, value);
            }
        }

        public string FilePath
        {
            get
            {
                return (string)GetValue(FilePathProperty);
            }

            set
            {
                SetValue(FilePathProperty, value);
            }
        }

        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public long StartMs
        {
            get
            {
                return (long)GetValue(StartMsProperty);
            }

            set => SetValue(StartMsProperty, value);
        }

        /* #endregion Public Properties */
        /* #region Public Constructors */
        public WavSampleControl()
        {
            this.ServiceProvider = (Application.Current as IApp).ServiceProvider;
            this.InvalidateTiles();
            this.Focusable = false;
        }

        /* #endregion Public Constructors */
        /* #region Public Methods */
        public void Initialize(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.InvalidateTiles();
        }
    /* #endregion Public Methods */
    }
}