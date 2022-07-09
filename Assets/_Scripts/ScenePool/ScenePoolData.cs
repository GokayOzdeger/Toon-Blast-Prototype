using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/ScenePoolData")]
public class ScenePoolData : ScriptableObject
{
    public List<ScenePoolObject> ScenePoolObjects;

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
                Debug.Log(sceneObj.name);
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
        CollectScenePools(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [EasyButtons.Button()]
    public void TestCurrentScenePools()
    {
        Scene tempScene = EditorSceneManager.CreateScene("TemporaryScene");
        EditorSceneManager.OpenScene(tempScene.path);
        EditorApplication.EnterPlaymode();
        
        foreach (ScenePoolObject scenePoolObject in ScenePoolObjects)
        {
            for (int i = 0; i < scenePoolObject.Positions.Count; i++)
            {
                GameObject obj = ObjectPooler.Instance.Spawn(scenePoolObject.Prefab.name, scenePoolObject.Positions[i], scenePoolObject.Rotations[i]);
                obj.transform.localScale = scenePoolObject.Scales[i];
            }
        }
    }
#endif

    [System.Serializable]
    public class ScenePoolObject
    {
        public GameObject Prefab;
        public List<Quaternion> Rotations = new List<Quaternion>();
        public List<Vector3> Positions = new List<Vector3>();
        public List<Vector3> Scales = new List<Vector3>();

        public ScenePoolObject(GameObject prefab)
        {
            Prefab = prefab;
        }

        public void WriteSceneObject(GameObject obj)
        {
            Rotations.Add(obj.transform.rotation);
            Positions.Add(obj.transform.position);
            Scales.Add(obj.transform.localScale);
        }
    }
}
