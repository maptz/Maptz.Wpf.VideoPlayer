using Maptz.Subtitler.App;
using Maptz.Subtitler.App.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Maptz.Subtitler.Wpf.Controls
{

    public class CommandPalette : WrapPanel
    {
        /* #region Private Methods */
        private void AppCommands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateControls();
        }
        private void InvalidateControls()
        {
            this.Children.Clear();
            foreach(var command in this.AppCommands)
            {
                var button = new Button();
                button.Command = command;
                button.Content = $"{command.Name} ({command.Shortcut?.ToString()})";
                this.Children.Add(button);
            }
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.InvalidateControls();
        }
        /* #endregion Protected Methods */
        /* #region Public Properties */
        public IAppCommandEngine AppComandEngine { get; }
        public ObservableCollection<IAppCommand> AppCommands { get; }
        public IServiceProvider ServiceProvider { get; }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public CommandPalette()
        {
            
            this.ServiceProvider = (System.Windows.Application.Current as IApp).ServiceProvider;

            this.AppComandEngine = this.ServiceProvider.GetRequiredService<IAppCommandEngine>();
            this.AppCommands = (this.AppComandEngine as IAppCommandEngine).AppCommands;
            this.AppCommands.CollectionChanged += this.AppCommands_CollectionChanged;
        }
        /* #endregion Public Constructors */
    }
}
