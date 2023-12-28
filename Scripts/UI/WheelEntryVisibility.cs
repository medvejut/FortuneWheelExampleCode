using UnityEngine;
using Zenject;

namespace FortuneWheel.Scripts.UI
{
    public class WheelEntryVisibility : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        private IFortuneWheelModel model;
        private GameController gameController;

        [Inject]
        public void Construct(IFortuneWheelModel model, GameController gameController)
        {
            this.gameController = gameController;
            this.model = model;
        }

        private void OnEnable()
        {
            UpdateVisibility();
            model.Activated += UpdateVisibility;
            gameController.OnGameTypeChanged += GameControllerOnGameTypeChanged;
        }

        private void OnDisable()
        {
            model.Activated -= UpdateVisibility;
            gameController.OnGameTypeChanged -= GameControllerOnGameTypeChanged;
        }

        private void GameControllerOnGameTypeChanged(GameType gameType)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            var visible = Config.FortuneWheelShow && model.IsActivated && gameController.GameType == GameType.Standart;
            target.SetActive(visible);
        }
    }
}