using Maptz.Subtitler.Wpf.App;
using Maptz.Subtitler.Wpf.Engine;
using System;

namespace Maptz.Subtitler.App.Projects
{

    public class ProjectManager : IProjectManager
    {

        public ProjectManager(AppState appState)
        {
            this.AppState = appState;
        }

        public AppState AppState { get; }

        public event EventHandler<EventArgs> ProjectChanged;

        public IProject NewProject()
        {
            var ret = new Project();
            return ret;
        }

        public void SetProject(IProject project)
        {
            AppState.Project = (Project) project;
            RaiseProjectChanged();
        }

        private void RaiseProjectChanged()
        {
            var pc = ProjectChanged;
            if (pc != null) pc(this, new EventArgs());
        }
    }
}