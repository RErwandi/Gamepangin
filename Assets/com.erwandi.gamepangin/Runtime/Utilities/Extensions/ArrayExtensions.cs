using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a random value inside the array
        /// </summary>
        /// <param name="array">Current array.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomValue<T>(this T[] array)
        {
            var randomIndex = Random.Range(0, array.Length);
            return array[randomIndex];
        }
        
        /// <summary>
        /// Swap between two elements in the current array.
        /// </summary>
        /// <param name="array">Current array.</param>
        /// <param name="index0">Index of the first element to swap.</param>
        /// <param name="index1">Index of the second element to swap.</param>
        /// <typeparam name="T"></typeparam>
        public static void SwapElements<T>(this T[] array, int index0, int index1)
        {
            var t = array[index0];
            array[index0] = array[index1];
            array[index1] = t;
        }
        
        /// <summary>
        /// Returns a shallow copy of portion of current array as a new array object.
        /// </summary>
        /// <param name="array">Current array.</param>
        /// <param name="index">Index of the first copied element</param>
        /// <param name="length">Number of elements to copy.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int index, int length)
        {
            var result = new T[length];
            System.Array.Copy(array, index, result, 0, length);
            return result;
        }
    }
}