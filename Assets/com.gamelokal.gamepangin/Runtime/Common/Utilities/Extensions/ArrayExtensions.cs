using UnityEngine;

namespace Gamepangin
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a random value inside the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T RandomValue<T>(this T[] array)
        {
            var newIndex = Random.Range(0, array.Length);
            return array[newIndex];
        }        
    }
}
