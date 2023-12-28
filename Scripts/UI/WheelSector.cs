using FortuneWheel.Scripts.Models;
using FortuneWheel.Scripts.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FortuneWheel.Scripts.UI
{
    public class WheelSector : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI countText;

        public FortuneWheelAward SectorAward { get; private set; }
        private FortuneWheelAwardSettings settings;

        [Inject]
        public void Construct(FortuneWheelAward sectorAward, FortuneWheelAwardSettings settings)
        {
            this.settings = settings;
            SectorAward = sectorAward;
        }

        private void Awake()
        {
            icon.sprite = settings.GetSprite(SectorAward.SpriteCurrency);
            var isGold = SectorAward.Currency == FortuneWheelAwardCurrency.GOLD;
            countText.text = isGold ? SectorAward.Count.ToString() : string.Empty;
        }

        public class Factory : PlaceholderFactory<FortuneWheelAward, WheelSector>
        {
        }
    }
}