using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VertigoSpin.Project.Scripts.UI;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private List<Panel> panels = new();
        [field: SerializeField] public Canvas Canvas{ get; private set; }

        private void OnEnable()
        {
            EventManager.GameEvents.OnBombHit += HandleBombHit;
            EventManager.GameEvents.OnGameOver += HandleGameOver;
            EventManager.GameEvents.OnVictory += HandleVictory;
        }

        private void OnDisable()
        {
            EventManager.GameEvents.OnBombHit -= HandleBombHit;
            EventManager.GameEvents.OnGameOver -= HandleGameOver;
            EventManager.GameEvents.OnVictory -= HandleVictory;
        }

        private void ShowPanel(PanelType panelType, bool hideOthers = false)
        {
            if (hideOthers)
            {
                IEnumerable<Panel> othersToHide = panels.Where(p => p.PanelType != panelType);
                foreach (Panel panel in othersToHide)
                    panel.Hide();
            }

            IEnumerable<Panel> toShow = panels.Where(p => p.PanelType == panelType);
            foreach (Panel panel in toShow)
                panel.Show();
        }

        public void HidePanel(PanelType panelType)
        {
            IEnumerable<Panel> toHide = panels.Where(p => p.PanelType == panelType);
            foreach (Panel panel in toHide)
                panel.Hide();
        }

        public Vector2 GetScreenPosition(Vector3 worldPosition)
        {
            if (!Canvas || Canvas.worldCamera == null)
                return Vector2.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Canvas.transform as RectTransform,
                Canvas.worldCamera.WorldToScreenPoint(worldPosition),
                Canvas.worldCamera,
                out Vector2 localPoint
            );

            return localPoint;
        }

        public static Tween TextAnimation(TMP_Text text, float target = 1.2f, float duration = 0.2f,
            Ease easeType = Ease.OutBack)
        {
            text.transform.DOKill();
            text.transform.localScale = Vector3.one;

            return text.transform.DOScale(target, duration)
                .SetEase(easeType)
                .SetLoops(2, LoopType.Yoyo);
        }

        private void HandleBombHit() => ShowPanel(PanelType.Revive);
        private void HandleGameOver() => ShowPanel(PanelType.GameOver, hideOthers: true);
        private void HandleVictory() => ShowPanel(PanelType.Victory, hideOthers: true);
    }
}
