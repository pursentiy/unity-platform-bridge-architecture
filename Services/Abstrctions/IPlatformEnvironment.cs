using System;
using Cysharp.Threading.Tasks;
using Services.BridgeRelatedServices.Platform.Enums;

namespace Services.BridgeRelatedServices.Platform.Abstractions
{
    public interface IPlatformEnvironment
    {
        PlatformDeviceType GetDeviceType();
        string GetLanguageCode();
        UniTask<DateTime?> GetServerTimeUtc();

        bool IsVisibilitySupported();
        bool IsVisible();

        event Action<bool> VisibilityChanged;
    }
}
