using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin
{
	public static class ItemUtility
    {
		public static int ChangeStack(this Item item, int amount)
		{
			int prevStack = item.StackCount;
			item.StackCount += amount;

			return Mathf.Abs(prevStack - item.StackCount);
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public static ItemProperty[] GetAllPropertiesWithId(this Item item, string id)
		{
			var matchedProperties = new List<ItemProperty>();
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (prop.Id.Equals(id))
					matchedProperties.Add(prop);
			}

			return matchedProperties.ToArray();
		}

		/// <summary>
		/// Returns true if the item has a property with the given id.
		/// </summary>
		public static bool HasPropertyWithId(this Item item, string id)
		{
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (prop.Id.Equals(id))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public static ItemProperty GetPropertyWithId(this Item item, string id)
		{
			var properties = item.Properties;
			
			foreach (var prop in properties)
			{
				if (!prop.Id.Equals(id)) 
					continue;
				
				return prop;
			}

			return null;
		}

		/// <summary>
		/// Use this if you are NOT sure the item has this property.
		/// </summary>
		public static bool TryGetPropertyWithId(this Item item, string id, out ItemProperty itemProperty)
		{
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (!prop.Id.Equals(id))
					continue;
				
				itemProperty = prop;
				return true;
			}

			itemProperty = null;
			return false;
		}
    }
}