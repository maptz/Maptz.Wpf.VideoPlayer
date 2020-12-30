using System.ComponentModel;

namespace Maptz.Subtitler.App.Projects
{

    public class ProjectSettingsBase : DirtyBase, IProjectSettings
    {
        private SmpteFrameRate _frameRate = SmpteFrameRate.Smpte25;
        public SmpteFrameRate FrameRate
        {
            get => this._frameRate;
            set
            {
                var oldValue = this._frameRate;
                if (this._frameRate != value)
                {
                    this._frameRate = value;
                    this.OnPropertyChanged(nameof(FrameRate), oldValue, value);
                    this.SetDirty();
                }
            }
        }

        public TimeCode OffsetTimeCode
        {
            get
            {
                var ret = TimeCode.FromFrames(this.OffsetFrames, this.FrameRate);
                return ret;
            }
        }

        private long _offsetFrames;
        public long OffsetFrames
        {
            get => this._offsetFrames;
            set
            {
                var oldValue = this._offsetFrames;
                if (this._offsetFrames != value)
                {
                    this._offsetFrames = value;
                    this.OnPropertyChanged(nameof(OffsetFrames), oldValue, value);
                    this.OnPropertyChanged(nameof(OffsetTimeCode), default, default(TimeCode));
                    this.SetDirty();
                }
            }
        }



        public ProjectSettingsBase()
        {
        }
    }
}