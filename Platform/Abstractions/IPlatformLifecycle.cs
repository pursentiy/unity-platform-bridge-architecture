using Core.Base;

namespace Platform.Abstractions
{
    public interface IPlatformLifecycle : IDisposableService
    {
        void SetGameReady();
        void StartGameplay();
        void StopGameplay();
    }
}
