using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace FortuneWheel.Scripts.UI
{
    public class WheelTimerText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private string localizationKey;

        private long timeEndTicks;

        public long TimeEndTicks
        {
            get => timeEndTicks;
            set
            {
                timeEndTicks = value;
                UpdateTime();
            }
        }

        private void OnEnable()
        {
            UpdateTime();
            DOVirtual.DelayedCall(1, UpdateTime).SetLoops(-1).SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        private void UpdateTime()
        {
            var timeLeft = GetTimeLeft();
            var hoursPostfix = LocalizationManager.GetTranslate(LocalizationKeys.HOURS_POSTFIX);
            var minutesPostfix = LocalizationManager.GetTranslate(LocalizationKeys.MINUTES_POSTFIX);
            var secondsPostfix = LocalizationManager.GetTranslate(LocalizationKeys.SECONDS_POSTFIX);

            var timeString = timeLeft.Hours > 0
                    ? string.Format("{0}{1} {2}{3} {4}{5}",
                            timeLeft.Hours, hoursPostfix, timeLeft.Minutes, minutesPostfix, timeLeft.Seconds, secondsPostfix)
                    : string.Format("{0}{1} {2}{3}",
                            timeLeft.Minutes, minutesPostfix, timeLeft.Seconds, secondsPostfix);

            text.text = LocalizationManager.GetTranslate(localizationKey, timeString);
        }

        private TimeSpan GetTimeLeft()
        {
            var timeLeft = new DateTime(timeEndTicks).Subtract(DateTime.Now);
            return timeLeft.TotalMilliseconds < 0 ? TimeSpan.Zero : timeLeft;
        }
    }
}