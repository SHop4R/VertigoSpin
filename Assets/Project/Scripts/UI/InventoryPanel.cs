using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class InventoryPanel : MonoBehaviour
    {
        [Header("Slot Setup")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private InventorySlotUI slotPrefab;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI totalCoinText;

        private readonly List<InventorySlotUI> _slots = new();
        private int _totalCoins;

        private void OnEnable()
        {
            EventManager.RewardEvents.OnRewardEarned += HandleRewardEarned;
            EventManager.GameEvents.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            EventManager.RewardEvents.OnRewardEarned -= HandleRewardEarned;
            EventManager.GameEvents.OnGameOver -= HandleGameOver;
        }

        private void HandleRewardEarned(RewardData reward)
        {
            if (!reward || !slotPrefab || !slotContainer) return;

            InventorySlotUI slot = Instantiate(slotPrefab, slotContainer);
            slot.Setup(reward);
            _slots.Add(slot);

            _totalCoins += reward.CoinValue;
            UpdateTotalText();
        }

        private void HandleGameOver()
        {
            Clear();
        }

        private void Clear()
        {
            foreach (InventorySlotUI slot in _slots.Where(slot => slot))
            {
                Destroy(slot.gameObject);
            }

            _slots.Clear();
            _totalCoins = 0;
            UpdateTotalText();
        }

        private void UpdateTotalText()
        {
            if (totalCoinText != null)
                totalCoinText.text = _totalCoins.ToString();
        }
    }
}
