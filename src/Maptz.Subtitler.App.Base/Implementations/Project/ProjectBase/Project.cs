using System.IO;
using System.Text.Json;

namespace Maptz.Subtitler.App.Projects
{
    /// <summary>
    /// A project. 
    /// </summary>
    public class Project : DirtyBase, IProject<ProjectDataBase, ProjectSettingsBase>
    {
        private ProjectDataBase _projectData;
        public ProjectDataBase ProjectData
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

        private ProjectSettingsBase _projectSettings;
        public ProjectSettingsBase ProjectSettings
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
            this.ProjectSettings = new ProjectSettingsBase();
            this.ProjectData = new ProjectDataBase();
        }
    }
}