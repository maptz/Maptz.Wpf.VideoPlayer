using Maptz.QuickVideoPlayer;
using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.App;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{
    public interface ILineSplitter
    {
        IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength);
    }
}