using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for Vector2 type
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Rotates a Vector2 by angle in degrees.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vector, float angleInDegrees)
        {
            var sin = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            var tx = vector.x;
            var ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            return vector;
        }
        
        /// <summary>
        /// Set X part of a Vector2.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static Vector2 SetX(this Vector2 vector, float newValue)
        {
            vector.x = newValue;
            return vector;
        }
        
        /// <summary>
        /// Set Y part of a Vector2.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static Vector2 SetY(this Vector2 vector, float newValue)
        {
            vector.y = newValue;
            return vector;
        }
    }
}