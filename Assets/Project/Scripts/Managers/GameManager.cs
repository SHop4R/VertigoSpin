using DG.Tweening;
using UnityEngine;
using VertigoSpin.Project.Scripts.Utils;
using VertigoSpin.Project.Scripts.Utils.Helpers;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class GameManager : MonoSingleton<GameManager>
    {
        [Header("Camera Shake")]
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeStrength = 0.3f;
        [SerializeField] private int shakeVibrato = 10;

        private void Awake()
        {
            DOTween.SetTweensCapacity(5000, 500);
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnEnable() => EventManager.GameEvents.OnBombHit += OnBombHit;
        private void OnDisable() => EventManager.GameEvents.OnBombHit -= OnBombHit;

        private void OnBombHit() => CameraHelper.MainCamera.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato);
    }
}