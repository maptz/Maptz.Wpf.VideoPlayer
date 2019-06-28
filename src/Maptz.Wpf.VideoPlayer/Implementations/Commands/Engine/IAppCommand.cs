using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Maptz.QuickVideoPlayer.Commands
{
    public interface IAppCommand : ICommand
    {
        IIconSource IconSource { get; }
        string Name { get; }
        KeyChords Shortcut { get; }
    }
}