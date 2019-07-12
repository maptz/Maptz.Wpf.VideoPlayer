
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.Editing.TimeCodeDocuments;
using Maptz.QuickVideoPlayer.Services;
using Maptz.Subtitler.Wpf.Controls.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
namespace Maptz.QuickVideoPlayer
{

    public class SubtitleHoverEventArgs : EventArgs
    {
        public ITimeCodeDocumentItem<string> Item { get; set; }
    }
}