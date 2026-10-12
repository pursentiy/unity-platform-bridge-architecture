using System;

namespace Core.Events
{
    public class GlobalEventsService
    {
        public event Action<bool> OnGameOverlayStarted;

        public void DispatchGameOverlayStarted(bool isPaused)
        {
            OnGameOverlayStarted?.Invoke(isPaused);
        }
    }
}
