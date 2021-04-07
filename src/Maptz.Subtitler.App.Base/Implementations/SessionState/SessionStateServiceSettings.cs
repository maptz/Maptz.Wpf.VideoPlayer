namespace Maptz.Subtitler.App.SessionState
{
    public class SessionStateServiceSettings
    {
        public string SessionStateFilePath
        {
            get;
            set;
        }

        = @"%USERPROFILE%/Maptz/Maptz.Subtitler.App/sessionstate.json";
    }
}