using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Pooling;

namespace VertigoSpin.Project.Scripts.Wheel
{
    public sealed class WheelSlice : MonoBehaviour, IPoolable
    {
        private Image _iconImage;

        public RewardData Reward{ get; private set; }
        public bool IsBomb{ get; private set; }
        
        public Vector3 IconWorldPosition 
            => _iconImage 
                ? _iconImage.transform.position
                : transform.position;

        private void Awake()
        {
            _iconImage = GetComponentInChildren<Image>();
            
            if (!_iconImage) return;
            _iconImage.raycastTarget = false;
            _iconImage.preserveAspect = true;
        }

        public void Setup(RewardData reward)
        {
            Reward = reward;
            IsBomb = false;

            _iconImage.sprite = reward.Icon;
            _iconImage.enabled = true;
        }

        public void SetupAsBomb(Sprite bombIcon)
        {
            Reward = null;
            IsBomb = true;

            _iconImage.sprite = bombIcon;
            _iconImage.enabled = true;
        }

        private void Clear()
        {
            Reward = null;
            IsBomb = false;

            _iconImage.sprite = null;
            _iconImage.enabled = false;
        }

        public void OnSpawn() {}
        public void OnReturn() => Clear();
    }
}
