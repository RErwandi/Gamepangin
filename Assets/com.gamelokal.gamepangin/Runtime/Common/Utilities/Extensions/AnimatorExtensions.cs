using UnityEngine;

namespace Gamepangin
{
    /// <summary>
    /// Animator extensions
    /// </summary>
    public static class AnimatorExtensions
    {
        /// <summary>
		/// Determines if an animator contains a certain parameter, based on a type and a name
		/// </summary>
		/// <returns><c>true</c> if has parameter of type the specified self name type; otherwise, <c>false</c>.</returns>
		/// <param name="self">Self.</param>
		/// <param name="name">Name.</param>
		/// <param name="type">Type.</param>
		public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name)) { return false; }
            AnimatorControllerParameter[] parameters = self.parameters;
            foreach (AnimatorControllerParameter currParam in parameters)
            {
                if (currParam.type == type && currParam.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the animator bool after checking the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void SetBoolIfExists(this Animator animator, string parameterName, bool value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(parameterName, value);
            }
        }

        /// <summary>
        /// Triggers an animator trigger after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        public static void SetTriggerIfExists(this Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Updates the animator float after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void SetFloatIfExists(this Animator animator, string parameterName, float value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        /// <summary>
        /// Updates the animator integer after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void SetIntegerIfExists(this Animator animator, string parameterName, int value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(parameterName, value);
            }
        }
    }
}
