using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Audio;
using VertigoSpin.Project.Scripts.Game;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class ZoneIndicatorUI : MonoBehaviour
    {
        [Header("Card Setup")]
        [SerializeField] private Transform cardContainer;

        [Header("Next Spin Labels")]
        [SerializeField] private RectTransform nextSilverParent;
        [SerializeField] private RectTransform nextGoldParent;
        [SerializeField] private TextMeshProUGUI nextSilverText;
        [SerializeField] private TextMeshProUGUI nextGoldText;

        [Header("Text Colors")]
        [SerializeField] private Color normalTextColor = Color.white;
        [SerializeField] private Color safeTextColor = new(0.2f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color superTextColor = new(1f, 0.84f, 0f, 1f);
        [SerializeField] private Color currentZoneTextColor = new(0.2f, 0.6f, 1f, 1f);
        [SerializeField] private Color passedZoneTint = new(0.3f, 0.3f, 0.3f, 1f);
        
        private const float ScrollDuration = 0.3f;
        private const float SlideInDuration = 0.6f;
        private const float SlideInOffset = 1200f;

        private const float CardWidth = 240f;
        private const float CardSpacing = 10f;
        private const float CardStride = CardWidth + CardSpacing;
        private const int VisibleBuffer = 5;

        private readonly Dictionary<int, RectTransform> _activeCards = new();
        private readonly Dictionary<int, TextMeshProUGUI> _activeCardTexts = new();
        private readonly List<int> _removalBuffer = new();
        private int _currentZone = 1;
        private int _totalZones;
        private RectTransform _viewportRect;
        private RectTransform _containerRect;

        private void Awake()
        {
            _viewportRect = transform as RectTransform;
            _containerRect = cardContainer as RectTransform;

            SetupContainer();
        }

        private void Start()
        {
            Initialize();
            PlaySlideIn();
            PlayLabelsSlideIn();
        }

        private void OnEnable() => EventManager.ZoneEvents.OnZoneAdvanced += HandleZoneAdvanced;
        private void OnDisable() => EventManager.ZoneEvents.OnZoneAdvanced -= HandleZoneAdvanced;

        private void Initialize(int totalZones = ZoneManager.MaxZone - 1)
        {
            _totalZones = totalZones;
            _currentZone = 1;
            ReturnAllCards();
            ReconcileActiveCards(1);
            UpdateColors(1);
            UpdateNextSpinLabels(1, animate: false);
            ScrollToZone(1, instant: true);
        }

        private (int min, int max) GetVisibleRange(int centerZone)
        {
            int min = Mathf.Max(1, centerZone - VisibleBuffer);
            int max = Mathf.Min(_totalZones, centerZone + VisibleBuffer);
            return (min, max);
        }

        private void ReconcileActiveCards(int centerZone)
        {
            if (!cardContainer) return;

            (int rangeMin, int rangeMax) = GetVisibleRange(centerZone);

            ReturnOutOfRangeCards(rangeMin, rangeMax);
            SpawnMissingCards(rangeMin, rangeMax);
        }

        private void ReturnOutOfRangeCards(int rangeMin, int rangeMax)
        {
            _removalBuffer.Clear();

            foreach (int zone in _activeCards.Keys)
            {
                if (zone < rangeMin || zone > rangeMax)
                    _removalBuffer.Add(zone);
            }

            foreach (int zone in _removalBuffer)
            {
                PoolManager.Instance.ReturnZoneCard(_activeCards[zone]);
                _activeCards.Remove(zone);
                _activeCardTexts.Remove(zone);
            }
        }

        private void SpawnMissingCards(int rangeMin, int rangeMax)
        {
            for (int zone = rangeMin; zone <= rangeMax; zone++)
            {
                if (_activeCards.ContainsKey(zone)) continue;
                SpawnCard(zone);
            }
        }

        private void SpawnCard(int zone)
        {
            RectTransform card = PoolManager.Instance.SpawnZoneCard(cardContainer);
            if (!card) return;

            card.localScale = Vector3.one;
            card.anchorMin = new(0f, 0.5f);
            card.anchorMax = new(0f, 0.5f);
            card.anchoredPosition = new((zone - 1) * CardStride + CardWidth / 2f, 0f);

            TextMeshProUGUI tmp = card.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp) tmp.SetText("{0}", zone);

            _activeCards[zone] = card;
            _activeCardTexts[zone] = tmp;
        }

        private void ReturnAllCards()
        {
            foreach (RectTransform card in _activeCards.Values)
            {
                if (card)
                    PoolManager.Instance.ReturnZoneCard(card);
            }

            _activeCards.Clear();
            _activeCardTexts.Clear();
        }

        private void HandleZoneAdvanced(int zone)
        {
            bool isRestart = zone < _currentZone;
            _currentZone = zone;

            if (isRestart)
            {
                ReturnAllCards();
                ReconcileActiveCards(zone);
                UpdateColors(zone);
                UpdateNextSpinLabels(zone);
                ScrollToZone(zone, instant: false);
                return;
            }

            ReconcileActiveCards(zone);
            UpdateColors(zone);
            UpdateNextSpinLabels(zone);
            ScrollToZone(zone, instant: false);

            if (zone > 1)
            {
                AudioManager.Instance.PlaySound(Sound.ZoneAdvance);
                HapticManager.Instance.PlayHaptic(HapticType.LightImpact);
            }
        }

        private void UpdateColors(int currentZone)
        {
            foreach (KeyValuePair<int, TextMeshProUGUI> kvp in _activeCardTexts)
            {
                SetCardColor(kvp.Value, kvp.Key, currentZone);
            }
        }

        private void ScrollToZone(int zone, bool instant)
        {
            if (!_containerRect || !_viewportRect) return;
            if (zone < 1 || zone > _totalZones) return;

            float cardCenterX = (zone - 1) * CardStride + CardWidth / 2f;
            float viewportWidth = _viewportRect.rect.width;
            float targetX = -(cardCenterX - viewportWidth / 2f) - 15f;

            if (instant)
            {
                _containerRect.anchoredPosition = new(targetX, _containerRect.anchoredPosition.y);
            }
            else
            {
                _containerRect.DOKill();
                _containerRect.DOAnchorPosX(targetX, ScrollDuration)
                    .SetEase(Ease.OutCubic);
            }
        }

        private void PlaySlideIn()
        {
            if (!_viewportRect) return;

            Vector2 restPos = _viewportRect.anchoredPosition;
            _viewportRect.anchoredPosition = restPos + Vector2.up * SlideInOffset;

            _viewportRect.DOAnchorPos(restPos, SlideInDuration)
                .SetEase(Ease.OutCubic);
        }

        private void PlayLabelsSlideIn()
        {
            SlideFromRight(nextSilverParent);
            SlideFromRight(nextGoldParent);
        }

        private static void SlideFromRight(RectTransform rect)
        {
            if (!rect) return;

            Vector2 restPos = rect.anchoredPosition;
            rect.anchoredPosition = restPos + Vector2.right * SlideInOffset;

            rect.DOAnchorPos(restPos, SlideInDuration)
                .SetEase(Ease.OutCubic);
        }

        private void SetupContainer()
        {
            if (!_containerRect) return;

            _containerRect.anchorMin = new(0f, 0.5f);
            _containerRect.anchorMax = new(0f, 0.5f);
            _containerRect.pivot = new(0f, 0.5f);

            // Disable layout components — manual positioning handles everything
            HorizontalLayoutGroup layout = _containerRect.GetComponent<HorizontalLayoutGroup>();
            if (layout)
                layout.enabled = false;

            ContentSizeFitter fitter = _containerRect.GetComponent<ContentSizeFitter>();
            if (fitter)
                fitter.enabled = false;

            // Set container size manually
            const float containerWidth = (ZoneManager.MaxZone - 1) * CardStride - CardSpacing;
            _containerRect.sizeDelta = new(containerWidth, _viewportRect.rect.height);
        }

        private void UpdateNextSpinLabels(int currentZone, bool animate = true)
        {
            int nextSilver = GetNextZoneOfType(currentZone, ZoneManager.SafeZoneInterval);
            int nextGold = GetNextZoneOfType(currentZone, ZoneManager.SuperZoneInterval);

            UpdateLabel(nextSilverText, nextSilver, "NEXT SILVER: ", animate);
            UpdateLabel(nextGoldText, nextGold, "NEXT GOLD: ", animate);
        }

        private static void UpdateLabel(TextMeshProUGUI text, int nextZone, string prefix, bool animate)
        {
            if (!text) return;

            string value = nextZone > 0 ? $"{prefix}{nextZone}" : string.Empty;
            if (text.text == value) return;

            text.SetText(value);

            if (animate && value.Length > 0)
                UIManager.TextAnimation(text);
        }

        private int GetNextZoneOfType(int currentZone, int interval)
        {
            int next = ((currentZone / interval) + 1) * interval;
            return next <= _totalZones ? next : 0;
        }

        private void SetCardColor(TextMeshProUGUI tmp, int zone, int currentZone)
        {
            if (!tmp) return;

            if (zone == currentZone)
                tmp.color = currentZoneTextColor;
            else if (zone < currentZone)
                tmp.color = passedZoneTint;
            else
                tmp.color = GetZoneTextColor(zone);
        }

        private Color GetZoneTextColor(int zone)
        {
            if (zone % ZoneManager.SuperZoneInterval == 0) return superTextColor;

            return zone % ZoneManager.SafeZoneInterval == 0
                ? safeTextColor
                : normalTextColor;
        }
    }
}
