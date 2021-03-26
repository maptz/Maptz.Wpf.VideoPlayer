using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.App.Commands;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.Engine;
using Maptz.Subtitler.Wpf.Engine.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Maptz.Subtitler.Wpf.App
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* #region Private Fields */
        private readonly List<BindingWatcherBase> _bindingWatchers;

        /* #endregion Private Fields */
        /* #region Private Methods */
        private void InitializeMenu()
        {
            var appCommands = this.ServiceProvider.GetRequiredService<AppCommands>();
            var projectCommands = this.ServiceProvider.GetRequiredService<ProjectCommands>();
            var projectSettingsCommands = this.ServiceProvider.GetRequiredService<ProjectSettingsCommands>();
            var menu = this.x_Menu;
            var fm = new MenuItem()
            { Header = "_File" };
            menu.Items.Add(fm);
            var comm = projectCommands.NewProjectCommand;
            ;
            fm.Items.Add(new MenuItem()
            { Header = "_New Project", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectCommands.OpenProjectCommand;
            fm.Items.Add(new MenuItem()
            { Header = "_Open Project", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectCommands.SaveProjectCommand;
            fm.Items.Add(new MenuItem()
            { Header = "_Save Project", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectCommands.SaveProjectAsCommand;
            fm.Items.Add(new MenuItem()
            { Header = "Save Project As", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = projectSettingsCommands.ShowProjectSettingsCommand;
            fm.Items.Add(new MenuItem()
            { Header = "Project Settings", Command = comm, Icon = comm.IconSource?.GetIconElement() });
            comm = appCommands.ExitAppCommand;
            fm.Items.Add(new MenuItem()
            { Header = "_Exit", Command = comm, Icon = comm.IconSource?.GetIconElement() });
        }
        private void InvalidateCommandMenus()
        {
            //var playbackCommands = this.ServiceProvider.GetRequiredService<PlaybackCommands>();
            //this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipFastBackwardVideoCommand));
            //this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipBackwardCommand));
            //this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.TogglePlayStateCommand));
            //this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipForwardCommand));
            //this.x_StackPanel_PlaybackCommands.Children.Add(AppCommandButton.FromAppCommand(playbackCommands.SkipFastForwardVideoCommand));
            ////
            //var timelineCommands = this.ServiceProvider.GetRequiredService<TimelineCommands>();
            //this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(timelineCommands.CentreTimelineCommand));
            //this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(timelineCommands.ZoomInTimelineCommand));
            //this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(timelineCommands.ZoomOutTimelineCommand));
            //this.x_StackPanel_TimelineCommands.Children.Add(new Canvas { Width = 100, Height = 1 });
            //var markingCommands = this.ServiceProvider.GetRequiredService<MarkingCommands>();
            //this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(markingCommands.ClearMarkInMsCommand));
            //this.x_StackPanel_TimelineCommands.Children.Add(AppCommandButton.FromAppCommand(markingCommands.SetMarkInCommand));

            //var comboBox = new ComboBox
            //{
            //    SelectedValuePath = "Key",
            //    DisplayMemberPath = "Value"
            //};
            //x_StackPanel_TimelineCommands.Children.Add(comboBox);
            //comboBox.Items.Add(new KeyValuePair<double, string>(0.25, "0.25"));
            //comboBox.Items.Add(new KeyValuePair<double, string>(0.5, "0.5"));
            //comboBox.Items.Add(new KeyValuePair<double, string>(0.75, "0.75"));
            //comboBox.Items.Add(new KeyValuePair<double, string>(1.0, "1.0"));
            //comboBox.Items.Add(new KeyValuePair<double, string>(1.2, "1.2"));
            //comboBox.Items.Add(new KeyValuePair<double, string>(2.0, "2.0"));
            //comboBox.Items.Add(new KeyValuePair<double, string>(4.0, "4.0"));
            //var binding = new Binding("VideoPlayerState.SpeedRatio")
            //{
            //    Mode = BindingMode.TwoWay
            //};
            //BindingOperations.SetBinding(comboBox, Selector.SelectedValueProperty, binding);

            //
            var textManipCommands = this.ServiceProvider.GetRequiredService<TextManipulationCommands>();
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.InsertTimeCodeFromCursorCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.InsertTimeCodeFromMarkInCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.PreviousGrammarPointCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.NextGrammarPointCommand));
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(textManipCommands.SplitSentencesCommand));
            var subManipCommands = this.ServiceProvider.GetRequiredService<SubtitleManipulationCommands>();
            this.x_StackPanel_TextCommands.Children.Add(AppCommandButton.FromAppCommand(subManipCommands.SplitCurrentSubtitleCommand));
        }


        private void SetTitle()
        {
            var project = this.ServiceProvider.GetService<IProject>();
            if (project == null) return;
            var projectIsDirty = project.IsDirty;
            var title = "Maptz.Subtitler";
            title += projectIsDirty ? " - [Unsaved]" : string.Empty;
            this.Title = title;
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected override void OnClosing(CancelEventArgs e)
        {

            base.OnClosing(e);
        }
        protected virtual void OnProjectTextChanged(object oldValue, object newValue)
        {

        }
        /* #endregion Protected Methods */
        /* #region Public Properties */
        public IServiceProvider ServiceProvider
        {
            get;
        }


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
            this._bindingWatchers = new List<BindingWatcherBase>();
            {
                var bw = new BindingWatcher<string>(appState, "Project.ProjectData.Text");
                bw.BindingChanged += async (s, e) =>
                {
                    OnProjectTextChanged(e.OldValue, e.NewValue);
                    RequestSubtitleUpdate();
                }

                ;
                RequestSubtitleUpdate();
                this._bindingWatchers.Add(bw);
            }


            {
                var bw = new BindingWatcher<long?>(appState, "Project.IsDirty");
                bw.BindingChanged += (s, e) =>
                {
                    this.SetTitle();
                }

                ;
                this.SetTitle();
                this._bindingWatchers.Add(bw);

            }

            this.InvalidateCommandMenus();
        }

        private void RequestSubtitleUpdate()
        {
            var sv = this.ServiceProvider.GetRequiredService<ISubtitleView>();
            sv.RequestSubtitleUpdate();
        }
        /* #endregion Public Constructors */
    }
}