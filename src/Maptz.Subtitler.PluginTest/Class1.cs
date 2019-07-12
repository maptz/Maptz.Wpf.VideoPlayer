using Maptz.QuickVideoPlayer;
using Maptz.QuickVideoPlayer.Commands;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Maptz.Subtitler.PluginTest
{
    //https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
    //    <ItemGroup>
    //<ProjectReference Include = "..\PluginBase\PluginBase.csproj" >
    //    < Private > false </ Private >
    //</ ProjectReference >
    //</ ItemGroup >
    public class TestPluginInstance : IPluginInstance
    {
        public IEnumerable<IAppCommand> Commands => new IAppCommand[]
        {
            this.DoSomethingCommand
        };

        public IServiceProvider ServiceProvider { get; }

        public TestPluginInstance(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IAppCommand DoSomethingCommand => new AppCommand("DoSomething", (object o) => this.DoSomething(), new KeyChords(new KeyChord(Key.R, ctrl: true, shift: true)));

        private void DoSomething()
        {
            MessageBox.Show("This is the plugin running");
        }
    }
}
