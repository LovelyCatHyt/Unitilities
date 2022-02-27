using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unitilities.Serialization
{
    [Serializable]
    public struct SerializableKeyValuePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public static implicit operator SerializableKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> kvPair)
        {
            return new SerializableKeyValuePair<TKey, TValue>(kvPair.Key, kvPair.Value);
        }

        public static implicit operator KeyValuePair<TKey, TValue>(SerializableKeyValuePair<TKey, TValue> kvPair)
        {
            return new KeyValuePair<TKey, TValue>(kvPair.key, kvPair.value);
        }
    }

    /// <summary>
    /// Unity 无法序列化字典, 而该类可以实现字典和列表的互换
    /// </summary>
    public static class UnityDictConverter
    {
        [Flags]
        public enum DictConstructSetting
        {
            /// <summary>
            /// 如果遇到同一个 Key, 则使用新的 Value 覆盖原有的
            /// </summary>
            OverrideOnSameKey = 0,
            /// <summary>
            /// 跳过同一个 Key
            /// </summary>
            SkipSameKey = 1 << 0,
        }


        public static Dictionary<TKey, TValue> ConvertToDict<TKey, TValue>(
            IEnumerable<SerializableKeyValuePair<TKey, TValue>> kvPairList, DictConstructSetting setting = DictConstructSetting.SkipSameKey)
        {
            var dict = new Dictionary<TKey, TValue>();
            foreach (var kv in kvPairList)
            {
                if (dict.ContainsKey(kv.key))
                {
                    if ((setting & DictConstructSetting.SkipSameKey) == 0)
                    {
                        dict[kv.key] = kv.value;
                    }
                }
                else
                {
                    dict[kv.key] = kv.value;
                }
            }
            return dict;
        }

        public static List<SerializableKeyValuePair<TKey, TValue>> ConvertToList<TKey, TValue>(
            Dictionary<TKey, TValue> dict)
        {
            var list = new List<SerializableKeyValuePair<TKey, TValue>>(dict.Count);
            foreach (var kvPair in dict)
            {
                list.Add(kvPair);
            }
            return list;
        }
    }
}
