using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Data;

namespace VertigoSpin.Project.Scripts.Wheel
{
    public sealed class WheelSlice : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        public RewardData Reward{ get; private set; }
        public bool IsBomb{ get; private set; }

        public void Setup(RewardData reward)
        {
            Reward = reward;
            IsBomb = false;

            iconImage.sprite = reward.Icon;
            iconImage.enabled = true;
        }

        public void SetupAsBomb(Sprite bombIcon)
        {
            Reward = null;
            IsBomb = true;

            iconImage.sprite = bombIcon;
            iconImage.enabled = true;
        }

        public void Clear()
        {
            Reward = null;
            IsBomb = false;

            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        private void OnValidate()
        {
            if (iconImage)
            {
                iconImage.raycastTarget = false;
                iconImage.preserveAspect = true;
            }
        }
    }
}
