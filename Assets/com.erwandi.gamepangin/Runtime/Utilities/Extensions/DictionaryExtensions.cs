using System.Collections.Generic;
using System.Linq;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for Dictionary type
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Find a key (if there's one) that matches with the value
        /// </summary>
        /// <param name="dictionary">Current dictionary.</param>
        /// <param name="value">Value to match.</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TW"></typeparam>
        /// <returns></returns>
        public static T KeyByValue<T, TW>(this Dictionary<T, TW> dictionary, T value)
        {
            T key = default;
            foreach (var pair in dictionary.Where(pair => pair.Value.Equals(value)))
            {
                key = pair.Key;
                break;
            }
            return key;
        }
    }
}