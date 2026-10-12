using Cysharp.Threading.Tasks;
using Platform.Abstractions;

namespace Platform.Mocks
{
    public class MockPlatformAds : IPlatformAds
    {
        public bool IsBannerSupported => false;
        public bool CanShowPrerollAd() => false;
        public bool IsInterstitialCooldownPassed() => true;
        public bool IsInterstitialSupported => false;
        public bool IsRewardedSupported => false;

        public void ShowBanner() { }
        public UniTask<bool> ShowInterstitial(string placement = "default") => UniTask.FromResult(false);
        public UniTask<bool> ShowRewardedAd(string placement = "default") => UniTask.FromResult(false);
    }
}
