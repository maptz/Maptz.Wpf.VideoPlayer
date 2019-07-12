using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Diagnostics;
using Maptz.Editing.TimeCodeDocuments;
using ICSharpCode.AvalonEdit;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using System.Linq;
namespace Maptz.QuickVideoPlayer
{
    public class CustomHighlightingDefinition : IHighlightingDefinition
    {
        private const string SMPTEREGEXSTRING = "(?<Hours>\\d{2})[:\\.](?<Minutes>\\d{2})[:\\.](?<Seconds>\\d{2})(?::|;|\\.)(?<Frames>\\d{2})";

        public string Name => "CustomHighligting";

        public HighlightingRuleSet MainRuleSet
        {
            get
            {
                var ret = new HighlightingRuleSet()
                {
                    Name = "main"

                };
                //ret.Spans.Add(new HighlightingSpan()
                //{
                //    SpanColor = this.GetNamedColor(""),
                //    StartExpression = new System.Text.RegularExpressions.Regex(SMPTEREGEXSTRING),
                //    EndExpression = new System.Text.RegularExpressions.Regex("[ ]*$"),
                //    SpanColorIncludesStart = true
                //});
                ret.Rules.Add(new HighlightingRule()
                {
                    Color = this.GetNamedColor(string.Empty),
                    Regex = new System.Text.RegularExpressions.Regex(SMPTEREGEXSTRING)
                }); ;
                return ret;
            }
        }


        public IEnumerable<HighlightingColor> NamedHighlightingColors => new HighlightingColor[] {
            new HighlightingColor() { Foreground = new SimpleHighlightingBrush(Colors.Red) }
        };

        public IDictionary<string, string> Properties => throw new NotImplementedException();

        public HighlightingColor GetNamedColor(string name)
        {
            return this.NamedHighlightingColors.First();
        }

        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            return this.MainRuleSet;

        }
    }
}