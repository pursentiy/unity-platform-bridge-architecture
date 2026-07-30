using Extensions;
using Services.Base;
using UnityEngine;
using Utilities.Disposable;

namespace Services.BridgeRelatedServices
{
    public class OnGamePauseManager : DisposableService
    {
        private readonly GlobalEventsService _globalEventsService;

        public OnGamePauseManager(GlobalEventsService globalEventsService)
        {
            _globalEventsService = globalEventsService;
        }

        protected override void OnInitialize()
        {
            _globalEventsService.OnGameOverlayStartedSignal.MapListener(OnGamePaused).DisposeWith(this);
        }

        protected override void OnDisposing()
        {
            
        }

        private void OnGamePaused(bool isPaused)
        {
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }
}