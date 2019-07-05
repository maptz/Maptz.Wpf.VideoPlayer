namespace Maptz.QuickVideoPlayer.Services
{
    public class SessionStateServiceSettings
    {
        public string SessionStateFilePath
        {
            get;
            set;
        }

        = @"%USERPROFILE%/Maptz/QuickVideoPlayer/sessionstate.json";
    }
}