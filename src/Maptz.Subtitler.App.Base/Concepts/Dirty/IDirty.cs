using System;

namespace Maptz.Subtitler.App
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