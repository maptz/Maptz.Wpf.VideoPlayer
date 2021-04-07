using Maptz.Subtitler.Wpf.App;
using Maptz.Subtitler.Wpf.Controls;
using Maptz.Subtitler.Wpf.VideoPlayer.Commands;
using System.ComponentModel;

namespace Maptz.Subtitler.Wpf.Engine
{

    public class AppState : INotifyPropertyChanged
    {
        public WrappedTextBox TextBox
        {
            get;
            set;
        }

        private VideoPlayerState _videoPlayerState = new VideoPlayerState();
        public VideoPlayerState VideoPlayerState
        {
            get => this._videoPlayerState;
            set
            {
                var oldValue = this._videoPlayerState;
                if (this._videoPlayerState != value)
                {
                    this._videoPlayerState = value;
                    this.OnPropertyChanged(nameof(VideoPlayerState), oldValue, value);
                }
            }
        }

        private Project _project;
        public Project Project
        {
            get => this._project;
            set
            {
                var oldValue = this._project;
                if (this._project != value)
                {
                    this._project = value;
                    this.OnPropertyChanged(nameof(Project), oldValue, value);
                }
            }
        }

        public AppState()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged<T>(string propertyName, T oldValue, T newValue)
        {
            this.RaisePropertyChanged(propertyName);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}