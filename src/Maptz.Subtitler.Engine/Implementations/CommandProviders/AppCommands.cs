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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unosquare.FFME;
namespace Maptz.QuickVideoPlayer.Commands
{


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
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            System.Windows.Application.Current.Shutdown();
        }
    }
}