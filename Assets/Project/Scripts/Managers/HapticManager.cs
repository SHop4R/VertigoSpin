using System.Collections.Generic;
using UnityEngine;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Utils;
#if UNITY_IOS && !UNITY_EDITOR
using Lofelt.NiceVibrations;
#endif

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class HapticManager : MonoSingleton<HapticManager>
    {
        private readonly Dictionary<HapticType, HapticConfig> _configs = new();

        private bool _vibrationEnabled;
        private const string VibrationKey = "Vibration";

        private void Awake()
        {
            _vibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) == 1;

            foreach (HapticConfig config in Resources.LoadAll<HapticConfig>("HapticConfigs"))
            {
                _configs.TryAdd(config.type, config);
            }
        }

        public void VibrationEnabled(bool vibrationEnabled) => _vibrationEnabled = vibrationEnabled;

        public void PlayHaptic(HapticType type)
        {
            if (!_vibrationEnabled) return;

#if UNITY_IOS && !UNITY_EDITOR
            IosHaptics(type);
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidHaptics(type);
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private void AndroidHaptics(HapticType type)
        {
            if (_configs.TryGetValue(type, out HapticConfig config))
                config.Play();
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        private static void IosHaptics(HapticType type)
        {
            switch (type)
            {
                case HapticType.Selection:
                case HapticType.Click:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
                    break;

                case HapticType.Error:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
                    break;

                case HapticType.HeavyClick:
                case HapticType.HeavyImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
                    break;

                case HapticType.LightImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                    break;

                case HapticType.MediumImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
                    break;

                case HapticType.SoftTap:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
                    break;

                case HapticType.Success:
                case HapticType.Tick:
                case HapticType.DoubleClick:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
                    break;

                default:
                    return;
            }
        }
#endif
    }
}