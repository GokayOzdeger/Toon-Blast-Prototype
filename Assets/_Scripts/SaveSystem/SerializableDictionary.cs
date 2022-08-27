using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new List<TKey>();
        [SerializeField] private List<TValue> _values = new List<TValue>();
        
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (var pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            if (_keys.Count != _values.Count)
            {
                Debug.Log($"Error: There are {_keys.Count} keys and {_values.Count} values after deserialization.");
            }
            
            for (var i = 0; i < _keys.Count; ++i)
            {
                this.Add(_keys[i], _values[i]);
            }
        }
    }

}
