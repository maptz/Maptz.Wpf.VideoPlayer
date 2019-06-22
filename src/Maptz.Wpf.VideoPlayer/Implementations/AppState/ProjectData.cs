using Maptz.Tiles;
using System;
using System.ComponentModel;
namespace Maptz.QuickVideoPlayer
{

    public class ProjectData : DirtyBase
    {

        private long? _markInMs;
        public long? MarkInMs
        {
            get => this._markInMs;
            set
            {
                var oldValue = this._markInMs;
                if (this._markInMs != value)
                {
                    this._markInMs = value;
                    this.OnPropertyChanged(nameof(MarkInMs), oldValue, value);
                }
            }
        }


        private long? _cursorMs = 0;
        public long? CursorMs
        {
            get => this._cursorMs;
            set
            {
                var ov = this._cursorMs;
                if (this._cursorMs != value)
                {
                    this._cursorMs = value;
                    this.OnPropertyChanged(nameof(CursorMs), ov, value);
                }
            }
        }


        private IViewMs _viewMs = new ViewMs();
        public IViewMs ViewMs
        {
            get => this._viewMs;
            set
            {
                var ov = this._viewMs;
                if (this._viewMs != value)
                {
                    this._viewMs = value;
                    this.OnPropertyChanged(nameof(ViewMs), ov, value);
                }
            }
        }

        private string _text;
        public string Text
        {
            get => this._text;
            set
            {
                var oldValue = this._text;
                if (this._text != value)
                {
                    this._text = value;
                    this.OnPropertyChanged(nameof(Text), oldValue, value);
                    this.SetDirty();
                }
            }
        }


        public ProjectData()
        {

        }


    }
}