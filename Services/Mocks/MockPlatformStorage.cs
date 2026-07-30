using System;
using Services.BridgeRelatedServices.Platform.Abstractions;
using Storage.Records;
using UnityEngine;

namespace Services.BridgeRelatedServices.Platform.Mock
{
    public class MockPlatformStorage : IPlatformStorage
    {
        private const string SaveKey = "user_profile_data";

        public void SaveProfileRecord(ProfileRecord profileRecord)
        {
            if (profileRecord == null) return;
            var json = JsonUtility.ToJson(profileRecord);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public void LoadProfileRecord(Action<ProfileRecord> onLoaded)
        {
            var json = PlayerPrefs.GetString(SaveKey, null);
            if (string.IsNullOrEmpty(json))
            {
                onLoaded?.Invoke(null);
                return;
            }

            try
            {
                var record = JsonUtility.FromJson<ProfileRecord>(json);
                if (record != null && !json.Contains("\"TutorialRecord\""))
                    record.TutorialRecord = null;
                onLoaded?.Invoke(record);
            }
            catch
            {
                onLoaded?.Invoke(null);
            }
        }
    }
}
