
using System.Windows;
using System.Windows.Controls;

namespace Maptz.QuickVideoPlayer
{

    public class TimelineBaseControl : FrameworkElement
    {
        public long StartMs
        {
            get
            {
                return (long)GetValue(StartMsProperty);
            }

            set
            {
                SetValue(StartMsProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for StartMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartMsProperty = DependencyProperty.Register("StartMs", typeof(long), typeof(TimelineBaseControl), new PropertyMetadata(0L, TimelineBaseControl.OnStartMsPropertyChanged));
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
            get
            {
                return (long)GetValue(EndMsProperty);
            }

            set
            {
                SetValue(EndMsProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for EndMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndMsProperty = DependencyProperty.Register("EndMs", typeof(long), typeof(TimelineBaseControl), new PropertyMetadata(1L, TimelineBaseControl.OnEndMsPropertyChanged));
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