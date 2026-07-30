using Services.Base;

namespace Services.BridgeRelatedServices.Platform.Abstractions
{
    public interface IPlatformLifecycle : IDisposableService
    {
        void SetGameReady();
        void StartGameplay();
        void StopGameplay();
    }
}
