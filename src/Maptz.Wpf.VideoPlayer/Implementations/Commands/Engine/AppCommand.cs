using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Maptz.QuickVideoPlayer.Commands
{




    [DebuggerDisplay("{Name} {Shortcut}")]
    public class AppCommand : IAppCommand
    {
        public KeyChords Shortcut { get; set; }
        public string Name { get; }
        public Action<object> ExecuteAction { get; }

        public AppCommand(string name, Action<object> executeAction, KeyChords shortcut = null)
        {
            this.Name = name;
            this.ExecuteAction = executeAction;
            this.Shortcut = shortcut;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (this.ExecuteAction != null)
            {
                this.ExecuteAction(parameter);
            }
        }
    }
}