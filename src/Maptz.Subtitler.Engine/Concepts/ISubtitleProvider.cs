using Maptz.Editing.TimeCodeDocuments;
using System.Threading.Tasks;

namespace Maptz.Subtitler.Engine
{
    public interface ISubtitleProvider
    {
        Task<ITimeCodeDocument<string>> GetSubtitlesAsync(string str);
    }
}