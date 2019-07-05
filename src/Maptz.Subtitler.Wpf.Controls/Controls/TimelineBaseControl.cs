
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


    public class TimelineBaseControl : FrameworkElement
    {



        public long StartMs
        {
            get { return (long)GetValue(StartMsProperty); }
            set { SetValue(StartMsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartMsProperty =
            DependencyProperty.Register("StartMs", typeof(long), typeof(TimelineBaseControl), new PropertyMetadata(0L, TimelineBaseControl.OnStartMsPropertyChanged));

        private static void OnStartMsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimelineBaseControl).OnStartMsChanged((long)e.OldValue, (long)e.NewValue);
        }

        private void OnStartMsChanged(long oldValue, long newValue)
        {
            InvalidateVisual();
        }

        public long EndMs
        {
            get { return (long)GetValue(EndMsProperty); }
            set { SetValue(EndMsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndMsProperty =
            DependencyProperty.Register("EndMs", typeof(long), typeof(TimelineBaseControl), new PropertyMetadata(1L, TimelineBaseControl.OnEndMsPropertyChanged));

        private static void OnEndMsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimelineBaseControl).OnEndMsChanged((long)e.OldValue, (long)e.NewValue);
        }

        private void OnEndMsChanged(long oldValue, long newValue)
        {
            InvalidateVisual();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

    }
}