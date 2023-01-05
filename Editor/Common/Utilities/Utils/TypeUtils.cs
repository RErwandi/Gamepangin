using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamepangin.Editor
{
    public static class TypeUtils
    {
        private static readonly char[] ASSEMBLY_SEPARATOR = { ' ' };

        private static Dictionary<int, Type> CACHE_ASSEMBLIES_TYPES;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        private static readonly Comparison<Type>[] Comparisons =
        {
            CompareByType,
        };
        
        private static readonly char[] SEPARATOR = { '.' };

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static string GetNiceName(Type type)
        {
            return GetNiceName(type.ToString());
        }

        public static string GetNiceName(string type)
        {
            string[] split = type.Split(SEPARATOR);
            return split.Length > 0 
                ? TextUtils.Humanize(split[^1]) 
                : string.Empty;
        }

        public static Type[] GetTypesDerivedFrom(Type type)
        {
            Type[] types = GetDerivedTypes(type);

            return types;
        }

        public static Type GetTypeFromProperty(SerializedProperty property, bool fullType)
        {
            if (property == null)
            {
                Debug.LogError("Null property was found at 'GetTypeFromProperty'");
                return null;
            }
            
            string[] split = fullType
                ? property.managedReferenceFullTypename.Split(ASSEMBLY_SEPARATOR)
                : property.managedReferenceFieldTypename.Split(ASSEMBLY_SEPARATOR);

            return split.Length != 2 
                ? null : 
                Type.GetType(Assembly.CreateQualifiedName(split[0], split[1]));
        }

        public static Type GetTypeFromName(string typeName)
        {
            RequireAssembliesTypesSetup();
            return CACHE_ASSEMBLIES_TYPES.TryGetValue(typeName.GetHashCode(), out Type typeFound)
                ? typeFound
                : null;
        }
        
        // ASSEMBLIES TYPES CACHE: ----------------------------------------------------------------

        private static void RequireAssembliesTypesSetup()
        {
            if (CACHE_ASSEMBLIES_TYPES != null) return;
            CACHE_ASSEMBLIES_TYPES = new Dictionary<int, Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    CACHE_ASSEMBLIES_TYPES[type.Name.GetHashCode()] = type;
                }
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Type[] GetDerivedTypes(Type type)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new List<Type>();
            
            if (type == null) return types.ToArray();
            
            foreach (Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type assemblyType in assemblyTypes)
                {
                    if (assemblyType.IsInterface) continue;
                    if (assemblyType.IsAbstract) continue;
                    if (type.IsAssignableFrom(assemblyType)) types.Add(assemblyType);   
                }
            }

            return types.ToArray();
        }

        private static int CompareByType(Type a, Type b)
        {
            return string.CompareOrdinal(a.ToString(), b.ToString());
        }
    }
}