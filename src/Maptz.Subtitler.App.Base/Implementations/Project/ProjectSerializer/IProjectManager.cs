using System.IO;
using System.Text.Json;
namespace Maptz.Subtitler.App.Projects
{
    public interface IProjectManager
    {
        IProject NewProject();
        void SetProject(IProject project);
    }

    public interface IProjectManager<TData, TSettings> : IProjectManager where TData : ProjectDataBase where TSettings : ProjectSettingsBase
    {
        new IProject<TData, TSettings> NewProject();
    }
}