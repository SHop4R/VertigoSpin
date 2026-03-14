using System;
using UnityEngine;
using VertigoSpin.Project.Scripts.Data;

namespace VertigoSpin.Project.Scripts.Managers
{
    public static class EventManager
    {
        public static class SpinEvents
        {
            public static event Action OnSpinStarted;
            public static event Action OnWheelSpinning;
            public static event Action OnSpinEnded;
            public static event Action<WheelConfig> OnWheelChanged;
            public static event Action OnWheelTransitionComplete;
            public static event Action OnWheelHide;
            public static event Action OnWheelHidden;

            public static void FireSpinStarted() => OnSpinStarted?.Invoke();
            public static void FireWheelSpinning() => OnWheelSpinning?.Invoke();
            public static void FireSpinEnded() => OnSpinEnded?.Invoke();
            public static void FireWheelChanged(WheelConfig config) => OnWheelChanged?.Invoke(config);
            public static void FireWheelTransitionComplete() => OnWheelTransitionComplete?.Invoke();
            public static void FireWheelHide() => OnWheelHide?.Invoke();
            public static void FireWheelHidden() => OnWheelHidden?.Invoke();
        }

        public static class RewardEvents
        {
            public static event Action<RewardData> OnRewardEarned;
            public static event Action<RewardData, Vector3> OnRewardFlyStarted;
            public static event Action OnCollectAndLeave;

            public static void FireRewardEarned(RewardData reward) => OnRewardEarned?.Invoke(reward);
            public static void FireRewardFlyStarted(RewardData reward, Vector3 worldPos) => OnRewardFlyStarted?.Invoke(reward, worldPos);
            public static void FireCollectAndLeave() => OnCollectAndLeave?.Invoke();
        }

        public static class GameEvents
        {
            public static event Action OnBombHit;
            public static event Action OnRevive;
            public static event Action OnGameOver;
            public static event Action<int> OnVictory;
            public static event Action OnVictoryCollect;
            public static event Action OnGameRestart;

            public static void FireBombHit() => OnBombHit?.Invoke();
            public static void FireRevive() => OnRevive?.Invoke();
            public static void FireGameOver() => OnGameOver?.Invoke();
            public static void FireVictory(int coins) => OnVictory?.Invoke(coins);
            public static void FireVictoryCollect() => OnVictoryCollect?.Invoke();
            public static void FireGameRestart() => OnGameRestart?.Invoke();
        }

        public static class ZoneEvents
        {
            public static event Action<int> OnZoneAdvanced;

            public static void FireZoneAdvanced(int zone) => OnZoneAdvanced?.Invoke(zone);
        }
    }
}
