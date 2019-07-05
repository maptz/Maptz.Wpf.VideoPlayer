using Maptz.Tiles;
using System;
using System.ComponentModel;
namespace Maptz.QuickVideoPlayer
{

    public interface IViewMs
    {
        long StartMs { get; set; }
        long EndMs { get; set; }
        long MaxStartMs { get; set; }
        long MaxEndMs { get; set; }
    }
}