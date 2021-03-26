using System;
using System.IO;
using System.Text.Json;
namespace Maptz.Subtitler.App.Projects
{
    public interface IProjectManager
    {
        event EventHandler<EventArgs> ProjectChanged;
        IProject NewProject();
        void SetProject(IProject project);
    }

    
}