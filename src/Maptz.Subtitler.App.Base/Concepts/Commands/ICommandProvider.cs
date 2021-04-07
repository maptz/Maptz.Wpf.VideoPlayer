using System.Collections.Generic;

namespace Maptz.Subtitler.App.Commands
{
    public interface ICommandProvider
    {
        //Should be GetCommands. 
        IEnumerable<IAppCommand> GetAllCommands();
    }
}