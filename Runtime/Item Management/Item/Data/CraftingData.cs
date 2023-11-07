using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin
{
    [System.Serializable]
    public class CraftRequirement
    {
        public ItemDefinition item;
        public int amount;
    }
    
    public class CraftingData : ItemData
    {
        [SerializeField] private int craftAmount = 1;
        [SerializeField] private float craftDuration = 3f;
        [SerializeField] private List<CraftRequirement> requirements = new List<CraftRequirement>();

        public int CraftAmount => craftAmount;
        public float CraftDuration => craftDuration;
        public List<CraftRequirement> Requirements => requirements;
    }
}