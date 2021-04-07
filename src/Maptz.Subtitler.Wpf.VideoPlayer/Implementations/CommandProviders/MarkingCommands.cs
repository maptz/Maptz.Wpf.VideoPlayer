using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Maptz.Subtitler.Wpf.VideoPlayer;
using Maptz.Subtitler.Wpf.VideoPlayer.Projects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.VideoPlayer.Commands
{
    public class MarkingCommands : CommandProviderBase
    {
        /* #region Private Methods */
        private void ClearMarkInMs()
        {
            var projectData = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectData>();
            projectData.MarkInMs = null;
        }
        private void SetMarkInMs(long? ms = null)
        {

            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            var projectData = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectData>();
            if (ms == null)
            {
                var markIn = (long)videoPlayerState.MediaElement.Position.TotalMilliseconds;
                projectData.MarkInMs = markIn;
            }
            else
            {
                projectData.MarkInMs = ms.Value;
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