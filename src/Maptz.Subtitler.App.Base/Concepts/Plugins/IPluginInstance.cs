using Maptz.Subtitler.App.Commands;
using System.Collections.Generic;

namespace Maptz.Subtitler.App.Plugins
{
    public interface IPluginInstance
    {
        IEnumerable<IAppCommand> Commands
        {
            get;
        }
    }
}