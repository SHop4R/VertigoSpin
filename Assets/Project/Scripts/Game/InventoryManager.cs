using System.Collections.Generic;
using System.Linq;
using VertigoSpin.Project.Scripts.Data;

namespace VertigoSpin.Project.Scripts.Game
{
    public sealed class InventoryManager
    {
        private readonly List<RewardData> _collectedRewards = new();

        public int TotalCoinValue => _collectedRewards.Sum(r => r.CoinValue);

        public void AddReward(RewardData reward)
        {
            if (!reward) return;
            _collectedRewards.Add(reward);
        }

        public void Clear() => _collectedRewards.Clear();
    }
}
