using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamepangin
{
	[Serializable]
	public abstract class TSerializableDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		public const string NAME_KEYS = nameof(keys);
		public const string NAME_VALUES = nameof(values);

		[NonSerialized] 
		protected Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

		[SerializeField] private TKey[] keys;
		[SerializeField] private TValue[] values;
		
		public int Count => dictionary.Count;
		
		public ICollection<TKey> Keys => dictionary.Keys;
		public ICollection<TValue> Values => dictionary.Values;

		public TValue this[TKey key]
		{
			get => dictionary[key];
			set => dictionary[key] = value;
		}
		
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

		public void Add(TKey key, TValue value)
		{
			dictionary.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			return dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			(dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return (dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			(dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return (dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (AssemblyUtils.IsReloading) return;
			BeforeSerialize();
		}
		
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (AssemblyUtils.IsReloading) return;
			AfterSerialize();
		}

		protected virtual void BeforeSerialize()
		{
			if (dictionary == null || dictionary.Count == 0)
			{
				keys = null;
				values = null;
			}
			else
			{
				int count = dictionary.Count;
				keys = new TKey[count];
				values = new TValue[count];
				int i = 0;

				using Dictionary<TKey, TValue>.Enumerator dict = dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					keys[i] = dict.Current.Key;
					values[i] = dict.Current.Value;
					i++;
				}
			}
		}

		protected virtual void AfterSerialize()
		{
			dictionary = new Dictionary<TKey, TValue>();
			if (keys == null || values == null) return;
			
			for (int i = 0; i < keys.Length; i++)
			{
				if (i < values.Length) dictionary[keys[i]] = values[i];
				else dictionary[keys[i]] = default;
			}
		}
	}
}