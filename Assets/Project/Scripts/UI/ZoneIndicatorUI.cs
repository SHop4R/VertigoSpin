using System.Collections.Generic;
using System.Linq;
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
        private readonly List<TextMeshProUGUI> _cardTexts = new();
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

        private void OnEnable() => EventManager.ZoneEvents.OnZoneAdvanced += HandleZoneAdvanced;
        private void OnDisable() => EventManager.ZoneEvents.OnZoneAdvanced -= HandleZoneAdvanced;

        private void Initialize(int totalZones = MaxZone)
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
            if (!cardContainer) return;

            for (int i = 0; i < _totalZones; i++)
            {
                RectTransform card = PoolManager.Instance.SpawnZoneCard(cardContainer);
                TextMeshProUGUI tmp = card.GetComponentInChildren<TextMeshProUGUI>();
                int zone = i + 1;

                if (tmp)
                    tmp.text = zone.ToString();

                _cards.Add(card);
                _cardTexts.Add(tmp);
            }
        }

        private void ClearCards()
        {
            foreach (RectTransform card in _cards.Where(card => card))
            {
                PoolManager.Instance.ReturnZoneCard(card);
            }
            
            _cards.Clear();
            _cardTexts.Clear();
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
                SetCardColor(i, i + 1, currentZone);
            }
        }

        private void ScrollToZone(int currentZone, bool instant)
        {
            if (!_containerRect || !_viewportRect) return;

            int cardIndex = currentZone - 1;
            if (cardIndex < 0 || cardIndex >= _cards.Count) return;

            Canvas.ForceUpdateCanvases();

            RectTransform cardRect = _cards[cardIndex];
            if (!cardRect) return;

            Vector3 cardWorldPos = cardRect.position;
            Vector3 cardInViewport = _viewportRect.InverseTransformPoint(cardWorldPos);
            float targetX = _containerRect.anchoredPosition.x - cardInViewport.x;

            if (instant)
                _containerRect.anchoredPosition = new(targetX, _containerRect.anchoredPosition.y);
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

        private void EnsureMask()
        {
            if (!GetComponent<RectMask2D>())
                gameObject.AddComponent<RectMask2D>();
        }

        private void SetupContainer()
        {
            if (!_containerRect) return;

            _containerRect.anchorMin = new(0f, 0.5f);
            _containerRect.anchorMax = new(0f, 0.5f);
            _containerRect.pivot = new(0f, 0.5f);

            ContentSizeFitter fitter = _containerRect.GetComponent<ContentSizeFitter>();
            
            if (!fitter)
                fitter = _containerRect.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            _containerRect.sizeDelta = new(0f, _viewportRect.rect.height);

            HorizontalLayoutGroup layout = _containerRect.GetComponent<HorizontalLayoutGroup>();
            
            if (!layout) return;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.MiddleLeft;
        }

        private void UpdateNextSpinLabels(int currentZone, bool animate = true)
        {
            int nextSilver = GetNextZoneOfType(currentZone, SafeZoneInterval);
            int nextGold = GetNextZoneOfType(currentZone, SuperZoneInterval);

            if (nextSilverText)
            {
                string silverValue = nextSilver > 0 
                    ? $"NEXT SILVER: {nextSilver}" 
                    : string.Empty;
                
                if (nextSilverText.text != silverValue)
                {
                    nextSilverText.text = silverValue;
                    if (animate && silverValue.Length > 0)
                        UIManager.TextAnimation(nextSilverText);
                }
            }

            if (!nextGoldText) return;
            
            string goldValue = nextGold > 0 
                ? $"NEXT GOLD: {nextGold}" 
                : string.Empty;
            
            if (nextGoldText.text == goldValue) return;
            nextGoldText.text = goldValue;
            
            if (animate && goldValue.Length > 0)
                UIManager.TextAnimation(nextGoldText);
        }

        private int GetNextZoneOfType(int currentZone, int interval)
        {
            int next = ((currentZone / interval) + 1) * interval;
            return next <= _totalZones ? next : 0;
        }

        private void SetCardColor(int cardIndex, int zone, int currentZone)
        {
            TextMeshProUGUI tmp = _cardTexts[cardIndex];
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
            if (zone % SuperZoneInterval == 0) return superTextColor;
            
            return zone % SafeZoneInterval == 0 
                ? safeTextColor 
                : normalTextColor;
        }
    }
}
