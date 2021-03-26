using Maptz.Subtitler.App.Commands;
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.App.SessionState;
using Maptz.Subtitler.Wpf.Engine.Icons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Maptz.Subtitler.Wpf.Engine.Commands
{

    public class ProjectCommands : CommandProviderBase
    {
        /* #region Private Methods */
        private void NewProject()
        {

            var project = this.ServiceProvider.GetRequiredService<IProject>();
            if (project != null)
            {
                if (project.IsDirty)
                {
                    var hasSaved = this.SaveProject();
                    if (!hasSaved)
                    {
                        string messageBoxText = "Do you want to discard changes?";
                        string caption = "New Project";
                        MessageBoxButton button = MessageBoxButton.OKCancel;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        var result = MessageBox.Show(messageBoxText, caption, button, icon);

                        switch (result)
                        {
                            case MessageBoxResult.OK:
                                break;
                            case MessageBoxResult.Cancel:
                                return;
                        }
                    }
                }
            }
            //Create a new Project.
            var projectInitializer = this.ServiceProvider.GetRequiredService<IProjectManager>();
            var newProject = projectInitializer.NewProject();
            projectInitializer.SetProject(newProject);

            var sessionState = this.ServiceProvider.GetRequiredService<SessionState>();
            sessionState.LastOpenProjectPath = null;
        }

        private bool SaveProject(bool overrideFilePath = false)
        {
            var sessionState = this.ServiceProvider.GetRequiredService<SessionState>();
            var project = this.ServiceProvider.GetService<IProject>();
            if (project == null) { return false; }
            if (overrideFilePath || project.ProjectFilePath == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                if (!string.IsNullOrEmpty(sessionState.SaveProjectDirectoryPath))
                {
                    saveFileDialog.InitialDirectory = sessionState.SaveProjectDirectoryPath;
                }
                var result = saveFileDialog.ShowDialog();
                if (!(result.HasValue && result.Value)) return false;
                project.ProjectFilePath = saveFileDialog.FileName;
            }
            var projectSerializer = this.ServiceProvider.GetRequiredService<IProjectSerializer>();
            projectSerializer.SaveProject(project);
            sessionState.LastOpenProjectPath = project.ProjectFilePath;
            return true;
        }

        /* #endregion Private Methods */
        /* #region Public Properties */
        public IServiceProvider ServiceProvider { get; }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public ProjectCommands(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        /* #endregion Public Constructors */
        /* #region Public Methods */
        public IAppCommand NewProjectCommand => new AppCommand("NewProject", (object o) => this.NewProject(), new KeyChords(new KeyChord(Key.N, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.video_plus));

        public void OpenProject(string projectFilePath = null)
        {
            var project = this.ServiceProvider.GetService<IProject>();
            if (project == null)
            {
                //TODO Remmeber to save project if it is null.
            }


            if (string.IsNullOrEmpty(projectFilePath))
            {
                OpenFileDialog ofg = new OpenFileDialog();
                var sessionState = this.ServiceProvider.GetRequiredService<SessionState>();
                if (!string.IsNullOrEmpty(sessionState.OpenProjectDirectoryPath) && Directory.Exists(sessionState.OpenProjectDirectoryPath))
                {
                    ofg.InitialDirectory = sessionState.OpenProjectDirectoryPath;
                }

                var result = ofg.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    projectFilePath = ofg.FileName;
                    sessionState.OpenProjectDirectoryPath = Path.GetDirectoryName(projectFilePath);
                    sessionState.LastOpenProjectPath = projectFilePath;
                }
                else
                {
                    return;
                }
            }

            var projectSerializer = this.ServiceProvider.GetRequiredService<IProjectSerializer>();
            var loadedProject = projectSerializer.ReadProject(projectFilePath);

            var projectManager = this.ServiceProvider.GetRequiredService<IProjectManager>();
            projectManager.SetProject(loadedProject);

        }
        public IAppCommand OpenProjectCommand => new AppCommand("OpenProject", (object o) => this.OpenProject((string)o), new KeyChords(new KeyChord(Key.O, ctrl: true)), new XamlIconSource(IconPaths3.file_video));

        public IAppCommand SaveProjectAsCommand => new AppCommand("SaveProjectAs", (object o) => this.SaveProject(true), new KeyChords(new KeyChord(Key.S, ctrl: true, shift: true)), new XamlIconSource(IconPaths3.content_save_edit));

        public IAppCommand SaveProjectCommand => new AppCommand("SaveProject", (object o) => this.SaveProject(), new KeyChords(new KeyChord(Key.S, ctrl: true, shift: false)), new XamlIconSource(IconPaths3.content_save));



        /* #endregion Public Methods */
        /* #region Interface: 'Maptz.QuickVideoPlayer.Commands.ICommandProvider' Methods */

        /* #endregion Interface: 'Maptz.QuickVideoPlayer.Commands.ICommandProvider' Methods */
    }
}