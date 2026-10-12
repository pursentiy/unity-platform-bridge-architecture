using Core.Events;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Platform.Abstractions;
using Core.Base;
using Services.Overlay;

namespace Services
{
    public class AdsService : DisposableService
    {
        private const float AdLoadingTimeoutDuration = 4f;
        private const float InterstitialAdHardTimeoutDuration = 35f;
        private const float RewardedAdHardTimeoutDuration = 90f;

        private readonly IPlatformAds _platformAds;
        private readonly OverlayTaskRunner _overlayRunner;

        public AdsService(IPlatformAds platformAds, GlobalEventsService globalEventsService)
        {
            _platformAds = platformAds;
            _overlayRunner = new OverlayTaskRunner(globalEventsService, GetType().Name);
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

            return _overlayRunner.Run(
                () => _platformAds.ShowInterstitial(placement),
                AdLoadingTimeoutDuration,
                InterstitialAdHardTimeoutDuration);
        }

        public UniTask<bool> ShowRewardedAd(string placement = "default")
        {
            if (!_platformAds.IsRewardedSupported)
                return UniTask.FromResult(false);

            return _overlayRunner.Run(
                () => _platformAds.ShowRewardedAd(placement),
                AdLoadingTimeoutDuration,
                RewardedAdHardTimeoutDuration);
        }

        protected override void OnInitialize() { }

        protected override void OnDisposing()
        {
            _overlayRunner.Cancel();
        }

        public UniTask<bool> SetOverlayAction(Action action, float loadingTimeoutDuration, float hardTimeoutDuration = 30f)
        {
            return _overlayRunner.Run(() =>
            {
                action.Invoke();
                return UniTask.Never<bool>(CancellationToken.None);
            }, loadingTimeoutDuration, hardTimeoutDuration);
        }
    }
}
