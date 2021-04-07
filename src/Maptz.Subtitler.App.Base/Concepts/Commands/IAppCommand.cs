using System.Windows.Input;

namespace Maptz.Subtitler.App.Commands
{
    public interface IAppCommand : ICommand
    {
        IIconSource IconSource
        {
            get;
        }

        string Name
        {
            get;
        }

        KeyChords Shortcut
        {
            get;
        }
    }
}