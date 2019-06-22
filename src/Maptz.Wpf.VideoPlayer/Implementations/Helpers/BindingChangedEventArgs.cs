using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Maptz.QuickVideoPlayer.Commands;
using Maptz.Tiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unosquare.FFME;
using Unosquare.FFME.Common;
namespace Maptz.QuickVideoPlayer
{
    public class BindingChangedEventArgs : EventArgs
    {
        public BindingChangedEventArgs(object oldValue, object newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public object OldValue { get; }
        public object NewValue { get; }
    }

    public class BindingChangedEventArgs<T> : BindingChangedEventArgs
    {
        public BindingChangedEventArgs(T oldValue, T newValue) : base(oldValue, newValue)
        {

        }

        public new T OldValue { get => (T)this.OldValue; }
        public new T NewValue { get => (T)this.NewValue; }
    }
}