using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    [Serializable, HideLabel]
    public class UniqueId
    {
        [Tooltip("You can change it but make sure that it's unique from others id")]
        [SerializeField] private string uniqueId;
        
        [NonSerialized] private int hash;
        public int Hash
        {
            get
            {
                if (hash == 0) hash = uniqueId.GetHashCode();
                return hash;
            }
        }

        public UniqueId()
        {
            uniqueId = new string(GenerateID());
        }

        public UniqueId(string defaultID)
        {
            defaultID = string.IsNullOrEmpty(defaultID) ? GenerateID() : defaultID;
            uniqueId = new string(defaultID);
        }

        public static string GenerateID() => Guid.NewGuid().ToString("D");
        public override string ToString() => uniqueId;

        public override bool Equals(object other)
        {
            return other is UniqueId otherUniqueId && Equals(otherUniqueId);
        }

        public bool Equals(string other)
        {
            return string.Equals(uniqueId, other);
        }

        public bool Equals(UniqueId other)
        {
            return uniqueId.Equals(other.uniqueId);
        }

        public override int GetHashCode() => Hash;
    }
}
