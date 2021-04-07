using System;
using System.Collections.Generic;
using System.Linq;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{

    public class ComplexLineSplitter : ILineSplitter
    {
        public IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').ToList();
            var linesPass1 = new List<List<string>>();
            var currentLine = new List<string>();
            linesPass1.Add(currentLine);

            for (int i = 0; i < words.Count; i++)
            {
                if (i== words.Count - 1)
                {

                }
                var word = words[i];
                var currentLineLength = currentLine.Sum(p => p.Length);
                if (currentLineLength == 0 || currentLineLength + word.Length < maximumLineLength)
                {
                    currentLine.Add(word);
                }
                else
                {
                    currentLine = new List<string>();
                    linesPass1.Add(currentLine);
                    currentLine.Add(word);
                }

            }

            var lineCount = linesPass1.Count;
            if (lineCount == 1)
            {
                var retval = linesPass1.Select(p => string.Join(" ", p)).ToArray();
                return retval;
            }
            else 
            {
                var averageLineLength = linesPass1.Average(p => p.Sum(q => q.Length));
                var retval = SimpleSplitToLines(stringToSplit, (int)Math.Floor(averageLineLength) + 10);
                return retval;
            }
        }

        public IEnumerable<string> SimpleSplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').Concat(new[] { "" });
            return
                words
                    .Skip(1)
                    .Aggregate(
                        words.Take(1).ToList(),
                        (a, w) =>
                        {
                            var last = a.Last();
                            while (last.Length > maximumLineLength)
                            {
                                a[a.Count() - 1] = last.Substring(0, maximumLineLength);
                                last = last.Substring(maximumLineLength);
                                a.Add(last);
                            }
                            var test = last + " " + w;
                            if (test.Length > maximumLineLength)
                            {
                                a.Add(w);
                            }
                            else
                            {
                                a[a.Count() - 1] = test;
                            }
                            return a;
                        });
        }
    }

    public class SimpleLineSplitter : ILineSplitter
    {
        public IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').Concat(new[] { "" });
            return
                words
                    .Skip(1)
                    .Aggregate(
                        words.Take(1).ToList(),
                        (a, w) =>
                        {
                            var last = a.Last();
                            while (last.Length > maximumLineLength)
                            {
                                a[a.Count() - 1] = last.Substring(0, maximumLineLength);
                                last = last.Substring(maximumLineLength);
                                a.Add(last);
                            }
                            var test = last + " " + w;
                            if (test.Length > maximumLineLength)
                            {
                                a.Add(w);
                            }
                            else
                            {
                                a[a.Count() - 1] = test;
                            }
                            return a;
                        });
        }
    }
}