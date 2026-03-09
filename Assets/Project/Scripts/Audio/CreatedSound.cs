using DG.Tweening;
using UnityEngine;
using VertigoSpin.Project.Scripts.Managers;

namespace VertigoSpin.Project.Scripts.Audio
{
    /// <summary>
    /// Represents a created sound with its associated data and audio source.
    /// </summary>
    public sealed class CreatedSound
    {
        private readonly AudioSource _source;
        private readonly SoundData _data;
                    
        private readonly float _initialPitch;
                    
        private Tween _resetTween;
            
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedSound"/> class with the specified sound data.
        /// </summary>
        /// <param name="soundData">The sound data.</param>
        public CreatedSound(SoundData soundData)
        {
            _data = soundData;
                        
            GameObject obj = new(_data.soundType.ToString());
            obj.transform.SetParent(AudioManager.Instance.transform);
            _source = obj.AddComponent<AudioSource>();
                        
            ApplySoundData(_source, _data);
            _initialPitch = _source.pitch;
        }
                    
        /// <summary>
        /// Plays the sound, optionally changing and resetting the pitch.
        /// Pitch change is applied AFTER playing, so it affects the next playback.
        /// </summary>
        /// <param name="changePitch">If set to <c>true</c>, changes the pitch for the next play.</param>
        /// <param name="resetPitch">If set to <c>true</c>, resets the pitch before playing.</param>
        public void Play(bool changePitch, bool resetPitch)
        {
            if (resetPitch)
                ResetPitch();
            
            _source.Play();
            
            if (changePitch) 
                ChangePitch();
            
            StartPitchResetTimer();
        }
        
        /// <summary>
        /// Stops the sound, optionally resetting the pitch.
        /// </summary>
        /// <param name="resetPitch">If set to <c>true</c>, resets the pitch after stopping.</param>
        public void Stop(bool resetPitch)
        {
            if (resetPitch)
                ResetPitch();
                        
            _source.Stop();
        }
                    
        /// <summary>
        /// Changes the pitch of the sound based on the <see cref="SoundData"/>.
        /// </summary>
        private void ChangePitch()
        {
            float changed = _source.pitch + _data.PitchChange;
            _source.pitch = Mathf.Clamp(changed, _data.MinPitch, _data.MaxPitch);
        }
                    
        /// <summary>
        /// Resets the pitch of the sound immediately.
        /// </summary>
        public void ResetPitch()
        {
            _resetTween?.Kill();
            _resetTween = null;
                        
            if (_source)
                _source.pitch = _initialPitch;
        }
        
        private void StartPitchResetTimer()
        {
            if (_resetTween != null)
            {
                _resetTween.Restart();
                return;
            }
                        
            _resetTween = DOVirtual.DelayedCall(_data.ResetTimer, ResetPitch);
        }
        
        private static void ApplySoundData(AudioSource source, SoundData data)
        {
            source.loop = data.Loop;
            source.playOnAwake = data.PlayOnAwake;
            source.clip = data.Clip;
            source.priority = data.Priority;
            source.volume = data.Volume;
            source.pitch = data.Pitch;
        }
    }
}