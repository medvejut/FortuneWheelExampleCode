using FortuneWheel.Scripts.Models;

namespace FortuneWheel.Scripts
{
    public interface IFortuneWheelAnalyticsService
    {
        void TrackFortuneWheelPopupShow();
        void TrackFortuneWheelPopupClose();
        void TrackFortuneWheelPopupTap();
        void TrackFortuneWheelPopupSpinTap();
        void TrackFortuneWheelPopupSpinResult(bool isFreeSpin, int spinNumber, FortuneWheelAward award);
    }
}