using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    public class ScriptableObjectWithId : ScriptableObject
    {
        [Title("Identifier")]
        [ValidateInput("IsUnique", "There is assets that has the same id with this one")]
        public UniqueId id;

        private bool IsUnique()
        {
            return Database.Get<ScriptableObjectWithIdRepository>().ScriptableObjects
                .Where(scriptableObject => scriptableObject != null)
                .Where(scriptableObject => scriptableObject.GetInstanceID() != GetInstanceID())
                .All(scriptableObject => !scriptableObject.id.Equals(id));
        }
    }
}