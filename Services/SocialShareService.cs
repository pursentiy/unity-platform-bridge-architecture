using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Services.BridgeRelatedServices.Platform.Abstractions;
using Services.Base;

namespace Services.BridgeRelatedServices
{
    public class SocialShareService : DisposableService
    {
        private const float ShareTimeoutDuration = 3f;
        private const float InviteFriendsHardTimeoutDuration = 600f;
        private const float ShareResultHardTimeoutDuration = 300f;

        private readonly IPlatformSocial _platformSocial;
        private readonly GlobalEventsService _globalEventsService;

        private bool _isResolved;
        private bool _hasOverlayStarted;
        private OverlayState _overlayState;

        private CancellationTokenSource _timeoutCts;
        private UniTaskCompletionSource<bool> _currentCompletionSource;

        public SocialShareService(IPlatformSocial platformSocial, GlobalEventsService globalEventsService)
        {
            _platformSocial = platformSocial;
            _globalEventsService = globalEventsService;
        }

        public bool IsShareSupported()
        {
            return _platformSocial.IsShareSupported;
        }

        public bool IsInviteSupported()
        {
            return _platformSocial.IsInviteSupported;
        }

        public UniTask<bool> ShareResult(string shareMessage, CancellationToken cancellationToken)
        {
            if (!IsShareSupported())
                return UniTask.FromResult(false);

            return ExecuteOverlayTask(
                () => _platformSocial.ShareResult(shareMessage, cancellationToken),
                ShareTimeoutDuration,
                ShareResultHardTimeoutDuration);
        }

        public UniTask<bool> InviteFriends(CancellationToken cancellationToken)
        {
            if (!IsInviteSupported())
                return UniTask.FromResult(false);

            return ExecuteOverlayTask(
                () => _platformSocial.InviteFriends(cancellationToken),
                ShareTimeoutDuration,
                InviteFriendsHardTimeoutDuration);
        }

        protected override void OnInitialize() { }

        protected override void OnDisposing()
        {
            FinalizeOverlayFlags(false);
        }

        private UniTask<bool> ExecuteOverlayTask(Func<UniTask<bool>> getTask, float loadingTimeoutDuration, float hardTimeoutDuration = 30f)
        {
            if (_overlayState == OverlayState.Showing)
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
                    FinalizeOverlayFlags(false);
                    return _currentCompletionSource.Task;
                }

                _hasOverlayStarted = true;
                ObserveTask(task.Value).Forget();
            }
            catch (Exception e)
            {
                LoggerService.LogWarning($"{GetType().Name} SDK Exception: {e.Message}");
                FinalizeOverlayFlags(false);
            }

            return _currentCompletionSource.Task;
        }

        private async UniTaskVoid ObserveTask(UniTask<bool> task)
        {
            try
            {
                var result = await task;
                FinalizeOverlayFlags(result);
            }
            catch (Exception e)
            {
                LoggerService.LogWarning($"{GetType().Name} SDK Task Error: {e.Message}");
                FinalizeOverlayFlags(false);
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

            LoggerService.LogWarning($"<color=orange>{GetType().Name}</color> {timeoutLabel} timeout reached.");
            FinalizeOverlayFlags(false);
        }

        private void FinalizeOverlayFlags(bool result)
        {
            if (_isResolved)
                return;

            _isResolved = true;
            _overlayState = OverlayState.Finished;
            _currentCompletionSource?.TrySetResult(result);

            _timeoutCts?.Cancel();
            _timeoutCts?.Dispose();
            _timeoutCts = null;
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
            _globalEventsService.OnGameOverlayStartedSignal.Dispatch(isPaused);
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
