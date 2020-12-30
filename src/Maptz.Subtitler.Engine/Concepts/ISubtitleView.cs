using Maptz.Editing.TimeCodeDocuments;
using System;
using System.Collections.Generic;
namespace Maptz.Subtitler.Wpf.App
{
    public interface ISubtitleView : IDisposable
    {
        IEnumerable<ITimeCodeDocumentItem<string>> SubtitleItems { get; }
        void RequestSubtitleUpdate();
    }
}