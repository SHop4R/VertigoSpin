using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class RevivePanel : MonoBehaviour
    {
        [field: Header("Buttons")]
        [field: SerializeField] public Button CoinReviveButton{ get; private set; }
        [field: SerializeField] public Button AdReviveButton{ get; private set; }
        [field: SerializeField] public Button GiveUpButton{ get; private set; }

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI reviveCostText;

        private void OnValidate()
        {
            if (!CoinReviveButton)
                CoinReviveButton = transform.Find("ui_button_revive_coin")?.GetComponent<Button>();

            if (!AdReviveButton)
                AdReviveButton = transform.Find("ui_button_revive_ad")?.GetComponent<Button>();

            if (!GiveUpButton)
                GiveUpButton = transform.Find("ui_button_revive_giveup")?.GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (CoinReviveButton)
                CoinReviveButton.onClick.AddListener(HandleCoinRevive);

            if (AdReviveButton)
                AdReviveButton.onClick.AddListener(HandleAdRevive);

            if (GiveUpButton)
                GiveUpButton.onClick.AddListener(HandleGiveUp);
        }

        private void OnDisable()
        {
            if (CoinReviveButton)
                CoinReviveButton.onClick.RemoveListener(HandleCoinRevive);

            if (AdReviveButton)
                AdReviveButton.onClick.RemoveListener(HandleAdRevive);

            if (GiveUpButton)
                GiveUpButton.onClick.RemoveListener(HandleGiveUp);
        }

        private static void HandleCoinRevive()
        {
            EventManager.GameEvents.FireRevive();
            UIManager.Instance.HidePanel(PanelType.Revive);
        }

        private static void HandleAdRevive()
        {
            EventManager.GameEvents.FireRevive();
            UIManager.Instance.HidePanel(PanelType.Revive);
        }

        private static void HandleGiveUp()
        {
            EventManager.GameEvents.FireGameOver();
            UIManager.Instance.HidePanel(PanelType.Revive);
        }
    }
}
