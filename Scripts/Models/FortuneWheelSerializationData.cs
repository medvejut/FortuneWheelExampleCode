namespace FortuneWheel.Scripts.Models
{
    public struct FortuneWheelSerializationData
    {
        public bool Activated;
        public long LastRestartTime;
        public int TodaySpinsCount;

        public static FortuneWheelSerializationData Empty => new();
    }
}