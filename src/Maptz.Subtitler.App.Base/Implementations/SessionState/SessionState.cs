namespace Maptz.Subtitler.App.SessionState
{

    /// <summary>
    /// A serializable object that stores the state of the application.
    /// </summary>
    public class SessionState
    {
        public string OpenProjectDirectoryPath
        {
            get;
            set;
        }

        public string SaveProjectDirectoryPath
        {
            get;
            set;
        }

        public string OpenVideoFileDirectoryPath
        {
            get;
            set;
        }

        public string LastOpenProjectPath
        {
            get;
            set;
        }
    }
}