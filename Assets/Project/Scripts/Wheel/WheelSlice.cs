using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Pooling;

namespace VertigoSpin.Project.Scripts.Wheel
{
    public sealed class WheelSlice : MonoBehaviour, IPoolable
    {
        [SerializeField] private Image iconImage;

        public RewardData Reward{ get; private set; }
        public bool IsBomb{ get; private set; }
        public Vector3 IconWorldPosition => iconImage ? iconImage.transform.position : transform.position;

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

        public void OnSpawn() { }

        public void OnReturn()
        {
            Clear();
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
