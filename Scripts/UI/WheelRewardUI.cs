using DefaultNamespace.Extensions;
using DG.Tweening;
using FortuneWheel.Scripts.Models;
using FortuneWheel.Scripts.Settings;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace FortuneWheel.Scripts.UI
{
    public class WheelRewardUI : MonoBehaviour
    {
        [SerializeField] private Image goldIcon;
        [SerializeField] private Image commonIcon;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button takeButton;
        [SerializeField] private DOTweenAnimation showAnimationParent;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image skinIcon;
        [SerializeField] private DOTweenAnimation skinAnimations;
        [SerializeField] private GameObject skinIconContainer;
        [SerializeField] private CanvasGroup skinIconCanvasGroup;

        public UnityEvent onHideInvoked;
        public UnityEvent onShowInvoked;

        private FortuneWheelAwardSettings settings;
        private IFortuneWheelService service;
        private FortuneWheelAward award;

        [Inject]
        public void Construct(FortuneWheelAwardSettings settings, IFortuneWheelService service)
        {
            this.service = service;
            this.settings = settings;
            
            skinAnimations.DOTurnAutoKillAndRecreateOnGameObject(false, autoPlay: false);
            showAnimationParent.DOTurnAutoKillAndRecreateOnGameObject(false, autoPlay: false);
        }

        private void Awake()
        {
            takeButton.onClick.AddListener(OnTakeClicked);
        }

        public void Show(FortuneWheelAward award)
        {
            service.ApplyAward();

            this.award = award;
            gameObject.SetActive(true);
            canvasGroup.interactable = true;

            var isGold = award.Currency == FortuneWheelAwardCurrency.GOLD;
            SetupIcon(award, isGold);

            countText.text = isGold ? $"+{award.Count}" : string.Empty;
            
            var isSkinAward = this.award.Currency is FortuneWheelAwardCurrency.WALLPAPER or FortuneWheelAwardCurrency.SKIN;
            skinIconContainer.SetActive(isSkinAward);
            if (isSkinAward)
            {
                SetSkinIcon();
            }

            ResetIconAfterAnimation();
            
            showAnimationParent.DORewindAndPlayAllOnGameObject();
            if (isSkinAward)
            {
                skinAnimations.DORewindAndPlayAllOnGameObject();
            }

            onShowInvoked.Invoke();
        }

        private void SetupIcon(FortuneWheelAward award, bool isGold)
        {
            var icon = isGold ? goldIcon : commonIcon;
            var hideIcon = isGold ? commonIcon : goldIcon;
            icon.gameObject.SetActive(true);
            hideIcon.gameObject.SetActive(false);
            icon.sprite = settings.GetSprite(award.SpriteCurrency);
        }

        private void ResetIconAfterAnimation()
        {
            commonIcon.SetAlpha(1f);
            goldIcon.SetAlpha(1f);
            commonIcon.transform.rotation = Quaternion.identity;
            goldIcon.transform.rotation = Quaternion.identity;
            skinIconCanvasGroup.alpha = 1f;
        }

        private void SetSkinIcon()
        {
            // ...
        }

        private void OnTakeClicked()
        {
            canvasGroup.interactable = false;
            onHideInvoked.Invoke();

            if (award.Currency == FortuneWheelAwardCurrency.GOLD)
            {
                // ...
            }
        }
    }
}