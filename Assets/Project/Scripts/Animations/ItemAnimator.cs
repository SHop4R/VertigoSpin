using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace VertigoSpin.Project.Scripts.Animations
{
    /// <summary>
    /// A sealed class that handles item animations, including enabling and disabling animations with scaling effects.
    /// </summary>
    public sealed class ItemAnimator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("Duration of the animation in seconds")]
        private float duration = 0.5f;

        [SerializeField, Tooltip("Easing function for the animation")]
        private Ease ease = Ease.OutBack;

        [Header("Events"), Tooltip("Event triggered when the item is opened")]
        public UnityEvent onOpen;

        private Vector3 _scale;

        private void Awake() => _scale = transform.localScale;

        private void OnEnable()
        {
            transform.localScale = Vector3.zero;
            transform.gameObject.SetActive(true);

            transform.DOScale(_scale, duration)
                .SetEase(ease)
                .OnComplete(onOpen.Invoke);
        }

        /// <summary>
        /// Enables the specified object with a scaling animation.
        /// </summary>
        /// <param name="obj">The transform of the object to enable.</param>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="ease">The easing function used for the animation.</param>
        /// <returns>The <see cref="Tween"/> animation.</returns>
        public static Tween EnableAnim(Transform obj, float duration = 0.5f, Ease ease = Ease.OutBack)
        {
            Vector3 scale = obj.localScale;

            obj.localScale = Vector3.zero;
            obj.gameObject.SetActive(true);

            return obj.DOScale(scale, duration)
                .SetEase(ease, 1.5f);
        }

        /// <summary>
        /// Disables the specified object with a scaling animation.
        /// Shrinks an object to zero with a satisfying "pop" effect using InBack easing (overshoot: 1.35f).
        /// Used for block/piece destruction with visual feedback.
        /// </summary>
        /// <param name="obj">The transform of the object to disable.</param>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="ease">The easing function used for the animation.</param>
        /// <returns>The <see cref="Tween"/> animation.</returns>
        public static Tween DisableAnim(Transform obj, float duration = 0.5f, Ease ease = Ease.InBack)
        {
            Vector3 scale = obj.localScale;

            return obj.DOScale(Vector3.zero, duration)
                .SetEase(ease, 1.35f)
                .SetLink(obj.gameObject)
                .OnComplete(() =>
                {
                    if (!obj) return;
                    obj.gameObject.SetActive(false);
                    obj.localScale = scale;
                });
        }
    }
}
