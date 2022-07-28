using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BayatGames.SaveGameFree;

public class DeleteAllData : EditorWindow
{
    [MenuItem("Tools/Delete Save Datas")]
    public static void Delete()
    {
        Debug.Log("Deleting all save datas...");
        SaveGame.DeleteAll(SaveGamePath.PersistentDataPath);
        Debug.Log("Deleted all save datas.");
    }
}
