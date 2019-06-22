using System;
using System.ComponentModel;
namespace Maptz.QuickVideoPlayer
{

    public class DirtyBase : INotifyPropertyChanged, IDirty
    {
        protected void SetDirty(bool dirtyState = true)
        {
            if (dirtyState)
                this.IsDirty = dirtyState;
            else
            {
                //we're setting it to false, so let's set all the DirtyBase children 
                var props = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                foreach(var prop in props)
                {
                    if (typeof(DirtyBase).IsAssignableFrom(prop.PropertyType))
                    {
                        var pv = (DirtyBase) prop.GetValue(this);
                        if (pv != null) pv.SetDirty(dirtyState);
                    }
                }
                this.IsDirty = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged<T>(string propertyName, T oldValue, T newValue)
        {
            switch (oldValue)
            {
                case IDirty p: p.HasBecomeDirty -= OnChildHasBecomeDirty; break;
            }
            switch (newValue)
            {
                case IDirty p: p.HasBecomeDirty += OnChildHasBecomeDirty; break;
            }

            this.RaisePropertyChanged(propertyName);
        }

        private void OnChildHasBecomeDirty(object sender, EventArgs e)
        {
            this.IsDirty = true;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler HasBecomeDirty;
        private void RaiseIsDirtyChanged()
        {
            var idc = this.HasBecomeDirty;
            if (idc != null) idc(this, new EventArgs());
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get => this._isDirty;
            protected set
            {
                var oldValue = this._isDirty;
                if (this._isDirty != value)
                {
                    this._isDirty = value;
                    this.OnPropertyChanged(nameof(IsDirty), oldValue, value);
                    if (value)
                    {
                        this.RaiseIsDirtyChanged();
                    }
                }
            }
        }

     
    }
}