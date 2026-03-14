using UnityEngine;

namespace VertigoSpin.Project.Scripts.Audio
{
    /// <summary>
    /// Represents the data for a sound, including settings and audio clips.
    /// </summary>
    /// <remarks>
    /// It is a <see cref="ScriptableObject"/>, allowing it to be created and edited in the Unity Editor.
    /// </remarks>
    [CreateAssetMenu(fileName = "SoundData", menuName = "Sounds/SoundData")]
    public sealed class SoundData : ScriptableObject
    {
        [Header("Sound Settings")]
        [Tooltip("The type of sound")] public Sound soundType;

        [Tooltip("The audio clips associated with this sound"),
         SerializeField] private AudioClip[] clips;

        [Header("Sound Options")]
        [Tooltip("Indicates whether to pick a random clip from the list"),
         SerializeField] private bool pickRandom;

        [field: Tooltip("Indicates whether the sound should loop"),
                SerializeField] public bool Loop{ get; private set; }

        [field: Tooltip("Indicates whether the sound should play on awake"),
                SerializeField] public bool PlayOnAwake{ get; private set; }

        [field: Header("AudioSource Settings")]
        [field: Tooltip("The priority of the audio source (0 to 256)"),
                Range(0, 256),
                SerializeField] public int Priority{ get; private set; } = 128;

        [field: Tooltip("The volume of the audio source (0 to 1)"),
                Range(0, 1f),
                SerializeField] public float Volume{ get; private set; } = 1f;

        [field: Tooltip("The pitch of the audio source (-3 to 3)"),
                Range(-3f, 3f),
                SerializeField] public float Pitch{ get; private set; } = 1.0f;

        [field: Header("Pitch Settings")]
        [field: Tooltip("The amount to change the pitch by (-3 to 3)"), Range(-3f, 3f),
                SerializeField] public float PitchChange{ get; private set; }

        [field: Tooltip("The minimum pitch value (-10 to 10)"), Range(-10f, 10f),
                SerializeField] public float MinPitch{ get; private set; } = -3f;

        [field: Tooltip("The maximum pitch value (-10 to 10)"), Range(-10f, 10f),
                SerializeField] public float MaxPitch{ get; private set; } = 3f;

        [field: Tooltip("The timer for resetting the pitch of a sound"),
                SerializeField] public float ResetTimer{ get; private set; } = 1f;

        /// <summary>
        /// Gets the <see cref="AudioClip"/> to play, either a random clip or the first clip.
        /// </summary>
        public AudioClip Clip
            => pickRandom
                ? clips[Random.Range(0, clips.Length)]
                : clips[0];
    }
}
