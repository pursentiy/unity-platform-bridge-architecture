using Core.Base;
using Platform.Abstractions;

namespace Platform.Mocks
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
