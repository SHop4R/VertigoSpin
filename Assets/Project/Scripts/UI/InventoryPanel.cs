using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class InventoryPanel : MonoBehaviour
    {
        [Header("Slot Setup")]
        [SerializeField] private Transform slotContainer;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI totalCoinText;

        private readonly List<InventorySlotUI> _slots = new();
        private int _totalCoins;
        private bool _isClearing;

        private const float DialDownDuration = 0.6f;
        private const float ShrinkDuration = 0.3f;
        private const float SlideInDuration = 0.6f;
        private const float SlideInOffset = 800f;
        private const float PopInDuration = 0.35f;

        private const float FlyDuration = 0.6f;
        private const float FlyPopScale = 3f;
        private const float FlyPopDuration = 0.15f;
        private const float FlyEndScale = 0.3f;
        private const float FlyIconSize = 100f;
        private const float FlyArcHeight = 300f;

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
            EventManager.RewardEvents.OnRewardFlyStarted += HandleRewardFlyStarted;
            EventManager.GameEvents.OnGameOver += HandleReset;
            EventManager.GameEvents.OnVictory += HandleReset;
        }

        private void OnDisable()
        {
            EventManager.RewardEvents.OnRewardFlyStarted -= HandleRewardFlyStarted;
            EventManager.GameEvents.OnGameOver -= HandleReset;
            EventManager.GameEvents.OnVictory -= HandleReset;
        }

        private void HandleRewardFlyStarted(RewardData reward, Vector3 startWorldPos)
        {
            if (!reward || !slotContainer) return;

            InventorySlotUI existingSlot = _slots.FirstOrDefault(s => s && s.Reward == reward);
            bool isNewSlot = existingSlot == null;

            InventorySlotUI targetSlot;
            if (isNewSlot)
            {
                targetSlot = PoolManager.Instance.SpawnInventorySlot(slotContainer);
                targetSlot.Setup(reward);
                targetSlot.transform.localScale = Vector3.zero;
                _slots.Add(targetSlot);
            }
            else
            {
                targetSlot = existingSlot;
            }

            // Force layout rebuild so the slot has its final position
            Canvas.ForceUpdateCanvases();

            Vector3 targetWorldPos = targetSlot.transform.position;

            PlayFlyAnimation(reward.Icon, startWorldPos, targetWorldPos, () =>
            {
                if (isNewSlot)
                {
                    targetSlot.transform.DOScale(Vector3.one, PopInDuration)
                        .SetEase(Ease.OutBack, 1.5f);
                }
                else
                {
                    targetSlot.IncrementCount();
                    targetSlot.transform.DOKill();
                    targetSlot.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 6);
                }

                _totalCoins += reward.CoinValue;
                UpdateTotalText();
                EventManager.RewardEvents.FireRewardEarned(reward);
            });
        }

        private void PlayFlyAnimation(Sprite icon, Vector3 startWorldPos, Vector3 targetWorldPos,
            TweenCallback onComplete)
        {
            Canvas canvas = UIManager.Instance.Canvas;
            if (!canvas || canvas.worldCamera == null)
            {
                onComplete?.Invoke();
                return;
            }

            GameObject flyObj = new("FlyingReward");
            flyObj.transform.SetParent(canvas.transform, false);

            RectTransform rt = flyObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(FlyIconSize, FlyIconSize);
            rt.SetAsLastSibling();

            Image img = flyObj.AddComponent<Image>();
            img.sprite = icon;
            img.raycastTarget = false;
            img.preserveAspect = true;

            Vector2 startPos = UIManager.Instance.GetScreenPosition(startWorldPos);
            Vector2 endPos = UIManager.Instance.GetScreenPosition(targetWorldPos);
            Vector2 control = (startPos + endPos) * 0.5f + Vector2.up * FlyArcHeight;

            rt.anchoredPosition = startPos;
            rt.localScale = Vector3.zero;

            Sequence flySeq = DOTween.Sequence();

            // Pop out from wheel
            flySeq.Append(rt.DOScale(FlyPopScale, FlyPopDuration).SetEase(Ease.OutBack));

            // Fly along bezier arc
            flySeq.Append(
                DOTween.To(() => 0f, t =>
                {
                    float inv = 1f - t;
                    Vector2 pos = inv * inv * startPos
                                  + 2f * inv * t * control
                                  + t * t * endPos;
                    rt.anchoredPosition = pos;
                }, 1f, FlyDuration)
                .SetEase(Ease.InOutQuad));

            // Shrink while flying
            flySeq.Join(
                rt.DOScale(FlyEndScale, FlyDuration).SetEase(Ease.InQuad));

            flySeq.OnComplete(() =>
            {
                Destroy(flyObj);
                onComplete?.Invoke();
            });
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

            // Shrink all slots simultaneously
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
                    if (slot) PoolManager.Instance.ReturnInventorySlot(slot);
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
