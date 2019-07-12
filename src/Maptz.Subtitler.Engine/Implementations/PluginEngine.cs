using Maptz.QuickVideoPlayer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Maptz.Subtitler.Engine.Implementations
{
    public interface IPluginEngine
    {
        IEnumerable<IPluginInstance> LoadPlugins();
    }


    class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }

    public class PluginEngine : IPluginEngine
    {


        private Assembly LoadPluginAssembly(string path)
        {
            PluginLoadContext loadContext = new PluginLoadContext(path);
            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
            return assembly;
        }

        public IEnumerable<IPluginInstance> LoadPlugins()
        {
            if (this.Settings.PluginPaths == null) return new IPluginInstance[0];
            List<IPluginInstance> pluginInstances = new List<IPluginInstance>();
            foreach (var path in this.Settings.PluginPaths)
            {
                var ass = LoadPluginAssembly(path);
                var instances = CreatePluginInstances(ass);
                pluginInstances.AddRange(instances);
            }

            return pluginInstances;
        }

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

        public PluginEngine(IOptions<PluginEngineSettings> settings, IServiceProvider serviceProvider)
        {
            this.Settings = settings.Value;
            ServiceProvider = serviceProvider;
        }

        public PluginEngineSettings Settings
        {
            get;
        }
        public IServiceProvider ServiceProvider { get; }
    }

    public class PluginEngineSettings
    {

        public List<string> PluginPaths { get; set; }
    }
}