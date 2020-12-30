using Maptz.Editing.TimeCodeDocuments;
using System;
namespace Maptz.Subtitler.Wpf.Controls.VideoPlayer
{

    public class SubtitleHoverEventArgs : EventArgs
    {
        public ITimeCodeDocumentItem<string> Item { get; set; }
    }
}