using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SaveSystem
{
    public class DeleteAllData : EditorWindow
    {
        [MenuItem("Tools/Delete Save Datas")]
        public static void Delete()
        {
            Debug.Log("Deleting all save datas...");
            SaveHandler.DeleteAll();
            Debug.Log("Deleted all save datas.");
        }
    }
}
