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
    public class TimelineCommands : CommandProviderBase
    {

        public void Zoom(double factor)
        {
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            var ems = (double)appState.Project.ProjectData.ViewMs.EndMs;
            var sms = (double)appState.Project.ProjectData.ViewMs.StartMs;
            var wid = (ems - sms);
            var halfWid = wid / 2.0;
            double cms = sms + wid / 2.0;
            bool centerCursor = true;
            if (centerCursor && appState.Project.ProjectData.CursorMs.HasValue)
            {
                cms = appState.Project.ProjectData.CursorMs.Value;
            }

            var newStartMs = (long)(cms - halfWid * factor);
            var newEndMs = (long)(cms + halfWid * factor); ;
            var delta = 0L;
            if (newStartMs < 0)
            {
                delta = 0 - newStartMs;
                newStartMs = 0;
                newEndMs += delta;
            }

            appState.Project.ProjectData.ViewMs.StartMs = newStartMs;
            appState.Project.ProjectData.ViewMs.EndMs = newEndMs;
        }

        public TimelineCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        
        public IAppCommand CentreTimelineCommand =>new AppCommand("CentreTimeline", (object o) => this.Zoom(1.0), new KeyChords(new KeyChord(Key.OemPlus, ctrl: true, shift: false)));
        public IAppCommand ZoomOutTimelineCommand => new AppCommand("ZoomOutTimeline", (object o) => this.Zoom(2.0), new KeyChords(new KeyChord(Key.OemMinus, ctrl: true, shift: true)));
        public IAppCommand ZoomInTimelineCommand => new AppCommand("ZoomInTimeline", (object o) => this.Zoom(0.5), new KeyChords(new KeyChord(Key.OemPlus, ctrl: true, shift: true)));

    }
}