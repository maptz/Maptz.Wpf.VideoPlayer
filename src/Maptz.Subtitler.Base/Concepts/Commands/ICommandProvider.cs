using System.Collections.Generic;

namespace Maptz.QuickVideoPlayer.Commands
{
    public interface ICommandProvider
    {
        IEnumerable<IAppCommand> GetAllCommands();
    }
}