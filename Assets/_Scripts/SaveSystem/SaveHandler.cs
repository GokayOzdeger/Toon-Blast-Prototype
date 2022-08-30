using System;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public class SaveHandler 
    {
        private const string SaveFolderName = "Saves";

        public static void Save(string saveKey, object data)
        {
            string fileName = GetJsonNameFromKeyName(saveKey);
            string fullPath = Path.Combine(Application.persistentDataPath, SaveFolderName, fileName);
            Debug.Log(fullPath);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                string dataToStore = JsonUtility.ToJson(data, true);
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving data: " + e.Message);
                throw;
            }
        }

        public static T Load<T>(string saveKey) where T : class
        {
            string fileName = GetJsonNameFromKeyName(saveKey);
            string fullPath = Path.Combine(Application.persistentDataPath, SaveFolderName, fileName);
            T loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    loadedData = JsonUtility.FromJson<T>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading data at {fullPath}, Message: " + e.Message);
                    throw;
                }
            }

            return loadedData;
        }
    
        public static void Delete(string saveKey)
        {
            string fileName = GetJsonNameFromKeyName(saveKey);
            string fullPath = Path.Combine(Application.persistentDataPath, SaveFolderName, fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public static void DeleteAll()
        {
            string saveDirectory = Path.Combine(Application.persistentDataPath, SaveFolderName);
            DirectoryInfo di = new DirectoryInfo(saveDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            Debug.Log("Deleted All Save Files !");
        }
    
        public static bool Exists(string saveKey)
        {
            string fileName = GetJsonNameFromKeyName(saveKey);
            string fullPath = Path.Combine(Application.persistentDataPath, SaveFolderName, fileName);
            return File.Exists(fullPath);
        }

        private static string GetJsonNameFromKeyName(string saveKey)
        {
            return saveKey + ".json";
        }
    }
}
