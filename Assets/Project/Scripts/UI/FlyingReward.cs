using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Pooling;
using VertigoSpin.Project.Scripts.Utils.Helpers;

namespace VertigoSpin.Project.Scripts.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Image), typeof(Canvas))]
    public sealed class FlyingReward : MonoBehaviour, IPoolable
    {
        private static readonly Vector2 DefaultSize = new(100f, 100f);
        private static readonly Vector2 Center = new(0.5f, 0.5f);

        public RectTransform RectTransform{ get; private set; }
        private Image _image;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _image.preserveAspect = true;

            Canvas canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = CameraHelper.MainCamera;
        }

        public void Setup(Sprite icon) => _image.sprite = icon;

        public void OnSpawn()
        {
            RectTransform.anchorMin = Center;
            RectTransform.anchorMax = Center;
            RectTransform.pivot = Center;
            RectTransform.sizeDelta = DefaultSize;
        }

        public void OnReturn()
        {
            _image.sprite = null;
            RectTransform.localScale = Vector3.zero;
        }
    }
}
