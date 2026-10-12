using System;
using Storage.Records;

namespace Platform.Abstractions
{
    public interface IPlatformStorage
    {
        void SaveProfileRecord(ProfileRecord profileRecord);
        void LoadProfileRecord(Action<ProfileRecord> onLoaded);
    }
}
