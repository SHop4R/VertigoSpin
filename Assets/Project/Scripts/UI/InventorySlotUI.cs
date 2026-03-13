using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;
using VertigoSpin.Project.Scripts.Pooling;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class InventorySlotUI : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;

        private Image _iconImage;
        private int _count;

        private void Awake()
        {
            _iconImage = GetComponentInChildren<Image>();
            if (_iconImage) _iconImage.preserveAspect = true;
        }

        public void OnSpawn()
        {
            transform.localScale = Vector3.one;
        }

        public void OnReturn()
        {
            _count = 0;
            if (_iconImage) _iconImage.sprite = null;
            if (nameText) nameText.text = "";
            if (countText) countText.text = "";
        }

        public void Setup(RewardData reward)
        {
            if (!reward) return;

            _count = 1;

            if (_iconImage)
                _iconImage.sprite = reward.Icon;

            if (nameText)
                nameText.text = reward.RewardName;

            UpdateCountText();
        }

        public void IncrementCount()
        {
            _count++;
            UpdateCountText();

            if (countText)
                UIManager.TextAnimation(countText);
        }

        private void UpdateCountText()
        {
            if (countText)
                countText.text = $"x{_count}";
        }
    }
}
