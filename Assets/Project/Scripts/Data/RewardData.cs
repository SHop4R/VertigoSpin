using UnityEngine;

namespace VertigoSpin.Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "RewardData", menuName = "VertigoSpin/Reward Data")]
    public sealed class RewardData : ScriptableObject
    {
        [field: Header("Reward Info")]
        [field: SerializeField] public string RewardName{ get; private set; }
        [field: SerializeField] public Sprite Icon{ get; private set; }
        [field: SerializeField] public int CoinValue{ get; private set; }

        [field: Header("Spawn Settings")]
        [field: SerializeField, Range(0f, 1f), Tooltip("Higher = more likely to appear on the wheel")]
        public float Rarity{ get; private set; } = 0.5f;

        [field: SerializeField, Tooltip("Only appears on Gold (Super) wheels")]
        public bool SuperZoneOnly{ get; private set; }
    }
}
