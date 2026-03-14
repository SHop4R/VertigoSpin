using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Audio;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Game;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Managers;
using Random = UnityEngine.Random;

namespace VertigoSpin.Project.Scripts.Wheel
{
    public sealed class WheelController : MonoBehaviour
    {
        [Header("Wheel References")]
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Image indicatorImage;

        [Header("Buttons")]
        [SerializeField] private Button spinButton;
        [SerializeField] private Button collectButton;

        private Image _wheelBaseImage;
        private readonly List<WheelSlice> _slices = new();
        private WheelConfig _currentConfig;
        private SpinGameController _gameController;
        private int _selectedSliceIndex;
        private bool _isSpinning;
        private bool _isTransitioning;
        private Vector3 _wheelRestPosition;
        private Transform _wheelParent;
        private Tween _pulseTween;

        private const float WindUpDuration = 1.2f;
        private const float WindUpAngle = 75f;
        private const float WindUpScale = 1.15f;
        private const float ScaleBackDuration = 0.25f;
        private const float SpinDuration = 4f;
        private const float SnapDuration = 0.5f;
        private const int MinFullRotations = 5;
        private const int MaxFullRotations = 9;
        private const float SliceAngle = 360f / WheelConfig.SliceCount;
        private const float SliceMinOffset = 2f;
        private const float SliceMaxOffset = 43f;
        private const float SliceRadius = 350f;

        private const float TransitionDuration = 0.8f;
        private const float OffScreenOffset = 2000f;
        private const float SlideInDuration = 0.6f;
        private const float SlideInOffset = 800f;

        private void Awake()
        {
            _gameController = FindObjectOfType<SpinGameController>();
            _wheelBaseImage = wheelTransform.GetComponent<Image>();

            if (_wheelBaseImage) _wheelBaseImage.preserveAspect = true;
            if (indicatorImage) indicatorImage.preserveAspect = true;

            SeedRandomFromCrypto();
            _wheelParent = wheelTransform.parent;
            _wheelRestPosition = _wheelParent.localPosition;

            _wheelParent.localPosition = _wheelRestPosition + Vector3.down * OffScreenOffset;
            _wheelParent.localScale = Vector3.zero;
        }

        private void Start()
        {
            InitializeSlices();
            PlayCollectButtonSlideIn();
        }

        private void PlayCollectButtonSlideIn()
        {
            if (!collectButton) return;

            RectTransform rt = collectButton.transform as RectTransform;
            if (!rt) return;

            Vector2 restPos = rt.anchoredPosition;
            rt.anchoredPosition = restPos + Vector2.right * SlideInOffset;

            rt.DOAnchorPos(restPos, SlideInDuration)
                .SetEase(Ease.OutCubic);
        }

        private static void SeedRandomFromCrypto()
        {
            byte[] bytes = new byte[4];
            using (RNGCryptoServiceProvider rng = new())
                rng.GetBytes(bytes);

            Random.InitState(BitConverter.ToInt32(bytes, 0));
        }

        private void OnEnable()
        {
            EventManager.SpinEvents.OnSpinStarted += Spin;
            EventManager.SpinEvents.OnWheelChanged += OnWheelConfigChanged;

            if (spinButton)
                spinButton.onClick.AddListener(HandleSpin);

            if (collectButton)
                collectButton.onClick.AddListener(HandleCollect);
        }

        private void OnDisable()
        {
            EventManager.SpinEvents.OnSpinStarted -= Spin;
            EventManager.SpinEvents.OnWheelChanged -= OnWheelConfigChanged;

            if (spinButton)
                spinButton.onClick.RemoveListener(HandleSpin);

            if (collectButton)
                collectButton.onClick.RemoveListener(HandleCollect);

            StopPulse();
        }

        private void HandleSpin()
        {
            if (_gameController)
                _gameController.RequestSpin();
        }

        private void HandleCollect()
        {
            AudioManager.Instance.PlaySound(Sound.ButtonClick);
            HapticManager.Instance.PlayHaptic(HapticType.Click);

            if (_gameController)
                _gameController.RequestCollect();
        }

        private void InitializeSlices()
        {
            if (_slices.Count > 0) return;

            for (int i = 0; i < WheelConfig.SliceCount; i++)
            {
                WheelSlice slice = PoolManager.Instance.SpawnWheelSlice(wheelTransform);
                slice.name = $"Slice_{i}";

                RectTransform rt = slice.GetComponent<RectTransform>();

                float angleRad = (90f - SliceAngle * i) * Mathf.Deg2Rad;
                rt.localPosition = new(
                    Mathf.Cos(angleRad) * SliceRadius,
                    Mathf.Sin(angleRad) * SliceRadius,
                    0f);
                rt.localRotation = Quaternion.Euler(0f, 0f, -SliceAngle * i);
                rt.localScale = Vector3.one;

                _slices.Add(slice);
            }
        }

        private void OnWheelConfigChanged(WheelConfig config)
        {
            bool isFirstSetup = !_currentConfig;
            _currentConfig = config;

            if (isFirstSetup)
            {
                SetupWheel();
                PlayOpenAnimation();
            }
            else
            {
                PlayCloseAnimation(() =>
                {
                    SetupWheel();
                    wheelTransform.localRotation = Quaternion.identity;
                    PlayOpenAnimation();
                });
            }
        }

        private void PlayOpenAnimation()
        {
            _isTransitioning = true;
            SetSpinButtonActive(false);
            SetCollectButtonActive(true);

            Vector3 startPos = _wheelRestPosition + Vector3.down * OffScreenOffset;
            _wheelParent.localPosition = startPos;
            _wheelParent.localScale = Vector3.zero;

            Sequence openSeq = DOTween.Sequence();

            openSeq.Append(
                _wheelParent.DOLocalMove(_wheelRestPosition, TransitionDuration)
                    .SetEase(Ease.OutBack));

            openSeq.Join(
                _wheelParent.DOScale(Vector3.one, TransitionDuration)
                    .SetEase(Ease.OutBack));

            openSeq.OnComplete(() =>
            {
                _isTransitioning = false;
                SetSpinButtonActive(true);
                SetCollectButtonActive(true);
                EventManager.SpinEvents.FireWheelTransitionComplete();
            });
        }

        private void PlayCloseAnimation(TweenCallback onComplete)
        {
            _isTransitioning = true;
            SetSpinButtonActive(false);
            SetCollectButtonActive(true);

            Vector3 targetPos = _wheelRestPosition + Vector3.down * OffScreenOffset;

            Sequence closeSeq = DOTween.Sequence();

            closeSeq.Append(
                _wheelParent.DOLocalMove(targetPos, TransitionDuration)
                    .SetEase(Ease.InBack));

            closeSeq.Join(
                _wheelParent.DOScale(Vector3.zero, TransitionDuration)
                    .SetEase(Ease.InBack));

            closeSeq.OnComplete(onComplete);
        }

        private void SetupWheel()
        {
            if (!_currentConfig) return;

            if (_wheelBaseImage)
                _wheelBaseImage.sprite = _currentConfig.WheelBaseSprite;

            if (indicatorImage)
                indicatorImage.sprite = _currentConfig.IndicatorSprite;

            List<RewardData> filteredRewards = RewardsManager.Instance.GetFilteredRewards(_currentConfig.WheelType);
            if (filteredRewards.Count == 0) return;

            int bombSliceIndex = _currentConfig.HasBomb
                ? Random.Range(0, WheelConfig.SliceCount)
                : -1;

            for (int i = 0; i < WheelConfig.SliceCount; i++)
            {
                if (i == bombSliceIndex)
                    _slices[i].SetupAsBomb(_currentConfig.BombIcon);
                else
                    _slices[i].Setup(SelectWeightedReward(filteredRewards));
            }
        }

        private static RewardData SelectWeightedReward(List<RewardData> rewards)
        {
            float totalWeight = rewards.Sum(reward => reward.Rarity);
            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            foreach (RewardData reward in rewards)
            {
                cumulativeWeight += reward.Rarity;
                if (cumulativeWeight >= randomValue)
                    return reward;
            }

            return rewards[^1];
        }

        private void Spin()
        {
            if (_isSpinning || _isTransitioning) return;

            _isSpinning = true;
            SetSpinButtonActive(false);
            SetCollectButtonActive(false);

            int fullRotations = Random.Range(MinFullRotations, MaxFullRotations);
            int targetSlice = Random.Range(0, WheelConfig.SliceCount);
            float totalRotation = fullRotations * 360f + targetSlice * SliceAngle
                                                       + Random.Range(SliceMinOffset, SliceMaxOffset);

            _selectedSliceIndex = targetSlice;

            Sequence spinSequence = DOTween.Sequence();

            spinSequence.Append(
                wheelTransform
                    .DORotate(new(0f, 0f, -WindUpAngle), WindUpDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InOutSine));

            spinSequence.Join(
                _wheelParent
                    .DOScale(WindUpScale, WindUpDuration)
                    .SetEase(Ease.InOutSine));

            spinSequence.Append(
                wheelTransform
                    .DORotate(new(0f, 0f, -totalRotation), SpinDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuart));

            spinSequence.Join(
                _wheelParent
                    .DOScale(1f, ScaleBackDuration)
                    .SetEase(Ease.Flash));

            spinSequence.OnComplete(SnapToNearestSlice);
        }

        private void SnapToNearestSlice()
        {
            float currentZ = wheelTransform.localEulerAngles.z;
            float nearestAngle = Mathf.Round(currentZ / SliceAngle) * SliceAngle;

            int sliceIndex = Mathf.RoundToInt(nearestAngle / SliceAngle) % WheelConfig.SliceCount;
            _selectedSliceIndex = sliceIndex;

            wheelTransform
                .DORotate(new(0f, 0f, nearestAngle), SnapDuration, RotateMode.Fast)
                .SetEase(Ease.OutBack)
                .OnComplete(OnSpinComplete);
        }

        private void OnSpinComplete()
        {
            _isSpinning = false;

            if (_slices[_selectedSliceIndex].IsBomb)
                EventManager.GameEvents.FireBombHit();
            else
            {
                WheelSlice winningSlice = _slices[_selectedSliceIndex];
                EventManager.RewardEvents.FireRewardFlyStarted(
                    winningSlice.Reward, winningSlice.IconWorldPosition);
            }

            EventManager.SpinEvents.FireSpinEnded();
        }

        private void SetSpinButtonActive(bool active)
        {
            if (!spinButton) return;

            spinButton.interactable = active;

            if (active)
                StartPulse();
            else
                StopPulse();
        }

        private void SetCollectButtonActive(bool active)
        {
            if (collectButton)
                collectButton.interactable = active;
        }

        private void StartPulse()
        {
            if (!spinButton) return;

            StopPulse();

            _pulseTween = spinButton.transform
                .DOScale(1.1f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopPulse()
        {
            if (_pulseTween != null && _pulseTween.IsActive())
                _pulseTween.Kill();

            if (spinButton)
                spinButton.transform.localScale = Vector3.one;
        }
    }
}
