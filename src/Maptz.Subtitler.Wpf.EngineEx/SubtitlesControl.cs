
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Editing.TimeCodeDocuments;
using Maptz.QuickVideoPlayer.Services;
using Maptz.Subtitler.Wpf.Controls.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Maptz.QuickVideoPlayer
{

    public class SubtitlesControl : TimelineBasePanel
    {
        /* #region Private Static Methods */
        private static void OnFrameRatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SubtitlesControl).OnFrameRateChanged((SmpteFrameRate)e.OldValue, (SmpteFrameRate)e.NewValue);
        }
        private static void OnOffsetFramesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SubtitlesControl).OnOffsetFramesChanged((long)e.OldValue, (long)e.NewValue);
        }
        /* #endregion Private Static Methods */
        /* #region Public Static Fields */
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(nameof(FrameRate), typeof(SmpteFrameRate), typeof(SubtitlesControl), new PropertyMetadata(default(SmpteFrameRate), SubtitlesControl.OnFrameRatePropertyChanged));
        public static readonly DependencyProperty OffsetFramesProperty = DependencyProperty.Register(nameof(OffsetFrames), typeof(long), typeof(SubtitlesControl), new PropertyMetadata(default(long), SubtitlesControl.OnOffsetFramesPropertyChanged));
        /* #endregion Public Static Fields */
        /* #region Private Sub-types */
        private class SubtitleElement
        {
            public Rect? Rect { get; set; }
            public ITimeCodeDocumentItem<string> Item { get; set; }
            public SubtitleControl Element { get; set; }
        }
        /* #endregion Private Sub-types */
        /* #region Private Fields */
        private List<Tuple<ITimeCodeDocumentItem<string>, UIElement, Rect>> _rects;
        private IEnumerable<ITimeCodeDocumentItem<string>> _subtitleItems;
        /* #endregion Private Fields */
        /* #region Private Properties */
        private List<SubtitleElement> SubtitleElements { get; } = new List<SubtitleElement>();
        /* #endregion Private Properties */
        /* #region Private Methods */
        private void Child_Click(object sender, ClickEventArgs e)
        {
            var sc = sender as SubtitleControl;
            var clickIndex = (int)sc.Item.ContentTextSpan.Start + e.Index;
            this.OnClick(clickIndex);
        }
        private void Child_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var sc = sender as SubtitleControl;
            this.OnHoverStart(sc.Item);
        }
        private void Child_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var sc = sender as SubtitleControl;
            this.OnHoverEnd(sc.Item);
        }
        private void OnHoverEnd(ITimeCodeDocumentItem<string> item)
        {
            var hov = this.HoverEnd; if (hov != null) hov(this, new SubtitleHoverEventArgs { Item = item });
        }
        private void OnHoverStart(ITimeCodeDocumentItem<string> item)
        {
            var hov = this.HoverStart; if (hov != null) hov(this, new SubtitleHoverEventArgs { Item = item });
        }
        private void OnSubtitleItemsChanged(IEnumerable<ITimeCodeDocumentItem<string>> oldValue, IEnumerable<ITimeCodeDocumentItem<string>> value)
        {
            this.PopulateUIElements();
            this.InvalidateMeasure();
        }
        private void PopulateUIElements()
        {
            foreach (SubtitleControl child in this.Children)
            {
                child.MouseEnter -= Child_MouseEnter;
                child.MouseLeave -= Child_MouseLeave;
                child.Click -= Child_Click;
            }
            this.Children.Clear();
            this.SubtitleElements.Clear();
            if (this.SubtitleItems != null)
                foreach (var subtitle in this.SubtitleItems)
                {
                    var child = this.CreateUIElement(subtitle);
                    child.MouseEnter += Child_MouseEnter;
                    child.MouseLeave += Child_MouseLeave;
                    child.Click += Child_Click;
                    this.Children.Add(child);
                    this.SubtitleElements.Add(new SubtitleElement
                    {
                        Element = child,
                        Item = subtitle,
                        Rect = null
                    }); ;
                }

            this.InvalidateCursorIndices();
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override Size ArrangeOverride(Size finalSize)
        {
            var baseVal = base.ArrangeOverride(finalSize);
            foreach (var elem in this.SubtitleElements)
            {
                if (elem.Rect.HasValue) elem.Element.Arrange(elem.Rect.Value);
            }
            return finalSize;
        }
        protected virtual SubtitleControl CreateUIElement(ITimeCodeDocumentItem<string> subtitle)
        {
            return new SubtitleControl
            {
                Item = subtitle
            };
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            const double heightProp = 1.0;
            var baseVal = base.MeasureOverride(availableSize);

            var tc = TimeCode.FromFrames(this.OffsetFrames, this.FrameRate);

            foreach (var elem in this.SubtitleElements)
            {
                var subtitle = elem.Item;
                var subStartMs = (subtitle.RecordIn.TotalSeconds - tc.TotalSeconds) * 1000.0;
                var subEndMs = (subtitle.RecordOut.TotalSeconds - tc.TotalSeconds) * 1000.0;
                var inRange = subStartMs >= this.StartMs && subStartMs <= this.EndMs || subEndMs >= this.StartMs && subEndMs <= this.EndMs || subStartMs < this.StartMs && subEndMs > this.EndMs;
                if (!inRange)
                    continue;
                var uspan = UVWavHelpers.GetUSpan(this.StartMs, this.EndMs);
                var subStartU = (double)subStartMs / (double)UVWavHelpers.FullRangeProjection.Width;
                var leftPix = this.ActualWidth * (subStartU - uspan.Value) / uspan.Width;
                var subEndU = (double)subEndMs / (double)UVWavHelpers.FullRangeProjection.Width;
                var subEndX = this.ActualWidth * (subEndU - uspan.Value) / uspan.Width;

                var pen = new Pen(Brushes.Purple, 1.0);
                pen = null;
                //var top = this.ActualHeight * ((1.0 - heightProp) / 2.0);
                var topPix = this.ActualHeight * ((1.0 - heightProp));
                var bottomPix = topPix + this.ActualHeight * heightProp;
                var rightPix = leftPix + subEndX - leftPix;

                elem.Element.HasCroppedLeft = leftPix < 0;
                elem.Element.HasCroppedRight = rightPix > this.ActualWidth;

                leftPix = leftPix < 0 ? 0 : leftPix;
                rightPix = rightPix > this.ActualWidth ? this.ActualWidth : rightPix;
                topPix = topPix < 0 ? 0 : topPix;
                bottomPix = bottomPix > this.ActualHeight ? this.ActualHeight : bottomPix;

                var rect = new Rect(leftPix, topPix, rightPix - leftPix, bottomPix - topPix);
                elem.Rect = rect;
                elem.Element.Measure(rect.Size);
            }
            return baseVal;
        }
        protected virtual void OnClick(int? index)
        {
            var click = this.Click;
            if (click != null) click(this, new ClickEventArgs(index));
        }
        protected override void OnEndMsChanged(long oldValue, long newValue)
        {
            this.PopulateUIElements();

            base.OnEndMsChanged(oldValue, newValue);
        }
        protected virtual void OnFrameRateChanged(SmpteFrameRate oldValue, SmpteFrameRate newValue)
        {
            this.InvalidateMeasure();
        }
        protected virtual void OnOffsetFramesChanged(long oldValue, long newValue)
        {
            this.InvalidateMeasure();
        }
        protected override void OnStartMsChanged(long oldValue, long newValue)
        {
            this.PopulateUIElements();
            base.OnStartMsChanged(oldValue, newValue);
        }
        /* #endregion Protected Methods */
        /* #region Public Delegates */
        public event EventHandler<ClickEventArgs> Click;
        public event EventHandler<SubtitleHoverEventArgs> HoverEnd;
        public event EventHandler<SubtitleHoverEventArgs> HoverStart;
        /* #endregion Public Delegates */
        /* #region Public Properties */
        public SmpteFrameRate FrameRate
        {
            get => (SmpteFrameRate)GetValue(FrameRateProperty);
            set => SetValue(FrameRateProperty, value);
        }
        public long OffsetFrames
        {
            get => (long)GetValue(OffsetFramesProperty);
            set => SetValue(OffsetFramesProperty, value);
        }
        public IServiceProvider ServiceProvider { get; }
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
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public SubtitlesControl()
        {
            this.ServiceProvider = (Application.Current as IApp).ServiceProvider;
            this.Focusable = false;

        }
        /* #endregion Public Constructors */


        public int? CursorIndex
        {
            get => (int?)GetValue(CursorIndexProperty);
            set => SetValue(CursorIndexProperty, value);
        }

        public static readonly DependencyProperty CursorIndexProperty = DependencyProperty.Register(nameof(CursorIndex), typeof(int?), typeof(SubtitlesControl), new PropertyMetadata(default(int?), SubtitlesControl.OnCursorIndexPropertyChanged));

        private static void OnCursorIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SubtitlesControl).OnCursorIndexChanged((int?)e.OldValue, (int?)e.NewValue);
        }

        protected virtual void OnCursorIndexChanged(int? oldValue, int? newValue)
        {
            this.InvalidateCursorIndices();
        }

        private void InvalidateCursorIndices()
        {
            foreach(var sube in this.SubtitleElements)
            {
                int? relativeCursorIndex = null;
                if (this.CursorIndex.HasValue)
                {
                    var cursorIsInRange =  (this.CursorIndex.Value >= sube.Item.ContentTextSpan.Start && this.CursorIndex.Value <= (sube.Item.ContentTextSpan.Start + sube.Item.ContentTextSpan.Length)) ;
                    if (cursorIsInRange)
                    {
                        relativeCursorIndex = this.CursorIndex.Value - sube.Item.ContentTextSpan.Start;
                    }
                }
                sube.Element.CursorIndex = relativeCursorIndex;
            }
        }
    }
}