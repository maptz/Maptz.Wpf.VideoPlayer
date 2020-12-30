using System.ComponentModel;
namespace Maptz.Subtitler.App.Projects
{

    public interface IProjectSettings : INotifyPropertyChanged, IDirty
    {
        SmpteFrameRate FrameRate { get; set; }
        TimeCode OffsetTimeCode { get;  }
        long OffsetFrames { get; set; }
    }
}