using System;
using System.Collections;
using System.Linq;
using FortuneWheel.Scripts.Models;
using FortuneWheel.Scripts.Settings;
using MyBox;
using Storage;
using Store;
using UnityEngine;
using Zenject;
using Global.Utilities;

namespace FortuneWheel.Scripts.Services
{
    public class FortuneWheelService : IInitializable, IDisposable, IFortuneWheelService
    {
        private readonly FortuneWheelModel model;
        private readonly ILevelsController levelController;
        private Coroutine restartTimer;

        public FortuneWheelService(FortuneWheelModel model, ILevelsController levelController)
        {
            this.model = model;
            this.levelController = levelController;

            UpdateRewards();
        }

        public void Initialize()
        {
            UpdateRewards();
            Config.OnUpdated += UpdateRewards;

            if (model.IsActivated)
            {
                StartRestartTimer();
                return;
            }

            if (!TryActivate())
            {
                levelController.LevelStarted.AddListener(OnLevelStarted);
            }
        }

        public void Dispose()
        {
            Config.OnUpdated -= UpdateRewards;
            levelController.LevelStarted.RemoveListener(OnLevelStarted);

            if (restartTimer != null)
            {
                MyCoroutines.StopCoroutine(restartTimer);
            }
        }

        private void UpdateRewards()
        {
            model.SectorAwards = Config.FortuneWheelRewards.Select(ParseSectorAwardFromConfig).ToArray();
        }

        private FortuneWheelAward ParseSectorAwardFromConfig(FortuneWheelRewardConfig reward)
        {
            var currency = reward.Currency;
            var spriteCurrency = currency;

            if (currency.IndexOf(FortuneWheelAwardCurrency.GOLD, StringComparison.Ordinal) == 0)
            {
                currency = FortuneWheelAwardCurrency.GOLD;
            }

            return new FortuneWheelAward {Currency = currency, Count = reward.Count, Weight = reward.Weight, SpriteCurrency = spriteCurrency};
        }

        private void OnLevelStarted(int id, ILevel level)
        {
            if (TryActivate())
            {
                levelController.LevelStarted.RemoveListener(OnLevelStarted);
            }
        }

        private bool TryActivate()
        {
            if (levelController.LevelId < Config.FortuneWheelFirstShow)
            {
                return false;
            }

            DevLog.Debug($"[fortune] Activate");
            model.Activate();
            StartRestartTimer();
            return true;
        }

        private void StartRestartTimer()
        {
            restartTimer = CoRestartLevels().StartCoroutine();
        }

        private IEnumerator CoRestartLevels()
        {
            while (true)
            {
                DevLog.Debug($"[fortune] Restart In: {new DateTime(model.NextRestartTimeTicks)}");
                yield return new WaitUntil(() => DateTime.Now.Ticks >= model.NextRestartTimeTicks);
                model.Restart();
            }
        }

        public FortuneWheelAward Spin()
        {
            model.Spin();

            CorrectSectorAwardsWeights();
            var award = model.SectorAwards.GetWeightedRandom(award => award.Weight);
            award.ProductId = CalculateProductIdForAward(award);
            model.LastAward = award;

            return model.LastAward.Value;
        }

        public void ApplyAward()
        {
            if (!model.LastAward.HasValue)
            {
                throw new Exception("No award in fortune wheel to apply");
            }

            ProcessAward(model.LastAward.Value);
            model.LastAward = null;
        }

        private string CalculateProductIdForAward(FortuneWheelAward award)
        {
            switch (award.Currency)
            {
                // ...
            }

            return string.Empty;
        }

        private void CorrectSectorAwardsWeights()
        {
            var noAvailableWallpapers = GetAvailableProducts(Config.StorageCategoryWallpapers).Length == 0;
            var noAvailableFlaskSkins = GetAvailableProducts(Config.StorageCategorySkins).Length == 0;

            if (noAvailableWallpapers || noAvailableFlaskSkins)
            {
                for (var i = 0; i < model.SectorAwards.Length; i++)
                {
                    var sectorAward = model.SectorAwards[i];

                    if (noAvailableWallpapers && sectorAward.Currency == FortuneWheelAwardCurrency.WALLPAPER ||
                        noAvailableFlaskSkins && sectorAward.Currency == FortuneWheelAwardCurrency.SKIN)
                    {
                        sectorAward.Weight = 0;
                        model.SectorAwards[i] = sectorAward;
                    }
                }
            }
        }

        private void ProcessAward(FortuneWheelAward award)
        {
            switch (award.Currency)
            {
                case FortuneWheelAwardCurrency.GOLD:
                    // ...
                    break;

                // ...

                default:
                    DevLog.Error($"[fortune] Unknown award to apply {award.Currency}");
                    break;
            }
        }

        private IStoreProductContainer<IProduct>[] GetAvailableProducts(string categoryId)
        {
            // ...
        }

        public void ForceRestart()
        {
            model.Restart();
        }
    }
}