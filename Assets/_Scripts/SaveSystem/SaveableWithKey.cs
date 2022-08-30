using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public abstract class SaveableWithKey<T> where T : class, new()
    {
        private static List<DataWithKey<T>> _allDatasWithKeys = new List<DataWithKey<T>>();

        protected static string SaveName(string key) { return typeof(T).Name + "-" + key; }
        public static T Data(string key)
        {
            key = key.ToLowerInvariant();
            return FindDataWithKey(key).data;
        }

        public static void Save(string key)
        {
            key = key.ToLowerInvariant();
            Debug.Log("Saved data of Type: " + SaveName(key));
            SaveHandler.Save(SaveName(key), FindDataWithKey(key).data);
        }

        public static void DeleteSave(string key)
        {
            key = key.ToLowerInvariant();
            Debug.Log("Deleted data of Type: " + SaveName(key));
            foreach (DataWithKey<T> dataWithKey in _allDatasWithKeys) if (dataWithKey.key == key) dataWithKey.ClearData();
            SaveHandler.Delete(SaveName(key));
        }

        private static T LoadData(string key)
        {
            key = key.ToLowerInvariant();
            Debug.Log("Loading Data of Type: " + SaveName(key));
            T loadedData = SaveHandler.Load<T>(SaveName(key));
            if (loadedData == null)
            {
                loadedData = new T();
            }
            return loadedData;
        }

        private static DataWithKey<T> FindDataWithKey(string key)
        {
            key = key.ToLowerInvariant();
            foreach (DataWithKey<T> dataWithKey in _allDatasWithKeys)
                if (dataWithKey.key == key)
                    return dataWithKey;

            DataWithKey<T> newDataWithKey = new DataWithKey<T>(key, LoadData(key) ?? new T());
            _allDatasWithKeys.Add(newDataWithKey);
            return newDataWithKey;
        }

        private partial class DataWithKey<TK> where TK : class, new()
        {
            public string key;
            public TK data;

            public DataWithKey(string _key, TK _data)
            {
                key = _key;
                data = _data;
            }

            public void ClearData()
            {
                data = new TK();
            }
        }
    }
}
