
using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.Engine;
using Maptz.Subtitler.Wpf.VideoPlayer.Commands;
using Maptz.Subtitler.Wpf.VideoPlayer.Projects;
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
namespace Maptz.Subtitler.Wpf.VideoPlayer
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

        internal void Initialize(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            var cursorData = serviceProvider.GetRequiredService<ICursorProjectData>();
            this._bindingWatchers = new List<BindingWatcherBase>();
            {
                var bw = new BindingWatcher<long?>(cursorData, "CursorMs");
                bw.BindingChanged += (s, e) =>
                {
                    this.CursorMs = (long?)e.NewValue;
                };
                this.CursorMs = (long?)cursorData.CursorMs;
                this._bindingWatchers.Add(bw);
            }

            var tlp = this.ServiceProvider.GetRequiredService<ITimelineProjectData>();
            {
                Binding b = new Binding("ViewMs.StartMs");
                b.Source = tlp;
                this.SetBinding(StartMsProperty, b);
            }
            {
                Binding b = new Binding("ViewMs.EndMs");
                b.Source = tlp;
                this.SetBinding(EndMsProperty, b);
            }
        }

        public bool IsScrubbing { get; set; }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            UIElement el = (UIElement)this;
            el.ReleaseMouseCapture();
            this.IsScrubbing = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            e.Handled = true;
            //Handles cursor downs on the timeline.
            var pos = e.GetPosition(this);
            var widthMs = this.EndMs - (double)this.StartMs;
            var newStartMs = this.StartMs + widthMs * pos.X / this.ActualWidth;
            var tc = TimeCode.FromSeconds(newStartMs / 1000.0, SmpteFrameRate.Smpte25);

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                var projectData = this.ServiceProvider.GetRequiredService<IMarkingProjectData>();
                projectData.MarkInMs = (long)newStartMs;
            }
            else
            {
                var media = this.ServiceProvider.GetRequiredService<IVideoPlayerState>().MediaElement;
                media.Seek(TimeSpan.FromMilliseconds(newStartMs));
                if (media.IsPaused)
                {
                    //Media.Play();
                }
            }

            this.IsScrubbing = true;
            UIElement el = (UIElement)this;
            el.CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.IsScrubbing)
            {
                //Handles cursor downs on the timeline.
                var pos = e.GetPosition(this);
                var widthMs = this.EndMs - (double)this.StartMs;
                var newStartMs = this.StartMs + widthMs * pos.X / this.ActualWidth;
                if (newStartMs < 0)
                    newStartMs = 0;
                var media = this.ServiceProvider.GetRequiredService<IVideoPlayerState>().MediaElement;
                if (newStartMs > media.NaturalDuration?.TotalSeconds * 1000)
                {
                    newStartMs = (long)media.NaturalDuration?.TotalSeconds * 1000;
                }

                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    var projectData = this.ServiceProvider.GetRequiredService<IMarkingProjectData>();
                    projectData.MarkInMs = (long)newStartMs;
                }
                else
                {
                    media.Seek(TimeSpan.FromMilliseconds(newStartMs));
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var timelineCommands = this.ServiceProvider.GetRequiredService<TimelineCommands>();
            if (e.Delta < 0)
            {
                timelineCommands.Zoom(2.0);
            }
            else
            {
                timelineCommands.Zoom(0.5);
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

        public IServiceProvider ServiceProvider { get; private set; }

        private List<BindingWatcherBase> _bindingWatchers;

        /* #endregion Public Properties */

        public CursorControl()
        {

        }
    }
}