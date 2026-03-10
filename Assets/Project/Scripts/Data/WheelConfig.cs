using UnityEngine;

namespace VertigoSpin.Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "WheelConfig", menuName = "VertigoSpin/Wheel Config")]
    public sealed class WheelConfig : ScriptableObject
    {
        [field: Header("Wheel Type")]
        [field: SerializeField] public WheelType WheelType{ get; private set; }

        [field: Header("Bomb Settings")]
        [field: SerializeField] public bool HasBomb{ get; private set; } = true;

        [field: SerializeField, Tooltip("Icon shown for the bomb slice")]
        public Sprite BombIcon{ get; private set; }

        [field: Header("Wheel Visuals")]
        [field: SerializeField] public Sprite WheelBaseSprite{ get; private set; }
        [field: SerializeField] public Sprite IndicatorSprite{ get; private set; }

        public const int SliceCount = 8;
    }
}
