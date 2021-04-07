using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace Maptz.Subtitler.App.Timelines
{

    public class TileImageRepository : ITileImageRepository
    {

        protected Dictionary<string, bool> ProcessingFiles { get;  } = new Dictionary<string, bool>();

        public TileImageRepository(IWavConverter wavConverter, IUWavSamplePlotter uWavSamplePlotter)
        {
            this.WavConverter = wavConverter;
            this.UWavSamplePlotter = uWavSamplePlotter;
        }

        public IWavConverter WavConverter { get; }
        public IUWavSamplePlotter UWavSamplePlotter { get; }

        public async Task<Stream> GetTileImageAsync(string bikey, string filePath)
        {
            var wavFilePath = filePath + ".8bit.wav";
            if (!File.Exists(wavFilePath))
            {
                if (this.ProcessingFiles.ContainsKey(filePath))
                {
                    while (this.ProcessingFiles.ContainsKey(filePath))
                    {
                        await Task.Delay(10);
                    }
                }
                else
                {
                    this.ProcessingFiles.Add(filePath, true);
                    await this.WavConverter.ConvertAsync(filePath, wavFilePath, 8000, 1, "pcm_u8", default(CancellationToken));
                    this.ProcessingFiles.Remove(filePath);
                }
            }

            var wav = new WavFile8Bit(wavFilePath);
            //var stream = await UWavSamplePlotter.GetSampleImageAsync(wav, bikey);
            var uspan = BiKeyHelpers.GetUVSpan(bikey);
            var startMs = (long)(UVWavHelpers.FullRangeProjection.Width * uspan.Value);
            var endMs = (long)(startMs + UVWavHelpers.FullRangeProjection.Width * uspan.Width);

            var stream = await UWavSamplePlotter.GetSampleImageAsync(wav, startMs, endMs);
            return stream;
          


        }
    }
}