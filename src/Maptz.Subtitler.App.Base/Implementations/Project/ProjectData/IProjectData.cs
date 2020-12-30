using System.ComponentModel;
namespace Maptz.Subtitler.App.Projects
{
    public interface IProjectData : INotifyPropertyChanged, IDirty
    {
        string Text { get; set; }
    }
}