using Maptz.Editing.TimeCodeDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Maptz.QuickVideoPlayer
{
    public interface ISubtitleProvider
    {
        ITimeCodeDocument<string> GetSubtitles(string str);
    }
}