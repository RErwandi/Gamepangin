using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    [Serializable]
    public class ItemProperty
    {
        //[ValueDropdown(Constants.EDITOR_ALL_ITEM_PROPERTY, FlattenTreeView = true)]
        [SerializeField] private ItemPropertyDefinition definition;
        [SerializeField] private float propertyValue;
        
        public string Id => definition.Id;
        public ItemPropertyType PropertyType => definition.propertyType;

        public ItemProperty()
        {
            definition = null;
            propertyValue = 100f;
        }
        
        public ItemProperty GetClone() => (ItemProperty)MemberwiseClone();

        public bool Boolean
        {
            get => propertyValue > 0f;
            set
            {
                if (PropertyType == ItemPropertyType.Boolean)
                {
                    SetIntervalValue(value ? 1 : 0);
                }
            }
        }

        public int Integer
        {
            get => (int)propertyValue;
            set
            {
                if (PropertyType == ItemPropertyType.Integer)
                {
                    SetIntervalValue(value);
                }
            }
        }

        public float Float
        {
            get => propertyValue;
            set
            {
                if (PropertyType == ItemPropertyType.Float)
                {
                    SetIntervalValue(value);
                }
            }
        }

        public UnityAction<ItemProperty> PropertyChanged;

        private void SetIntervalValue(float value)
        {
            var oldValue = propertyValue;
            propertyValue = value;

            if (Math.Abs(oldValue - propertyValue) > 0.01f)
            {
                PropertyChanged?.Invoke(this);
            }
        }
    }
}