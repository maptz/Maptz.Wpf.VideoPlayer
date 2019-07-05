using System;
using System.ComponentModel;
using Unosquare.FFME;

namespace Maptz.QuickVideoPlayer
{


    public class VideoPlayerState : INotifyPropertyChanged
    {
        /* #region Private Methods */
        private void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /* #endregion Private Methods */
        /* #region Protected Methods */
        protected virtual void OnPropertyChanged<T>(string propertyName, T oldValue, T newValue)
        {
            this.RaisePropertyChanged(propertyName);
        }
        /* #endregion Protected Methods */
        /* #region Public Delegates */
        public event PropertyChangedEventHandler PropertyChanged;
        /* #endregion Public Delegates */
        /* #region Public Properties */
        public bool IsPaused => this.MediaElement.IsPaused;
        public bool IsPlaying => this.MediaElement.IsPlaying;
        public MediaElement MediaElement { get;  }
        public TimeSpan Position => this.MediaElement.Position;


        private double _speedRatio = 1.0;
        public double SpeedRatio
        {
            get => this._speedRatio;
            set
            {
                var oldValue = this._speedRatio;
                if (this._speedRatio != value)
                {
                    this._speedRatio = value;
                    this.OnPropertyChanged(nameof(SpeedRatio), oldValue, value);
                }
            }
        }

        /* #endregion Public Properties */
        /* #region Public Constructors */
        public VideoPlayerState(MediaElement mediaElement)
        {
            this.MediaElement = mediaElement;
        }
        /* #endregion Public Constructors */
    }
}