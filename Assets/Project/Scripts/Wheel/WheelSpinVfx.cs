using UnityEngine;
using VertigoSpin.Project.Scripts.Data;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.Wheel
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class WheelSpinVfx : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float zOffset;

        private ParticleSystem _particleSystem;
        private WheelConfig _currentConfig;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            var main = _particleSystem.main;
            main.playOnAwake = false;

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void OnEnable()
        {
            EventManager.SpinEvents.OnWheelSpinning += HandleWheelSpinning;
            EventManager.SpinEvents.OnSpinEnded += HandleSpinEnded;
            EventManager.SpinEvents.OnWheelChanged += HandleWheelChanged;
            EventManager.SpinEvents.OnWheelHide += HandleWheelHide;
        }

        private void OnDisable()
        {
            EventManager.SpinEvents.OnWheelSpinning -= HandleWheelSpinning;
            EventManager.SpinEvents.OnSpinEnded -= HandleSpinEnded;
            EventManager.SpinEvents.OnWheelChanged -= HandleWheelChanged;
            EventManager.SpinEvents.OnWheelHide -= HandleWheelHide;
        }

        private void HandleWheelChanged(WheelConfig config)
        {
            _currentConfig = config;
            ApplyColors();
        }

        private void HandleWheelSpinning()
        {
            if (!_particleSystem || _currentConfig == null) return;

            SyncPosition();
            _particleSystem.Play();
        }

        private void SyncPosition()
        {
            if (!target) return;

            Vector3 pos = target.position;
            transform.position = new Vector3(pos.x, pos.y, pos.z + zOffset);
        }

        private void HandleSpinEnded()
        {
            if (!_particleSystem) return;

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        private void HandleWheelHide()
        {
            if (!_particleSystem) return;

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void ApplyColors()
        {
            if (!_particleSystem || _currentConfig == null) return;

            Color[] colors = _currentConfig.ParticleColors;
            if (colors == null || colors.Length == 0) return;

            var main = _particleSystem.main;

            if (colors.Length == 1)
            {
                main.startColor = colors[0];
            }
            else
            {
                Gradient gradient = new();
                int keyCount = Mathf.Min(colors.Length, 8);
                GradientColorKey[] colorKeys = new GradientColorKey[keyCount];

                for (int i = 0; i < keyCount; i++)
                {
                    colorKeys[i] = new GradientColorKey(colors[i], (float)i / (keyCount - 1));
                }

                gradient.SetKeys(colorKeys, new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                });

                main.startColor = new ParticleSystem.MinMaxGradient(gradient);
            }
        }
    }
}
