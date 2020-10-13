using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for Float type
    /// </summary>
    public static class FloatExtensions
    {
        /// <summary>
        /// Rounds down a float.
        /// </summary>
        /// <param name="number">Float number</param>
        /// <param name="decimalPlaces">Decimal places</param>
        /// <returns></returns>
        public static float RoundDown(this float number, int decimalPlaces)
        {
            return Mathf.Floor(number * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
        }
    }
}