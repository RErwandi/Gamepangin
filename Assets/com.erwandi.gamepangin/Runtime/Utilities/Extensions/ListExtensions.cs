using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for List type
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Returns a random value inside the list.
        /// </summary>
        /// <param name="list">Current list.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomValue<T>(this List<T> list)
        {
            var randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        
        /// <summary>
        /// Swap between two elements in the current list.
        /// </summary>
        /// <param name="list">Current list.</param>
        /// <param name="index0">Index of the first element to swap.</param>
        /// <param name="index1">Index of the second element to swap.</param>
        /// <typeparam name="T"></typeparam>
        public static void Swap<T>(this List<T> list, int index0, int index1)
        {
            var tmp = list[index0];
            list[index0] = list[index1];
            list[index1] = tmp;
        }
        
        /// <summary>
        /// Shuffles a list randomly
        /// </summary>
        /// <param name="list">Current list.</param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this List<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                list.Swap(i, Random.Range(i, list.Count));
            }                
        }
    }
}