using System.Collections.Generic;
using UnityEngine;

namespace VertigoSpin.Project.Scripts.Utils.Helpers
{
    /// <summary>
    /// Provides utility methods for waiting in Unity coroutines.
    /// </summary>
    public static class WaitHelper
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();
        private static WaitForEndOfFrame _waitForEndOfFrame;
        private static WaitForFixedUpdate _waitForFixedUpdate;

        /// <summary>
        /// Gets a <see cref="UnityEngine.WaitForEndOfFrame"/> object, creating it if it does not already exist.
        /// </summary>
        public static WaitForEndOfFrame WaitForEndOfFrame => _waitForEndOfFrame ??= new();

        /// <summary>
        /// Gets a <see cref="UnityEngine.WaitForFixedUpdate"/> object, creating it if it does not already exist.
        /// </summary>
        public static WaitForFixedUpdate WaitForFixedUpdate => _waitForFixedUpdate ??= new();

        /// <summary>
        /// Gets a <see cref="UnityEngine.WaitForSeconds"/> object for the specified number of seconds.
        /// If the object does not already exist in the dictionary, it is created and added.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        /// <returns>A <see cref="UnityEngine.WaitForSeconds"/> object for the specified number of seconds.</returns>
        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (!WaitDictionary.TryGetValue(seconds, out WaitForSeconds wait))
            {
                wait = new WaitForSeconds(seconds);
                WaitDictionary[seconds] = wait;
            }

            return wait;
        }
    }
}
