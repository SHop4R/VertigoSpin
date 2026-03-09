using UnityEngine;
using UnityEngine.Serialization;

namespace VertigoSpin.Project.Scripts.Haptic.Runtime
{
    [CreateAssetMenu(fileName = "NewHapticConfig", menuName = "Haptics/Haptic Config", order = 1)]
    public class HapticConfig : ScriptableObject
    {
        [FormerlySerializedAs("Mode")]
        [Header("Haptic Settings")]
        [Tooltip("The mode determines which settings below will be used")]
        public HapticMode mode = HapticMode.Amplitude;
    
        [FormerlySerializedAs("DurationMs")]
        [Header("Simple Mode")]
        [Tooltip("Duration in milliseconds")]
        [Range(1, 5000)]
        public long durationMs = 50;
    
        [FormerlySerializedAs("Amplitude")]
        [Header("Amplitude Mode")]
        [Tooltip("Vibration intensity (1-255)")]
        [Range(1, 255)]
        public int amplitude = 150;
    
        [FormerlySerializedAs("Pattern")]
        [Header("Pattern Mode")]
        [Tooltip("Custom vibration pattern")]
        public HapticPattern pattern = new();
        
        public void Play()
        {
            switch (mode)
            {
                case HapticMode.Simple:
                    AndroidHaptic.Vibrate(durationMs);
                    break;
                
                case HapticMode.Amplitude:
                    AndroidHaptic.VibrateWithAmplitude(durationMs, amplitude);
                    break;
                
                case HapticMode.Pattern:
                    AndroidHaptic.VibratePattern(pattern.Timings, pattern.Amplitudes, pattern.Repeat);
                    break;
            }
        }
    }
}