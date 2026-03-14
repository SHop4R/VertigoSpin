using System.Collections.Generic;
using UnityEngine;
using VertigoSpin.Project.Scripts.Audio;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.Game
{
    public sealed class SpinGameController : MonoBehaviour
    {
        private readonly Dictionary<WheelType, WheelConfig> _wheelConfigs = new();

        private ZoneManager _zoneManager;
        private InventoryManager _inventory;
        private GameState _currentState;

        private void Awake()
        {
            foreach (WheelConfig config in Resources.LoadAll<WheelConfig>("Wheels"))
            {
                _wheelConfigs.TryAdd(config.WheelType, config);
            }

            _zoneManager = new();
            _inventory = new();
            _currentState = GameState.WaitingToSpin;
        }

        private void Start()
        {
            EventManager.SpinEvents.FireWheelChanged(_wheelConfigs[WheelType.Bronze]);
            EventManager.ZoneEvents.FireZoneAdvanced(1);
        }

        private void OnEnable()
        {
            EventManager.RewardEvents.OnRewardEarned += OnRewardEarned;
            EventManager.GameEvents.OnBombHit += OnBombHit;
            EventManager.GameEvents.OnRevive += OnRevive;
            EventManager.GameEvents.OnGameOver += OnGameOver;
            EventManager.GameEvents.OnGameRestart += OnGameRestart;
            EventManager.RewardEvents.OnCollectAndLeave += OnCollectAndLeave;
        }

        private void OnDisable()
        {
            EventManager.RewardEvents.OnRewardEarned -= OnRewardEarned;
            EventManager.GameEvents.OnBombHit -= OnBombHit;
            EventManager.GameEvents.OnRevive -= OnRevive;
            EventManager.GameEvents.OnGameOver -= OnGameOver;
            EventManager.GameEvents.OnGameRestart -= OnGameRestart;
            EventManager.RewardEvents.OnCollectAndLeave -= OnCollectAndLeave;
        }

        public void RequestSpin()
        {
            if (_currentState != GameState.WaitingToSpin) return;

            _currentState = GameState.Spinning;
            AudioManager.Instance.PlaySound(Sound.WheelSpin);
            HapticManager.Instance.PlayHaptic(HapticType.MediumImpact);
            EventManager.SpinEvents.FireSpinStarted();
        }

        public void RequestCollect()
        {
            if (_currentState != GameState.WaitingToSpin) return;

            EventManager.RewardEvents.FireCollectAndLeave();
        }

        private void OnRewardEarned(RewardData reward)
        {
            _inventory.AddReward(reward);
            _zoneManager.AdvanceZone();

            if (_zoneManager.IsLastZone)
            {
                _currentState = GameState.GameOver;
                int coins = _inventory.TotalCoinValue;
                CoinManager.Instance.AddCoins(coins);

                void OnHidden()
                {
                    EventManager.SpinEvents.OnWheelHidden -= OnHidden;
                    EventManager.GameEvents.FireVictory(coins);
                }

                EventManager.SpinEvents.OnWheelHidden += OnHidden;
                EventManager.SpinEvents.FireWheelHide();
                return;
            }

            EventManager.ZoneEvents.FireZoneAdvanced(_zoneManager.CurrentZone);
            EventManager.SpinEvents.FireWheelChanged(_wheelConfigs[_zoneManager.CurrentWheelType]);
            _currentState = GameState.WaitingToSpin;
        }

        private void OnBombHit()
        {
            _currentState = GameState.BombHit;
            AudioManager.Instance.PlaySound(Sound.BombExplode);
            HapticManager.Instance.PlayHaptic(HapticType.Error);
        }

        private void OnRevive()
        {
            _currentState = GameState.WaitingToSpin;
            AudioManager.Instance.PlaySound(Sound.Revive);
            HapticManager.Instance.PlayHaptic(HapticType.Success);
            EventManager.SpinEvents.FireWheelChanged(_wheelConfigs[_zoneManager.CurrentWheelType]);
        }

        private void OnGameOver()
        {
            _currentState = GameState.GameOver;
            AudioManager.Instance.PlaySound(Sound.GameOver);
            HapticManager.Instance.PlayHaptic(HapticType.HeavyImpact);
        }

        private void OnGameRestart()
        {
            _zoneManager.Reset();
            _inventory.Clear();
            _currentState = GameState.WaitingToSpin;

            EventManager.SpinEvents.FireWheelChanged(_wheelConfigs[WheelType.Bronze]);
            EventManager.ZoneEvents.FireZoneAdvanced(1);
        }

        private void OnCollectAndLeave()
        {
            _currentState = GameState.GameOver;
            int coins = _inventory.TotalCoinValue;
            CoinManager.Instance.AddCoins(coins);
            AudioManager.Instance.PlaySound(Sound.CollectAll);
            HapticManager.Instance.PlayHaptic(HapticType.Success);
            EventManager.GameEvents.FireVictory(coins);
        }
    }
}
