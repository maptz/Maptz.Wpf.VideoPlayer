using Maptz.Subtitler.App.Commands;
using System;
using System.Diagnostics;

namespace Maptz.Subtitler.App.Commands
{
    [DebuggerDisplay("{Name} {Shortcut}")]
    public class AppCommand : IAppCommand
    {
        public KeyChords Shortcut
        {
            get;
            set;
        }

        public IIconSource IconSource
        {
            get;
        }

        public string Name
        {
            get;
        }

        public Action<object> ExecuteAction
        {
            get;
        }

        public AppCommand(string name, Action<object> executeAction, KeyChords shortcut = null, IIconSource iconSource = null)
        {
            this.Name = name;
            this.ExecuteAction = executeAction;
            this.Shortcut = shortcut;
            this.IconSource = iconSource;
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