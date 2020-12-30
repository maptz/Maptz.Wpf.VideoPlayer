using System.IO;
using System.Text.Json;
namespace Maptz.Subtitler.App.Projects
{

    public interface IProjectSerializer
    {
        IProject ReadProject(string projectFilePath);
        void SaveProject(IProject project);
    }

    public interface IProjectSerializer<TData, TSettings> : IProjectSerializer where TData : ProjectDataBase where TSettings : ProjectSettingsBase
    {
        IProject<TData, TSettings> ReadProject(string projectFilePath);
        void SaveProject(IProject<TData, TSettings> project);
    }
}