
using Maptz.QuickVideoPlayer;
using Maptz.QuickVideoPlayer.Commands;
using Maptz.QuickVideoPlayer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unosquare.FFME;
namespace Maptz.QuickVideoPlayer.Commands
{


    public class CommandProviderBase : ICommandProvider
    {
        public IEnumerable<IAppCommand> GetAllCommands()
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