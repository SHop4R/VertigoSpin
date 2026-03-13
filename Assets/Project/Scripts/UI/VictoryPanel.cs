using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class VictoryPanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button continueButton;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI coinsText;

        private void OnValidate()
        {
            if (!continueButton)
                continueButton = transform.Find("ui_button_victory_continue")?.GetComponent<Button>();

            if (!coinsText)
                coinsText = transform.Find("ui_text_victory_coins_value")?.GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (continueButton)
                continueButton.onClick.AddListener(HandleContinue);
        }

        private void OnDisable()
        {
            if (continueButton)
                continueButton.onClick.RemoveListener(HandleContinue);
        }

        public void SetTotalCoins(int coins)
        {
            if (coinsText)
                coinsText.text = coins.ToString();
        }

        private static void HandleContinue()
        {
            EventManager.GameEvents.FireGameRestart();
            UIManager.Instance.HidePanel(PanelType.Victory);
        }
    }
}
