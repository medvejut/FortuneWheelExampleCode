using MyBox;
using Newtonsoft.Json;
using UnityEngine;

namespace FortuneWheel.Scripts.Models
{
    public class FortuneWheelDataLayer
    {
        private FortuneWheelSerializationData data;
        private readonly PlayerPrefsString pref = new("fortune_wheel_data");

        public FortuneWheelDataLayer Load()
        {
            if (!pref.IsSet)
            {
                data = FortuneWheelSerializationData.Empty;
                return this;
            }

            data = JsonConvert.DeserializeObject<FortuneWheelSerializationData>(pref.Value);
            return this;
        }

        public FortuneWheelSerializationData GetData() => data;

        public void SetActivated(bool value) => data.Activated = value;
        public void SetLastRestartTime(long value) => data.LastRestartTime = value;
        public void SetTodaySpinsCount(int value) => data.TodaySpinsCount = value;

        public void Save()
        {
            pref.Value = JsonConvert.SerializeObject(data);
            PlayerPrefs.Save();
        }
    }
}