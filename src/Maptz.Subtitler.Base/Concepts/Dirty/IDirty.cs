using System;

namespace Maptz.QuickVideoPlayer
{
    public interface IDirty
    {
        bool IsDirty
        {
            get;
        }

        event EventHandler HasBecomeDirty;
    }
}