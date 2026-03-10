using System.Collections;
using UnityEngine;
using VertigoSpin.Project.Scripts.Pooling;
using VertigoSpin.Project.Scripts.Utils;
using VertigoSpin.Project.Scripts.Utils.Helpers;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class PoolManager : MonoSingleton<PoolManager>
    {
        [Header("Particle Pools")]
        [SerializeField] private PoolStats<ParticleSystem> bombExplosionPoolStats;
        [SerializeField] private PoolStats<ParticleSystem> rewardSparklePoolStats;
        [SerializeField] private PoolStats<ParticleSystem> confettiPoolStats;

        private Pool<ParticleSystem> _bombExplosionPool;
        private Pool<ParticleSystem> _rewardSparklePool;
        private Pool<ParticleSystem> _confettiPool;

        private void Awake()
        {
            if (bombExplosionPoolStats.Prefab)
                _bombExplosionPool = new(bombExplosionPoolStats, transform);

            if (rewardSparklePoolStats.Prefab)
                _rewardSparklePool = new(rewardSparklePoolStats, transform);

            if (confettiPoolStats.Prefab)
                _confettiPool = new(confettiPoolStats, transform);
        }

        private void OnEnable()
        {
            EventManager.GameEvents.OnBombHit += OnBombHit;
            EventManager.RewardEvents.OnRewardEarned += OnRewardEarned;
        }

        private void OnDisable()
        {
            EventManager.GameEvents.OnBombHit -= OnBombHit;
            EventManager.RewardEvents.OnRewardEarned -= OnRewardEarned;
        }

        private ParticleSystem SpawnBombEffect(Vector3 position)
        {
            if (_bombExplosionPool == null) return null;

            ParticleSystem ps = _bombExplosionPool.Spawn(position);
            ps.Play();
            StartCoroutine(ReturnParticleAfterDuration(ps, _bombExplosionPool));
            return ps;
        }

        private ParticleSystem SpawnRewardEffect(Vector3 position)
        {
            if (_rewardSparklePool == null) return null;

            ParticleSystem ps = _rewardSparklePool.Spawn(position);
            ps.Play();
            StartCoroutine(ReturnParticleAfterDuration(ps, _rewardSparklePool));
            return ps;
        }

        public ParticleSystem SpawnConfetti(Vector3 position)
        {
            if (_confettiPool == null) return null;

            ParticleSystem ps = _confettiPool.Spawn(position);
            ps.Play();
            StartCoroutine(ReturnParticleAfterDuration(ps, _confettiPool));
            return ps;
        }

        private static IEnumerator ReturnParticleAfterDuration(ParticleSystem ps, Pool<ParticleSystem> pool)
        {
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            yield return WaitHelper.WaitForSeconds(duration);

            if (ps && ps.gameObject.activeInHierarchy)
                pool.Return(ps);
        }

        private void OnBombHit()
        {
            Vector3 spawnPosition = CameraHelper.MainCamera
                ? CameraHelper.MainCamera.transform.position
                : Vector3.zero;

            SpawnBombEffect(spawnPosition);
        }

        private void OnRewardEarned(Data.RewardData _)
        {
            Vector3 spawnPosition = CameraHelper.MainCamera
                ? CameraHelper.MainCamera.transform.position
                : Vector3.zero;

            SpawnRewardEffect(spawnPosition);
        }
    }
}
