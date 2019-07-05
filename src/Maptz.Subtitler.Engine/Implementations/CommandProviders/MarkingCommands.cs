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
using System.Windows.Media;
using Unosquare.FFME;
namespace Maptz.QuickVideoPlayer.Commands
{


    public class MarkingCommands : CommandProviderBase
    {
        /* #region Private Methods */
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
        /* #endregion Private Methods */
        /* #region Public Properties */
        public IAppCommand ClearMarkInMsCommand => new AppCommand("ClearMarkInMs", (object o) => this.ClearMarkInMs(), new KeyChords(new KeyChord(Key.D, ctrl: true)), new XamlIconSource(IconPaths3.flag_remove));
        public IServiceProvider ServiceProvider { get; }
        public IAppCommand SetMarkInCommand => new AppCommand("SetMarkInMs", (object o) => this.SetMarkInMs(), new KeyChords(new KeyChord(Key.I, ctrl: true)), new XamlIconSource(IconPaths3.flag_plus));
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public MarkingCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        /* #endregion Public Constructors */
    }
}