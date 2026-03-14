using UnityEngine;
using VertigoSpin.Project.Scripts.Pooling;
using VertigoSpin.Project.Scripts.UI;
using VertigoSpin.Project.Scripts.Utils;
using VertigoSpin.Project.Scripts.Wheel;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class PoolManager : MonoSingleton<PoolManager>
    {
        [Header("Wheel Pools")]
        [SerializeField] private PoolStats<WheelSlice> wheelSlicePoolStats;

        [Header("UI Pools")]
        [SerializeField] private PoolStats<InventorySlotUI> inventorySlotPoolStats;
        [SerializeField] private PoolStats<RectTransform> zoneCardPoolStats;
        [SerializeField] private PoolStats<FlyingReward> flyingRewardPoolStats;

        private Pool<WheelSlice> _wheelSlicePool;
        private Pool<InventorySlotUI> _inventorySlotPool;
        private Pool<RectTransform> _zoneCardPool;
        private Pool<FlyingReward> _flyingRewardPool;

        private void Awake()
        {
            if (wheelSlicePoolStats.Prefab)
                _wheelSlicePool = new(wheelSlicePoolStats, transform);

            if (inventorySlotPoolStats.Prefab)
                _inventorySlotPool = new(inventorySlotPoolStats, transform);

            if (zoneCardPoolStats.Prefab)
                _zoneCardPool = new(zoneCardPoolStats, transform);

            if (flyingRewardPoolStats.Prefab)
                _flyingRewardPool = new(flyingRewardPoolStats, transform);
        }

        public WheelSlice SpawnWheelSlice(Transform parent)
        {
            if (_wheelSlicePool == null) return null;
            WheelSlice slice = _wheelSlicePool.Spawn();
            slice.transform.SetParent(parent, false);
            return slice;
        }

        public void ReturnWheelSlice(WheelSlice slice)
        {
            if (_wheelSlicePool == null || !slice) return;
            _wheelSlicePool.Return(slice);
        }

        public InventorySlotUI SpawnInventorySlot(Transform parent)
        {
            if (_inventorySlotPool == null) return null;
            InventorySlotUI slot = _inventorySlotPool.Spawn();
            slot.transform.SetParent(parent, false);
            return slot;
        }

        public void ReturnInventorySlot(InventorySlotUI slot)
        {
            if (_inventorySlotPool == null || !slot) return;
            _inventorySlotPool.Return(slot);
        }

        public RectTransform SpawnZoneCard(Transform parent)
        {
            if (_zoneCardPool == null) return null;
            RectTransform card = _zoneCardPool.Spawn();
            card.SetParent(parent, false);
            return card;
        }

        public void ReturnZoneCard(RectTransform card)
        {
            if (_zoneCardPool == null || !card) return;
            _zoneCardPool.Return(card);
        }

        public FlyingReward SpawnFlyingReward(Transform parent)
        {
            if (_flyingRewardPool == null) return null;
            FlyingReward reward = _flyingRewardPool.Spawn();
            reward.transform.SetParent(parent, false);
            return reward;
        }

        public void ReturnFlyingReward(FlyingReward reward)
        {
            if (_flyingRewardPool == null || !reward) return;
            _flyingRewardPool.Return(reward);
        }
    }
}
