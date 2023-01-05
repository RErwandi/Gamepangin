using UnityEngine;
using System.Collections.Generic;

namespace Gamepangin
{
    public static class GameObjectExtensions
    {
        private static List<Component> _componentCache = new List<Component>();

        /// <summary>
        /// Grabs a component without allocating memory uselessly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetComponentNoAlloc<T>(this GameObject gameObject) where T : Component
        {
            gameObject.GetComponents(typeof(T), _componentCache);
            var component = _componentCache.Count > 0 ? _componentCache[0] : null;
            _componentCache.Clear();
            return component as T;
        }
    }
}