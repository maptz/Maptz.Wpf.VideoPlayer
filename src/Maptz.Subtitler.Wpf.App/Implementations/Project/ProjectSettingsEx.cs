using Maptz.Subtitler.Wpf.VideoPlayer.Projects;

namespace Maptz.Subtitler.App.Projects
{

    
    public class ProjectSettingsEx : ProjectSettingsBase, IVideoPlayerProjectSettings
    {
        private string _videoFilePath;
        public string VideoFilePath
        {
            get => this._videoFilePath;
            set
            {
                var oldValue = this._videoFilePath;
                if (this._videoFilePath != value)
                {
                    this._videoFilePath = value;
                    this.OnPropertyChanged(nameof(VideoFilePath), oldValue, value);
                    this.SetDirty();
                }
            }
        }
    }
}