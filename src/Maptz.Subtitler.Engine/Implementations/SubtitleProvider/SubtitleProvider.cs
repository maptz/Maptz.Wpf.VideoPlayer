using Maptz.Editing.TimeCodeDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maptz.QuickVideoPlayer
{


    public class SubtitleProvider : ISubtitleProvider
    {

        public SubtitleProvider(ITimeCodeDocumentParser<string> timeCodeDocumentParser, ITimeCodeDocumentTimeValidator<string> timeCodeDocumentTimeValidator)
        {
            this.TimeCodeDocumentParser = timeCodeDocumentParser;
            this.TimeCodeDocumentTimeValidator = timeCodeDocumentTimeValidator;
        }

        public ITimeCodeDocumentParser<string> TimeCodeDocumentParser { get; }
        public ITimeCodeDocumentTimeValidator<string> TimeCodeDocumentTimeValidator { get; }

        public Task<ITimeCodeDocument<string>> GetSubtitlesAsync(string str)
        {
            return Task.Run(() =>
            {
                var tcd = this.TimeCodeDocumentParser.Parse(str);
                tcd = this.TimeCodeDocumentTimeValidator.EnsureValidTimes(tcd);
                return tcd;
            });
        }
    }
}
