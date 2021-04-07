
using Maptz.Subtitler.App.Projects;
using Maptz.Subtitler.Wpf.App;
using System.IO;
namespace Maptz.Subtitler.App.Wpf.App
{


    public class ProjectSerializer : IProjectSerializer
    {
        public IProject ReadProject(string projectFilePath)
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

        public void SaveProject(IProject project)
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

      
    }
}