
using System.Windows;
using System.Windows.Controls;
namespace Maptz.QuickVideoPlayer
{
    public class TimelineBasePanel : Panel
    {
        /* #region Private Static Methods */
        private static void OnEndMsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimelineBasePanel).OnEndMsChanged((long)e.OldValue, (long)e.NewValue);
        }
        private static void OnStartMsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimelineBasePanel).OnStartMsChanged((long)e.OldValue, (long)e.NewValue);
        }
        /* #endregion Private Static Methods */
        /* #region Public Static Fields */

        // Using a DependencyProperty as the backing store for EndMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndMsProperty = DependencyProperty.Register("EndMs", typeof(long), typeof(TimelineBasePanel), new PropertyMetadata(1L, TimelineBasePanel.OnEndMsPropertyChanged));

        // Using a DependencyProperty as the backing store for StartMs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartMsProperty = DependencyProperty.Register("StartMs", typeof(long), typeof(TimelineBasePanel), new PropertyMetadata(0L, TimelineBasePanel.OnStartMsPropertyChanged));
        /* #endregion Public Static Fields */
        /* #region Private Methods */
        protected virtual void OnEndMsChanged(long oldValue, long newValue)
        {
            InvalidateVisual();
        }
        protected virtual void OnStartMsChanged(long oldValue, long newValue)
        {
            InvalidateVisual();
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }
        /* #endregion Protected Methods */
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
        /* #endregion Public Properties */
    }
}