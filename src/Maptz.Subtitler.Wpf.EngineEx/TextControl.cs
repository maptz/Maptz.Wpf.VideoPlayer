
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
namespace Maptz.Subtitler.Wpf.Controls
{
    public class TextControl : Control
    {
        /* #region Private Static Methods */
        private static string GetStringSummary(string str, int i)
        {
            var len = 4;
            var start = i > len ? i - len : 0;
            var end = i + len > str.Length ? str.Length : i + len;
            var endStr = i + 1 < str.Length ? str.Substring(i + 1, end - (i + 1)) : string.Empty;
            var startStr = start < str.Length ? str.Substring(start, i - start) : string.Empty;
            return startStr + "[" + str[i].ToString() + "]" + endStr;
        }
        /* #endregion Private Static Methods */
        /* #region Private Sub-types */
        private class TextRecord
        {

            public TextRecord(string text)
            {
                this.Text = text;
            }

            private FormattedText _formattedText;
            public FormattedText FormattedText
            {
                get => this._formattedText;
                set
                {
                    var oldValue = this._formattedText;
                    if (this._formattedText != value)
                    {
                        this._formattedText = value;
                        this.OnFormattedTextChanged(oldValue, value);
                    }
                }
            }

            private async void OnFormattedTextChanged(FormattedText oldValue, FormattedText value)
            {
                await this.InvalidateRectsAsync();
            }

            public string Text { get;  }
            public IList<Rect> Rects { get; private set; }  = null;
            public bool HasGeneratedRects => this.Rects != null;

            private async Task InvalidateRectsAsync()
            {
                await Task.Run(() =>
                {
                    if (this.FormattedText == null || this.Text == null)
                        return;
                    var rects = new List<Rect>();
                    for (int i = 0; i < this.Text.Length; i++)
                    {
                        var geo = this.FormattedText.BuildHighlightGeometry(new Point(0, 0), i, 1);
                        if (geo != null)
                            rects.Add(geo.Bounds);
                        else
                        {
                            rects.Add(new Rect());
                        }
                    }

                    this.Rects = rects;
                });
                
            }
        }
        /* #endregion Private Sub-types */
        /* #region Private Fields */
        private string _text;
        private TextRecord _textRecord = new TextRecord(string.Empty);
        /* #endregion Private Fields */
        /* #region Private Methods */
        private void OnTextChanged(string oldValue, string value)
        {
            this._textRecord = new TextRecord(value);
            this.InvalidateVisual();
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override void OnRender(DrawingContext drawingContext)
        {
            
            // Create the initial formatted text string.
            FormattedText formattedText = new FormattedText(this.Text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Green, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            this._textRecord.FormattedText = formattedText;
            // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
            formattedText.MaxTextWidth = this.ActualWidth;
            formattedText.MaxTextHeight = 240;

            drawingContext.DrawText(formattedText, new Point(0, 0));
        }
        /* #endregion Protected Methods */
        /* #region Public Properties */
        public string Text
        {
            get => this._text;
            set
            {
                var oldValue = this._text;
                if (this._text != value)
                {
                    this._text = value;
                    this.OnTextChanged(oldValue, value);
                }
            }
        }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public TextControl()
        {
            this.Cursor = Cursors.IBeam;
        }
        /* #endregion Public Constructors */
        /* #region Public Methods */
        public int? GetOffsetAtPoint(Point pos)
        {
            if (this._textRecord.Rects == null) return null;

            var rects = this._textRecord.Rects;
            for (int i = 0; i < rects.Count; i++)
            {
                var rect = rects[i];
                var match = pos.X > rect.Left && pos.X < rect.Right && pos.Y > rect.Top && pos.Y < rect.Bottom;
                if (match)
                {
                    return i;
                }
            }
            return null;
        }
        public Rect? GetRectFromOffset(int i)
        {
            if (this._textRecord.Rects == null) return null;

            var rects = this._textRecord.Rects;
            if (i < 0 || i >= rects.Count) throw new ArgumentOutOfRangeException();
            return rects[i];
        }
        /* #endregion Public Methods */
    }
}