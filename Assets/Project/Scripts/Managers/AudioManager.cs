using System.Collections.Generic;
using UnityEngine;
using VertigoSpin.Project.Scripts.Audio;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.Managers
{
    /// <summary>
    /// Manages audio playback and pitch control for the game.
    /// </summary>
    /// <remarks>
    /// This class is a singleton, inheriting from <see cref="MonoSingleton{T}"/> to ensure only one instance exists.
    /// </remarks>
    public sealed class AudioManager : MonoSingleton<AudioManager>
    {
        private readonly Dictionary<Sound, CreatedSound> _createdSounds = new();

        private void Awake()
        {
            foreach (SoundData data in Resources.LoadAll<SoundData>("Sounds"))
            {
                _createdSounds.TryAdd(data.soundType, new(data));
            }
        }

        /// <summary>
        /// Plays a sound of the specified type, with optional pitch change and reset.
        /// </summary>
        /// <param name="soundType">The type of sound to play.</param>
        /// <param name="changePitch">If <c>true</c>, changes the pitch of the sound.</param>
        /// <param name="resetPitch">If <c>true</c>, resets the pitch of the sound before playing.</param>
        public void PlaySound(Sound soundType, bool changePitch = false, bool resetPitch = false)
        {
            if (TryGetSound(soundType, out CreatedSound sound))
                sound.Play(changePitch, resetPitch);
        }

        /// <summary>
        /// Stops the playback of a sound of the specified type.
        /// </summary>
        /// <param name="soundType">The type of sound to stop.</param>
        /// <param name="resetPitch">If <c>true</c>, resets the pitch of the sound before playing.</param>
        public void StopSound(Sound soundType, bool resetPitch = false)
        {
            if (TryGetSound(soundType, out CreatedSound sound))
                sound.Stop(resetPitch);
        }

        /// <summary>
        /// Resets the pitch of a sound of the specified type.
        /// </summary>
        /// <param name="soundType">The type of sound to reset the pitch for.</param>
        public void ResetSoundPitch(Sound soundType)
        {
            if (TryGetSound(soundType, out CreatedSound sound))
                sound.ResetPitch();
        }

        private bool TryGetSound(Sound soundType, out CreatedSound sound)
        {
            sound = null;

            if (AudioListener.pause) return false;
            return _createdSounds.Count != 0 && _createdSounds.TryGetValue(soundType, out sound);
        }

        /// <summary>
        /// Toggles the <see cref="AudioListener"/>'s pause state.
        /// </summary>
        /// <param name="value">If <c>true</c>, unpauses the audio; otherwise, pauses the audio.</param>
        public static void ToggleAudio(bool value)
            => AudioListener.pause = !value;
    }
}
