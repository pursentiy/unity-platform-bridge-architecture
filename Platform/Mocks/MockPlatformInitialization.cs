using System.Collections;
using Platform.Abstractions;
using Platform.Enums;

namespace Platform.Mocks
{
    public class MockPlatformInitialization : IPlatformInitialization
    {
        public IEnumerator Initialize() { yield break; }
        public PlatformType PlatformType => PlatformType.Mock;
        public bool IsInitialized => true;
        public bool CheckPlatform(PlatformType platformType) => platformType == PlatformType.Mock;
    }
}
