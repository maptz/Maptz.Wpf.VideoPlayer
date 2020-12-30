using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.Wpf.Engine.Icons;
using System;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{


    /// <summary>
    /// Global App commands.
    /// </summary>
    public class AppCommands : CommandProviderBase
    {
        public AppCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        

        public IAppCommand ExitAppCommand
        {
            get => new AppCommand("ExitApp", (object o) => this.ExitApp(), new KeyChords(new KeyChord(Key.F4, ctrl: false, alt: true)), new XamlIconSource(IconPaths3.exit_run));
        }

        private void ExitApp()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}