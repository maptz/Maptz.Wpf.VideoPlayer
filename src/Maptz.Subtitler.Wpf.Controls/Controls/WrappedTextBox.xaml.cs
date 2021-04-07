using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using Maptz.Editing.TimeCodeDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Maptz.Subtitler.Wpf.Controls
{

    /// <summary>
    /// Interaction logic for WrappedTextBox.xaml
    /// </summary>
    public partial class WrappedTextBox : UserControl
    {
        /* #region Private Static Methods */
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WrappedTextBox).OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }
        /* #endregion Private Static Methods */
        /* #region Public Static Fields */

        // Using a DependencyProperty as the backing store for TextProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(WrappedTextBox), new PropertyMetadata(string.Empty, OnTextPropertyChanged));
        /* #endregion Public Static Fields */
        /* #region Private Fields */
        private const string SMPTEREGEXSTRING = "(?<Hours>\\d{2})[:\\.](?<Minutes>\\d{2})[:\\.](?<Seconds>\\d{2})(?::|;|\\.)(?<Frames>\\d{2})";
        /* #endregion Private Fields */
        /* #region Private Methods */
        private void OnTextChanged(string oldValue, string newValue)
        {
            if (this.x_TextEditor.Text != this.Text)
            {
                this.x_TextEditor.Document.Text = newValue;
            }
        }
        private void X_TextEditor_TextChanged(object sender, EventArgs e)
        {
            if (this.x_TextEditor.Text != this.Text)
            {
                this.Text = this.x_TextEditor.Text;
            }
        }
        /* #endregion Private Methods */
        /* #region Public Properties */
        public int CaretIndex
        {
            get
            {
                return this.x_TextEditor.CaretOffset;
            }

            set
            {
                this.x_TextEditor.CaretOffset = value;
            }
        }
        public string SelectedText
        {
            get
            {
                return this.x_TextEditor.SelectedText;
            }

            set
            {
                this.x_TextEditor.SelectedText = value;
            }
        }
        public int SelectionLength
        {
            get
            {
                return this.SelectedText.Length;
            }

            set
            {
                if (value == 0)
                {
                    this.x_TextEditor.Select(this.x_TextEditor.CaretOffset, 0);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }
        public TextEditor TextEditor => this.x_TextEditor;
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public WrappedTextBox()
        {
            InitializeComponent();
            this.x_TextEditor.TextChanged += X_TextEditor_TextChanged;

            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            this.x_TextEditor.SyntaxHighlighting = new CustomHighlightingDefinition();

            this.x_TextEditor.TextArea.Caret.CaretBrush = new SolidColorBrush(Colors.Red);

        }
        /* #endregion Public Constructors */
        /* #region Public Methods */
        public void ClearHighlights(Func<HighlightRegion, bool> predicate)
        {
            this.ClearHighlightsInternal(predicate);
            this.InvalidateVisual();
        }
        private void ClearHighlightsInternal(Func<HighlightRegion, bool> predicate)
        {
            var idsToRemove = new List<int>();
            var arr = this.HighlightRegions.ToArray();
            for (int i = this.HighlightRegions.Count - 1; i >= 0; i--)
            {
                var hs = this.HighlightRegions[i];
                if (predicate(hs))
                {
                    idsToRemove.Add(i);
                }
            }
            foreach (var id in idsToRemove)
            {
                this.HighlightRegions.RemoveAt(id);
            }
        }

        public void EndHighlight(ITextSpan span, HighlightKind kind)
        {
            var idsToRemove = new List<int>();
            var arr = this.HighlightRegions.ToArray();
            for (int i = this.HighlightRegions.Count - 1; i >= 0; i--)
            {
                var hs = this.HighlightRegions[i];
                if (hs.TextSpan.Start == span.Start && hs.TextSpan.Length == span.Length && kind == hs.Kind)
                {
                    idsToRemove.Add(i);
                }
            }
            foreach (var id in idsToRemove)
            {
                this.HighlightRegions.RemoveAt(id);
            }

            this.InvalidateVisual();
        }

        public enum HighlightKind { Hover, Caret, VideoCursor }

        public void StartHighlight(ITextSpan span, HighlightKind kind, bool clearAllOfKind = false)
        {
            if (clearAllOfKind) this.ClearHighlightsInternal(p => p.Kind == kind);
            this.HighlightRegions.Add(new HighlightRegion { TextSpan = span, Kind = kind });
            this.InvalidateVisual();
        }

        public IList<HighlightRegion> HighlightRegions = new List<HighlightRegion>();

        public class HighlightRegion
        {
            public ITextSpan TextSpan { get; set; }
            public HighlightKind Kind { get; set; }
        }

        public Brush GetBrush(HighlightRegion region)
        {
            switch (region.Kind)
            {
                case HighlightKind.Caret:
                    return new SolidColorBrush(Colors.Red);
                case HighlightKind.Hover:
                    return new SolidColorBrush(Colors.Yellow);
                case HighlightKind.VideoCursor:
                    return new SolidColorBrush(Colors.Blue);
                default:
                    throw new NotSupportedException();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            try
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Colors.Black), null, new Rect(new Size(this.ActualWidth, this.ActualHeight)));
                base.OnRender(drawingContext);
                foreach (var highlightRegion in this.HighlightRegions)
                {
                    var region = highlightRegion;
                    var lineStart = this.x_TextEditor.Document.GetLineByOffset(highlightRegion.TextSpan.Start).LineNumber;
                    var lineEnd = this.x_TextEditor.Document.GetLineByOffset(highlightRegion.TextSpan.Start + highlightRegion.TextSpan.Length).LineNumber;
                    var viewPos = this.TranslatePoint(new Point(), this.x_TextEditor.TextArea.TextView);

                    var highlightBrush = this.GetBrush(highlightRegion);
                    for (int i = lineStart; i < lineEnd; i++)
                    {
                        var currentLine = this.x_TextEditor.Document.GetLineByNumber(i);
                        foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(this.x_TextEditor.TextArea.TextView, currentLine))
                        {
                            var wid = 3.0;
                            var spacing = 2.0;
                            var left = 0.0;
                            switch (highlightRegion.Kind)
                            {
                                case HighlightKind.Caret:
                                    left = rect.Location.X - viewPos.X - 3.0 * (wid + spacing) - spacing;
                                    break;
                                case HighlightKind.Hover:
                                    left = rect.Location.X - viewPos.X - 2.0 * (wid + spacing) - spacing;
                                    break;
                                case HighlightKind.VideoCursor:
                                    left = rect.Location.X - viewPos.X - 1.0 * (wid + spacing) - spacing;
                                    break;
                                default:
                                    throw new NotSupportedException();
                            }

                            var loc = new Point(left, rect.Location.Y - viewPos.Y);
                            var rect2 = new Rect(loc, new Size(wid, rect.Height));
                            drawingContext.DrawRectangle(highlightBrush, null, rect2);
                        }
                    }

                }
            }
            catch
            {
                //TODO fix issue ere
            }


        }
        /* #endregion Public Methods */
    }
}