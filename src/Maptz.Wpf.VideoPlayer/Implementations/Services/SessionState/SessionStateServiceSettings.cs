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

    public class SessionStateServiceSettings
    {
        public string SessionStateFilePath { get; set; } = @"%USERPROFILE%/Maptz/QuickVideoPlayer/sessionstate.json";
    }
}