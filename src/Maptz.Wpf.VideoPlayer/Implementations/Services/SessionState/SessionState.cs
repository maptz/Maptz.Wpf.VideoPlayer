using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
namespace Maptz.QuickVideoPlayer.Services
{
    public class SessionState
    {
        public string OpenProjectDirectoryPath { get; set; }
        public string SaveProjectDirectoryPath { get; set; }
        public string OpenVideoFileDirectoryPath { get;  set; }
        public string LastOpenProjectPath { get; set; }
    }
}