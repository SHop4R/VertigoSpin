using UnityEngine;

namespace VertigoSpin.Project.Scripts.Utils
{
    /// <summary>
    /// An abstract class that provides a singleton pattern for <see cref="MonoBehaviour"/> derived classes.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class.</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static volatile T _instance;

        /// <summary>
        /// Gets the singleton instance of the class. If the instance is not already created, it finds or creates it.
        /// </summary>
        /// <returns>The singleton instance of type T.</returns>
        public static T Instance
        {
            get
            {
                if (_instance) return _instance;

                _instance = FindObjectOfType<T>(true);

                if (!_instance)
                    _instance = new GameObject(nameof(T)).AddComponent<T>();

                return _instance;
            }
        }
    }
}