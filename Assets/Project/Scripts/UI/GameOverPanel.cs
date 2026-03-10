using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class GameOverPanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button restartButton;

        private void OnValidate()
        {
            if (!restartButton)
                restartButton = transform.Find("ui_button_restart")?.GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (restartButton)
                restartButton.onClick.AddListener(HandleRestart);
        }

        private void OnDisable()
        {
            if (restartButton)
                restartButton.onClick.RemoveListener(HandleRestart);
        }

        private static void HandleRestart()
        {
            EventManager.GameEvents.FireGameRestart();
            UIManager.Instance.HidePanel(PanelType.GameOver);
        }
    }
}
