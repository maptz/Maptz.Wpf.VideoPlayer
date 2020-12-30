using Maptz.Subtitler.App.Commands;
using System.Collections.Generic;
namespace Maptz.Subtitler.App.Commands
{

    /// <summary>
    /// A class that exposes commands as properties. 
    /// </summary>
    public class CommandProviderBase : ICommandProvider
    {
        public virtual IEnumerable<IAppCommand> GetAllCommands()
        {
            List<IAppCommand> retval = new List<IAppCommand>();
            var props = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var prop in props)
            {
                if (typeof(IAppCommand).IsAssignableFrom(prop.PropertyType))
                {
                    var iappCommand = (IAppCommand)prop.GetValue(this);
                    retval.Add(iappCommand);
                }
            }
            return retval;
        }
    }
}