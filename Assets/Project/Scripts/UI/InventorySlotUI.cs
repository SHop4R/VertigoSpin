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
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;

        private int _count;

        public RewardData Reward { get; private set; }

        public void OnSpawn()
        {
            transform.localScale = Vector3.one;
        }

        public void OnReturn()
        {
            Reward = null;
            _count = 0;
            if (iconImage) iconImage.sprite = null;
            if (nameText) nameText.text = "";
            if (countText) countText.text = "";
        }

        public void Setup(RewardData reward)
        {
            if (!reward) return;

            Reward = reward;
            _count = 1;

            if (iconImage)
                iconImage.sprite = reward.Icon;

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
