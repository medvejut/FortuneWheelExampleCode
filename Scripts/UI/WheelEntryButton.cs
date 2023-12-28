using DefaultNamespace.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FortuneWheel.Scripts.UI
{
    public class WheelEntryButton : MonoBehaviour
    {
        private enum State
        {
            Off,
            FreeSpins,
            RewardedSpins
        }

        [SerializeField] private Button button;
        [SerializeField] private StateView<State> stateView;
        [SerializeField] private TextMeshProUGUI spinsCountText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float offStateAlpha = 0.6f;
        [SerializeField] private DOTweenAnimation spinAnimation;
        [SerializeField] private float spinAnimationInterval = 10f;

        private UIController uiController;
        private IFortuneWheelModel model;
        private Tween spinLoopTween;
        private IFortuneWheelAnalyticsService analytics;

        [Inject]
        public void Construct(UIController uiController, IFortuneWheelModel model, IFortuneWheelAnalyticsService analytics)
        {
            this.analytics = analytics;
            this.model = model;
            this.uiController = uiController;
        }

        private void Awake()
        {
            spinAnimation.DOTurnAutoKillAndRecreateOnGameObject(false, autoPlay: false);
        }

        private void OnEnable()
        {
            UpdateState();
            model.SpinInvoked += UpdateState;
            model.Restarted += UpdateState;
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            model.SpinInvoked -= UpdateState;
            model.Restarted -= UpdateState;
            button.onClick.RemoveListener(OnButtonClick);
        }

        private void UpdateState()
        {
            stateView.Current = GetState();
            spinsCountText.text = model.AvailableSpinsCount.ToString();
            canvasGroup.alpha = stateView.Current == State.Off ? offStateAlpha : 1f;

            spinLoopTween?.Kill();
            if (stateView.Current == State.FreeSpins)
            {
                spinLoopTween = DOVirtual.DelayedCall(spinAnimationInterval, spinAnimation.DORewindAndPlayAllOnGameObject, false)
                    .SetLoops(-1)
                    .OnStart(spinAnimation.DORewindAndPlayAllOnGameObject)
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            }
        }

        private State GetState()
        {
            if (!model.HasSpins)
            {
                return State.Off;
            }

            return model.HasFreeSpins ? State.FreeSpins : State.RewardedSpins;
        }

        private void OnButtonClick()
        {
            analytics.TrackFortuneWheelPopupTap();
            uiController.ChangeState(UIViews.FortuneWheel);
        }
    }
}