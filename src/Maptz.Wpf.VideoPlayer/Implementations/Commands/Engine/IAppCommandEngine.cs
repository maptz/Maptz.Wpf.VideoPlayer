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

    public interface IAppCommandEngine
    {
        void AddCommand(IAppCommand appCommand);
        void ExecuteNamedCommand(string commandName, object parameter = null);
        void RegisterKeyEvent(KeyEventArgs ev);
    }
}