using Maptz.Editing.TimeCodeDocuments;
using Maptz.Editing.TimeCodeDocuments.Converters.All;
using Maptz.Editing.TimeCodeDocuments.StringDocuments;
using Maptz.Subtitler.App;
using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.App.SessionState;
using Maptz.Subtitler.App.Wpf.App;
using Maptz.Subtitler.Engine;
using Maptz.Subtitler.Wpf.App.Commands;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.Engine;
using Maptz.Subtitler.Wpf.Engine.Commands;
using Maptz.Subtitler.Wpf.Engine.Plugins;
using Maptz.Subtitler.Wpf.VideoPlayer;
using Maptz.Subtitler.Wpf.VideoPlayer.Commands;
using Maptz.Subtitler.Wpf.VideoPlayer.Projects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Maptz.Subtitler.Wpf.App
{
    public static class AppServiceConfiguration
    {

    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IApp
    {
        public App()
        {
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler((s, e) =>
            {
                System.Diagnostics.Debugger.Break();
            });

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debugger.Break();
            };
        }

        /* #region Private Methods */
        private void ConfigureWavServices(IServiceCollection services)
        {
            //sc.AddSingleton<ITileImageRepository, TileImageRepository>();
            //sc.AddTransient<IWavConverter, WavConverter>();
            //sc.Configure<WavConverterSettings>(settings =>
            //{
            //    settings.FFMPEGPath = FFMPEGPath;
            //}

            //);
            //sc.Configure<SampleImageGeneratorSettings>(settings =>
            //{
            //    settings.ForegroundColorHex = "#999999FF";
            //    settings.BackgroundColorHex = "#FFFFFF00";
            //}

            //);
            //sc.AddWavSamplePlotter();
        }

        private void ConfigureVideoPlayer(IServiceCollection services)
        {
            services.AddSingleton<VideoPlayerState>(this.AppState.VideoPlayerState);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Unosquare.FFME.Library.FFmpegDirectory = Path.GetDirectoryName(FFMPEGPath);
            services.Configure<AppSettings>(this.Configuration.GetSection(nameof(AppSettings)));
            var sc = services;
            sc.AddLogging();
            sc.AddSingleton<IAppCommandEngine, AppCommandEngine>();
            sc.AddTransient<ISessionStateService, SessionStateService>();

            services.AddTransient(typeof(MainWindow));
            services.AddTransient<App>(sp => this);
            services.AddTransient<AppState>(sp => this.AppState);
            services.AddTransient<SessionState>(sp => this.SessionState);

            /* #region Project */
            services.AddTransient<IProject>(sp =>
                     {
                         return this.AppState.Project;
                     });
            services.AddTransient<IProjectSerializer, ProjectSerializer>();
            services.AddSingleton<IProjectManager>(sp => new ProjectManager(this.AppState));
            services.AddTransient<IProjectSettings>(Span => this.AppState.Project.ProjectSettings);
            services.AddTransient<IVideoPlayerProjectSettings>(Span => this.AppState.Project.ProjectSettings);

            services.AddTransient<IProjectData>(Span => this.AppState.Project.ProjectData);
            services.AddTransient<ITimelineProjectData>(Span => this.AppState.Project.ProjectData);
            services.AddTransient<IVideoPlayerProjectData>(Span => this.AppState.Project.ProjectData);
            services.AddTransient<ICursorProjectData>(Span => this.AppState.Project.ProjectData);
            services.AddTransient<IMarkingProjectData>(Span => this.AppState.Project.ProjectData);
            /* #endregion*/


            services.AddTransient<ProjectSettingsCommands>();
            services.AddSingleton<ISubtitleView>(sp => new SubtitleView(sp));
            //services.AddTransient<ILineSplitter, SimpleLineSplitter>();
            services.AddTransient<ILineSplitter, ComplexLineSplitter>();
            services.AddTransient<WrappedTextBox>(sp => (this.MainWindow as MainWindow).x_TextBox);

            /* #region Video Playback */
            services.AddTransient<MarkingCommands>();
            services.AddTransient<PlaybackCommands>();
            services.AddTransient<TimelineCommands>();
            services.AddTransient<IVideoPlayerState>(sp => this.AppState.VideoPlayerState);
            services.AddTransient<VideoWindow>();
            /* #endregion*/

            /* #region TimeCodeDocuments */
            services.AddTransient<ISubtitleProvider, SubtitleProvider>();
            services.AddTimeCodeDocumentConverters();
            services.Configure<TimeCodeStringDocumentParserSettings>(settings =>
            {
                settings.FrameRate = SmpteFrameRate.Smpte25;
            }

            );
            services.Configure<TimeCodeDocumentTimeValidatorSettings>(settings =>
            {
                settings.DefaultDurationFrames = 60;
            });
            /* #endregion*/

            /* #region Plugins */
            services.AddSingleton<IPluginEngine, PluginEngine>();
            services.Configure<PluginEngineSettings>(settings =>
            {
                settings.PluginPaths = new List<string>
                {
                    @"X:\+++DEV\MaptzGitHub\wpf\maptz.wpf.videoplayer\src\Maptz.Subtitler.PluginTest\bin\Debug\netcoreapp3.0\Maptz.Subtitler.PluginTest.dll"
                };
            });

            /* #endregion*/

            DefaultCommands.AddCommandProviders(services);

        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override void OnExit(ExitEventArgs e)
        {
            (this.ServiceProvider as ServiceProvider).Dispose();

            base.OnExit(e);
            this.SessionStateService.SaveSessionState(this.SessionState);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            /* #region Initialize Configuration */
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            this.Configuration = builder.Build();
            /* #endregion*/
            /* #region Initialize Services */
            this.AppState = new AppState();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            this.ServiceProvider = serviceCollection.BuildServiceProvider();
            /* #endregion*/
            this.CommandEngine = this.ServiceProvider.GetRequiredService<IAppCommandEngine>();
            this.ConfigureCommands(this.CommandEngine);

            this.SessionStateService = this.ServiceProvider.GetRequiredService<ISessionStateService>();
            this.SessionState = this.SessionStateService.GetLastSessionState();

            if (!string.IsNullOrEmpty(this.SessionState.LastOpenProjectPath) && File.Exists(this.SessionState.LastOpenProjectPath))
            {
                this.ServiceProvider.GetRequiredService<ProjectCommands>().OpenProject(this.SessionState.LastOpenProjectPath);
            }
            else
            {
                this.AppState.Project = new Project { };
            }


            /* #region Create the Main Window */
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            mainWindow.PreviewKeyDown += (s, ev) =>
            {
                this.CommandEngine.RegisterKeyEvent(ev);
            };

            /* #endregion*/
            //var videoWindow = ServiceProvider.GetRequiredService<VideoWindow>();
            //videoWindow.Show(); ;
            //this.AppState.VideoPlayerState.MediaElement = videoWindow.x_VideoPlayer.MediaElement;
            this.AppState.TextBox = mainWindow.x_TextBox;
            base.OnStartup(e);
        }

        private void ConfigureCommands(IAppCommandEngine commandEngine)
        {
            commandEngine.AddCommandsFromType<MarkingCommands>();
            commandEngine.AddCommandsFromType<PlaybackCommands>();
            commandEngine.AddCommandsFromType<TimelineCommands>();
            commandEngine.AddCommandsFromType<ProjectSettingsCommands>();
        }

        /* #endregion Protected Methods */
        /* #region Public Properties */
        public AppState AppState
        {
            get;
            private set;
        }
        public IAppCommandEngine CommandEngine
        {
            get;
            private set;
        }
        public IConfiguration Configuration
        {
            get;
            private set;
        }
        public string FFMPEGPath => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "ffmpeg.exe");
        public SessionState SessionState
        {
            get;
            private set;
        }
        public ISessionStateService SessionStateService
        {
            get;
            private set;
        }
        /* #endregion Public Properties */
        /* #region Interface: 'Maptz.QuickVideoPlayer.Services.IApp' Properties */
        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }
        /* #endregion Interface: 'Maptz.QuickVideoPlayer.Services.IApp' Properties */
    }
}