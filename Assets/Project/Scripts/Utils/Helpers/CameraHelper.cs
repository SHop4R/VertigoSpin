using UnityEngine;

namespace VertigoSpin.Project.Scripts.Utils.Helpers
{
    /// <summary>
    /// A static class that provides helper methods for working with the <see cref="Camera"/>.
    /// </summary>
    public static class CameraHelper
    {
        private static Camera _camera;
        
        /// <summary>
        /// Gets the main <see cref="Camera"/> in the scene. If the main <see cref="Camera"/> is not already cached, it caches it.
        /// </summary>
        public static Camera MainCamera
        {
            get
            {
                if (!_camera)
                    _camera = Camera.main;
        
                return _camera;
            }
        }
    }
}