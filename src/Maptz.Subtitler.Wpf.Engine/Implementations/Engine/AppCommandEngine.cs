using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.Wpf.Engine.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
namespace Maptz.Subtitler.Wpf.Engine.Commands
{

    public class AppCommandEngine : IAppCommandEngine
    {
        /* #region Private Methods */
        private void AddDefaultCommands()
        {
            var defaultCommands = this.ServiceProvider.GetRequiredService<DefaultCommands>().GetDefaultCommands();
            foreach (var command in defaultCommands)
            {
                this.AppCommands.Add(command);
            }
            var pluginEngine = this.ServiceProvider.GetRequiredService<IPluginEngine>();
            var pluginInstances = pluginEngine.LoadPlugins();
            foreach(var instance in pluginInstances)
            {
                foreach(var command in instance.Commands)
                {
                    this.AppCommands.Add(command);
                }
            }

        }
        /* #endregion Private Methods */
        /* #region Public Properties */
        public ObservableCollection<IAppCommand> AppCommands { get; }
        public ILogger<AppCommandEngine> Logger { get; }
        public IServiceProvider ServiceProvider { get; }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public AppCommandEngine(ILogger<AppCommandEngine> logger, IServiceProvider serviceProvider)
        {
            this.AppCommands = new ObservableCollection<IAppCommand>();
            this.Logger = logger;
            this.ServiceProvider = serviceProvider;

            this.AddDefaultCommands();
        }
        /* #endregion Public Constructors */
        /* #region Interface: 'Maptz.QuickVideoPlayer.Commands.IAppCommandEngine' Methods */
        public void AddCommand(IAppCommand appCommand)
        {
            if (this.AppCommands.Any(p => string.Equals(p.Name, appCommand.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"A command named {appCommand.Name} has already been installed");
            }
            this.AppCommands.Add(appCommand);
        }
        public void ExecuteNamedCommand(string commandName, object parameter = null)
        {
            var command = this.AppCommands.FirstOrDefault(p => string.Equals(p.Name, commandName, StringComparison.OrdinalIgnoreCase));
            if (command != null)
            {
                var canExecute = command.CanExecute(parameter);
                command.Execute(parameter);
            }
        }
        public void RegisterKeyEvent(KeyEventArgs ev)
        {

            
            var isShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            var isAltDown = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
            var isCtrlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            var matchingCommands = new List<IAppCommand>();

            
            foreach (var appCommand in this.AppCommands)
            {
                if (ev.Key == Key.Space && appCommand.Name == "ToggleVideoPlayState")
                {

                }
                var shortcut = appCommand.Shortcut;
                if (shortcut == null || !shortcut.Chords.Any()) continue;
                var firstChord = shortcut.Chords.First();
                var shiftMatch = !(firstChord.Shift ^ isShiftDown);
                var ctrlMatch = !(firstChord.Ctrl ^ isCtrlDown);
                var altMatch = !(firstChord.Alt ^ isAltDown);
                var keyMatch = firstChord.Key == ev.Key || firstChord.Key == ev.SystemKey;

                if (shiftMatch && ctrlMatch && altMatch && keyMatch)
                {
                    matchingCommands.Add(appCommand);
                }
            }

            var matchingCommand = matchingCommands.FirstOrDefault();
            if (matchingCommand != null)
            {
                this.Logger.LogInformation($"Executing command {matchingCommand.Name}");
                matchingCommand.Execute(null);
                ev.Handled = true;
            }
        }

        /* #endregion Interface: 'Maptz.QuickVideoPlayer.Commands.IAppCommandEngine' Methods */
    }
}