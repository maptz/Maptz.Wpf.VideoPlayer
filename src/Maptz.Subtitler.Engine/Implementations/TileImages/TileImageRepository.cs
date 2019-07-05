
using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Tiles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace Maptz.QuickVideoPlayer
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