using Maptz.Editing.TimeCodeDocuments;
using System.Threading.Tasks;

namespace Maptz.QuickVideoPlayer
{
    public interface ISubtitleProvider
    {
        Task<ITimeCodeDocument<string>> GetSubtitlesAsync(string str);
    }
}