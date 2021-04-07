using Maptz.Subtitler.App;
using Maptz.Subtitler.App.Projects;

namespace Maptz.Subtitler.Wpf.App
{
    /// <summary>
    /// A project. 
    /// </summary>
    public class Project : DirtyBase, IProject
    {
        private ProjectDataEx _projectData;
        public ProjectDataEx ProjectData
        {
            get => this._projectData;
            set
            {
                var oldValue = this._projectData;
                if (this._projectData != value)
                {
                    this._projectData = value;
                    this.OnPropertyChanged(nameof(ProjectData), oldValue, value);
                    this.SetDirty();
                }
            }
        }

        private ProjectSettingsEx _projectSettings;
        public ProjectSettingsEx ProjectSettings
        {
            get => this._projectSettings;
            set
            {
                var oldValue = this._projectSettings;
                if (this._projectSettings != value)
                {
                    this._projectSettings = value;
                    this.OnPropertyChanged(nameof(ProjectSettings), oldValue, value);
                    this.SetDirty();
                }
            }
        }

        private string _projectFilePath;
        public string ProjectFilePath
        {
            get => this._projectFilePath;
            set
            {
                var oldValue = this._projectFilePath;
                if (this._projectFilePath != value)
                {
                    this._projectFilePath = value;
                    this.OnPropertyChanged(nameof(ProjectFilePath), oldValue, value);
                    this.SetDirty();
                }
            }
        }

        IProjectData IProject.ProjectData => this.ProjectData;

        IProjectSettings IProject.ProjectSettings => this.ProjectSettings;

        public Project()
        {
            this.ProjectSettings = new ProjectSettingsEx();
            this.ProjectData = new ProjectDataEx();
        }
    }
}