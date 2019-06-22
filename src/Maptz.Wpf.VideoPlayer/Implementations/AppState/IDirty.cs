using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
namespace Maptz.QuickVideoPlayer
{
    

    public interface IDirty
    {
        bool IsDirty { get; }
        event EventHandler HasBecomeDirty;
    }
}