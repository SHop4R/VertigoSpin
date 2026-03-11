using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;

        private int _count;

        public RewardData Reward { get; private set; }

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
