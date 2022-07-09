using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Utilities;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "ScriptableObjects/ScenePoolData")]
public partial class ScenePoolData : ScriptableObject
{
    public List<ScenePoolObject> ScenePoolObjects = new List<ScenePoolObject>();


    
#if UNITY_EDITOR

    private static void CollectScenePools(ScenePoolData dataObjectToAssing)
    {
        dataObjectToAssing.ScenePoolObjects.Clear();

        HashSet<Object> poolTypesToCollect = new HashSet<Object>();
        GameObject[] allActiveSceneObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject sceneObj in allActiveSceneObjects)
        {
            if (!sceneObj.TryGetComponent(out PoolObject poolable)) continue;
            if (!poolable.IncludedInScenePool) continue;
            poolTypesToCollect.Add(PrefabUtility.GetPrefabParent(sceneObj));
        }

        foreach (GameObject prefab in poolTypesToCollect)
        {
            ScenePoolObject scenePoolObject = new ScenePoolObject(prefab);
            dataObjectToAssing.ScenePoolObjects.Add(scenePoolObject);

            // find all instancces of prefab in the scene
            foreach (GameObject sceneObj in allActiveSceneObjects)
            {
                if (PrefabUtility.GetPrefabParent(sceneObj) == prefab)
                {
                    scenePoolObject.WriteSceneObject(sceneObj);
                }
            }
        }
    }

    [MenuItem("Tools/Create Scene Pool")]
    public static void CreateScenePoolFromScene()
    {
        ScenePoolData newSceneData = CreateInstance<ScenePoolData>();
        CollectScenePools(newSceneData);
        
        string path = $"Assets/Resources/ScriptableObjects/Scene Pools/{EditorSceneManager.GetActiveScene().name}.asset";
        AssetDatabase.CreateAsset(newSceneData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newSceneData;
    }

    [EasyButtons.Button()]
    public void SaveCurrentScenePools()
    {
        Undo.RecordObject(this, "Save Scene Pool");
        CollectScenePools(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

#endif
}
