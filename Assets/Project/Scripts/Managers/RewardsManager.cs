using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class RewardsManager : MonoSingleton<RewardsManager>
    {
        private RewardData[] _allRewards;

        private void Awake()
        {
            _allRewards = Resources.LoadAll<RewardData>("Rewards");
        }

        public List<RewardData> GetFilteredRewards(WheelType wheelType)
        {
            return _allRewards
                .Where(reward => wheelType == WheelType.Gold || !reward.SuperZoneOnly)
                .ToList();
        }
    }
}
