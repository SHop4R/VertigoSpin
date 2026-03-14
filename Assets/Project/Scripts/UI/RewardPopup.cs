using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Animations;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;
using VertigoSpin.Project.Scripts.Utils.Helpers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class RewardPopup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TextMeshProUGUI rewardNameText;
        [SerializeField] private TextMeshProUGUI coinValueText;
        [SerializeField] private Transform popupRoot;

        private const float DisplayDuration = 1.5f;

        private Coroutine _hideCoroutine;

        private void OnEnable() => EventManager.RewardEvents.OnRewardEarned += HandleRewardEarned;
        private void OnDisable() => EventManager.RewardEvents.OnRewardEarned -= HandleRewardEarned;

        private void HandleRewardEarned(RewardData reward)
        {
            if (!reward || !popupRoot) return;

            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);

            SetRewardDisplay(reward);
            ItemAnimator.EnableAnim(popupRoot);
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private void SetRewardDisplay(RewardData reward)
        {
            if (rewardIcon)
                rewardIcon.sprite = reward.Icon;

            if (rewardNameText)
                rewardNameText.SetText(reward.RewardName);

            if (coinValueText)
                coinValueText.SetText("{0}", reward.CoinValue);
        }

        private IEnumerator HideAfterDelay()
        {
            yield return WaitHelper.WaitForSeconds(DisplayDuration);

            if (popupRoot)
                ItemAnimator.DisableAnim(popupRoot);

            _hideCoroutine = null;
        }
    }
}
