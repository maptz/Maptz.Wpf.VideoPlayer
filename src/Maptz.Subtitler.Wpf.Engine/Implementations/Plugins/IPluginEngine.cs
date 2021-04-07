using Maptz.Subtitler.App.Plugins;
using System.Collections.Generic;
namespace Maptz.Subtitler.Wpf.Engine.Plugins
{
    public interface IPluginEngine
    {
        IEnumerable<IPluginInstance> LoadPlugins();
    }
}