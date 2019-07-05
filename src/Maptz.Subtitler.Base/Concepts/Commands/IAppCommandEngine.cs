using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Maptz.QuickVideoPlayer.Commands
{
    public interface IAppCommandEngine
    {
        void AddCommand(IAppCommand appCommand);
        void ExecuteNamedCommand(string commandName, object parameter = null);
        void RegisterKeyEvent(KeyEventArgs ev);
        ObservableCollection<IAppCommand> AppCommands { get; }
    }
}