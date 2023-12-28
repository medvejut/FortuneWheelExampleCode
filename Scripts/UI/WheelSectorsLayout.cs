using System.Linq;
using FortuneWheel.Scripts.Models;
using MyBox;
using UnityEngine;
using Zenject;

namespace FortuneWheel.Scripts.UI
{
    public class WheelSectorsLayout : MonoBehaviour
    {
        [SerializeField] private Transform background;
        [SerializeField] private Transform sectorsParent;

        private WheelSector[] sectors;
        private WheelSector.Factory sectorFactory;
        private IFortuneWheelModel model;

        [Inject]
        public void Construct(WheelSector.Factory sectorFactory, IFortuneWheelModel model)
        {
            this.model = model;
            this.sectorFactory = sectorFactory;
        }

        private void OnEnable()
        {
            sectors?.ForEach(sector => Destroy(sector.gameObject));

            var sectorAngle = -360f / model.SectorAwards.Length;
            background.rotation = Quaternion.Euler(0, 0, sectorAngle * 0.5f);
            sectors = model.SectorAwards.Select((sector, i) => SetupSector(sector, sectorAngle, i)).ToArray();
        }

        private WheelSector SetupSector(FortuneWheelAward award, float sectorAngle, int i)
        {
            var sector = sectorFactory.Create(award);
            sector.transform.SetParent(sectorsParent, false);
            sector.transform.rotation = Quaternion.Euler(0, 0, sectorAngle * i);
            return sector;
        }

        public WheelSector GetSectorByAward(FortuneWheelAward award)
        {
            return sectors.First(sector => sector.SectorAward == award);
        }
    }
}