using System.IO;
using System.Text.Json;
namespace Maptz.Subtitler.App.Projects
{

    public interface IProjectSerializer
    {
        IProject ReadProject(string projectFilePath);
        void SaveProject(IProject project);
    }

}