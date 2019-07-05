using System;

namespace Maptz.QuickVideoPlayer.Services
{
    public interface IApp
    {
        IServiceProvider ServiceProvider
        {
            get;
        }
    }
}