using System;
using Cysharp.Threading.Tasks;
using Services.BridgeRelatedServices.Platform.Abstractions;
using Services.BridgeRelatedServices.Platform.Enums;

namespace Services.BridgeRelatedServices.Platform.Mock
{
    public class MockPlatformEnvironment : IPlatformEnvironment
    {
        public PlatformDeviceType GetDeviceType() => PlatformDeviceType.Desktop;
        public string GetLanguageCode() => "en";
        public UniTask<DateTime?> GetServerTimeUtc() => UniTask.FromResult<DateTime?>(null);
        public bool IsVisibilitySupported() => false;
        public bool IsVisible() => true;
        public event Action<bool> VisibilityChanged { add { } remove { } }
    }
}
