using BayatGames.SaveGameFree;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveableWithKey<T> where T : new()
{
    private static List<DataWithKey<T>> _allDatasWithKeys = new List<DataWithKey<T>>();

    protected static string SaveName(string key) { return typeof(T).FullName + "-" + key; }
    public static T Data(string key) 
    {
        return FindDataWithKey(key).data;
    }

    public static void Save(string key)
    {
        Debug.Log("Saved data of Type: " + SaveName(key));
        SaveGame.Save<T>(SaveName(key), FindDataWithKey(key).data, false);
    }

    public static void DeleteSave(string key)
    {
        Debug.Log("Deleted data of Type: " + SaveName(key));
        foreach (DataWithKey<T> dataWithKey in _allDatasWithKeys) if (dataWithKey.key == key) dataWithKey.ClearData();
        SaveGame.Delete(SaveName(key));
    }

    private static T LoadData(string key)
    {
        Debug.Log("Loading Data of Type: "+typeof(T).Name);
        T loadedData = SaveGame.Load<T>(SaveName(key), default(T), false);
        if (loadedData == null) 
        {
            Debug.Log("No Data of Type: " + SaveName(key) + " found. Creating default Data");
            loadedData = new T(); 
        }
        return loadedData;
    }

    private static DataWithKey<T> FindDataWithKey(string key) {
        foreach (DataWithKey<T> dataWithKey in _allDatasWithKeys) 
            if (dataWithKey.key == key) 
                return dataWithKey;

        DataWithKey<T> newDataWithKey = new DataWithKey<T>(key, LoadData(key) ?? new T());
        _allDatasWithKeys.Add(newDataWithKey);
        return newDataWithKey;
    }

    private partial class DataWithKey<TK> where TK : new()
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
