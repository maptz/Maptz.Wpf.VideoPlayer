namespace Maptz.Subtitler.App.SessionState
{
    public interface ISessionStateService
    {
        SessionState GetLastSessionState();
        void SaveSessionState(SessionState sessionState);
    }
}