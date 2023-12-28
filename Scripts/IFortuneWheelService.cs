using FortuneWheel.Scripts.Models;

namespace FortuneWheel.Scripts
{
    public interface IFortuneWheelService
    {
        public FortuneWheelAward Spin();
        public void ApplyAward();
    }
}