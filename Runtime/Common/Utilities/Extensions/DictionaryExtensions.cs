using System.Collections.Generic;

namespace Gamepangin
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> TryAddKeyValuePair(this Dictionary<string, object> dict, string key, object value)
        {
            if (dict == null) dict = new Dictionary<string, object>();

            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }

            return dict;
        }     
        
        public static void ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }
    }
}