using Newtonsoft.Json;

namespace FortuneWheel.Scripts.Settings
{
    public class FortuneWheelRewardConfig
    {
        [JsonProperty("currency")]
        public string Currency;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("weight")]
        public float Weight;
    }
}