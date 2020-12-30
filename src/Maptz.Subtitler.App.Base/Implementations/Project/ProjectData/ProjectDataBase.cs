using System.ComponentModel;

namespace Maptz.Subtitler.App.Projects
{

    public class ProjectDataBase : DirtyBase, IProjectData
    {
        private string _text = string.Empty;
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
    }
}