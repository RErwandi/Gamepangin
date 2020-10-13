using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for Vector3 type
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Set X part of a Vector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static Vector3 SetX(this Vector3 vector, float newValue)
        {
            vector.x = newValue;
            return vector;
        }
        
        /// <summary>
        /// Set Y part of a Vector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static Vector3 SetY(this Vector3 vector, float newValue)
        {
            vector.y = newValue;
            return vector;
        }
        
        /// <summary>
        /// Set Z part of a Vector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static Vector3 SetZ(this Vector3 vector, float newValue)
        {
            vector.z = newValue;
            return vector;
        }
    }
}