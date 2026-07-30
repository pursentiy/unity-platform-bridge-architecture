using System.Collections;
using Services.BridgeRelatedServices.Platform.Abstractions;
using Services.BridgeRelatedServices.Platform.Enums;

namespace Services.BridgeRelatedServices.Platform.Mock
{
    public class MockPlatformInitialization : IPlatformInitialization
    {
        public IEnumerator Initialize() { yield break; }
        public PlatformType PlatformType => PlatformType.Mock;
        public bool IsInitialized => true;
        public bool CheckPlatform(PlatformType platformType) => platformType == PlatformType.Mock;
    }
}
