
using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
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

    public class CursorControl : TimelineBaseControl
    {
        /* #region Private Static Methods */
        private static void OnCursorMsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CursorControl).OnCursorMsChanged((long?)e.OldValue, (long?)e.NewValue);
        }
        private static void OnCursorWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CursorControl).OnCursorWidthChanged((double)e.OldValue, (double)e.NewValue);
        }
        private static void OnForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CursorControl).OnForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }
        /* #endregion Private Static Methods */
        /* #region Public Static Fields */

        // Using a DependencyProperty as the backing store for CursorU.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorMsProperty =
            DependencyProperty.Register("CursorMs", typeof(long?), typeof(CursorControl), new PropertyMetadata(null, CursorControl.OnCursorMsPropertyChanged));

        // Using a DependencyProperty as the backing store for CursorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorWidthProperty =
            DependencyProperty.Register("CursorWidth", typeof(double), typeof(CursorControl), new PropertyMetadata(1.0, CursorControl.OnCursorWidthPropertyChanged));

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(CursorControl), new PropertyMetadata(Brushes.Red, CursorControl.OnForegroundPropertyChanged));
        /* #endregion Public Static Fields */
        /* #region Private Methods */
        private void OnCursorMsChanged(double? oldValue, double? newValue)
        {
            this.InvalidateVisual();
        }
        private void OnCursorWidthChanged(double oldValue, double newValue)
        {
            this.InvalidateVisual();
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected virtual void OnForegroundChanged(Brush oldValue, Brush newValue)
        {
            this.InvalidateVisual();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)), (Pen)null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            if (!this.CursorMs.HasValue) { }
            else
            {
                var cu = (double)this.CursorMs.Value / (double)UVWavHelpers.FullRangeProjection.Width;
                var uspan = UVWavHelpers.GetUSpan(this.StartMs, this.EndMs);
                var lineX = this.ActualWidth * (cu - uspan.Value) / uspan.Width;
                var pen = new Pen(this.Foreground, this.CursorWidth);
                drawingContext.DrawLine(pen, new System.Windows.Point(lineX, 0), new System.Windows.Point(lineX, this.ActualHeight));
            }



        }
        /* #endregion Protected Methods */
        /* #region Public Properties */
        public long? CursorMs
        {
            get { return (long?)GetValue(CursorMsProperty); }
            set { SetValue(CursorMsProperty, value); }
        }
        public double CursorWidth
        {
            get { return (double)GetValue(CursorWidthProperty); }
            set { SetValue(CursorWidthProperty, value); }
        }
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        /* #endregion Public Properties */

        public CursorControl()
        {
            
        }
    }
}