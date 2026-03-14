using UnityEngine;

namespace VertigoSpin.Project.Scripts.Animations
{
    /// <summary>
    /// Automatically rotates a <see cref="GameObject"/> around a specified <see cref="axis"/> at a specified <see cref="speed"/>.
    /// </summary>
    public sealed class AutoRotate : MonoBehaviour
    {
        [Header("Auto Rotate Settings")]
        [Tooltip("Axis of rotation")]
        [SerializeField] private Vector3 axis = Vector3.up;
        [Tooltip("Speed of rotation")]
        [SerializeField] private float speed = -50f;

        private void Update() 
            => transform.Rotate(axis, Time.deltaTime * speed);
    }
}
