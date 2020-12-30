using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Maptz.Subtitler.Wpf.Controls
{



    public class TouchTextControl : Panel
    {
        /* #region Private Static Methods */
        private static void OnCursorIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TouchTextControl).OnCursorIndexChanged((int?)e.OldValue, (int?)e.NewValue);
        }
        private static void OnSelectionEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TouchTextControl).OnSelectionEndChanged((int?)e.OldValue, (int?)e.NewValue);
        }
        private static void OnSelectionStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TouchTextControl).OnSelectionStartChanged((int?)e.OldValue, (int?)e.NewValue);
        }
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TouchTextControl).OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }
        /* #endregion Private Static Methods */
        /* #region Public Static Fields */
        public static readonly DependencyProperty CursorIndexProperty = DependencyProperty.Register(nameof(CursorIndex), typeof(int?), typeof(TouchTextControl), new PropertyMetadata(default(int?), TouchTextControl.OnCursorIndexPropertyChanged));
        public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register(nameof(SelectionEnd), typeof(int?), typeof(TouchTextControl), new PropertyMetadata(default(int?), TouchTextControl.OnSelectionEndPropertyChanged));
        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(nameof(SelectionStart), typeof(int?), typeof(TouchTextControl), new PropertyMetadata(default(int?), TouchTextControl.OnSelectionStartPropertyChanged));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TouchTextControl), new PropertyMetadata(default(string), TouchTextControl.OnTextPropertyChanged));
        /* #endregion Public Static Fields */
        /* #region Private Fields */
        private bool _blink = true;
        private DispatcherTimer _caretBlinkTimer = new DispatcherTimer();
        private TextControl _textControl;
        /* #endregion Private Fields */
        /* #region Private Methods */
        void caretBlinkTimer_Tick(object sender, EventArgs e)
        {
            _blink = !_blink;
            InvalidateVisual();
        }
        void StartBlinkAnimation()
        {
            TimeSpan blinkTime = TimeSpan.FromSeconds(0.5);
            _blink = true; // the caret should visible initially
            // This is important if blinking is disabled (system reports a negative blinkTime)
            if (blinkTime.TotalMilliseconds > 0)
            {
                _caretBlinkTimer.Interval = blinkTime;
                _caretBlinkTimer.Start();
            }
        }
        void StopBlinkAnimation()
        {
            _caretBlinkTimer.Stop();
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var baseVal = base.ArrangeOverride(arrangeBounds);
            this._textControl.Arrange(new Rect(new Point(), arrangeBounds));
            return arrangeBounds;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            var baseVal = base.MeasureOverride(constraint);
            this._textControl.Measure(constraint);
            return baseVal;
        }
        protected virtual void OnCursorIndexChanged(int? oldValue, int? newValue)
        {
            this._blink = true;
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (this.CursorIndex.HasValue & _blink)
            {
                var idx = this.CursorIndex.Value < this.Text.Length ? this.CursorIndex.Value : this.Text.Length - 1;
                var rect = this._textControl.GetRectFromOffset(idx);
                if (rect.HasValue)
                {
                    drawingContext.DrawLine(new Pen(Brushes.Red, 2), rect.Value.TopLeft, rect.Value.BottomLeft);
                }
            }
        }
        protected virtual void OnSelectionEndChanged(int? oldValue, int? newValue)
        {
            InvalidateVisual();
        }
        protected virtual void OnSelectionStartChanged(int? oldValue, int? newValue)
        {
            InvalidateVisual();
        }
        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            this._textControl.Text = newValue;
            InvalidateVisual();
        }
        /* #endregion Protected Methods */
        /* #region Public Properties */
        public int? CursorIndex
        {
            get => (int?)GetValue(CursorIndexProperty);
            set => SetValue(CursorIndexProperty, value);
        }
        public int? SelectionEnd
        {
            get => (int?)GetValue(SelectionEndProperty);
            set => SetValue(SelectionEndProperty, value);
        }
        public int? SelectionStart
        {
            get => (int?)GetValue(SelectionStartProperty);
            set => SetValue(SelectionStartProperty, value);
        }
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public TouchTextControl()
        {
            this._textControl = new TextControl();
            this.Children.Add(this._textControl);
            this._textControl.MouseUp += (s, e) =>
            {
                var p = e.GetPosition((UIElement)s);
                var idx = this._textControl.GetOffsetAtPoint(p);
                this.RaiseClick(idx);
            };

            _caretBlinkTimer.Tick += new EventHandler(caretBlinkTimer_Tick);
            StartBlinkAnimation();

        }
        public event EventHandler<ClickEventArgs> Click;

        private void RaiseClick(int? idx)
        {
            var c = this.Click; if (c != null) c(this, new ClickEventArgs(idx));
        }
        /* #endregion Public Constructors */
    }
}