using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class ZoneIndicatorUI : MonoBehaviour
    {
        [Header("Card Setup")]
        [SerializeField] private Transform cardContainer;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private int visibleCardCount = 5;

        [Header("Colors")]
        [SerializeField] private Color bronzeColor = new(0.8f, 0.5f, 0.2f, 1f);
        [SerializeField] private Color silverColor = new(0.75f, 0.75f, 0.75f, 1f);
        [SerializeField] private Color goldColor = new(1f, 0.84f, 0f, 1f);
        [SerializeField] private Color currentZoneColor = new(0.2f, 0.6f, 1f, 1f);

        private const int MaxZone = 41;
        private const int SafeZoneInterval = 5;
        private const int SuperZoneInterval = 30;
        private const float ScrollDuration = 0.3f;

        private readonly List<GameObject> _cards = new();
        private int _totalZones;

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            EventManager.ZoneEvents.OnZoneAdvanced += HandleZoneAdvanced;
            EventManager.GameEvents.OnGameRestart += HandleGameRestart;
        }

        private void OnDisable()
        {
            EventManager.ZoneEvents.OnZoneAdvanced -= HandleZoneAdvanced;
            EventManager.GameEvents.OnGameRestart -= HandleGameRestart;
        }

        private void HandleGameRestart()
        {
            Initialize();
        }

        public void Initialize(int totalZones = MaxZone)
        {
            _totalZones = totalZones;
            ClearCards();
            CreateCards();
            UpdateDisplay(1);
        }

        private void CreateCards()
        {
            if (cardPrefab == null || cardContainer == null) return;

            for (int i = 0; i < _totalZones; i++)
            {
                GameObject card = Instantiate(cardPrefab, cardContainer);
                int zone = i + 1;
                SetCardZoneNumber(card, zone);
                SetCardColor(card, zone, zone);
                _cards.Add(card);
            }
        }

        private void ClearCards()
        {
            foreach (GameObject card in _cards)
            {
                if (card != null)
                    Destroy(card);
            }
            _cards.Clear();
        }

        private void HandleZoneAdvanced(int zone)
        {
            UpdateDisplay(zone);
            AnimateScroll(zone);
        }

        private void UpdateDisplay(int currentZone)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                int zone = i + 1;
                SetCardColor(_cards[i], zone, currentZone);

                bool isVisible = zone >= currentZone - visibleCardCount / 2
                              && zone <= currentZone + visibleCardCount / 2;
                _cards[i].SetActive(isVisible);
            }
        }

        private void AnimateScroll(int currentZone)
        {
            if (cardContainer == null) return;

            int cardIndex = currentZone - 1;
            if (cardIndex < 0 || cardIndex >= _cards.Count) return;

            RectTransform containerRect = cardContainer as RectTransform;
            if (containerRect == null) return;

            float cardWidth = GetCardWidth();
            float targetX = -(cardIndex * cardWidth);

            containerRect.DOAnchorPosX(targetX, ScrollDuration)
                .SetEase(Ease.OutCubic);
        }

        private float GetCardWidth()
        {
            if (_cards.Count == 0) return 0f;

            RectTransform cardRect = _cards[0].transform as RectTransform;
            return cardRect != null ? cardRect.rect.width : 100f;
        }

        private void SetCardZoneNumber(GameObject card, int zone)
        {
            TextMeshProUGUI tmp = card.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = zone.ToString();
        }

        private void SetCardColor(GameObject card, int zone, int currentZone)
        {
            UnityEngine.UI.Image bg = card.GetComponent<UnityEngine.UI.Image>();
            if (bg == null) return;

            if (zone == currentZone)
            {
                bg.color = currentZoneColor;
                return;
            }

            bg.color = GetZoneColor(zone);
        }

        private Color GetZoneColor(int zone)
        {
            if (zone % SuperZoneInterval == 0) return goldColor;
            if (zone % SafeZoneInterval == 0) return silverColor;
            return bronzeColor;
        }
    }
}
