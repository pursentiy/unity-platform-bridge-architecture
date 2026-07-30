using System;
using Storage.Records;

namespace Services.BridgeRelatedServices.Platform.Abstractions
{
    public interface IPlatformStorage
    {
        void SaveProfileRecord(ProfileRecord profileRecord);
        void LoadProfileRecord(Action<ProfileRecord> onLoaded);
    }
}
