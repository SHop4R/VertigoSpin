using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Game;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class GamePlayPanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button spinButton;
        [SerializeField] private Button collectButton;

        [Header("Game Controller")]
        [SerializeField] private SpinGameController gameController;

        private void OnValidate()
        {
            if (!spinButton)
                spinButton = transform.Find("ui_button_spin")?.GetComponent<Button>();

            if (!collectButton)
                collectButton = transform.Find("ui_button_collect")?.GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (spinButton)
                spinButton.onClick.AddListener(HandleSpin);

            if (collectButton)
                collectButton.onClick.AddListener(HandleCollect);

            EventManager.SpinEvents.OnSpinStarted += HandleSpinStarted;
            EventManager.SpinEvents.OnSpinEnded += HandleSpinEnded;
            EventManager.SpinEvents.OnWheelChanged += HandleWheelChanged;
            EventManager.GameEvents.OnGameRestart += HandleGameRestart;
        }

        private void OnDisable()
        {
            if (spinButton)
                spinButton.onClick.RemoveListener(HandleSpin);

            if (collectButton)
                collectButton.onClick.RemoveListener(HandleCollect);

            EventManager.SpinEvents.OnSpinStarted -= HandleSpinStarted;
            EventManager.SpinEvents.OnSpinEnded -= HandleSpinEnded;
            EventManager.SpinEvents.OnWheelChanged -= HandleWheelChanged;
            EventManager.GameEvents.OnGameRestart -= HandleGameRestart;
        }

        private void HandleSpin()
        {
            if (gameController)
                gameController.RequestSpin();
        }

        private void HandleCollect()
        {
            if (gameController)
                gameController.RequestCollect();
        }

        private void HandleSpinStarted()
        {
            spinButton.interactable = false;
            if (collectButton)
                collectButton.gameObject.SetActive(false);
        }

        private void HandleSpinEnded()
        {
            spinButton.interactable = true;
            UpdateCollectButton();
        }

        private void HandleWheelChanged(Data.WheelConfig _)
        {
            UpdateCollectButton();
        }

        private void HandleGameRestart()
        {
            spinButton.interactable = true;
            if (collectButton)
                collectButton.gameObject.SetActive(false);
        }

        private void UpdateCollectButton()
        {
            if (!collectButton || !gameController) return;
            collectButton.gameObject.SetActive(gameController.CanCollect);
        }
    }
}
