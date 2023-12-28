using System;
using FortuneWheel.Scripts.Models;
using MyBox;
using Global.Analytics;

namespace FortuneWheel.Scripts.Services
{
    public class FortuneWheelAnalyticsService : IFortuneWheelAnalyticsService
    {
        private const string PARAM_SPIN_TYPE = "spin_type";
        private const string PARAM_SPIN_NUMBER = "spin_number";
        private const string PARAM_REWARD_TYPE = "reward_type";
        private const string PARAM_REWARD_ID = "reward_id";

        private readonly IAnalyticsTracker tracker;
        private readonly IFortuneWheelModel model;
        private readonly GameController gameController;
        private readonly Settings settings;

        public FortuneWheelAnalyticsService(Settings settings, IAnalyticsTracker tracker, IFortuneWheelModel model, GameController gameController)
        {
            this.settings = settings;
            this.gameController = gameController;
            this.model = model;
            this.tracker = tracker;
        }

        public void TrackFortuneWheelPopupShow()
        {
            var eventBuilder = tracker.NewEvent(settings.eventFortuneWheelPopupShow);
            AddNextSpinParams(eventBuilder);
            eventBuilder.Track();
        }

        public void TrackFortuneWheelPopupClose()
        {
            var eventBuilder = tracker.NewEvent(settings.eventFortuneWheelPopupClose);
            AddNextSpinParams(eventBuilder);
            eventBuilder.Track();
        }

        public void TrackFortuneWheelPopupTap()
        {
            var eventBuilder = tracker.NewEvent(settings.eventFortuneWheelPopupTap);
            AddNextSpinParams(eventBuilder);
            eventBuilder.Track();
        }

        public void TrackFortuneWheelPopupSpinTap()
        {
            var eventBuilder = tracker.NewEvent(settings.eventFortuneWheelPopupSpinTap);
            AddNextSpinParams(eventBuilder);
            eventBuilder.Track();
        }

        public void TrackFortuneWheelPopupSpinResult(bool isFreeSpin, int spinNumber, FortuneWheelAward award)
        {
            var eventBuilder = tracker.NewEvent(settings.eventFortuneWheelPopupSpinResult);
            AddLevelParams(eventBuilder);
            AddSpinParams(eventBuilder, GetSpinType(true, isFreeSpin), spinNumber);
            eventBuilder.Add(PARAM_REWARD_TYPE, award.Currency);
            eventBuilder.Add(PARAM_REWARD_ID, award.ProductId.IsNullOrEmpty() ? award.Count.ToString() : award.ProductId);
            eventBuilder.Track();
        }

        private static string GetSpinType(bool hasSpins, bool isFreeSpin)
        {
            if (!hasSpins)
            {
                return "none";
            }

            return isFreeSpin ? "free_spin" : "rewarded_spin";
        }

        private void AddNextSpinParams(TrackEventBuilder eventBuilder)
        {
            AddLevelParams(eventBuilder);
            AddSpinParams(eventBuilder, GetSpinType(model.HasSpins, model.HasFreeSpins), model.TodaySpinsCount + 1);
        }

        private void AddLevelParams(TrackEventBuilder eventBuilder)
        {
            eventBuilder
                .Add(AnalyticsData.PARAM_LEVEL_ID, gameController.CurrentLevelsController.ActualLevelId)
                .Add(AnalyticsData.PARAM_POSITION, gameController.CurrentLevelsController.DisplayLevelId.ToString());
        }

        private void AddSpinParams(TrackEventBuilder eventBuilder, string spinType, int spinNumber)
        {
            eventBuilder
                .Add(PARAM_SPIN_TYPE, spinType)
                .Add(PARAM_SPIN_NUMBER, spinNumber);
        }

        [Serializable]
        public class Settings
        {
            public AnalyticEvent eventFortuneWheelPopupSpinResult;
            public AnalyticEvent eventFortuneWheelPopupSpinTap;
            public AnalyticEvent eventFortuneWheelPopupClose;
            public AnalyticEvent eventFortuneWheelPopupTap;
            public AnalyticEvent eventFortuneWheelPopupShow;
        }
    }
}