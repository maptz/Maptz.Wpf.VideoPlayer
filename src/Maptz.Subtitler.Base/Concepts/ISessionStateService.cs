namespace Maptz.QuickVideoPlayer.Services
{
    public interface ISessionStateService
    {
        SessionState GetLastSessionState();
        void SaveSessionState(SessionState sessionState);
    }
}