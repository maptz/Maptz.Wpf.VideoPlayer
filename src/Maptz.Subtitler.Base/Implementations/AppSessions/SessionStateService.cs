using System;
using System.IO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Maptz.QuickVideoPlayer.Services
{
    public class SessionStateService : ISessionStateService
    {
        public SessionStateService(IOptions<SessionStateServiceSettings> settings)
        {
            this.Settings = settings.Value;
        }

        public SessionStateServiceSettings Settings
        {
            get;
        }

        private FileInfo GetSessionStateFileInfo()
        {
            if (string.IsNullOrEmpty(this.Settings.SessionStateFilePath))
                throw new Exception("SessionStateFilePath invalid");
            var filePath = Environment.ExpandEnvironmentVariables(this.Settings.SessionStateFilePath);
            return new FileInfo(filePath);
        }

        public SessionState GetLastSessionState()
        {
            var fi = this.GetSessionStateFileInfo();
            if (!fi.Exists)
                return new SessionState();
            string json;
            using (var sr = fi.OpenText())
            {
                json = sr.ReadToEnd();
            }


            var retval = JsonConvert.DeserializeObject<SessionState>(json);
            //retval.LastOpenProjectPath = @"C:\Users\steph\OneDrive\Desktop\SUBTITLES\PULLS.IV.4000.04.02.json";
            return retval;
        }

        public void SaveSessionState(SessionState sessionState)
        {
            var fi = this.GetSessionStateFileInfo();
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            var json = JsonConvert.SerializeObject(sessionState);
            using (var sw = fi.CreateText())
            {
                sw.Write(json);
            }
        }
    }
}