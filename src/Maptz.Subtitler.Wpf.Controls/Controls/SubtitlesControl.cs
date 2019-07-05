
using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Editing.TimeCodeDocuments;
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
    public class SubtitlesControl : TimelineBaseControl
    {
        public SubtitlesControl()
        {
            this.ServiceProvider = (Application.Current as IApp).ServiceProvider;
        }

        public IServiceProvider ServiceProvider { get; }



        private IEnumerable<ITimeCodeDocumentItem<string>> _subtitleItems;
        public IEnumerable<ITimeCodeDocumentItem<string>> SubtitleItems
        {
            get => this._subtitleItems;
            set
            {
                var oldValue = this._subtitleItems;
                if (this._subtitleItems != value)
                {
                    this._subtitleItems = value;
                    this.OnSubtitleItemsChanged(oldValue, value);
                }
            }
        }

        private void OnSubtitleItemsChanged(IEnumerable<ITimeCodeDocumentItem<string>> oldValue, IEnumerable<ITimeCodeDocumentItem<string>> value)
        {
            this.InvalidateVisual();
        }




        public long OffsetFrames
        {
            get => (long)GetValue(OffsetFramesProperty);
            set => SetValue(OffsetFramesProperty, value);
        }

        public static readonly DependencyProperty OffsetFramesProperty = DependencyProperty.Register(nameof(OffsetFrames), typeof(long), typeof(SubtitlesControl), new PropertyMetadata(default(long), SubtitlesControl.OnOffsetFramesPropertyChanged));

        private static void OnOffsetFramesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SubtitlesControl).OnOffsetFramesChanged((long)e.OldValue, (long)e.NewValue);
        }

        protected virtual void OnOffsetFramesChanged(long oldValue, long newValue)
        {
            this.InvalidateVisual();
        }



        public SmpteFrameRate FrameRate
        {
            get => (SmpteFrameRate)GetValue(FrameRateProperty);
            set => SetValue(FrameRateProperty, value);
        }

        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(nameof(FrameRate), typeof(SmpteFrameRate), typeof(SubtitlesControl), new PropertyMetadata(default(SmpteFrameRate), SubtitlesControl.OnFrameRatePropertyChanged));

        private static void OnFrameRatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SubtitlesControl).OnFrameRateChanged((SmpteFrameRate)e.OldValue, (SmpteFrameRate)e.NewValue);
        }

        protected virtual void OnFrameRateChanged(SmpteFrameRate oldValue, SmpteFrameRate newValue)
        {
            this.InvalidateVisual();
        }
    


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);


            //var appState = this.ServiceProvider.GetRequiredService<AppState>();
            //var tc = TimeCode.FromFrames(appState.Project.ProjectSettings.OffsetFrames, appState.Project.ProjectSettings.FrameRate);

            var tc = TimeCode.FromFrames(this.OffsetFrames, this.FrameRate);

            foreach (var subtitle in this.SubtitleItems)
            {
                var subStartMs = (subtitle.RecordIn.TotalSeconds  - tc.TotalSeconds) * 1000.0;
                var subEndMs =( subtitle.RecordOut.TotalSeconds  - tc.TotalSeconds) * 1000.0;
                var inRange = subStartMs >= this.StartMs && subStartMs <= this.EndMs || subEndMs >= this.StartMs && subEndMs <= this.EndMs || subStartMs < this.StartMs && subEndMs > this.EndMs;
                if (!inRange) continue;

                var uspan = UVWavHelpers.GetUSpan(this.StartMs, this.EndMs);
                var subStartU = (double)subStartMs / (double)UVWavHelpers.FullRangeProjection.Width;
                var subStartX = this.ActualWidth * (subStartU - uspan.Value) / uspan.Width;

                var subEndU = (double)subEndMs / (double)UVWavHelpers.FullRangeProjection.Width;
                var subEndX = this.ActualWidth * (subEndU - uspan.Value) / uspan.Width;

                var heightProp = 0.1;
                var pen = new Pen(Brushes.Purple, 1.0);
                pen = null;
                //var top = this.ActualHeight * ((1.0 - heightProp) / 2.0);
                var top = this.ActualHeight * ((1.0 - heightProp));
                var heig = this.ActualHeight * heightProp;
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(60, 0, 255, 0)), pen, new Rect(subStartX, top, subEndX - subStartX, heig));
            }



        }
    }
}