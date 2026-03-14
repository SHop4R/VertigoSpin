using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Audio;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Managers;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class InventoryPanel : MonoBehaviour
    {
        [Header("Slot Setup")]
        [SerializeField] private Transform slotContainer;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI totalCoinText;

        private readonly List<InventorySlotUI> _slots = new();
        private readonly Dictionary<RewardData, InventorySlotUI> _slotLookup = new();
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
            RectTransform rect = transform as RectTransform;
            if (!rect) return;

            Vector2 restPos = rect.anchoredPosition;
            rect.anchoredPosition = restPos + Vector2.left * SlideInOffset;

            rect.DOAnchorPos(restPos, SlideInDuration)
                .SetEase(Ease.OutCubic);
        }

        private void OnEnable()
        {
            EventManager.RewardEvents.OnRewardFlyStarted += HandleRewardFlyStarted;
            EventManager.GameEvents.OnGameOver += HandleReset;
            EventManager.GameEvents.OnVictoryCollect += HandleVictoryReset;
        }

        private void OnDisable()
        {
            EventManager.RewardEvents.OnRewardFlyStarted -= HandleRewardFlyStarted;
            EventManager.GameEvents.OnGameOver -= HandleReset;
            EventManager.GameEvents.OnVictoryCollect -= HandleVictoryReset;
        }

        private void HandleRewardFlyStarted(RewardData reward, Vector3 startWorldPos)
        {
            if (!reward || !slotContainer) return;

            bool isNewSlot = !_slotLookup.TryGetValue(reward, out InventorySlotUI targetSlot);

            if (isNewSlot)
            {
                targetSlot = PoolManager.Instance.SpawnInventorySlot(slotContainer);
                targetSlot.Setup(reward);
                targetSlot.transform.localScale = Vector3.zero;
                _slots.Add(targetSlot);
                _slotLookup[reward] = targetSlot;
            }

            Canvas.ForceUpdateCanvases();

            Vector3 targetWorldPos = targetSlot.transform.position;

            PlayFlyAnimation(reward.Icon, startWorldPos, targetWorldPos, () =>
            {
                AudioManager.Instance.PlaySound(Sound.RewardCollect);
                HapticManager.Instance.PlayHaptic(HapticType.LightImpact);

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

        private static void PlayFlyAnimation(Sprite icon, Vector3 startWorldPos, Vector3 targetWorldPos,
            TweenCallback onComplete)
        {
            Canvas canvas = UIManager.Instance.Canvas;
            if (!canvas || !canvas.worldCamera)
            {
                onComplete?.Invoke();
                return;
            }

            FlyingReward flyingReward = PoolManager.Instance.SpawnFlyingReward(canvas.transform);
            if (!flyingReward)
            {
                onComplete?.Invoke();
                return;
            }

            flyingReward.Setup(icon);

            RectTransform rt = flyingReward.RectTransform;
            rt.SetAsLastSibling();

            Vector2 startPos = UIManager.Instance.GetScreenPosition(startWorldPos);
            Vector2 endPos = UIManager.Instance.GetScreenPosition(targetWorldPos);
            Vector2 control = (startPos + endPos) * 0.5f + Vector2.up * FlyArcHeight;

            rt.anchoredPosition = startPos;
            rt.localScale = Vector3.zero;

            Sequence flySeq = DOTween.Sequence();

            flySeq.Append(rt.DOScale(FlyPopScale, FlyPopDuration).SetEase(Ease.OutBack));

            flySeq.Append(
                DOVirtual.Float(0f, 1f, FlyDuration, t =>
                    {
                        rt.anchoredPosition = VectorExtensions.QuadraticBezier(startPos, control, endPos, t);
                    })
                    .SetEase(Ease.InOutQuad));

            flySeq.Join(
                rt.DOScale(FlyEndScale, FlyDuration).SetEase(Ease.InQuad));

            flySeq.OnComplete(() =>
            {
                PoolManager.Instance.ReturnFlyingReward(flyingReward);
                onComplete?.Invoke();
            });
        }

        private void HandleReset()
        {
            if (_isClearing) return;
            _isClearing = true;

            AnimatedClear();
        }

        private void HandleVictoryReset()
        {
            if (_isClearing) return;
            _isClearing = true;

            AnimatedClear(fireRestart: false);
        }

        private void AnimatedClear(bool fireRestart = true)
        {
            Sequence clearSeq = DOTween.Sequence();

            if (totalCoinText && _totalCoins > 0)
            {
                int current = _totalCoins;
                clearSeq.Join(
                    totalCoinText.DOCounter(current, 0, DialDownDuration, false)
                        .SetEase(Ease.OutQuad));
            }

            foreach (InventorySlotUI slot in _slots)
            {
                if (!slot) continue;
                clearSeq.Join(
                    slot.transform.DOScale(Vector3.zero, ShrinkDuration)
                        .SetEase(Ease.InBack, 1.35f));
            }

            clearSeq.OnComplete(() =>
            {
                foreach (InventorySlotUI slot in _slots.Where(slot => slot))
                {
                    PoolManager.Instance.ReturnInventorySlot(slot);
                }

                _slots.Clear();
                _slotLookup.Clear();
                _totalCoins = 0;
                UpdateTotalText(false);
                _isClearing = false;

                if (fireRestart)
                    EventManager.GameEvents.FireGameRestart();
            });
        }

        private void UpdateTotalText(bool animate = true)
        {
            if (!totalCoinText) return;

            totalCoinText.text = _totalCoins.ToString();

            if (animate && _totalCoins > 0)
                UIManager.TextAnimation(totalCoinText);
        }
    }
}
