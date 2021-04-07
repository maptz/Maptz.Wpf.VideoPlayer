using System;
using System.Windows;
using System.Windows.Data;

namespace Maptz.Subtitler.Wpf.Engine
{
    public class BindingWatcherBase : FrameworkElement
    {
        public event EventHandler<BindingChangedEventArgs> BindingChanged;
        public void RaiseBindingChanged<T>(T old, T newValue)
        {
            var bc = this.BindingChanged;
            if (bc != null)
                bc(this, new BindingChangedEventArgs<T>(old, newValue));
        }
    }

    public class BindingWatcher<T> : BindingWatcherBase
    {
        public BindingWatcher(object source, string path)
        {
            this.Source = source;
            this.Path = path;
            Binding myBinding = new Binding(path);
            myBinding.Source = source;
            this.SetBinding(BoundPropertyProperty, myBinding);
        }

        public object Source
        {
            get;
        }

        public string Path
        {
            get;
        }

        public T BoundProperty
        {
            get
            {
                return (T)GetValue(BoundPropertyProperty);
            }

            set
            {
                SetValue(BoundPropertyProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for BoundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoundPropertyProperty = DependencyProperty.Register("BoundProperty", typeof(T), typeof(BindingWatcher<T>), new PropertyMetadata(default(T), BoundPropertyPropertyChanged));
        private static void BoundPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BindingWatcher<T>).BoundPropertyChanged((T)e.OldValue, (T)e.NewValue);
        }

        public void BoundPropertyChanged(T oldValue, T newValue)
        {
            this.RaiseBindingChanged(oldValue, newValue);
        }
    }
}