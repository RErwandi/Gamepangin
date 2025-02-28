using System;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Gamepangin
{
    public abstract class DataDefinitionBase : ScriptableObject, IEquatable<DataDefinitionBase>
    {
        [Title("Identifier")] [SerializeField] protected string id;

        public string Id => id;

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

        private void OnValidate()
        {
            // If the ID is empty or duplicated, generate a new one.
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
                // Save changes in the editor.
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}