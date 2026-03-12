using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using VertigoSpin.Project.Scripts.Animations;
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
        private bool _isClearing;

        private const float DialDownDuration = 0.6f;
        private const float ShrinkDuration = 0.3f;
        private const float ShrinkStagger = 0.08f;
        private const float SlideInDuration = 0.6f;
        private const float SlideInOffset = 800f;

        private void Start()
        {
            UpdateTotalText(false);
            PlaySlideIn();
        }

        private void PlaySlideIn()
        {
            RectTransform rt = transform as RectTransform;
            if (rt == null) return;

            Vector2 restPos = rt.anchoredPosition;
            rt.anchoredPosition = restPos + Vector2.left * SlideInOffset;

            rt.DOAnchorPos(restPos, SlideInDuration)
                .SetEase(Ease.OutCubic);
        }

        private void OnEnable()
        {
            EventManager.RewardEvents.OnRewardEarned += HandleRewardEarned;
            EventManager.GameEvents.OnGameOver += HandleReset;
            EventManager.GameEvents.OnVictory += HandleReset;
        }

        private void OnDisable()
        {
            EventManager.RewardEvents.OnRewardEarned -= HandleRewardEarned;
            EventManager.GameEvents.OnGameOver -= HandleReset;
            EventManager.GameEvents.OnVictory -= HandleReset;
        }

        private void HandleRewardEarned(RewardData reward)
        {
            if (!reward || !slotPrefab || !slotContainer) return;

            InventorySlotUI existingSlot = _slots.FirstOrDefault(s => s && s.Reward == reward);

            if (existingSlot)
            {
                existingSlot.IncrementCount();
            }
            else
            {
                InventorySlotUI slot = Instantiate(slotPrefab, slotContainer);
                slot.Setup(reward);
                _slots.Add(slot);
            }

            _totalCoins += reward.CoinValue;
            UpdateTotalText();
        }

        private void HandleReset()
        {
            if (_isClearing) return;
            _isClearing = true;

            AnimatedClear();
        }

        private void AnimatedClear()
        {
            Sequence clearSeq = DOTween.Sequence();

            // Dial down coin text to 0
            if (totalCoinText != null && _totalCoins > 0)
            {
                int current = _totalCoins;
                clearSeq.Join(
                    DOTween.To(() => current, value =>
                    {
                        current = value;
                        totalCoinText.text = value.ToString();
                    }, 0, DialDownDuration)
                    .SetEase(Ease.OutQuad));
            }

            // Shrink all slots simultaneously (no deactivation to avoid layout shifts)
            foreach (InventorySlotUI slot in _slots)
            {
                if (!slot) continue;
                clearSeq.Join(
                    slot.transform.DOScale(Vector3.zero, ShrinkDuration)
                        .SetEase(Ease.InBack, 1.35f));
            }

            clearSeq.OnComplete(() =>
            {
                foreach (InventorySlotUI slot in _slots)
                {
                    if (slot) Destroy(slot.gameObject);
                }

                _slots.Clear();
                _totalCoins = 0;
                UpdateTotalText(false);
                _isClearing = false;

                EventManager.GameEvents.FireGameRestart();
            });
        }

        private void UpdateTotalText(bool animate = true)
        {
            if (totalCoinText == null) return;

            totalCoinText.text = _totalCoins.ToString();

            if (animate && _totalCoins > 0)
                UIManager.TextAnimation(totalCoinText);
        }
    }
}
