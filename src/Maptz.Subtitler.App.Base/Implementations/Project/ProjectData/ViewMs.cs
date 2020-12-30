using Maptz.Subtitler.App;
using System;
using System.ComponentModel;
namespace Maptz.Subtitler.App.Projects
{



    public class ViewMs : IViewMs, INotifyPropertyChanged
    {
        /* #region Private Fields */
        private long _endMs = 0;
        private long _maxEndMs;
        private long _maxStartMs;
        private long _startMs = 0;
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
        public long EndMs
        {
            get => this._endMs;
            set
            {
                if (this._endMs != value)
                {
                    var old = this._endMs;
                    this._endMs = value;
                    this.OnPropertyChanged(nameof(EndMs), old, value);
                }
            }
        }
        public long MaxEndMs
        {
            get => this._maxEndMs;
            set
            {
                var oldValue = this._maxEndMs;
                if (this._maxEndMs != value)
                {
                    this._maxEndMs = value;
                    this.OnPropertyChanged(nameof(MaxEndMs), oldValue, value);
                }
            }
        }
        public long MaxStartMs
        {
            get => this._maxStartMs;
            set
            {
                var oldValue = this._maxStartMs;
                if (this._maxStartMs != value)
                {
                    this._maxStartMs = value;
                    this.OnPropertyChanged(nameof(MaxStartMs), oldValue, value);
                }
            }
        }
        public long StartMs
        {
            get => this._startMs;
            set
            {
                if (this._startMs != value)
                {
                    var old = this._startMs;
                    this._startMs = value;
                    this.OnPropertyChanged(nameof(StartMs), old, value);
                }
            }
        }
        /* #endregion Public Properties */
    }
}