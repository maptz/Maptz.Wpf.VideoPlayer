using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.App.SessionState;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Maptz.Subtitler.Wpf.VideoPlayer;
using Maptz.Subtitler.Wpf.VideoPlayer.Projects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Unosquare.FFME;
namespace Maptz.Subtitler.Wpf.VideoPlayer.Commands
{


    public class PlaybackCommands : CommandProviderBase
    {
<<<<<<< HEAD:src/Maptz.Subtitler.Engine/Implementations/CommandProviders/PlaybackCommands.cs
=======
        public IAppCommand OpenVideoFileCommand => new AppCommand("OpenVideoFile", (object o) => this.OpenVideoFile((string)o), new KeyChords(new KeyChord(Key.O, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.file_video));

        private void OpenVideoFile(string videoFilePath)
        {
            var sessionState = this.ServiceProvider?.GetRequiredService<SessionState>();
            if (string.IsNullOrEmpty(videoFilePath))
            {
                OpenFileDialog ofd = new OpenFileDialog();

                if (sessionState != null && !string.IsNullOrEmpty(sessionState.OpenVideoFileDirectoryPath) && Directory.Exists(sessionState.OpenVideoFileDirectoryPath))
                {
                    ofd.InitialDirectory = sessionState.OpenVideoFileDirectoryPath;
                }
                var result = ofd.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    videoFilePath = ofd.FileName;
                }
            }

            var projectSettings = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectSettings>();
            projectSettings.VideoFilePath = videoFilePath;
            if (sessionState != null)
            {
                sessionState.OpenVideoFileDirectoryPath = System.IO.Path.GetDirectoryName(videoFilePath);
            }

        }

>>>>>>> b9ac07991107fb18621da0bac11db92000d39ca9:src/Maptz.Subtitler.Wpf.VideoPlayer/Implementations/CommandProviders/PlaybackCommands.cs
        public override IEnumerable<IAppCommand> GetAllCommands()
        {
            var ret =  base.GetAllCommands();
            return ret;
        }

        /* #region Private Fields */
        private const string SMPTEREGEXSTRING = "(?<intc>\\d{2}[:\\.]\\d{2}[:\\.]\\d{2}([:\\.]\\d{2})*)\\s*(?<outtc>(\\d{2}[:\\.]\\d{2}[:\\.]\\d{2}([:\\.]\\d{2})*)*)"; //\\s*(?<outtc>(\\d{2}[:\\.]\\d{2}[:\\.]\\d{2}([:\\.]\\d{2})*)*
        /* #endregion Private Fields */
        /* #region Private Methods */
        private void SeekToPreviousTimecode()
        {
            var textBox = this.ServiceProvider.GetRequiredService<WrappedTextBox>();
            var tb = textBox.Text.Substring(0, textBox.CaretIndex);
            var lastTimeCodeMatches =Regex.Matches(tb, SMPTEREGEXSTRING);
            if (lastTimeCodeMatches.Count > 0)
            {
                var lastmatch = lastTimeCodeMatches[lastTimeCodeMatches.Count -1];
                var lastIn = lastmatch.Groups["intc"].Value;
<<<<<<< HEAD:src/Maptz.Subtitler.Engine/Implementations/CommandProviders/PlaybackCommands.cs
                var tc = new TimeCode(lastIn, appState.Project.ProjectSettings.FrameRate);
                var offsetMs = 1000.0 * (tc.TotalSeconds - appState.Project.ProjectSettings.OffsetTimeCode.TotalSeconds);
                //appState.Project.ProjectData.CursorMs = (long)offsetMs;
                appState.VideoPlayerState.MediaElement.Seek(TimeSpan.FromMilliseconds(offsetMs));
=======
                lastIn = lastIn.Replace('.', ':');
                try
                {
                    var projectSettings = this.ServiceProvider.GetRequiredService<IProjectSettings>();
                    var tc = new TimeCode(lastIn, projectSettings.FrameRate);
                    var offsetMs = 1000.0 * (tc.TotalSeconds - projectSettings.OffsetTimeCode.TotalSeconds);
                    //appState.Project.ProjectData.CursorMs = (long)offsetMs;
                    var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
                    videoPlayerState.MediaElement.Seek(TimeSpan.FromMilliseconds(offsetMs));
                }
                catch
                {
                    //TODO: Exception has been swallowed
                }
                
>>>>>>> b9ac07991107fb18621da0bac11db92000d39ca9:src/Maptz.Subtitler.Wpf.VideoPlayer/Implementations/CommandProviders/PlaybackCommands.cs
            }
        }
        private bool ValidatePosition(MediaElement me, TimeSpan newPosition)
        {
            if (me.NaturalDuration.HasValue)
            {
                return newPosition < me.NaturalDuration.Value;
            }
            return false;
        }
        /* #endregion Private Methods */
        /* #region Public Properties */
        public IAppCommand SpeedUpPlaybackCommand => new AppCommand("SpeedUpPlayback", (object o) => this.Retime(increase: true), new KeyChords(new KeyChord(Key.OemPlus, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.skip_previous));

        private void Retime(bool increase)
        {
            var multiple = increase ? 2.0 : 0.5;
            
            
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            var rat = videoPlayerState.SpeedRatio;
            if (rat > 1.0)
            {

            }
            else
            {

            }

            var newrat = rat* multiple;

            var maxRat = 4.0;
            var minRat = 0.5;
            newrat = newrat > maxRat ? maxRat : newrat;
            newrat = newrat < minRat ? minRat : newrat;

            videoPlayerState.SpeedRatio = newrat;
        }

        public IAppCommand SlowDownPlaybackCommand => new AppCommand("SlowDownPlayback", (object o) => this.Retime(increase: false), new KeyChords(new KeyChord(Key.OemMinus, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.skip_previous));

        public IAppCommand FrameBackwardCommand => new AppCommand("FrameBackwardVideo", (object o) => this.SkipFrames(-1), new KeyChords(new KeyChord(Key.OemComma, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.skip_previous));
        public IAppCommand FrameForwardCommand => new AppCommand("FrameForwardVideo", (object o) => this.SkipFrames(1), new KeyChords(new KeyChord(Key.OemPeriod, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.skip_next));
        public IAppCommand PauseCommand => new AppCommand("PauseVideo", (object o) => this.Pause(),  null,new XamlIconSource(IconPaths3.play_pause));
        public IAppCommand PlayCommand => new AppCommand("PlayVideo", (object o) => this.Play(), null,  new XamlIconSource(IconPaths3.play_pause));
        public IAppCommand SeekToPreviousTimecodeCommand => new AppCommand(nameof(SeekToPreviousTimecodeCommand), (object o) => this.SeekToPreviousTimecode(), new KeyChords(new KeyChord(Key.Q, ctrl: true, shift: false)), new XamlIconSource(IconPaths3.play_pause));
        public IAppCommand SeekToStartCommand => new AppCommand("SeekStartVideo", (object o) => this.Seek(0.0), new KeyChords(new KeyChord(Key.Home, ctrl: true, alt: true)), new XamlIconSource(IconPaths3.skip_backward));
        public IServiceProvider ServiceProvider { get; }
        public IAppCommand SkipBackwardCommand => new AppCommand("SkipBackwardVideo", (object o) => this.Skip(-2.0), new KeyChords(new KeyChord(Key.J, ctrl: true, shift: false)), new XamlIconSource(IconPaths3.skip_previous));
        public IAppCommand SkipFastBackwardVideoCommand => new AppCommand("SkipFastBackwardVideo", (object o) => this.Skip(-5.0), new KeyChords(new KeyChord(Key.J, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.skip_backward));
        public IAppCommand SkipFastForwardVideoCommand => new AppCommand("SkipFastForwardVideo", (object o) => this.Skip(-5.0), new KeyChords(new KeyChord(Key.J, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.skip_forward));
        public IAppCommand SkipForwardCommand => new AppCommand("SkipForwardVideo", (object o) => this.Skip(2.0), new KeyChords(new KeyChord(Key.L, ctrl: true, shift: false)), new XamlIconSource(IconPaths3.skip_next));
        public IAppCommand TogglePlayStateCommand => new AppCommand("ToggleVideoPlayState", (object o) => this.TogglePlayState(), new KeyChords(new KeyChord(Key.K, ctrl: true, shift: false)), new XamlIconSource(IconPaths3.play_pause));
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public PlaybackCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        /* #endregion Public Constructors */
        /* #region Public Methods */
        public void Pause()
        {
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            videoPlayerState.MediaElement.Pause();
        }
        public void Play()
        {
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            videoPlayerState.MediaElement.Play();
        }
        public void Seek(double seconds)
        {
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            var me = videoPlayerState.MediaElement;

            //TODO validate seek is in range
            var newPosition = TimeSpan.FromSeconds(seconds);
            if (this.ValidatePosition(me, newPosition))
            {
                me.Seek(newPosition);
            }
        }
        public void Skip(double seconds)
        {
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            var me = videoPlayerState.MediaElement;

            //TODO validate seek is in range
            var newPosition = me.Position.Add(TimeSpan.FromSeconds(seconds));
            if (this.ValidatePosition(me, newPosition))
            {
                me.Seek(newPosition);
            }
        }
        public void SkipFrames(long frames)
        {
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            var projectSettings = this.ServiceProvider.GetRequiredService<IProjectSettings>();
            var me = videoPlayerState.MediaElement;

            var fr = projectSettings.FrameRate;
            var currentTC = TimeCode.FromSeconds(me.Position.TotalSeconds, fr);
            var newTC = currentTC.Add(TimeCode.FromFrames(frames, fr));

            var newPosition = me.Position.Add(TimeSpan.FromSeconds(newTC.TotalSeconds));
            if (this.ValidatePosition(me, newPosition))
            {
                me.Seek(newPosition);
            }

        }
        public void TogglePlayState()
        {
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            var me = videoPlayerState.MediaElement;
            if (me.IsPaused)
            {
                me.Play();
            }
            else
            {
                me.Pause();
            }

        }
        /* #endregion Public Methods */
    }
}