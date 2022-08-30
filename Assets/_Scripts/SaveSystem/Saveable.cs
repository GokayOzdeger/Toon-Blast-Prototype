using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SaveSystem
{
    /// <summary>
    /// Saveable is a basic save system, each saveable class acts as a signleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Saveable<T> where T : class, new()
    {
        private static T _data;
        public static T Data
        {
            get
            {
                if (_data == null) _data = LoadData();
                return _data;
            }
        }

        public virtual void Save()
        {
            SaveHandler.Save(typeof(T).FullName, _data);
        }

        public static void DeleteSave()
        {
            _data = new T();
            SaveHandler.Delete(typeof(T).FullName);
        }

        private static T LoadData() 
        {
            T loadedData = SaveHandler.Load<T>(typeof(T).FullName);
            if (loadedData == null)
            {
                loadedData = new T();
            }
            return loadedData;
        }
    }
}
