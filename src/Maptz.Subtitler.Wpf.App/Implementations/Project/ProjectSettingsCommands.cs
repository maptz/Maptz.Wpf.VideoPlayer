using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.App.Commands
{
    public class ProjectSettingsCommands : CommandProviderBase
    {
        public IAppCommand ShowProjectSettingsCommand => new AppCommand("ShowProjectSettings", (object o) => this.ShowProjectSettings(), new KeyChords(new KeyChord(Key.P, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.card_bulleted_settings));

        public IServiceProvider ServiceProvider { get; }

        private void ShowProjectSettings()
        {
            var projectSettings = this.ServiceProvider.GetRequiredService<IProjectSettings>() as ProjectSettingsEx;
            var projectSettingsWindow = new ProjectSettingsWindow(projectSettings, this.ServiceProvider);
            var result = projectSettingsWindow.ShowDialog();
        }

        public ProjectSettingsCommands(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}