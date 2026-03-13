using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        private const int MaxZone = 41;
        private const int SafeZoneInterval = 5;
        private const int SuperZoneInterval = 30;
        private const float ScrollDuration = 0.3f;
        private const float SlideInDuration = 0.6f;
        private const float SlideInOffset = 1200f;

        private readonly List<RectTransform> _cards = new();
        private int _totalZones;
        private RectTransform _viewportRect;
        private RectTransform _containerRect;

        private void Awake()
        {
            _viewportRect = transform as RectTransform;
            _containerRect = cardContainer as RectTransform;

            EnsureMask();
            SetupContainer();
        }

        private void Start()
        {
            Initialize();
            PlaySlideIn();
            PlayLabelsSlideIn();
        }

        private void OnEnable()
        {
            EventManager.ZoneEvents.OnZoneAdvanced += HandleZoneAdvanced;
        }

        private void OnDisable()
        {
            EventManager.ZoneEvents.OnZoneAdvanced -= HandleZoneAdvanced;
        }

        public void Initialize(int totalZones = MaxZone)
        {
            _totalZones = totalZones;
            ClearCards();
            CreateCards();
            UpdateColors(1);
            UpdateNextSpinLabels(1, animate: false);
            ScrollToZone(1, instant: true);
        }

        private void CreateCards()
        {
            if (cardContainer == null) return;

            for (int i = 0; i < _totalZones; i++)
            {
                RectTransform card = PoolManager.Instance.SpawnZoneCard(cardContainer);
                int zone = i + 1;
                SetCardZoneNumber(card, zone);
                _cards.Add(card);
            }
        }

        private void ClearCards()
        {
            foreach (RectTransform card in _cards)
            {
                if (card != null)
                    PoolManager.Instance.ReturnZoneCard(card);
            }
            _cards.Clear();
        }

        private void HandleZoneAdvanced(int zone)
        {
            UpdateColors(zone);
            UpdateNextSpinLabels(zone);
            ScrollToZone(zone, instant: false);
        }

        private void UpdateColors(int currentZone)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                int zone = i + 1;
                SetCardColor(_cards[i], zone, currentZone);
            }
        }

        private void ScrollToZone(int currentZone, bool instant)
        {
            if (_containerRect == null || _viewportRect == null) return;

            int cardIndex = currentZone - 1;
            if (cardIndex < 0 || cardIndex >= _cards.Count) return;

            Canvas.ForceUpdateCanvases();

            RectTransform cardRect = _cards[cardIndex];
            if (cardRect == null) return;

            Vector3 cardWorldPos = cardRect.position;
            Vector3 cardInViewport = _viewportRect.InverseTransformPoint(cardWorldPos);
            float targetX = _containerRect.anchoredPosition.x - cardInViewport.x;

            if (instant)
            {
                _containerRect.anchoredPosition = new Vector2(targetX, _containerRect.anchoredPosition.y);
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
            if (_viewportRect == null) return;

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

        private static void SlideFromRight(RectTransform rt)
        {
            if (rt == null) return;

            Vector2 restPos = rt.anchoredPosition;
            rt.anchoredPosition = restPos + Vector2.right * SlideInOffset;

            rt.DOAnchorPos(restPos, SlideInDuration)
                .SetEase(Ease.OutCubic);
        }

        private void EnsureMask()
        {
            if (GetComponent<RectMask2D>() == null)
                gameObject.AddComponent<RectMask2D>();
        }

        private void SetupContainer()
        {
            if (_containerRect == null) return;

            // Anchor to left-center so container extends rightward from the left edge
            _containerRect.anchorMin = new Vector2(0f, 0.5f);
            _containerRect.anchorMax = new Vector2(0f, 0.5f);
            _containerRect.pivot = new Vector2(0f, 0.5f);

            // Auto-size container width to fit all cards
            ContentSizeFitter fitter = _containerRect.GetComponent<ContentSizeFitter>();
            if (fitter == null)
                fitter = _containerRect.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            // Match container height to viewport
            _containerRect.sizeDelta = new Vector2(0f, _viewportRect.rect.height);

            // Fix layout group so cards keep their natural size
            HorizontalLayoutGroup layout = _containerRect.GetComponent<HorizontalLayoutGroup>();
            if (layout != null)
            {
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                layout.childControlWidth = false;
                layout.childControlHeight = false;
                layout.spacing = 10f;
                layout.childAlignment = TextAnchor.MiddleLeft;
            }
        }

        private void UpdateNextSpinLabels(int currentZone, bool animate = true)
        {
            int nextSilver = GetNextZoneOfType(currentZone, SafeZoneInterval);
            int nextGold = GetNextZoneOfType(currentZone, SuperZoneInterval);

            if (nextSilverText != null)
            {
                string silverValue = nextSilver > 0 ? $"NEXT SILVER: {nextSilver}" : "";
                if (nextSilverText.text != silverValue)
                {
                    nextSilverText.text = silverValue;
                    if (animate && silverValue.Length > 0)
                        UIManager.TextAnimation(nextSilverText);
                }
            }

            if (nextGoldText != null)
            {
                string goldValue = nextGold > 0 ? $"NEXT GOLD: {nextGold}" : "";
                if (nextGoldText.text != goldValue)
                {
                    nextGoldText.text = goldValue;
                    if (animate && goldValue.Length > 0)
                        UIManager.TextAnimation(nextGoldText);
                }
            }
        }

        private int GetNextZoneOfType(int currentZone, int interval)
        {
            int next = ((currentZone / interval) + 1) * interval;
            return next <= _totalZones ? next : 0;
        }

        private void SetCardZoneNumber(RectTransform card, int zone)
        {
            TextMeshProUGUI tmp = card.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = zone.ToString();
        }

        private void SetCardColor(RectTransform card, int zone, int currentZone)
        {
            TextMeshProUGUI tmp = card.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null) return;

            if (zone == currentZone)
            {
                tmp.color = currentZoneTextColor;
            }
            else if (zone < currentZone)
            {
                tmp.color = passedZoneTint;
            }
            else
            {
                tmp.color = GetZoneTextColor(zone);
            }
        }

        private Color GetZoneTextColor(int zone)
        {
            if (zone % SuperZoneInterval == 0) return superTextColor;
            if (zone % SafeZoneInterval == 0) return safeTextColor;
            return normalTextColor;
        }
    }
}
