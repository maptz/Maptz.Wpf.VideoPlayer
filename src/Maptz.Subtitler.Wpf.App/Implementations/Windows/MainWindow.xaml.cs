using Maptz.Editing.TimeCodeDocuments;
using Maptz.QuickVideoPlayer.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Unosquare.FFME;
using Unosquare.FFME.Common;

namespace Maptz.QuickVideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<BindingWatcherBase> _bindingWatchers;
        private void InitializeMenu()
        {
            var appCommands = this.ServiceProvider.GetRequiredService<AppCommands>();
            var projectCommands = this.ServiceProvider.GetRequiredService<ProjectCommands>();
            var menu = this.x_Menu;
            var fm = new MenuItem()
            {Header = "_File"};
            menu.Items.Add(fm);
            var comm = projectCommands.NewProjectCommand;
            ;
            fm.Items.Add(new MenuItem()
            {Header = "_New Project", Command = comm, Icon = comm.IconSource?.GetIconElement()});
            comm = projectCommands.OpenProjectCommand;
            fm.Items.Add(new MenuItem()
            {Header = "_Open Project", Command = comm, Icon = comm.IconSource?.GetIconElement()});
            comm = projectCommands.SaveProjectCommand;
            fm.Items.Add(new MenuItem()
            {Header = "_Save Project", Command = comm, Icon = comm.IconSource?.GetIconElement()});
            comm = projectCommands.SaveProjectAsCommand;
            fm.Items.Add(new MenuItem()
            {Header = "Save Project As", Command = comm, Icon = comm.IconSource?.GetIconElement()});
            comm = projectCommands.ShowProjectSettingsCommand;
            fm.Items.Add(new MenuItem()
            {Header = "Project Settings", Command = comm, Icon = comm.IconSource?.GetIconElement()});
            comm = appCommands.ExitAppCommand;
            fm.Items.Add(new MenuItem()
            {Header = "_Exit", Command = comm, Icon = comm.IconSource?.GetIconElement()});
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
            this.x_StackPanel_TimelineCommands.Children.Add(new Canvas{Width = 100, Height = 1});
            var markingCommands = this.ServiceProvider.GetRequiredService<MarkingCommands>();
            this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(markingCommands.ClearMarkInMsCommand));
            this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(markingCommands.SetMarkInCommand));

            var comboBox = new ComboBox
            {
                SelectedValuePath = "Key",
                DisplayMemberPath = "Value"
            };
            x_StackPanel_TimelineCommands.Children.Add(comboBox);
            comboBox.Items.Add(new KeyValuePair<double, string>(0.25, "0.25"));
            comboBox.Items.Add(new KeyValuePair<double, string>(0.5, "0.5"));
            comboBox.Items.Add(new KeyValuePair<double, string>(0.75, "0.75"));
            comboBox.Items.Add(new KeyValuePair<double, string>(1.0, "1.0"));
            comboBox.Items.Add(new KeyValuePair<double, string>(1.2, "1.2"));
            comboBox.Items.Add(new KeyValuePair<double, string>(2.0, "2.0"));
            comboBox.Items.Add(new KeyValuePair<double, string>(4.0, "4.0"));
            var binding = new Binding("VideoPlayerState.SpeedRatio")
            {
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(comboBox, Selector.SelectedValueProperty, binding);

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

        protected virtual void OnProjectTextChanged(object oldValue, object newValue)
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
                var cursorTC = offsetTC.Add(TimeCode.FromSeconds(appState.Project.ProjectData.CursorMs.Value / 1000.0, fr));
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

        private bool IsScrubbing
        {
            get;
            set;
        }

        private void X_CursorControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            //Handles cursor downs on the timeline.
            var pos = e.GetPosition(this.x_CursorControl);
            var widthMs = this.x_CursorControl.EndMs - (double)this.x_CursorControl.StartMs;
            var newStartMs = this.x_CursorControl.StartMs + widthMs * pos.X / this.x_CursorControl.ActualWidth;
            var tc = TimeCode.FromSeconds(newStartMs / 1000.0, SmpteFrameRate.Smpte25);
            Debug.WriteLine("Seeking to: " + tc);
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
                //Media.Play();
                }
            }

            this.IsScrubbing = true;
            UIElement el = (UIElement)sender;
            el.CaptureMouse();
        }

        private void X_CursorControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var timelineCommands = this.ServiceProvider.GetRequiredService<TimelineCommands>();
            if (e.Delta < 0)
            {
                timelineCommands.Zoom(2.0);
            }
            else
            {
                timelineCommands.Zoom(0.5);
            }
        }

        public IServiceProvider ServiceProvider
        {
            get;
        }

        public IEnumerable<ITimeCodeDocumentItem<string>> SubtitleItems
        {
            get;
            private set;
        }

        public ISubtitleProvider SubtitleProvider
        {
            get;
        }

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
            //Allow scrubbing
            this.Media.ScrubbingEnabled = true;
            this._bindingWatchers = new List<BindingWatcherBase>();
            {
                var bw = new BindingWatcher<string>(appState, "Project.ProjectData.Text");
                bw.BindingChanged += (s, e) =>
                {
                    OnProjectTextChanged(e.OldValue, e.NewValue);
                    InvalidateSubtitles();
                }

                ;
                InvalidateSubtitles();
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<string>(appState, "Project.ProjectSettings.VideoFilePath");
                bw.BindingChanged += (s, e) =>
                {
                    SetMediaSource((string)e.NewValue);
                }

                ;
                SetMediaSource(bw.BoundProperty);
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<SmpteFrameRate>(appState, "Project.ProjectSettings.FrameRate");
                bw.BindingChanged += (s, e) =>
                {
                    this.UpdateTimeCode();
                }

                ;
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<long>(appState, "Project.ProjectSettings.OffsetFrames");
                bw.BindingChanged += (s, e) =>
                {
                    this.UpdateTimeCode();
                }

                ;
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<long? >(appState, "Project.ProjectData.CursorMs");
                bw.BindingChanged += (s, e) =>
                {
                    this.UpdateTimeCode();
                }

                ;
                this.UpdateTimeCode();
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<long?>(appState, "VideoPlayerState.SpeedRatio");
                bw.BindingChanged += (s, e) =>
                {
                    this.Media.SpeedRatio = appState.VideoPlayerState != null ? appState.VideoPlayerState.SpeedRatio : 1.0;
                }

                ;
                this.Media.SpeedRatio = appState.VideoPlayerState != null ?  appState.VideoPlayerState.SpeedRatio : 1.0;
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<long? >(appState, "Project.IsDirty");
                bw.BindingChanged += (s, e) =>
                {
                    this.SetTitle();
                }

                ;
                this.SetTitle();
                this._bindingWatchers.Add(bw);

                BindMediaRenderingEvents();
            }

            //this.Media.Source = new Uri(ViewModel.FilePath);
            // Global FFmpeg message handlerx_StackPanel_TimelineCommands
            Unosquare.FFME.MediaElement.FFmpegMessageLogged += OnMediaFFmpegMessageLogged;
            Media.MessageLogged += OnMediaMessageLogged;
            Media.MediaFailed += OnMediaFailed;
            Media.MediaOpened += this.Media_MediaOpened;
            Media.SourceUpdated += (s, e) =>
            {
            }

            ;
            Media.PositionChanged += (s, e) =>
            {
                appState.Project.ProjectData.CursorMs = (long)this.Media.ActualPosition?.TotalMilliseconds;
            }

            ;
            //Media.SpeedRatio = 1.0;
            this.x_CursorControl.MouseLeftButtonDown += this.X_CursorControl_MouseLeftButtonDown;
            this.x_CursorControl.MouseMove += X_CursorControl_MouseMove;
            this.x_CursorControl.MouseLeftButtonUp += X_CursorControl_MouseLeftButtonUp;
            this.x_CursorControl.MouseWheel += this.X_CursorControl_MouseWheel;
            /* #endregion*/
            this.InvalidateCommandMenus();
        }

        private void BindMediaRenderingEvents()
        {


            // Setup GDI+ graphics
            Bitmap overlayBitmap = null;
            Graphics overlayGraphics = null;
            var overlayTextFont = new Font("Courier New", 14, System.Drawing.FontStyle.Bold);
            var overlayTextFontBrush = Brushes.WhiteSmoke;
            var overlayTextOffset = new PointF(12, 8);
            var overlayBackBuffer = IntPtr.Zero;

            var drawVuMeterLeftPen = new Pen(Color.OrangeRed, 12);
            var drawVuMeterRightPen = new Pen(Color.GreenYellow, 12);
            var drawVuMeterClock = TimeSpan.Zero;
            var drawVuMeterLatency = TimeSpan.Zero;
            var drawVuMeterRmsLock = new object();

            var drawVuMeterLeftValue = 0d;
            var drawVuMeterRightValue = 0d;
            double[] drawVuMeterLeftSamples = null;
            double[] drawVuMeterRightSamples = null;

            const float drawVuMeterLeftOffset = 36;
            const float drawVuMeterTopSpacing = 21;
            const float drawVuMeterTopOffset = 101;
            const float drawVuMeterMinWidth = 5;
            const float drawVuMeterScaleFactor = 20; // RMS * pixel factor = the length of the VU meter lines


            Media.RenderingVideo += (s, e) =>
            {
                #region Create the overlay buffer to work with

                if (overlayBackBuffer != e.Bitmap.Scan0)
                {
                    lock (drawVuMeterRmsLock)
                    {
                        drawVuMeterLeftValue = 0;
                        drawVuMeterRightValue = 0;
                    }

                    overlayGraphics?.Dispose();
                    overlayBitmap?.Dispose();

                    overlayBitmap = e.Bitmap.CreateDrawingBitmap();

                    overlayBackBuffer = e.Bitmap.Scan0;
                    overlayGraphics = Graphics.FromImage(overlayBitmap);
                    overlayGraphics.InterpolationMode = InterpolationMode.Default;
                }



                var differenceMillis = 0d;
                var leftChannelWidth = 0f;
                var rightChannelWidth = 0f;
                var audioLatency = 0d;
                if (e.EngineState.HasAudio)
                {
                    lock (drawVuMeterRmsLock)
                    {
                        differenceMillis = Math.Round(TimeSpan.FromTicks(drawVuMeterClock.Ticks - e.StartTime.Ticks).TotalMilliseconds, 0);
                        leftChannelWidth = drawVuMeterMinWidth + (Convert.ToSingle(drawVuMeterLeftValue) * drawVuMeterScaleFactor);
                        rightChannelWidth = drawVuMeterMinWidth + (Convert.ToSingle(drawVuMeterRightValue) * drawVuMeterScaleFactor);
                        audioLatency = drawVuMeterLatency.TotalMilliseconds;
                    }
                }

                overlayGraphics?.DrawString(
                    $"Clock: {e.Clock.TotalSeconds:00.00}\r\n" +
                    $"PN   : {e.PictureNumber}\r\n" +
                    $"A/V  : {Math.Round(differenceMillis, 0):+000;-000;+000}\r\n" +
                    $"A/C  : {Math.Round(audioLatency, 0):+000;-000;+000}\r\n" +
                    "L \r\nR",
                    overlayTextFont,
                    overlayTextFontBrush,
                    overlayTextOffset);

                // draw a simple VU meter
                overlayGraphics?.DrawLine(drawVuMeterLeftPen,
                    drawVuMeterLeftOffset,
                    drawVuMeterTopOffset * overlayGraphics.DpiY / 96f,
                    drawVuMeterLeftOffset + leftChannelWidth,
                    drawVuMeterTopOffset * overlayGraphics.DpiY / 96f);

                overlayGraphics?.DrawLine(drawVuMeterRightPen,
                    drawVuMeterLeftOffset,
                    (drawVuMeterTopOffset + drawVuMeterTopSpacing) * overlayGraphics.DpiY / 96f,
                    drawVuMeterLeftOffset + rightChannelWidth,
                    (drawVuMeterTopOffset + drawVuMeterTopSpacing) * overlayGraphics.DpiY / 96f);

                #endregion
            };

            Media.RenderingAudio += (s, e) =>
            {
                // If we don't have video, we don't need to draw a thing.
                if (e.EngineState.HasVideo == false) return;

                // We need to split the samples into left and right sample channels
                if (drawVuMeterLeftSamples == null || drawVuMeterLeftSamples.Length != e.SamplesPerChannel)
                    drawVuMeterLeftSamples = new double[e.SamplesPerChannel];

                if (drawVuMeterRightSamples == null || drawVuMeterRightSamples.Length != e.SamplesPerChannel)
                    drawVuMeterRightSamples = new double[e.SamplesPerChannel];

                // Iterate through the buffer
                var isLeftSample = true;
                var sampleIndex = 0;
                var bufferData = e.GetBufferData();

                for (var i = 0; i < e.BufferLength; i += e.BitsPerSample / 8)
                {
                    var samplePercent = 100d * bufferData.GetAudioSampleLevel(i);

                    if (isLeftSample)
                        drawVuMeterLeftSamples[sampleIndex] = samplePercent;
                    else
                        drawVuMeterRightSamples[sampleIndex] = samplePercent;

                    sampleIndex += !isLeftSample ? 1 : 0;
                    isLeftSample = !isLeftSample;
                }

                // Compute the RMS of the samples and save it for the given point in time.
                lock (drawVuMeterRmsLock)
                {
                    // The VU meter should show the audio RMS, we compute it and save it in a dictionary.
                    drawVuMeterClock = e.StartTime;
                    drawVuMeterLatency = e.Latency;
                    drawVuMeterLeftValue = Math.Sqrt((1d / drawVuMeterLeftSamples.Length) * drawVuMeterLeftSamples.Sum(n => n));
                    drawVuMeterRightValue = Math.Sqrt((1d / drawVuMeterRightSamples.Length) * drawVuMeterRightSamples.Sum(n => n));
                }
            };

            Media.RenderingSubtitles += (s, e) =>
            {
                // a simple example of suffixing subtitles:
                // if (e.Text != null && e.Text.Count > 0 && e.Text[e.Text.Count - 1] != "(subtitles)")
                //    e.Text.Add("(subtitles)");
            };

            Media.AudioDeviceStopped += async (s, e) =>
            {
                // If we detect that the audio device has stopped, simply
                // call the ChangeMedia command so the default audio device gets selected
                // and reopened. See issue #93
                await Media.ChangeMedia();
            };
        }

        private void X_CursorControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement el = (UIElement)sender;
            el.ReleaseMouseCapture();
            this.IsScrubbing = false;
        }

        private void X_CursorControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsScrubbing)
            {
                //Handles cursor downs on the timeline.
                var pos = e.GetPosition(this.x_CursorControl);
                var widthMs =this.x_CursorControl.EndMs - (double)this.x_CursorControl.StartMs;
                var newStartMs = this.x_CursorControl.StartMs + widthMs * pos.X / this.x_CursorControl.ActualWidth;
                if (newStartMs < 0)
                    newStartMs = 0;
                if (newStartMs > this.Media.NaturalDuration?.TotalSeconds * 1000)
                {
                    newStartMs = (long)this.Media.NaturalDuration?.TotalSeconds * 1000;
                }

                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    var appState = this.ServiceProvider.GetRequiredService<AppState>();
                    appState.Project.ProjectData.MarkInMs = (long)newStartMs;
                }
                else
                {
                    this.Media.Seek(TimeSpan.FromMilliseconds(newStartMs));
                }
            }
        }
    }
}