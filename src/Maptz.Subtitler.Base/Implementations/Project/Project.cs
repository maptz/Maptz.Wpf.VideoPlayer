using Newtonsoft.Json;
using System.IO;

namespace Maptz.QuickVideoPlayer
{
    public class Project : DirtyBase
    {
        private ProjectData _projectData;
        public ProjectData ProjectData
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

        private ProjectSettings _projectSettings;
        public ProjectSettings ProjectSettings
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

        public static Project ReadFromFile(string projectFilePath)
        {
            var fi = new FileInfo(projectFilePath);
            if (!fi.Exists)
                throw new FileNotFoundException();
            string json;
            using (var sr = fi.OpenText())
            {
                json = sr.ReadToEnd();
            }

            var project = JsonConvert.DeserializeObject<Project>(json);
            project.ProjectFilePath = projectFilePath;
            project.SetDirty(false);
            return project;
        }

        public static void SaveToFile(Project project)
        {
            string json = JsonConvert.SerializeObject(project);
            var fi = new FileInfo(project.ProjectFilePath);
            using (var sw = fi.CreateText())
            {
                sw.Write(json);
            }

            project.SetDirty(false);
        }

        public Project()
        {
            this.ProjectSettings = new ProjectSettings();
            this.ProjectData = new ProjectData();
        }
    }
}