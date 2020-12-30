using System.IO;
using System.Text.Json;
namespace Maptz.Subtitler.App.Projects
{

    public interface IProject : IDirty
    {
        string ProjectFilePath { get; set; }
        IProjectData ProjectData { get; }
        IProjectSettings ProjectSettings { get; }
    }


    public interface IProject<TData, TSettings> : IProject where TData : ProjectDataBase where TSettings : ProjectSettingsBase
    {
        string ProjectFilePath { get; set; }
    }
}