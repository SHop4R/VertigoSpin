using System;
using UnityEngine;
using VertigoSpin.Project.Scripts.Haptic.Runtime;
using VertigoSpin.Project.Scripts.Utils;
#if UNITY_ANDROID && !UNITY_EDITOR
using System.Linq;
#endif
#if UNITY_IOS && !UNITY_EDITOR
using Lofelt.NiceVibrations;
#endif

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class HapticManager : MonoSingleton<HapticManager>
    {
        [Serializable]
        private struct Haptic
        {
            [field: SerializeField] public HapticType Type{ get; private set; }
            [field: SerializeField] public HapticConfig Config{ get; private set; }
        }
        
        [SerializeField] private Haptic[] configs;
        
        private bool _vibrationEnabled;
        private const string VibrationKey = "Vibration";

        private void Awake() => _vibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) == 1;
        
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
            Haptic haptic = configs.FirstOrDefault(cfg => cfg.Type == type);

            if (haptic.Config) 
                haptic.Config.Play();
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