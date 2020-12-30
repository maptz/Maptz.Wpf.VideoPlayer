using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Maptz.Subtitler.App.SessionState
{
    /// <summary>
    /// Used to persist SessionState objects. 
    /// </summary>
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

            var retval = Newtonsoft.Json.JsonConvert.DeserializeObject<SessionState>(json);
            //var retval = JsonSerializer.Deserialize<SessionState>(json);
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

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(sessionState);
            //var json = JsonSerializer.Serialize(sessionState);
            using (var sw = fi.CreateText())
            {
                sw.Write(json);
            }
        }
    }
}