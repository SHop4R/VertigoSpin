using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Data;

namespace VertigoSpin.Project.Scripts.Wheel
{
    public sealed class WheelSlice : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI rewardText;

        public RewardData Reward{ get; private set; }
        public bool IsBomb{ get; private set; }

        public void Setup(RewardData reward)
        {
            Reward = reward;
            IsBomb = false;

            iconImage.sprite = reward.Icon;
            iconImage.enabled = true;
            rewardText.text = reward.CoinValue > 0 ? reward.CoinValue.ToString() : reward.RewardName;
        }

        public void SetupAsBomb(Sprite bombIcon)
        {
            Reward = null;
            IsBomb = true;

            iconImage.sprite = bombIcon;
            iconImage.enabled = true;
            rewardText.text = "BOMB";
        }

        public void Clear()
        {
            Reward = null;
            IsBomb = false;

            iconImage.sprite = null;
            iconImage.enabled = false;
            rewardText.text = string.Empty;
        }

        private void OnValidate()
        {
            if (iconImage)
                iconImage.raycastTarget = false;
        }
    }
}
