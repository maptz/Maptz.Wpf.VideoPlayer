using Maptz.Subtitler.Wpf.Engine;
namespace Maptz.Subtitler.App.Projects
{

    public class ProjectManager : IProjectManager<ProjectDataBase, ProjectSettingsBase>
    {

        public ProjectManager(AppState appState)
        {
            this.AppState = appState;
        }

        public AppState AppState { get; }

        public IProject<ProjectDataBase, ProjectSettingsBase> NewProject()
        {
            var ret = new Project();
            return ret;
        }

        public void SetProject(IProject project)
        {
            AppState.Project = project;
        }

        IProject IProjectManager.NewProject()
        {
            return this.NewProject();
        }
    }
}