using DG.Tweening;
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
        [field: SerializeField] public Button GiveUpButton{ get; private set; }

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI reviveCostText;
        [SerializeField] private TextMeshProUGUI playerCoinsText;

        private void OnEnable()
        {
            if (CoinReviveButton)
                CoinReviveButton.onClick.AddListener(HandleCoinRevive);

            if (GiveUpButton)
                GiveUpButton.onClick.AddListener(HandleGiveUp);

            UpdateUI();
        }

        private void OnDisable()
        {
            if (CoinReviveButton)
                CoinReviveButton.onClick.RemoveListener(HandleCoinRevive);

            if (GiveUpButton)
                GiveUpButton.onClick.RemoveListener(HandleGiveUp);
        }

        private void UpdateUI()
        {
            int cost = CoinManager.Instance.CurrentReviveCost;
            bool canAfford = CoinManager.Instance.CanAffordRevive;

            if (CoinReviveButton)
                CoinReviveButton.interactable = canAfford;

            if (GiveUpButton)
                GiveUpButton.interactable = true;

            if (reviveCostText)
                reviveCostText.text = $"Revive {cost}";

            if (playerCoinsText)
                playerCoinsText.text = CoinManager.Instance.Coins.ToString();
        }

        private const float DialDownDuration = 0.6f;
        private const float PanelCloseDelay = 0.3f;

        private void HandleCoinRevive()
        {
            int coinsBefore = CoinManager.Instance.Coins;
            if (!CoinManager.Instance.TrySpendRevive()) return;

            int coinsAfter = CoinManager.Instance.Coins;

            if (CoinReviveButton) CoinReviveButton.interactable = false;
            if (GiveUpButton) GiveUpButton.interactable = false;

            if (playerCoinsText)
            {
                playerCoinsText.DOCounter(coinsBefore, coinsAfter, DialDownDuration, false)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        UIManager.TextAnimation(playerCoinsText);
                        DOVirtual.DelayedCall(PanelCloseDelay, () =>
                        {
                            EventManager.GameEvents.FireRevive();
                            UIManager.Instance.HidePanel(PanelType.Revive);
                        });
                    });
            }
            else
            {
                EventManager.GameEvents.FireRevive();
                UIManager.Instance.HidePanel(PanelType.Revive);
            }
        }

        private static void HandleGiveUp()
        {
            EventManager.GameEvents.FireGameOver();
            UIManager.Instance.HidePanel(PanelType.Revive);
        }
    }
}
