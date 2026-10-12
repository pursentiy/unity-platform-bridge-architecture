using System;
using System.Threading;
using Core.Events;
using Core.Logging;
using Cysharp.Threading.Tasks;

namespace Services.Overlay
{
    // Runs one platform overlay (ad, share dialog, invite dialog) at a time:
    // pauses the game, watches a loading and a hard timeout, resolves exactly once, unpauses.
    public class OverlayTaskRunner
    {
        private readonly GlobalEventsService _globalEventsService;
        private readonly string _ownerName;

        private bool _isResolved;
        private bool _hasOverlayStarted;
        private OverlayState _overlayState;

        private CancellationTokenSource _timeoutCts;
        private UniTaskCompletionSource<bool> _currentCompletionSource;

        public OverlayTaskRunner(GlobalEventsService globalEventsService, string ownerName)
        {
            _globalEventsService = globalEventsService;
            _ownerName = ownerName;
        }

        public bool IsBusy => _overlayState == OverlayState.Loading || _overlayState == OverlayState.Showing;

        public UniTask<bool> Run(Func<UniTask<bool>> getTask, float loadingTimeoutDuration, float hardTimeoutDuration)
        {
            if (IsBusy)
                return UniTask.FromResult(false);

            ResetState();
            SetPause(true);
            _overlayState = OverlayState.Loading;

            _timeoutCts = new CancellationTokenSource();
            WatchTimeout(loadingTimeoutDuration, _timeoutCts.Token, onlyWhileLoading: true, "Loading").Forget();
            WatchTimeout(hardTimeoutDuration, _timeoutCts.Token, onlyWhileLoading: false, "Hard").Forget();

            try
            {
                var task = getTask?.Invoke();
                if (task == null)
                {
                    Finalize(false);
                    return _currentCompletionSource.Task;
                }

                _hasOverlayStarted = true;
                _overlayState = OverlayState.Showing;
                ObserveTask(task.Value).Forget();
            }
            catch (Exception e)
            {
                LoggerService.LogWarning($"{_ownerName} SDK Exception: {e.Message}");
                Finalize(false);
            }

            return _currentCompletionSource.Task;
        }

        public void Cancel()
        {
            Finalize(false);
        }

        private async UniTaskVoid ObserveTask(UniTask<bool> task)
        {
            try
            {
                var result = await task;
                Finalize(result);
            }
            catch (Exception e)
            {
                LoggerService.LogWarning($"{_ownerName} SDK Task Error: {e.Message}");
                Finalize(false);
            }
        }

        private async UniTaskVoid WatchTimeout(float duration, CancellationToken token, bool onlyWhileLoading, string timeoutLabel)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: true, cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (_isResolved || (onlyWhileLoading && _hasOverlayStarted))
                return;

            LoggerService.LogWarning($"<color=orange>{_ownerName}</color> {timeoutLabel} timeout reached.");
            Finalize(false);
        }

        private void Finalize(bool result)
        {
            if (_isResolved)
                return;

            _isResolved = true;
            _overlayState = OverlayState.Finished;

            _timeoutCts?.Cancel();
            _timeoutCts?.Dispose();
            _timeoutCts = null;

            SetPause(false);
            _currentCompletionSource?.TrySetResult(result);
        }

        private void ResetState()
        {
            _timeoutCts?.Cancel();
            _timeoutCts?.Dispose();
            _timeoutCts = null;

            _currentCompletionSource?.TrySetResult(false);
            _currentCompletionSource = new UniTaskCompletionSource<bool>();

            _isResolved = false;
            _hasOverlayStarted = false;
        }

        private void SetPause(bool isPaused)
        {
            _globalEventsService.DispatchGameOverlayStarted(isPaused);
        }

        private enum OverlayState
        {
            Disabled,
            Finished,
            Loading,
            Showing
        }
    }
}
