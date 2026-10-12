using Core.Events;
using Core.Base;
using UnityEngine;

namespace Gameplay
{
    public class OnGamePauseManager : DisposableService
    {
        private readonly GlobalEventsService _globalEventsService;

        private bool _isPaused;
        private float _timeScaleBeforePause = 1f;

        public OnGamePauseManager(GlobalEventsService globalEventsService)
        {
            _globalEventsService = globalEventsService;
        }

        protected override void OnInitialize()
        {
            _globalEventsService.OnGameOverlayStarted += OnGamePaused;
        }

        protected override void OnDisposing()
        {
            _globalEventsService.OnGameOverlayStarted -= OnGamePaused;
        }

        private void OnGamePaused(bool isPaused)
        {
            if (isPaused == _isPaused)
                return;

            _isPaused = isPaused;

            if (isPaused)
            {
                _timeScaleBeforePause = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = _timeScaleBeforePause;
            }
        }
    }
}
