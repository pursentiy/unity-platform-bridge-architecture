using System.Collections;
using Platform.Enums;

namespace Platform.Abstractions
{
    public interface IPlatformInitialization
    {
        IEnumerator Initialize();
        PlatformType PlatformType { get; }
        bool IsInitialized { get; }
        bool CheckPlatform(PlatformType platformType);
    }
}
