using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.Wheel
{
    public sealed class WheelController : MonoBehaviour
    {
        [Header("Wheel References")]
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Image wheelBaseImage;
        [SerializeField] private Image indicatorImage;

        [Header("Slice Prefab")]
        [SerializeField] private WheelSlice slicePrefab;
        [SerializeField] private Transform sliceContainer;

        private readonly List<WheelSlice> _slices = new();
        private WheelConfig _currentConfig;
        private int _selectedSliceIndex;
        private bool _isSpinning;

        private const float WindUpDuration = 0.4f;
        private const float WindUpAngle = 30f;
        private const float SpinDuration = 4f;
        private const float SnapDuration = 0.5f;
        private const int MinFullRotations = 5;
        private const int MaxFullRotations = 9;
        private const float SliceAngle = 360f / WheelConfig.SliceCount;
        private const float SliceMinOffset = 2f;
        private const float SliceMaxOffset = 43f;
        private const float SliceSize = 200f;
        private const float SliceRadius = 350f;

        private void Awake()
        {
            SeedRandomFromCrypto();
            InitializeSlices();
        }

        private static void SeedRandomFromCrypto()
        {
            byte[] bytes = new byte[4];
            using (RNGCryptoServiceProvider rng = new())
                rng.GetBytes(bytes);

            Random.InitState(System.BitConverter.ToInt32(bytes, 0));
        }

        private void OnEnable()
        {
            EventManager.SpinEvents.OnSpinStarted += Spin;
            EventManager.SpinEvents.OnWheelChanged += OnWheelConfigChanged;
        }

        private void OnDisable()
        {
            EventManager.SpinEvents.OnSpinStarted -= Spin;
            EventManager.SpinEvents.OnWheelChanged -= OnWheelConfigChanged;
        }

        private void InitializeSlices()
        {
            if (_slices.Count > 0) return;

            for (int i = 0; i < WheelConfig.SliceCount; i++)
            {
                WheelSlice slice = Instantiate(slicePrefab, sliceContainer);
                slice.name = $"Slice_{i}";

                RectTransform rt = slice.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(SliceSize, SliceSize);

                float angleRad = (90f - SliceAngle * i) * Mathf.Deg2Rad;
                rt.localPosition = new Vector3(
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
            _currentConfig = config;
            SetupWheel();
        }

        private void SetupWheel()
        {
            if (_currentConfig == null) return;

            if (wheelBaseImage != null)
                wheelBaseImage.sprite = _currentConfig.WheelBaseSprite;

            if (indicatorImage != null)
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
            if (_isSpinning) return;

            _isSpinning = true;

            int fullRotations = Random.Range(MinFullRotations, MaxFullRotations);
            int targetSlice = Random.Range(0, WheelConfig.SliceCount);
            float totalRotation = fullRotations * 360f + targetSlice * SliceAngle
                                  + Random.Range(SliceMinOffset, SliceMaxOffset);

            _selectedSliceIndex = targetSlice;

            Sequence spinSequence = DOTween.Sequence();

            // Wind-up: pull back slightly with InBack
            spinSequence.Append(
                wheelTransform
                    .DORotate(new(0f, 0f, WindUpAngle), WindUpDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InBack));

            // Main spin
            spinSequence.Append(
                wheelTransform
                    .DORotate(new(0f, 0f, -totalRotation), SpinDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuart));

            // Main spin completes, then snap to nearest 45° and fire result
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
                EventManager.RewardEvents.FireRewardEarned(_slices[_selectedSliceIndex].Reward);

            EventManager.SpinEvents.FireSpinEnded();
        }
    }
}
