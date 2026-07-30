using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Services.BridgeRelatedServices.Platform.Abstractions;
using Services.Base;

namespace Services.BridgeRelatedServices
{
    public class AdsService : DisposableService
    {
        private const float AdLoadingTimeoutDuration = 4f;
        private const float InterstitialAdHardTimeoutDuration = 35f;
        private const float RewardedAdHardTimeoutDuration = 90f;

        private readonly IPlatformAds _platformAds;
        private readonly GlobalEventsService _globalEventsService;

        private bool _isResolved;
        private bool _hasOverlayStarted;
        private OverlayState _overlayState;

        private CancellationTokenSource _timeoutCts;
        private UniTaskCompletionSource<bool> _currentCompletionSource;

        public AdsService(IPlatformAds platformAds, GlobalEventsService globalEventsService)
        {
            _platformAds = platformAds;
            _globalEventsService = globalEventsService;
        }

        public bool CanShowPrerollAd() => _platformAds.CanShowPrerollAd();

        public void ShowBanner()
        {
            if (_platformAds.IsBannerSupported)
                _platformAds.ShowBanner();
        }

        public bool IsInterstitialCooldownPassed() => _platformAds.IsInterstitialCooldownPassed();

        public UniTask<bool> ShowInterstitial(string placement = "default")
        {
            if (!_platformAds.IsInterstitialSupported)
                return UniTask.FromResult(false);
            if (!_platformAds.IsInterstitialCooldownPassed())
                return UniTask.FromResult(false);

            return ExecuteOverlayTask(
                () => _platformAds.ShowInterstitial(placement),
                AdLoadingTimeoutDuration,
                InterstitialAdHardTimeoutDuration);
        }

        public UniTask<bool> ShowRewardedAd(string placement = "default")
        {
            return ExecuteOverlayTask(
                () => _platformAds.ShowRewardedAd(placement),
                AdLoadingTimeoutDuration,
                RewardedAdHardTimeoutDuration);
        }

        protected override void OnInitialize() { }

        protected override void OnDisposing()
        {
            FinalizeOverlayFlags(false);
        }

        public UniTask<bool> ExecuteOverlayTask(Func<UniTask<bool>> getTask, float loadingTimeoutDuration, float hardTimeoutDuration = 30f)
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

        public UniTask<bool> SetOverlayAction(Action action, float loadingTimeoutDuration, float hardTimeoutDuration = 30f)
        {
            return ExecuteOverlayTask(() =>
            {
                action.Invoke();
                return UniTask.Never<bool>(CancellationToken.None);
            }, loadingTimeoutDuration, hardTimeoutDuration);
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
