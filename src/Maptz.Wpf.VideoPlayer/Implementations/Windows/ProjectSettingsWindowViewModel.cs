using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace Maptz.QuickVideoPlayer
{

    public class ProjectSettingsWindowViewModel : INotifyPropertyChanged
    {
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


        private string _timeCode;
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



        private string _frameRate;
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
                }
            }
        }
        public IEnumerable<string> AvailableFrameRates
        {
            get
            {
                var availabeFrameRates = Enum.GetNames(typeof(SmpteFrameRate));
                return availabeFrameRates;
            }
        }
    }
}