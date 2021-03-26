using System;
using System.ComponentModel;
using Unosquare.FFME;
namespace Maptz.Subtitler.Wpf.VideoPlayer
{

    public interface IVideoPlayerState
    {
        bool IsPaused { get; }
        TimeSpan Position { get; }
        double SpeedRatio { get; set; }
        MediaElement MediaElement { get; set; }
        bool IsPlaying { get; }
    }
}