using FortuneWheel.Scripts.Models;
using FortuneWheel.Scripts.Services;
using FortuneWheel.Scripts.UI;
using UnityEngine;
using Zenject;

namespace FortuneWheel.Scripts.Settings
{
    [CreateAssetMenu(fileName = "FortuneWheelSettings", menuName = "ZephyrMobile/Fortune Wheel Settings", order = 0)]
    public class FortuneWheelSettings : ScriptableObjectInstaller<FortuneWheelSettings>
    {
        public FortuneWheelModel.Settings modelSettings;
        public FortuneWheelStartPopupService.Settings startPopupSettings;
        public FortuneWheelAwardSettings sectorViewSettings;
        public FortuneWheelAnalyticsService.Settings analytics;

        public override void InstallBindings()
        {
            Container.BindInstance(modelSettings);
            Container.BindInstance(startPopupSettings);
            Container.BindInstance(sectorViewSettings);
            Container.BindInstance(analytics);
        }
    }
}