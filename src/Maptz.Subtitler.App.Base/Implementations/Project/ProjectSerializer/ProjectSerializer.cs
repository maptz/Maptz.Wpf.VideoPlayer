
using System.IO;
using System.Text.Json;
namespace Maptz.Subtitler.App.Projects
{


    public class ProjectSerializer : IProjectSerializer<ProjectDataBase, ProjectSettingsBase>
    {
        public IProject<ProjectDataBase, ProjectSettingsBase> ReadProject(string projectFilePath)
        {
            var fi = new FileInfo(projectFilePath);
            if (!fi.Exists)
                throw new FileNotFoundException();
            string json;
            using (var sr = fi.OpenText())
            {
                json = sr.ReadToEnd();
            }


            //var project = JsonSerializer.Deserialize<Project>(json);
            var project = Newtonsoft.Json.JsonConvert.DeserializeObject<Project>(json);
            project.ProjectFilePath = projectFilePath;
            project.SetDirty(false);
            return project;
        }

        public void SaveProject(IProject<ProjectDataBase, ProjectSettingsBase> project)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(project);
            //string json = JsonSerializer.Serialize(project); ;
            var fi = new FileInfo(project.ProjectFilePath);
            using (var sw = fi.CreateText())
            {
                sw.Write(json);
            }

            (project as Project).SetDirty(false);
        }

        IProject IProjectSerializer.ReadProject(string projectFilePath) => this.ReadProject(projectFilePath);

        void IProjectSerializer.SaveProject(IProject project) => this.SaveProject(project as IProject<ProjectDataBase, ProjectSettingsBase>);
    }
}