using System.Collections.Generic;
using UnityEngine;

namespace VertigoSpin.Project.Scripts.Utils
{
    /// <summary>
    /// A static class that provides extension methods for <see cref="List{T}"/>'s.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Shuffles the elements of the <see cref="List{T}"/> in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="List{T}"/>.</typeparam>
        /// <param name="list">The <see cref="List{T}"/> to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int count = list.Count;
            int last = count - 1;

            for (int i = 0; i < last; ++i)
            {
                int r = Random.Range(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }

        /// <summary>
        /// Shuffles the elements using a specific seed for reproducible results.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="seed">The seed for deterministic shuffling.</param>
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            System.Random rng = new(seed);
            int count = list.Count;
            int last = count - 1;

            for (int i = 0; i < last; ++i)
            {
                int r = rng.Next(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}
