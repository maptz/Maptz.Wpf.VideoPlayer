using System;

namespace Maptz.Subtitler.App
{
    public interface IApp
    {
        IServiceProvider ServiceProvider
        {
            get;
        }
    }
}