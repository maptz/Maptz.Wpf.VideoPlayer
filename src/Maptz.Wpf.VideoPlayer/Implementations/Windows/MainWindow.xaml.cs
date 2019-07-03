using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Editing.TimeCodeDocuments;
using Maptz.QuickVideoPlayer.Commands;
using Maptz.Tiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unosquare.FFME;
using Unosquare.FFME.Common;

namespace Maptz.QuickVideoPlayer
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* #region Private Fields */
        private List<BindingWatcherBase> _bindingWatchers;
        /* #endregion Private Fields */
        /* #region Private Methods */
        private void InitializeMenu()
        {
            var appCommands = this.ServiceProvider.GetRequiredService<AppCommands>();
            var projectCommands = this.ServiceProvider.GetRequiredService<ProjectCommands>();

            var menu = this.x_Menu;
            var fm = new MenuItem() { Header = "_File" };
            menu.Items.Add(fm);

            var comm = projectCommands.NewProjectCommand; ;
            fm.Items.Add(new MenuItem() { Header = "_New Project", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectCommands.OpenProjectCommand;
            fm.Items.Add(new MenuItem() { Header = "_Open Project", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectCommands.SaveProjectCommand;
            fm.Items.Add(new MenuItem() { Header = "_Save Project", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectCommands.SaveProjectAsCommand;
            fm.Items.Add(new MenuItem() { Header = "Save Project As", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectCommands.ShowProjectSettingsCommand;
            fm.Items.Add(new MenuItem() { Header = "Project Settings", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = appCommands.ExitAppCommand;
            fm.Items.Add(new MenuItem() { Header = "_Exit", Command = comm, Icon = comm.IconSource?.GetIconElement() });
        }
        private void InvalidateCommandMenus()
        {
            var playbackCommands = this.ServiceProvider.GetRequiredService<PlaybackCommands>();
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipFastBackwardVideoCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipBackwardCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.TogglePlayStateCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipForwardCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipFastForwardVideoCommand));

            //
            var timelineCommands = this.ServiceProvider.GetRequiredService<TimelineCommands>();
            this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(timelineCommands.CentreTimelineCommand));
            this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(timelineCommands.ZoomInTimelineCommand));
            this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(timelineCommands.ZoomOutTimelineCommand));
            this.x_StackPanel_TimelineCommands.Children.Add(new Canvas { Width = 100, Height = 1 });
            var markingCommands = this.ServiceProvider.GetRequiredService<MarkingCommands>();
            this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(markingCommands.ClearMarkInMsCommand));
            this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(markingCommands.SetMarkInCommand));

            //
            var textManipCommands = this.ServiceProvider.GetRequiredService<TextManipulationCommands>();
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.InsertTimeCodeFromCursorCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.InsertTimeCodeFromMarkInCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.PreviousGrammarPointCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.NextGrammarPointCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.SplitSentencesCommand));

        }
        private void InvalidateSubtitles()
        {
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            var doc = this.SubtitleProvider.GetSubtitles(appState.Project.ProjectData.Text);
            this.SubtitleItems = doc.Items;
            this.x_SubtitlesControl.SubtitleItems = doc.Items;

            this.UpdateTimeCode();
        }
        private void Media_MediaOpened(object sender, MediaOpenedEventArgs e)
        {
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            appState.Project.ProjectData.CursorMs = (long)this.Media.Position.TotalMilliseconds;
            appState.Project.ProjectData.ViewMs.MaxStartMs = 0;
            appState.Project.ProjectData.ViewMs.MaxEndMs = (long)this.Media.NaturalDuration.Value.TotalMilliseconds;
            appState.Project.ProjectData.ViewMs.StartMs = 0;
            appState.Project.ProjectData.ViewMs.EndMs = (long)this.Media.NaturalDuration.Value.TotalMilliseconds;
        }
        private void OnProjectTextChanged(object oldValue, object newValue)
        {
            
        }
        private void SetMediaSource(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                this.Media.Source = new Uri(filePath);
            }
            else
            {
                this.Media.Source = null;
            }
        }
        private void SetTitle()
        {
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            var projectIsDirty = appState.Project.IsDirty;
            var title = "Video player";
            title += projectIsDirty ? " - [Unsaved]" : string.Empty;
            this.Title = title;
        }
        private void UpdateTimeCode()
        {
            var appState = this.ServiceProvider.GetRequiredService<AppState>();
            var currentTCStr = "UNKNOWN";
            var currentSubStr = string.Empty;
            if (appState.Project.ProjectData.CursorMs.HasValue)
            {
                var fr = appState.Project.ProjectSettings.FrameRate;
                var offsetTC = TimeCode.FromFrames(appState.Project.ProjectSettings.OffsetFrames, fr);
                var cursorTC = offsetTC.Add(TimeCode.FromSeconds((double)appState.Project.ProjectData.CursorMs.Value / 1000.0, fr));
                currentTCStr = cursorTC.ToString();

                if (this.SubtitleItems != null && this.SubtitleItems.Any())
                {
                    var currentSub = this.SubtitleItems.FirstOrDefault(p => cursorTC.TotalFrames >= p.RecordIn.TotalFrames && cursorTC.TotalFrames <= p.RecordOut.TotalFrames);
                    if (currentSub != null)
                    {
                        currentSubStr = currentSub.Content;
                    }
                }
            }

            this.x_TextBlock_TimeCode.Text = currentTCStr;
            this.x_TextBlock_CurrentSubtitle.Text = currentSubStr;

        }
        private void X_CursorControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this.x_CursorControl);
            var widthMs = (double)this.x_CursorControl.EndMs - (double)this.x_CursorControl.StartMs;
            var newStartMs = (double)this.x_CursorControl.StartMs + widthMs * pos.X / this.x_CursorControl.ActualWidth;
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                var appState = this.ServiceProvider.GetRequiredService<AppState>();
                appState.Project.ProjectData.MarkInMs = (long)newStartMs;
            }
            else
            {
                this.Media.Seek(TimeSpan.FromMilliseconds(newStartMs));
                if (Media.IsPaused)
                {
                    Media.Play();
                }
            }


        }
        private void X_CursorControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var timelineCommands = this.ServiceProvider.GetRequiredService<TimelineCommands>();
            var pos = e.GetPosition(this.x_CursorControl);
            var widthMs = (double)this.x_CursorControl.EndMs - (double)this.x_CursorControl.StartMs;
            if (e.Delta < 0)
            {
                timelineCommands.Zoom(2.0);
            }
            else
            {
                timelineCommands.Zoom(0.5);
            }

        }
        /* #endregion Private Methods */
        /* #region Public Properties */
        public IServiceProvider ServiceProvider { get; }
        public IEnumerable<ITimeCodeDocumentItem<string>> SubtitleItems { get; private set; }
        public ISubtitleProvider SubtitleProvider { get; }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public MainWindow()
        {

        }
        public MainWindow(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            var appState = this.ServiceProvider.GetRequiredService<AppState>();

            InitializeComponent();
            this.x_Grid.DataContext = appState;
            this.InitializeMenu();

            this.SubtitleProvider = this.ServiceProvider.GetRequiredService<ISubtitleProvider>();

            this._bindingWatchers = new List<BindingWatcherBase>();
            {
                var bw = new BindingWatcher<string>(appState, "Project.ProjectData.Text");
                bw.BindingChanged += (s, e) =>
                {
                    OnProjectTextChanged(e.OldValue, e.NewValue);
                    InvalidateSubtitles();
                };
                InvalidateSubtitles();
                this._bindingWatchers.Add(bw);
            }
            
            {
                var bw = new BindingWatcher<string>(appState, "Project.ProjectSettings.VideoFilePath");
                bw.BindingChanged += (s, e) =>
                {
                    SetMediaSource((string)e.NewValue);
                };
                SetMediaSource(bw.BoundProperty);
                this._bindingWatchers.Add(bw);
            }
            {
                var bw = new BindingWatcher<SmpteFrameRate>(appState, "Project.ProjectSettings.FrameRate");
                bw.BindingChanged += (s, e) =>
                {
                    this.UpdateTimeCode();
                };
                this._bindingWatchers.Add(bw);
            }
            {
                var bw = new BindingWatcher<long>(appState, "Project.ProjectSettings.OffsetFrames");
                bw.BindingChanged += (s, e) =>
                {
                    this.UpdateTimeCode();
                };
                this._bindingWatchers.Add(bw);
            }
            {
                var bw = new BindingWatcher<long?>(appState, "Project.ProjectData.CursorMs");
                bw.BindingChanged += (s, e) =>
                {
                    this.UpdateTimeCode();
                };
                this.UpdateTimeCode();
                this._bindingWatchers.Add(bw);
            }
            {
                var bw = new BindingWatcher<long?>(appState, "Project.IsDirty");

                bw.BindingChanged += (s, e) =>
                {
                    this.SetTitle();
                };
                this.SetTitle();
                this._bindingWatchers.Add(bw);
            }


            /* #region Wire up media player */
            //this.Media.Source = new Uri(ViewModel.FilePath);

            // Global FFmpeg message handler
            Unosquare.FFME.MediaElement.FFmpegMessageLogged += OnMediaFFmpegMessageLogged;
            Media.MessageLogged += OnMediaMessageLogged;
            Media.MediaFailed += OnMediaFailed;
            Media.MediaOpened += this.Media_MediaOpened;
            Media.SourceUpdated += (s, e) =>
            {

            };
            Media.PositionChanged += (s, e) =>
            {
                appState.Project.ProjectData.CursorMs = (long)this.Media.Position.TotalMilliseconds;

            };
            Media.SpeedRatio = 1.0;
            

            this.x_CursorControl.MouseLeftButtonDown += this.X_CursorControl_MouseLeftButtonDown;
            this.x_CursorControl.MouseWheel += this.X_CursorControl_MouseWheel;
            /* #endregion*/


            this.InvalidateCommandMenus();
        }
        /* #endregion Public Constructors */
    }



}
