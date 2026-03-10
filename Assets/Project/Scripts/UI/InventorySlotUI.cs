using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoSpin.Project.Scripts.Data;

namespace VertigoSpin.Project.Scripts.UI
{
    public sealed class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Setup(RewardData reward)
        {
            if (!reward) return;

            if (iconImage)
                iconImage.sprite = reward.Icon;

            if (nameText)
                nameText.text = reward.RewardName;
        }
    }
}
