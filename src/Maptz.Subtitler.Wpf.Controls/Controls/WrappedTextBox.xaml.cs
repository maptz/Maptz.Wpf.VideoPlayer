using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;

namespace Maptz.QuickVideoPlayer
{
    public static class RichTextBoxExt
    {
        public static int TextPointerToAbsoluteCursor(this RichTextBox rtb, TextPointer tp)
        {
            TextRange documentRange = new TextRange(rtb.Document.ContentStart, tp);
            return documentRange.Text.Length;

        }

        public static TextPointer AbsoluteCursorToTextPointer2(this RichTextBox rtb, int offset)
        {
            var pt = rtb.Document.ContentStart;
            TextRange documentRange = new TextRange(rtb.Document.ContentStart, pt);
            while (documentRange.Text.Length < offset)
            {
                pt = pt.GetNextInsertionPosition(LogicalDirection.Forward);
                documentRange = new TextRange(rtb.Document.ContentStart, pt);
            }
            return pt;

        }

        public static TextPointer AbsoluteCursorToTextPointer(this RichTextBox richTextBox, int offset)
        {
            var navigator = richTextBox.Document.ContentStart;
            int cnt = 0;

            while (navigator.CompareTo(richTextBox.Document.ContentEnd) < 0)
            {
                switch (navigator.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.ElementStart:
                        break;
                    case TextPointerContext.ElementEnd:
                        if (navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            cnt += 2;
                        break;
                    case TextPointerContext.EmbeddedElement:
                        // TODO: Find out what to do here?
                        cnt++;
                        break;
                    case TextPointerContext.Text:
                        int runLength = navigator.GetTextRunLength(LogicalDirection.Forward);

                        if (runLength > 0 && runLength + cnt < offset)
                        {
                            cnt += runLength;
                            navigator = navigator.GetPositionAtOffset(runLength);
                            if (cnt > offset)
                                break;
                            continue;
                        }
                        cnt++;
                        break;
                }

                if (cnt > offset)
                    break;

                navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

            } // End while.

            return navigator;
        }
    }

    /// <summary>
    /// Interaction logic for WrappedTextBox.xaml
    /// </summary>
    public partial class WrappedTextBox : UserControl
    {

        private void X_RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextRange documentRange = new TextRange(this.x_RichTextBox.Document.ContentStart, this.x_RichTextBox.Document.ContentEnd);
            this.Text = documentRange.Text;

            this.UpdateColoring();
        }

        private void UpdateColoring()
        {
            TextRange documentRange = new TextRange(this.x_RichTextBox.Document.ContentStart, this.x_RichTextBox.Document.ContentEnd);
            documentRange.ClearAllProperties();
            var TextInput = this.x_RichTextBox;
            TextPointer navigator = TextInput.Document.ContentStart;
            while (navigator.CompareTo(TextInput.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    CheckWordsInRun((Run)navigator.Parent);
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

        }

        private void CheckWordsInRun(Run parent)
        {
            var txt = parent.Text;

            var matches = (Regex.Matches(txt, SMPTEREGEXSTRING) as IEnumerable<Match>).Reverse().ToArray();
            this.x_RichTextBox.TextChanged -= this.X_RichTextBox_TextChanged;
            foreach (var match in matches)
            {
                TextRange range = new TextRange(parent.ContentStart.GetPositionAtOffset(match.Index, LogicalDirection.Forward), parent.ContentStart.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Forward));
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDFF00")));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            this.x_RichTextBox.TextChanged += this.X_RichTextBox_TextChanged;

        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WrappedTextBox), new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WrappedTextBox).OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        private void OnTextChanged(string oldValue, string newValue)
        {
            TextRange documentRange = new TextRange(this.x_RichTextBox.Document.ContentStart, this.x_RichTextBox.Document.ContentEnd);
            if (documentRange.Text == newValue) return;

            Paragraph myParagraph = new Paragraph();
            myParagraph.Inlines.Add(new Run(newValue));
            FlowDocument myFlowDocument = new FlowDocument(myParagraph);
            myFlowDocument.Blocks.Add(myParagraph);
            this.x_RichTextBox.Document = myFlowDocument;
        }


        private const string SMPTEREGEXSTRING = "(?<Hours>\\d{2})[:\\.](?<Minutes>\\d{2})[:\\.](?<Seconds>\\d{2})(?::|;|\\.)(?<Frames>\\d{2})";



        public WrappedTextBox()
        {
            InitializeComponent();

            this.x_RichTextBox.TextChanged += X_RichTextBox_TextChanged;
        }


        public int CaretIndex
        {
            get
            {
                return this.x_RichTextBox.TextPointerToAbsoluteCursor(this.x_RichTextBox.CaretPosition);
            }
            set
            {
                var tp = this.x_RichTextBox.AbsoluteCursorToTextPointer(value);
                this.x_RichTextBox.CaretPosition = tp;

                //var pos = this.x_RichTextBox.Document.ContentStart.GetPositionAtOffset(value);
                
                //https://archive.codeplex.com/?p=wpfsyntax
                //var direction = LogicalDirection.Backward;
                //var caretPosition = this.x_RichTextBox.CaretPosition;

                //if (value > 0)
                //{
                //    direction = LogicalDirection.Forward;
                //}
                //    var i = value < 0 ? 0 - value : value;
                //caretPosition = caretPosition.GetPositionAtOffset(i, direction);

                //    //}
                //    //while (i > 0)
                //    //{
                //    //    caretPosition = caretPosition.GetNextContextPosition(direction);
                //    //    i--;
                //    //}

                //    this.x_RichTextBox.CaretPosition = caretPosition;
            }
        }



        public int SelectionLength
        {
            get { return this.SelectedText.Length; }
            set
            {
                if (value == 0)
                {
                    this.x_RichTextBox.Selection.Select(this.x_RichTextBox.CaretPosition, this.x_RichTextBox.CaretPosition);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public string SelectedText
        {
            get
            {
                TextRange documentRange = new TextRange(this.x_RichTextBox.Selection.Start, this.x_RichTextBox.Selection.End);
                return documentRange.Text;
            }
            set
            {
                if (string.IsNullOrEmpty(this.x_RichTextBox.Selection.Text))
                {
                    this.x_RichTextBox.Selection.Text = value;
                }
                else
                {
                    this.x_RichTextBox.Selection.Text = this.x_RichTextBox.Selection.Text.Replace(this.x_RichTextBox.Selection.Text, value);

                }

            }
        }

    }
}
