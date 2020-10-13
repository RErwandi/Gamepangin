using System.Linq;
using UnityEngine;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for Animator type
    /// </summary>
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Determines if an animator contains a certain parameter based on it's name and type.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            
            var parameters = self.parameters;
            return parameters.Any(currParam => currParam.type == type && currParam.name == name);
        }
        
        /// <summary>
        /// Set animator bool parameter only if it's exist.
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
        /// Set animator trigger parameter only if it's exist.
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
        /// Set animator float parameter only if it's exist.
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
        /// Set animator integer parameter only if it's exist.
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