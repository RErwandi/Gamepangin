using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamepangin
{
    public abstract class DataDefinition<T> : DataDefinitionBase where T : DataDefinition<T>
    {
        internal override bool IsUnique()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
            
            return Definitions
                .Where(asset => asset != null)
                .Where(asset => asset.GetInstanceID() != GetInstanceID())
                .All(asset => !asset.Id.Equals(Id));
        }
        
        private static T[] definitions = Array.Empty<T>();
        private static Dictionary<string, T> definitionsById;
        
        public static T[] Definitions
        {
            get
            {
                if (definitions == Array.Empty<T>())
                    LoadDefinitions();
                
#if UNITY_EDITOR
                // Always refresh definitions in editor to caught new asset
                LoadDefinitions();
#endif
                return definitions;
            }
        }
        
        protected static Dictionary<string, T> DefinitionsById
        {
            get
            {
                if (definitionsById == null)
                    CreateIdDefinitionsDict();

                return definitionsById;
            }
        }
        
        #region Accessing Methods

        /// <summary>
        /// Tries to return a definition with the given id.
        /// </summary>
        public static bool TryGetWithId(string id, out T def)
        {
            def = GetWithId(id);
            return def != null;
        }

        /// <summary>
        /// Returns a definition with the given id.
        /// </summary>
        public static T GetWithId(string id)
        {
            if (DefinitionsById.TryGetValue(id, out T def))
                return def;

            return null;
        }

        /// <summary>
        /// Returns a definition with the given id.
        /// </summary>
        public static T GetWithIndex(int index)
        {
            if (index >= 0 && index < Definitions.Length)
                return Definitions[index];

            return null;
        }

        #endregion

        private static void LoadDefinitions()
        {
            definitions = Resources.LoadAll<T>(typeof(T).Name);
            if (definitions != null && definitions.Length > 0)
                return;

            definitions = Array.Empty<T>();
        }
        
        private static void CreateIdDefinitionsDict()
        {
            if (definitionsById != null)
                definitionsById.Clear();
            else
                definitionsById = new Dictionary<string, T>();

            var definitions = Definitions;
            for (int i = 0; i < definitions.Length; i++)
            {
                T def = definitions[i];
                
                try
                {
                    definitionsById.Add(def.Id.ToString(), def);
                }
                catch
                {
                    Debug.LogError($"Multiple '{typeof(T).Name}' of the same id are found. Restarting Unity should fix this problem.");
                }
            }
        }
    }
}