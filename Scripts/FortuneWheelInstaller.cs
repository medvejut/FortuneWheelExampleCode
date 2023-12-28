using FortuneWheel.Scripts.Models;
using FortuneWheel.Scripts.Services;
using FortuneWheel.Scripts.UI;
using UnityEngine;
using Zenject;

namespace FortuneWheel.Scripts
{
    public class FortuneWheelInstaller : MonoInstaller
    {
        [SerializeField]
        private WheelSector wheelSectorPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<FortuneWheelModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<FortuneWheelService>().AsSingle();
            Container.BindInterfacesAndSelfTo<FortuneWheelStartPopupService>().AsSingle();
            Container.BindInterfacesAndSelfTo<FortuneWheelAnalyticsService>().AsSingle();

            Container.BindFactory<FortuneWheelAward, WheelSector, WheelSector.Factory>().FromComponentInNewPrefab(wheelSectorPrefab);
        }
    }
}