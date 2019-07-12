using Maptz.Editing.TimeCodeDocuments;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;

namespace Maptz.QuickVideoPlayer
{
    /// <summary>
    /// Finds the word 'AvalonEdit' and makes it bold and italic.
    /// </summary>
    public class HighlightTextspanColorizer : DocumentColorizingTransformer
    {
        private ITextSpan TextSpan;
        public HighlightTextspanColorizer(ITextSpan textSpan, string kind)
        {
            this.TextSpan = textSpan;
            Kind = kind;
        }

        public string Kind { get; }

        protected override void ColorizeLine(DocumentLine line)
        {
            int absLineStartOffset = line.Offset;
            int absLineEndOffset = line.Offset + line.Length;
            var absTextSpanStart = this.TextSpan.Start;
            var absTextSpanEnd = this.TextSpan.Start + this.TextSpan.Length;
            if (absLineStartOffset > absTextSpanEnd || absLineEndOffset < absTextSpanStart)
                return;
            string text = CurrentContext.Document.GetText(line);
            var lineRelativeStartOffset = this.TextSpan.Start - absLineStartOffset;
            lineRelativeStartOffset = lineRelativeStartOffset < 0 ? 0 : lineRelativeStartOffset;
            var lineRelativeEndOffset = absTextSpanEnd - absLineStartOffset;
            lineRelativeEndOffset = lineRelativeEndOffset > line.Length ? line.Length : lineRelativeEndOffset;
            base.ChangeLinePart(line.Offset + lineRelativeStartOffset, line.Offset + lineRelativeEndOffset, (VisualLineElement element) =>
            {
                // This lambda gets called once for every VisualLineElement
                // between the specified offsets.
                Typeface tf = element.TextRunProperties.Typeface;
                // Replace the typeface with a modified version of
                // the same typeface
                element.TextRunProperties.SetBackgroundBrush(new SolidColorBrush(Color.FromArgb(255, 200, 200, 0)));
            }

            );
        }
    }
}