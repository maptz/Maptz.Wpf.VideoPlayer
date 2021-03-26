using Maptz.Editing.TimeCodeDocuments;
using System;
namespace Maptz.Subtitler.Wpf.VideoPlayer.Commands
{

    public class SubtitleHoverEventArgs : EventArgs
    {
        public ITimeCodeDocumentItem<string> Item { get; set; }
    }
}