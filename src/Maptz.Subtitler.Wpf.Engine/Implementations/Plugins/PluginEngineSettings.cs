using Maptz.QuickVideoPlayer;
using Maptz.Subtitler.App.Plugins;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
namespace Maptz.Subtitler.Wpf.Engine.Plugins
{

    public class PluginEngineSettings
    {

        public List<string> PluginPaths { get; set; }
    }
}