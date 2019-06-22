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

    public class MarkingCommands : CommandProviderBase
    {
        public MarkingCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }


        public IAppCommand ClearMarkInMsCommand => new AppCommand("ClearMarkInMs", (object o) => this.ClearMarkInMs(), new KeyChords(new KeyChord(Key.D, ctrl: true)));
        public IAppCommand SetMarkInCommand => new AppCommand("SetMarkInMs", (object o) => this.SetMarkInMs(), new KeyChords(new KeyChord(Key.I, ctrl: true)));


        private void ClearMarkInMs()
        {
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            appState.Project.ProjectData.MarkInMs = null;
        }


        private void SetMarkInMs(long? ms = null)
        {
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            if (ms == null)
            {
                var markIn = (long)appState.VideoPlayerState.MediaElement.Position.TotalMilliseconds;
                appState.Project.ProjectData.MarkInMs = markIn;
            }
            else
            {
                appState.Project.ProjectData.MarkInMs = ms.Value;
            }
        }
    }
}