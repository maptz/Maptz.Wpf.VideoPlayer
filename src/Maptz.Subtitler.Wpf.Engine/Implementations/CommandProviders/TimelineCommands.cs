using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;

namespace Maptz.Subtitler.Wpf.Engine.Commands
{
    public class TimelineCommands : CommandProviderBase
    {
        /* #region Public Properties */
        public IAppCommand CentreTimelineCommand => new AppCommand("CentreTimeline", (object o) => this.Zoom(1.0), new KeyChords(new KeyChord(Key.OemPlus, ctrl: true, shift: false)), new XamlIconSource(IconPaths3.image_filter_center_focus));
        public IServiceProvider ServiceProvider
        {
            get;
        }

        public IAppCommand ZoomInTimelineCommand => new AppCommand("ZoomInTimeline", (object o) => this.Zoom(0.5), new KeyChords(new KeyChord(Key.OemPlus, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.magnify_plus_outline));
        public IAppCommand ZoomOutTimelineCommand => new AppCommand("ZoomOutTimeline", (object o) => this.Zoom(2.0), new KeyChords(new KeyChord(Key.OemMinus, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.magnify_minus_outline));
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public TimelineCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /* #endregion Public Constructors */
        /* #region Public Methods */
        public void Zoom(double factor, double? centerMs = null)
        {
            var projectData = this.ServiceProvider.GetRequiredService<ProjectDataEx>();
            var ems = (double)projectData.ViewMs.EndMs;
            var sms = (double)projectData.ViewMs.StartMs;
            var maxSms = (double)projectData.ViewMs.MaxStartMs;
            var maxEms = (double)projectData.ViewMs.MaxEndMs;
            var wid = (ems - sms);
            var halfWid = wid / 2.0;
            double cms = sms + wid / 2.0;
            if (centerMs.HasValue)
            {
                cms = centerMs.Value;
            }
            else if (projectData.CursorMs.HasValue)
            {
                cms = projectData.CursorMs.Value;
            }

            var newStartMs = (long)(cms - halfWid * factor);
            var newEndMs = (long)(cms + halfWid * factor);
            ;
            var newRangeMs = newEndMs - newStartMs;
            var maxRangeMs = maxEms - maxSms;
            if (newRangeMs >= maxRangeMs)
            {
                newStartMs = (long)maxSms;
                newEndMs = (long)maxEms;
            }
            else
            {
                //We know our range is less than. 
                
                if (newStartMs < maxSms)
                {
                    var delta = (long)maxSms - newStartMs;
                    newStartMs = (long)maxSms;
                    newEndMs += delta;
                }

                if (newEndMs > maxEms)
                {
                    var delta = (long)maxEms - newEndMs;
                    newStartMs += delta;
                    newEndMs += delta;
                }
            }

            projectData.ViewMs.StartMs = newStartMs;
            projectData.ViewMs.EndMs = newEndMs;
        }
    /* #endregion Public Methods */
    }
}