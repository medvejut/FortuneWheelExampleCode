using DG.Tweening;
using MyBox;
using Services;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace FortuneWheel.Scripts.UI
{
    public class SpinAnimator : MonoBehaviour
    {
        [SerializeField] private Transform sectorsRotation;
        [SerializeField] private float extraSpinAngle = 360f;
        [SerializeField] private float spinSpeed = -300f;
        [SerializeField] private Ease spinEase = Ease.OutQuad;

        public UnityEvent onSpinStarted;
        public UnityEvent onSpinCompleted;

        public bool IsSpinning => tween?.IsPlaying() ?? false;
        public int SectorsCount { get; set; }

        private Tween tween;
        private int lastPassedHapticSector;
        private IHapticService hapticService;

        [Inject]
        private void Construct(IHapticService hapticService)
        {
            this.hapticService = hapticService;
        }

        public void SpinToSector(Transform sectorTransform)
        {
            tween?.Kill();
            tween = StopTween(sectorTransform.localEulerAngles)
                .OnStart(onSpinStarted.Invoke)
                .OnComplete(onSpinCompleted.Invoke);
        }

        private Tween StopTween(Vector3 sectorLocalEulerAngles)
        {
            var angles = -sectorLocalEulerAngles.OffsetZ(extraSpinAngle);
            var difference = sectorsRotation.localEulerAngles.z - angles.z;
            var time = Mathf.Abs(difference / spinSpeed);

            return sectorsRotation.DOLocalRotate(angles, time, RotateMode.FastBeyond360).SetEase(spinEase);
        }

        private void Update()
        {
            HapticPassedSector();
        }

        private void HapticPassedSector()
        {
            var sector = GetCurrentHapticSector();

            if (sector == lastPassedHapticSector)
            {
                return;
            }

            lastPassedHapticSector = sector;

#if UNITY_IOS
            hapticService.Vibrate();
#endif
        }

        private int GetCurrentHapticSector()
        {
            var sectorAngle = 360 / SectorsCount;
            var sector = (int) (sectorsRotation.localEulerAngles.z / sectorAngle);

            return sector == SectorsCount ? 0 : sector;
        }
    }
}