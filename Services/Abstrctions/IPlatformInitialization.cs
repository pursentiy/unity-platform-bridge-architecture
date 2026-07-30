using System.Collections;
using Services.BridgeRelatedServices;
using Services.BridgeRelatedServices.Platform.Enums;

namespace Services.BridgeRelatedServices.Platform.Abstractions
{
    public interface IPlatformInitialization
    {
        IEnumerator Initialize();
        PlatformType PlatformType { get; }
        bool IsInitialized { get; }
        bool CheckPlatform(PlatformType platformType);
    }
}
