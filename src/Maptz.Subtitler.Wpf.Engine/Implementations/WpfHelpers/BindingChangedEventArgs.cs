using System;
namespace Maptz.Subtitler.Wpf.Engine
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