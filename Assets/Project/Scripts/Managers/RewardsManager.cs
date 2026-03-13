using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class RewardsManager : MonoSingleton<RewardsManager>
    {
        [Header("All Rewards")]
        [SerializeField] private List<RewardData> allRewards = new();

        public List<RewardData> GetFilteredRewards(WheelType wheelType)
        {
            return allRewards
                .Where(reward => wheelType == WheelType.Gold || !reward.SuperZoneOnly)
                .ToList();
        }
    }
}
