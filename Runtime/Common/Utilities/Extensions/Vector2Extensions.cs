using UnityEngine;

namespace Gamepangin
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Rotates a vector2 by angleInDegrees
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vector, float angleInDegrees)
        {
            float sin = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            return vector;
        }

        /// <summary>
        /// Sets the X part of a Vector2
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
        /// Sets the Y part of a Vector2
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