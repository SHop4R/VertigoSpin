using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Audio;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class VictoryPanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button continueButton;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI coinsText;

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
            AudioManager.Instance.PlaySound(Sound.ButtonClick);
            HapticManager.Instance.PlayHaptic(HapticType.Click);
            EventManager.GameEvents.FireGameRestart();
            UIManager.Instance.HidePanel(PanelType.Victory);
        }
    }
}
