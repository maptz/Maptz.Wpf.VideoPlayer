using System.Windows.Input;

namespace Maptz.QuickVideoPlayer.Commands
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