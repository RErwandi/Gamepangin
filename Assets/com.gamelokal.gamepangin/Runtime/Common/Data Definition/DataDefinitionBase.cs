using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif


namespace Gamepangin
{
    public abstract class DataDefinitionBase : ScriptableObject, IEquatable<DataDefinitionBase>
    {
        [Title("Identifier")]
        [ValidateInput("IsUnique", "Id has been used in another asset in this definition")]
        [SerializeField] protected string id;

        public string Id => id;
        
        internal abstract bool IsUnique();
        
        public bool Equals(DataDefinitionBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataDefinitionBase)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(DataDefinitionBase left, DataDefinitionBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataDefinitionBase left, DataDefinitionBase right)
        {
            return !Equals(left, right);
        }

#if UNITY_EDITOR
        public class DataChecker : BuildPlayerProcessor
        {
            public override int callbackOrder => -1;

            public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
            {
                var ids = new HashSet<string>();

                var guids = AssetDatabase.FindAssets("t:" + typeof(DataDefinitionBase));
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var so = AssetDatabase.LoadAssetAtPath<DataDefinitionBase>(path);
                    if (string.IsNullOrEmpty(so.Id))
                    {
                        Debug.LogError("There are Data Definition without Id", so);
                        throw new Exception();
                    }
                    if (!ids.Add(so.Id))
                    {
                        Debug.LogError("There are Data Definition with duplicated Id", so);
                        throw new Exception();
                    }
                }
            }
        }
#endif
    }
}