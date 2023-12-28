using JetBrains.Annotations;

namespace FortuneWheel.Scripts.Models
{
    public struct FortuneWheelAward
    {
        public string Currency;
        public int Count;

        public float Weight;

        public string SpriteCurrency;

        [CanBeNull] public string ProductId;

        public static bool operator ==(FortuneWheelAward a, FortuneWheelAward b)
        {
            return a.Currency == b.Currency && a.Count == b.Count;
        }

        public static bool operator !=(FortuneWheelAward a, FortuneWheelAward b)
        {
            return !(a == b);
        }
    }
}