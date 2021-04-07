
using System.IO;
using System.Threading.Tasks;

namespace Maptz.Subtitler.App.Timelines
{
    public interface ITileImageRepository
    {
        Task<Stream> GetTileImageAsync(string bikey, string filePath);
    }
}