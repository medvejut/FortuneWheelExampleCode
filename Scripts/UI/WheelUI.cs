using DG.Tweening;
using FortuneWheel.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FortuneWheel.Scripts.UI
{
    public class WheelUI : UIState
    {
        private enum SpinState
        {
            FreeSpin,
            Rewarded,
            Wait
        }

        [SerializeField] private Button closeButton;
        [SerializeField] private StateView<SpinState> spinStateView;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button freeSpinButton;
        [SerializeField] private WheelTimerText timerText;
        [SerializeField] private SpinAnimator spinAnimator;
        [SerializeField] private WheelSectorsLayout sectorsLayout;
        [SerializeField] private WheelRewardUI rewardUI;
        [SerializeField] private float stateUpdateDelay = 0.5f;
        [SerializeField] private GameObject interactionLocker;

        private IFortuneWheelModel model;
        private IFortuneWheelService service;
        private IFortuneWheelAnalyticsService analytics;

        private FortuneWheelAward currentAward;
        private bool isFreeSpinInvoked;
        private int lastSpinNumber;

        [Inject]
        public void Construct(IFortuneWheelModel model,
            IFortuneWheelService service,
            IFortuneWheelAnalyticsService analytics)
        {
            this.analytics = analytics;
            this.service = service;
            this.model = model;
        }

        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseClick);
            spinButton.onClick.AddListener(OnRewardedSpinClicked);
            freeSpinButton.onClick.AddListener(OnSpinClicked);
            spinAnimator.onSpinCompleted.AddListener(OnSpinCompleted);
            rewardUI.onHideInvoked.AddListener(OnRewardHided);
        }

        public override void Enter()
        {
            base.Enter();

            interactionLocker.SetActive(false);
            spinAnimator.SectorsCount = model.SectorAwards.Length;

            UpdateState();
            model.Restarted += UpdateState;
            model.SpinInvoked += UpdateStateWithDelay;

            analytics.TrackFortuneWheelPopupShow();
        }

        public override void Exit()
        {
            model.Restarted -= UpdateState;
            model.SpinInvoked -= UpdateStateWithDelay;

            base.Exit();
        }

        private void UpdateState()
        {
            spinStateView.Current = GetSpinState();
            timerText.TimeEndTicks = model.NextRestartTimeTicks;
        }

        private void UpdateStateWithDelay()
        {
            DOVirtual.DelayedCall(stateUpdateDelay, UpdateState, false)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        private SpinState GetSpinState()
        {
            if (model.HasSpins)
            {
                return model.HasFreeSpins ? SpinState.FreeSpin : SpinState.Rewarded;
            }

            return SpinState.Wait;
        }

        private void OnSpinClicked()
        {
            analytics.TrackFortuneWheelPopupSpinTap();
            Spin(true);
        }

        private void Spin(bool isFreeSpin)
        {
            isFreeSpinInvoked = isFreeSpin;
            lastSpinNumber = model.TodaySpinsCount + 1;

            currentAward = service.Spin();
            var wheelSector = sectorsLayout.GetSectorByAward(currentAward);
            spinAnimator.SpinToSector(wheelSector.transform);
            interactionLocker.SetActive(true);
        }

        private void OnRewardedSpinClicked()
        {
            analytics.TrackFortuneWheelPopupSpinTap();
            AdsController.Instance.ShowRewardedAd(Config.FORTUNE_WHEEL_PLACEMENT, () => Spin(false));
        }

        private void OnSpinCompleted()
        {
            analytics.TrackFortuneWheelPopupSpinResult(isFreeSpinInvoked, lastSpinNumber, currentAward);
            rewardUI.Show(currentAward);
        }

        private void OnRewardHided()
        {
            interactionLocker.SetActive(false);
        }

        private void OnCloseClick()
        {
            analytics.TrackFortuneWheelPopupClose();
            stateMachine.ChangeState(UIViews.Game);
        }
    }
}