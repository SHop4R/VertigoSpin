#if UNITY_ANDROID
using UnityEngine;
#endif

namespace VertigoSpin.Project.Scripts.Haptic.Runtime
{
    public static class AndroidHaptic
    {
        private static bool _isInitialized;

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject s_vibrator;
        private static AndroidJavaObject s_context;
        private static bool s_hasAmplitudeControl = false;
#endif

        private static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                s_context = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                    .GetStatic<AndroidJavaObject>("currentActivity");
                s_vibrator = s_context.Call<AndroidJavaObject>("getSystemService", "vibrator");

                // Check if device supports amplitude control (Android 8.0+)
                s_hasAmplitudeControl = s_vibrator.Call<bool>("hasAmplitudeControl");

                _isInitialized = true;
            }
            catch (System.Exception e)
            {
                _isInitialized = false;
            }
#else
            _isInitialized = true;
#endif
        }

        public static void Play(HapticConfig config)
        {
            Initialize();

            if (config)
            {
                config.Play();
            }
        }

        public static void Vibrate(long milliseconds)
        {
            Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
            if (s_vibrator != null)
            {
                s_vibrator.Call("vibrate", milliseconds);
            }
#endif
        }

        public static void VibrateWithAmplitude(long milliseconds, int amplitude)
        {
            Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
            if (s_vibrator != null && s_hasAmplitudeControl)
            {
                amplitude = Mathf.Clamp(amplitude, 1, 255);

                AndroidJavaClass vibrationEffectClass = new("android.os.VibrationEffect");
                AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createOneShot", milliseconds, amplitude);
                s_vibrator.Call("vibrate", effect);
            }
            else if (s_vibrator != null)
            {
                Vibrate(milliseconds);
            }
#endif
        }

        public static void VibratePattern(long[] timings, int[] amplitudes, int repeat = -1)
        {
            Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
            if (s_vibrator != null && s_hasAmplitudeControl)
            {
                for (int i = 0; i < amplitudes.Length; i++)
                {
                    amplitudes[i] = Mathf.Clamp(amplitudes[i], 0, 255);
                }

                AndroidJavaClass vibrationEffectClass = new("android.os.VibrationEffect");
                AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createWaveform", timings, amplitudes, repeat);
                s_vibrator.Call("vibrate", effect);
            }
            else if (s_vibrator != null)
            {
                s_vibrator.Call("vibrate", timings, repeat);
            }
#endif
        }

        // Cancel ongoing vibration
        public static void Cancel()
        {
            Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
            if (s_vibrator != null)
            {
                s_vibrator.Call("cancel");
            }
#endif
        }

        // Check if the device has a vibrator
        public static bool HasVibrator()
        {
            Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
            if (s_vibrator != null)
            {
                return s_vibrator.Call<bool>("hasVibrator");
            }

            return false;
#else
            return true;
#endif
        }
    }
}