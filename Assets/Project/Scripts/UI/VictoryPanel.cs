using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Audio;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class VictoryPanel : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Image victoryImage;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private Button continueButton;

        [Header("Timing")]
        [SerializeField] private float imageScaleDuration = 0.5f;
        [SerializeField] private float coinShowDelay = 0.5f;
        [SerializeField] private float coinCountDuration = 0.6f;

        private int _pendingCoins;
        private Sequence _animSequence;

        private void OnDisable()
        {
            if (continueButton)
                continueButton.onClick.RemoveAllListeners();

            _animSequence?.Kill();
        }

        public void Show(int coins)
        {
            _pendingCoins = coins;
            PlayOpenSequence();

            if (continueButton)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(HandleContinue);
            }
        }

        private void PlayOpenSequence()
        {
            if (victoryImage)
            {
                victoryImage.gameObject.SetActive(true);
                victoryImage.transform.localScale = new(1f, 0f, 1f);
            }

            if (coinsText)
            {
                coinsText.gameObject.SetActive(true);
                coinsText.transform.localScale = Vector3.zero;
            }

            if (continueButton)
            {
                continueButton.gameObject.SetActive(true);
                continueButton.transform.localScale = Vector3.zero;
            }

            _animSequence?.Kill();
            _animSequence = DOTween.Sequence();

            if (victoryImage)
            {
                _animSequence.Append(
                    victoryImage.transform.DOScaleY(1f, imageScaleDuration)
                        .SetEase(Ease.OutBack));
            }

            // Step 2: Show coin text starting at player's coins before this run
            int totalCoins = CoinManager.Instance.Coins;
            int startCoins = totalCoins - _pendingCoins;

            _animSequence.AppendCallback(() =>
            {
                if (coinsText)
                    coinsText.text = startCoins.ToString();
            });

            _animSequence.AppendInterval(coinShowDelay);

            if (coinsText)
            {
                _animSequence.Append(
                    coinsText.transform.DOScale(1f, 0.3f)
                        .SetEase(Ease.OutBack));
            }

            // Wait after scale-in before counting
            _animSequence.AppendInterval(coinShowDelay);

            // Step 3: Count up from previous coins to new total, sync inventory clear
            _animSequence.AppendCallback(() => EventManager.GameEvents.FireVictoryCollect());

            if (coinsText && _pendingCoins > 0)
            {
                _animSequence.Append(
                    coinsText.DOCounter(startCoins, totalCoins, coinCountDuration, false)
                        .SetEase(Ease.OutQuad));

                _animSequence.AppendCallback(() =>
                {
                    AudioManager.Instance.PlaySound(Sound.CollectAll);
                    HapticManager.Instance.PlayHaptic(HapticType.Success);
                });
            }

            // Step 4: Scale in continue button
            if (continueButton)
            {
                _animSequence.Append(
                    continueButton.transform.DOScale(1f, 0.3f)
                        .SetEase(Ease.OutBack));
            }
        }

        private void HandleContinue()
        {
            AudioManager.Instance.PlaySound(Sound.ButtonClick);
            HapticManager.Instance.PlayHaptic(HapticType.Click);
            UIManager.Instance.HidePanel(PanelType.Victory);
            EventManager.GameEvents.FireGameRestart();
        }
    }
}
