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

    public class ViewMs : IViewMs, INotifyPropertyChanged
    {


        private long _maxEndMs;
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
        private long _maxStartMs;
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

        private long _startMs = 0;
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

        private long _endMs = 0;
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

      

    
    }
}