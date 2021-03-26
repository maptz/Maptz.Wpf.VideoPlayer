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
}