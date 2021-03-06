using Maptz.Subtitler.App.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{

    public class DefaultCommands
    {
        public DefaultCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        private static IEnumerable<Type> CommandProviders
        {
            get
            {
                return new Type[]
            {
                typeof(AppCommands),
                typeof(ProjectCommands),
                typeof(TextManipulationCommands),
                typeof(SubtitleManipulationCommands),
            };
            }
        }


        public static void AddCommandProviders(IServiceCollection services)
        {
            services.AddSingleton<DefaultCommands>();
            foreach (var t in CommandProviders)
            {
                services.AddSingleton(t);
            }
        }

        public IServiceProvider ServiceProvider { get; }

        public IEnumerable<IAppCommand> GetDefaultCommands()
        {
            var retval = new List<IAppCommand>();
            foreach(var t in CommandProviders)
            {
                var commandProvider =(ICommandProvider)this.ServiceProvider.GetRequiredService(t);
                retval.AddRange(commandProvider.GetAllCommands());
            }

            return retval;
        }
    }
}