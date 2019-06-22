using System;
using System.ComponentModel;
using Unosquare.FFME;

namespace Maptz.QuickVideoPlayer
{


    public class VideoPlayerState : INotifyPropertyChanged
    {

        public MediaElement MediaElement { get;  }

        public VideoPlayerState(MediaElement mediaElement)
        {
            this.MediaElement = mediaElement;
        }

        public TimeSpan Position { get
            {
                return this.MediaElement.Position;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return this.MediaElement.IsPlaying;
            }
        }

        public bool IsPaused
        {
            get
            {
                return this.MediaElement.IsPaused;
            }
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