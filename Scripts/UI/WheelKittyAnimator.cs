using Spine.Unity;
using UnityEngine;

namespace FortuneWheel.Scripts.UI
{
    public class WheelKittyAnimator : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic skeleton;
        [SerializeField] private SpinAnimator spinAnimator;
        [SerializeField] private WheelRewardUI rewardUI;
        [SerializeField, SpineAnimation] private string idleAnimation;
        [SerializeField, SpineAnimation] private string spinAnimation;
        [SerializeField, SpineAnimation] private string rewardAnimation;
        [SerializeField, SpineAnimation] private string hideAnimation;

        private void Awake()
        {
            spinAnimator.onSpinStarted.AddListener(OnSpinStarted);
            spinAnimator.onSpinCompleted.AddListener(OnSpinCompleted);
            rewardUI.onHideInvoked.AddListener(OnHideInvoked);
        }

        private void OnEnable()
        {
            skeleton.AnimationState.SetAnimation(0, idleAnimation, false);
        }

        private void OnSpinStarted()
        {
            skeleton.AnimationState.SetAnimation(0, spinAnimation, false);
        }

        private void OnSpinCompleted()
        {
            skeleton.AnimationState.SetAnimation(0, rewardAnimation, false);
        }

        private void OnHideInvoked()
        {
            skeleton.AnimationState.SetAnimation(0, hideAnimation, false);
            skeleton.AnimationState.AddAnimation(0, idleAnimation, false, 0);
        }
    }
}