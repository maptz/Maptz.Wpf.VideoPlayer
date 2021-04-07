using Maptz.Subtitler.App.Plugins;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Maptz.Subtitler.Wpf.Engine.Plugins
{

    public class PluginEngine : IPluginEngine
    {
        /* #region Private Methods */
        private Assembly LoadPluginAssembly(string path)
        {
            PluginLoadContext loadContext = new PluginLoadContext(path);
            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
            return assembly;
        }
        /* #endregion Private Methods */
        /* #region Public Properties */
        public IServiceProvider ServiceProvider { get; }
        public PluginEngineSettings Settings
        {
            get;
        }
        /* #endregion Public Properties */
        /* #region Public Constructors */
        public PluginEngine(IOptions<PluginEngineSettings> settings, IServiceProvider serviceProvider)
        {
            this.Settings = settings.Value;
            ServiceProvider = serviceProvider;
        }
        /* #endregion Public Constructors */
        /* #region Public Methods */
        public IEnumerable<IPluginInstance> CreatePluginInstances(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPluginInstance).IsAssignableFrom(type))
                {
                    var ctors = type.GetConstructors();
                    var spCtor = ctors.FirstOrDefault(p =>
                    {
                        var parms = p.GetParameters();
                        if (parms.Count() != 1) return false;
                        var parm = parms.First();
                        return parm.ParameterType == typeof(IServiceProvider);
                    });
                    if (spCtor == null)
                    {
                        spCtor = ctors.FirstOrDefault(p => p.GetParameters().Count() == 0);
                    }
                    if (spCtor == null)
                    {
                        throw new Exception($"No valid constructor found on type {type}");
                    }

                    IPluginInstance result = Activator.CreateInstance(type, this.ServiceProvider) as IPluginInstance;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IPluginInstance in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
        /* #endregion Public Methods */
        /* #region Interface: 'Maptz.Subtitler.Wpf.Engine.Plugins.IPluginEngine' Methods */
        public IEnumerable<IPluginInstance> LoadPlugins()
        {
            if (this.Settings.PluginPaths == null) return new IPluginInstance[0];
            List<IPluginInstance> pluginInstances = new List<IPluginInstance>();
            foreach (var path in this.Settings.PluginPaths)
            {
                if (File.Exists(path))
                {
                    var ass = LoadPluginAssembly(path);
                    var instances = CreatePluginInstances(ass);
                    pluginInstances.AddRange(instances);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Warning cannot find plugin at path '{path}'.");
                }

            }

            return pluginInstances;
        }
        /* #endregion Interface: 'Maptz.Subtitler.Wpf.Engine.Plugins.IPluginEngine' Methods */
    }
}