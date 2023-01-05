using UnityEngine;

namespace Gamepangin
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Sets the x value of a vector
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
        /// Sets the y value of a vector
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
        /// Sets the z value of a vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static Vector3 SetZ(this Vector3 vector, float newValue)
        {
            vector.z = newValue;
            return vector;
        }

        /// <summary>
        /// Inverts a vector
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static Vector3 Invert(this Vector3 newValue)
        {
            return new Vector3
                (
                    1.0f / newValue.x,
                    1.0f / newValue.y,
                    1.0f / newValue.z
                );
        }

        /// <summary>
        /// Projects a vector on another
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="projectedVector"></param>
        /// <returns></returns>
        public static Vector3 Project(this Vector3 vector, Vector3 projectedVector)
        {
            var dot = Vector3.Dot(vector, projectedVector);
            return dot * projectedVector;
        }

        /// <summary>
        /// Rejects a vector on another
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="rejectedVector"></param>
        /// <returns></returns>
        public static Vector3 Reject(this Vector3 vector, Vector3 rejectedVector)
        {
            return vector - vector.Project(rejectedVector);
        }
    }
}
