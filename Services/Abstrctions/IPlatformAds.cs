using Cysharp.Threading.Tasks;

namespace Services.BridgeRelatedServices.Platform.Abstractions
{
    public interface IPlatformAds
    {
        bool IsBannerSupported { get; }
        bool CanShowPrerollAd();
        bool IsInterstitialCooldownPassed();
        bool IsInterstitialSupported { get; }
        bool IsRewardedSupported { get; }

        void ShowBanner();
        UniTask<bool> ShowInterstitial(string placement = "default");
        UniTask<bool> ShowRewardedAd(string placement = "default");
    }
}
