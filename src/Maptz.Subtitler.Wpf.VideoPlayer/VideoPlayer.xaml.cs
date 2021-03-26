using Maptz.Editing.TimeCodeDocuments;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Engine;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.Engine;
using Maptz.Subtitler.Wpf.Engine.Commands;
using Maptz.Subtitler.Wpf.VideoPlayer.Commands;
using Maptz.Subtitler.Wpf.VideoPlayer.Projects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Unosquare.FFME;
using Unosquare.FFME.Common;


namespace Maptz.Subtitler.Wpf.VideoPlayer
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {

        
        public IServiceProvider ServiceProvider { get; private set; }

        public Unosquare.FFME.MediaElement MediaElement => this.Media;

        /* #region Private Methods */
        /// <summary>
        /// Handles the MediaFailed event of the Media control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "MediaFailedEventArgs"/> instance containing the event data.</param>
        private void OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            MessageBox.Show(Application.Current.MainWindow, $"Media Failed: {e.ErrorException.GetType()}\r\n{e.ErrorException.Message}", $"{nameof(Unosquare.FFME.MediaElement)} Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        /// <summary>
        /// Handles the FFmpegMessageLogged event of the MediaElement control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "MediaLogMessageEventArgs"/> instance containing the event data.</param>
        private void OnMediaFFmpegMessageLogged(object sender, MediaLogMessageEventArgs e)
        {
            if (e.MessageType != MediaLogMessageType.Warning && e.MessageType != MediaLogMessageType.Error)
                return;
            if (string.IsNullOrWhiteSpace(e.Message) == false && e.Message.ContainsOrdinal("Using non-standard frame rate"))
                return;

        }

        /// <summary>
        /// Handles the MessageLogged event of the Media control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "MediaLogMessageEventArgs"/> instance containing the event data.</param>
        private void OnMediaMessageLogged(object sender, MediaLogMessageEventArgs e)
        {
            if (e.MessageType == MediaLogMessageType.Trace)
                return;

        }
        /* #endregion Private Methods */

        public VideoPlayer()
        {
            InitializeComponent();
        }


        private List<BindingWatcherBase> _bindingWatchers;

        public void Initialize(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            InvalidateCommandMenus();

            this.x_CursorControl.Initialize(ServiceProvider);

            this.Media.ScrubbingEnabled = true;

            this._bindingWatchers = new List<BindingWatcherBase>();

            var projectSettings = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectSettings>();
            var projectData = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectData>();
            var videoPlayerState = this.ServiceProvider.GetRequiredService<IVideoPlayerState>();
            {
                var bw = new BindingWatcher<string>(projectSettings, "VideoFilePath");
                bw.BindingChanged += (s, e) =>
                {
                    SetMediaSource((string)e.NewValue);
                }

                ;
                SetMediaSource(bw.BoundProperty);
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<SmpteFrameRate>(projectSettings, "FrameRate");
                bw.BindingChanged += (s, e) =>
                {
                    this.SyncVideoCursor();
                }

                ;
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<long>(projectSettings, "OffsetFrames");
                bw.BindingChanged += (s, e) =>
                {
                    this.SyncVideoCursor();
                };
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<long?>(projectData, "CursorMs");
                bw.BindingChanged += (s, e) =>
                {
                    this.SyncVideoCursor();
                };
                this.SyncVideoCursor();
                this._bindingWatchers.Add(bw);
            }

            {
                var bw = new BindingWatcher<long?>(videoPlayerState, "SpeedRatio");
                bw.BindingChanged += (s, e) =>
                {
                    this.Media.SpeedRatio = videoPlayerState != null ? videoPlayerState.SpeedRatio : 1.0;
                }

                ;
                this.Media.SpeedRatio = videoPlayerState != null ? videoPlayerState.SpeedRatio : 1.0;
                this._bindingWatchers.Add(bw);
            }

            {
               
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
                var projectData = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectData>();
                projectData.CursorMs = (long)this.Media.ActualPosition?.TotalMilliseconds;
            }

            ;
           
            /* #endregion*/

          
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




        private void InvalidateCommandMenus()
        {
            var playbackCommands = this.ServiceProvider.GetRequiredService<PlaybackCommands>();
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipFastBackwardVideoCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipBackwardCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.TogglePlayStateCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipForwardCommand));
            this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipFastForwardVideoCommand));


            var comboBox = new ComboBox
            {
                SelectedValuePath = "Key",
                DisplayMemberPath = "Value"
            };
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
        }

        private void Media_MediaOpened(object sender, MediaOpenedEventArgs e)
        {
            var projectData = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectData>();
            projectData.CursorMs = (long)this.Media.Position.TotalMilliseconds;
            var timelineProjectData = this.ServiceProvider.GetRequiredService<ITimelineProjectData>();
            timelineProjectData.ViewMs.MaxStartMs = 0;
            timelineProjectData.ViewMs.MaxEndMs = (long)this.Media.NaturalDuration.Value.TotalMilliseconds;
            timelineProjectData.ViewMs.StartMs = 0;
            timelineProjectData.ViewMs.EndMs = (long)this.Media.NaturalDuration.Value.TotalMilliseconds;
        }

        private void SetMediaSource(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                this.Media.Open(new Uri(filePath));
                //this.Media.Source = new Uri(filePath);
            }
            else
            {
                this.Media.Close();
                //this.Media.Source = null;
            }
        }

        private void SyncVideoCursor()
        {
            var projectData = this.ServiceProvider.GetRequiredService<IVideoPlayerProjectData>();
            var projectSettings = this.ServiceProvider.GetRequiredService<IProjectSettings>();
            var currentTCStr = "UNKNOWN";
            var currentSubStr = string.Empty;
            ITimeCodeDocumentItem<string> currentSub = null;
            if (projectData.CursorMs.HasValue)
            {
                var fr =projectSettings.FrameRate;
                var offsetTC = TimeCode.FromFrames(projectSettings.OffsetFrames, fr);
                var cursorTC = offsetTC.Add(TimeCode.FromSeconds(projectData.CursorMs.Value / 1000.0, fr));
                currentTCStr = cursorTC.ToString();

                //TODO Get current subtitle
                //if (this.SubtitleItems != null && this.SubtitleItems.Any())
                //{
                //    currentSub = this.SubtitleItems.FirstOrDefault(p => cursorTC.TotalFrames >= p.RecordIn.TotalFrames && cursorTC.TotalFrames <= p.RecordOut.TotalFrames);
                //}
            }

            //if (currentSub != null)
            //{
            //    currentSubStr = currentSub.Content;
            //    this.x_TextBox.StartHighlight(currentSub.TextSpan, WrappedTextBox.HighlightKind.VideoCursor, true);
            //}
            //else
            //{
            //    this.x_TextBox.ClearHighlights(p => p.Kind == WrappedTextBox.HighlightKind.VideoCursor);
            //}

            //this.x_TextBlock_TimeCode.Text = currentTCStr;
            //this.x_TextBlock_CurrentSubtitle.Text = currentSubStr;
        }


        private bool IsScrubbing
        {
            get;
            set;
        }
    }
}
