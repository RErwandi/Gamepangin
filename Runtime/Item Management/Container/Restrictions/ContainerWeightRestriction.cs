using UnityEngine;
using UnityEngine.Events;

namespace Gamepangin
{
    [System.Serializable]
    public sealed class ContainerWeightRestriction : ContainerRestriction
    {
        public float MaxWeight => m_MaxWeight;
        public float WeightPerSlot
        {
            get => m_WeightPerSlot;
            set => m_WeightPerSlot = value;
        }

        public float TotalWeight
        {
            get => m_TotalWeight;
            private set
            {
                m_TotalWeight = value;
                WeightChanged?.Invoke(m_TotalWeight);
            }
        }

        public event UnityAction<float> WeightChanged;

        [SerializeField]
        private float m_TotalWeight;

        [SerializeField]
        private float m_MaxWeight;

        [SerializeField]
        private float m_WeightPerSlot;
        
        
        public ContainerWeightRestriction(float weightPerSlot)
        {
            m_WeightPerSlot = weightPerSlot;
        }

        public override void OnInitialized(ItemContainer container)
        {
            base.OnInitialized(container);
            container.ContainerChanged += RecalculateWeight;
            RecalculateWeight();
        }

        public override int GetAllowedAddAmount(Item item, int count)
        {
            int allowCount = count;
			
            if (count == 1)
            {
                if (m_TotalWeight + item.Definition.Weight * count > m_MaxWeight)
                    return 0;
            }
            else
            {
                allowCount = (int)Mathf.Clamp(count, 0f, (m_MaxWeight - m_TotalWeight) / item.Definition.Weight * count);
                
                if (allowCount == 0)
                    return 0;
            }

            return allowCount;
        }

        public override int GetAllowedRemoveAmount(Item item, int count) => count;
        public override string GetRejectionString() => "Inventory weight limit reached.";

        public void RecalculateWeight()
        {
            float weight = 0f;
            var slots = itemContainer.Slots;
            
            foreach (var slot in slots)
            {
                if (slot.HasItem)
                    weight += slot.Item.TotalWeight;
            }

            TotalWeight = weight;
            m_MaxWeight = m_WeightPerSlot * itemContainer.Slots.Count;
        }
    }
}