using System;
using MyBox;
using Application = UnityEngine.Device.Application;

namespace FortuneWheel.Scripts.Models
{
    public class FortuneWheelModel : IFortuneWheelModel
    {
        private long lastRestartTimeTicks;
        private readonly Settings settings;
        private readonly FortuneWheelDataLayer dataLayer = new();
        private bool IsRestartFromNowTestMode => Application.isEditor && settings.restartFromNowTestMode;
        private int SpinsPerDay => settings.freeSpinsPerDay + settings.rewardedSpinsPerDay;

        public bool IsActivated { get; private set; }
        public event Action Activated;
        public long NextRestartTimeTicks { get; private set; }
        public event Action Restarted;

        public int TodaySpinsCount { get; private set; }
        public int AvailableSpinsCount => SpinsPerDay - TodaySpinsCount;
        public bool HasSpins => TodaySpinsCount < SpinsPerDay;
        public bool HasFreeSpins => TodaySpinsCount < settings.freeSpinsPerDay;
        public event Action SpinInvoked;

        public FortuneWheelAward[] SectorAwards { get; set; } = Array.Empty<FortuneWheelAward>();
        public FortuneWheelAward? LastAward;

        public FortuneWheelModel(Settings settings)
        {
            this.settings = settings;
            Deserialize();
        }

        private void Deserialize()
        {
            var data = dataLayer.Load().GetData();
            IsActivated = data.Activated;
            SetRestartTime(data.LastRestartTime);
            TodaySpinsCount = data.TodaySpinsCount;
        }

        public void Activate()
        {
            if (IsActivated)
            {
                throw new Exception("Fortune wheel already activated");
            }

            IsActivated = true;
            SetRestartTime(CalculateLastRestartTime());

            dataLayer.SetActivated(true);
            dataLayer.SetLastRestartTime(lastRestartTimeTicks);
            dataLayer.Save();

            Activated?.Invoke();
        }

        public void Restart()
        {
            SetRestartTime(CalculateLastRestartTime());
            TodaySpinsCount = 0;

            dataLayer.SetLastRestartTime(lastRestartTimeTicks);
            dataLayer.SetTodaySpinsCount(TodaySpinsCount);
            dataLayer.Save();

            Restarted?.Invoke();
        }

        private void SetRestartTime(long timeTicks)
        {
            lastRestartTimeTicks = timeTicks;
            NextRestartTimeTicks = lastRestartTimeTicks + CalculateRestartInterval();
        }

        private long CalculateLastRestartTime()
        {
            return IsRestartFromNowTestMode ? DateTime.Now.Ticks : DateTime.Today.Ticks;
        }

        private long CalculateRestartInterval()
        {
            return IsRestartFromNowTestMode
                ? settings.restartFromNowTestModeInterval * TimeSpan.TicksPerSecond
                : TimeSpan.TicksPerDay;
        }

        public void Spin()
        {
            TodaySpinsCount++;

            dataLayer.SetTodaySpinsCount(TodaySpinsCount);
            dataLayer.Save();

            SpinInvoked?.Invoke();
        }

        [Serializable]
        public class Settings
        {
            public bool restartFromNowTestMode = true;
            [ConditionalField(nameof(restartFromNowTestMode))]
            public int restartFromNowTestModeInterval = 60;

            public int freeSpinsPerDay = 1;
            public int rewardedSpinsPerDay = 2;
        }
    }
}