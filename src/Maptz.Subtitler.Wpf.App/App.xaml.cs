using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Editing.TimeCodeDocuments;
using Maptz.Editing.TimeCodeDocuments.Converters.All;
using Maptz.Editing.TimeCodeDocuments.StringDocuments;
using Maptz.QuickVideoPlayer.Commands;
using Maptz.QuickVideoPlayer.Services;
using Maptz.Subtitler.Engine.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Maptz.QuickVideoPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IApp
    { 
        public string FFMPEGPath => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "ffmpeg.exe");
        

        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public IAppCommandEngine CommandEngine
        {
            get;
            private set;
        }

        public ISessionStateService SessionStateService
        {
            get;
            private set;
        }

        public SessionState SessionState
        {
            get;
            private set;
        }

        public AppState AppState
        {
            get;
            private set;
        }

        public IConfiguration Configuration
        {
            get;
            private set;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Unosquare.FFME.Library.FFmpegDirectory = Path.GetDirectoryName(FFMPEGPath);
            services.Configure<AppSettings>(this.Configuration.GetSection(nameof(AppSettings)));
            var sc = services;
            sc.AddLogging();
            sc.AddSingleton<IAppCommandEngine, AppCommandEngine>();
            sc.AddSingleton<ITileImageRepository, TileImageRepository>();
            sc.AddTransient<ISessionStateService, SessionStateService>();
            sc.AddTransient<IWavConverter, WavConverter>();
            sc.Configure<WavConverterSettings>(settings =>
            {
                settings.FFMPEGPath = FFMPEGPath;
            }

            );
            sc.Configure<SampleImageGeneratorSettings>(settings =>
            {
                settings.ForegroundColorHex = "#999999FF";
                settings.BackgroundColorHex = "#FFFFFF00";
            }

            );
            sc.AddWavSamplePlotter();
            services.AddTransient(typeof(MainWindow));
            services.AddSingleton<App>(sp => this);
            services.AddSingleton<AppState>(sp => this.AppState);
            services.AddSingleton<SessionState>(sp => this.SessionState);
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

        protected override void OnExit(ExitEventArgs e)
        {
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
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            this.ServiceProvider = serviceCollection.BuildServiceProvider();
            /* #endregion*/
            this.CommandEngine = this.ServiceProvider.GetRequiredService<IAppCommandEngine>();
            this.SessionStateService = this.ServiceProvider.GetRequiredService<ISessionStateService>();
            this.SessionState = this.SessionStateService.GetLastSessionState();
            this.AppState = new AppState();
            if (!string.IsNullOrEmpty(this.SessionState.LastOpenProjectPath) && File.Exists(this.SessionState.LastOpenProjectPath))
            {
                this.ServiceProvider.GetRequiredService<ProjectCommands>().OpenProject(this.SessionState.LastOpenProjectPath);
            }
            else
            {
                this.AppState.Project = new Project{};
            }

            /* #region Create the Main Window */
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            mainWindow.PreviewKeyDown += (s, ev) =>
            {
                this.CommandEngine.RegisterKeyEvent(ev);
            }

            ;
            /* #endregion*/
            this.AppState.VideoPlayerState = new VideoPlayerState(mainWindow.Media);
            this.AppState.TextBox = mainWindow.x_TextBox;
            base.OnStartup(e);
        }
    }
}