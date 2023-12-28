using System;
using FortuneWheel.Scripts.Models;

namespace FortuneWheel.Scripts
{
    public interface IFortuneWheelModel
    {
        bool IsActivated { get; }
        event Action Activated;

        long NextRestartTimeTicks { get; }
        event Action Restarted;

        int TodaySpinsCount { get; }
        int AvailableSpinsCount { get; }
        bool HasSpins { get; }
        bool HasFreeSpins { get; }
        public event Action SpinInvoked;

        FortuneWheelAward[] SectorAwards { get; }
    }
}