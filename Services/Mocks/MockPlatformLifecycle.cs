using Services.Base;
using Services.BridgeRelatedServices.Platform.Abstractions;
using Utilities.Disposable;

namespace Services.BridgeRelatedServices.Platform.Mock
{
    public class MockPlatformLifecycle : DisposableService, IPlatformLifecycle
    {
        public void SetGameReady() { }
        public void StartGameplay() { }
        public void StopGameplay() { }
        
        protected override void OnInitialize()
        {
            
        }

        protected override void OnDisposing()
        {
            
        }
    }
}
