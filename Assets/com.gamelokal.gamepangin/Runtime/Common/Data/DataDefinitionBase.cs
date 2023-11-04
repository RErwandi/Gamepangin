using Sirenix.OdinInspector;
using UnityEngine;

namespace Gamepangin
{
    public abstract class DataDefinitionBase : ScriptableObject
    {
        [Title("Identifier")]
        [ValidateInput("IsUnique", "Id has been used in another asset in this definition")]
        [SerializeField] UniqueId id;

        public UniqueId Id => id;

        public virtual string Name
        {
            get => string.Empty;
            set => Name = value;
        }
        
        public virtual string Description => string.Empty;
        public virtual Sprite Icon => null;
        
        internal abstract bool IsUnique();
    }
}