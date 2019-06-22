using Maptz.Audio.WavFiles;
using Maptz.Audio.WavFiles.SamplePlotter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
namespace Maptz.QuickVideoPlayer
{
    public class AppSettings
    {
        public string StringSetting { get; set; }

        public int IntegerSetting { get; set; }

        public bool BooleanSetting { get; set; }
    }
}