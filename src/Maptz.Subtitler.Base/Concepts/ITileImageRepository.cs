
using System.IO;
using System.Threading.Tasks;

namespace Maptz.QuickVideoPlayer
{
    public interface ITileImageRepository
    {
        Task<Stream> GetTileImageAsync(string bikey, string filePath);
    }
}