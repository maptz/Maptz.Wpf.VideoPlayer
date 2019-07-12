using System.Collections.Generic;
using Maptz.QuickVideoPlayer.Commands;

namespace Maptz.QuickVideoPlayer
{
    public interface IPluginInstance
    {
        IEnumerable<IAppCommand> Commands
        {
            get;
        }
    }
}