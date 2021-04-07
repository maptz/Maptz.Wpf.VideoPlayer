using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace Maptz.Subtitler.Wpf.App
{

    public class ProjectSettingsWindowViewModel : INotifyPropertyChanged
    {
        /* #region Private Fields */
        private string _frameRate;
        private string _timeCode;
        private string _videoFilePath;
        /* #endregion Private Fields */
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
        public IEnumerable<string> AvailableFrameRates
        {
            get
            {
                var availabeFrameRates = Enum.GetNames(typeof(SmpteFrameRate));
                return availabeFrameRates;
            }
        }
        public string FrameRate
        {
            get => this._frameRate;
            set
            {
                var oldValue = this._frameRate;
                if (this._frameRate != value)
                {
                    this._frameRate = value;
                    this.OnPropertyChanged(nameof(FrameRate), oldValue, value);
                }
            }
        }
        public string TimeCode
        {
            get => this._timeCode;
            set
            {
                var oldValue = this._timeCode;
                if (this._timeCode != value)
                {
                    this._timeCode = value;
                    this.OnPropertyChanged(nameof(TimeCode), oldValue, value);
                }
            }
        }
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
                }
            }
        }
        /* #endregion Public Properties */
    }
}