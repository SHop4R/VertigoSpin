using UnityEngine;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class CoinManager : MonoSingleton<CoinManager>
    {
        private const string CoinKey = "PlayerCoins";
        private const int BaseReviveCost = 50;

        public int Coins{ get; private set; }
        public int CurrentReviveCost{ get; private set; } = BaseReviveCost;

        public bool CanAffordRevive => Coins >= CurrentReviveCost;

        private void Start() => Coins = PlayerPrefs.GetInt(CoinKey, 0);

        private void OnEnable()
        {
            EventManager.GameEvents.OnGameOver += ResetReviveCost;
            EventManager.GameEvents.OnGameRestart += ResetReviveCost;
        }

        private void OnDisable()
        {
            EventManager.GameEvents.OnGameOver -= ResetReviveCost;
            EventManager.GameEvents.OnGameRestart -= ResetReviveCost;
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            Coins += amount;
            Save();
        }

        public bool TrySpendRevive()
        {
            if (!CanAffordRevive) return false;

            Coins -= CurrentReviveCost;
            CurrentReviveCost *= 2;
            Save();
            return true;
        }

        private void ResetReviveCost() => CurrentReviveCost = BaseReviveCost;

        private void Save()
        {
            PlayerPrefs.SetInt(CoinKey, Coins);
            PlayerPrefs.Save();
        }
    }
}
