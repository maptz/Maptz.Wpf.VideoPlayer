using System.Collections.Generic;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{
    public interface ILineSplitter
    {
        IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength);
    }
}