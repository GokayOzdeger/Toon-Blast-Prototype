using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/ ScenePoolLevelBehaviour")]
public class ScenePoolLevelBehaviour : ALevelBehaviourSO
{
    public override void OnSetup()
    {
        BuildScene(Config.ScenePoolSettings.ScenePoolData);
    }

    private void BuildScene(ScenePoolData scenePoolData)
    {
        Debug.Log("Building scene");
        foreach (ScenePoolObject scenePoolObject in scenePoolData.ScenePoolObjects)
        {
            for (int i = 0; i < scenePoolObject.Positions.Count; i++)
            {
                GameObject obj = ObjectPooler.Instance.Spawn(scenePoolObject.Prefab.name, scenePoolObject.Positions[i], scenePoolObject.Rotations[i]);
                obj.transform.localScale = scenePoolObject.Scales[i];
            }
        }
    }

    public override void OnTick(float deltaTime)
    {
        //
    }


    [System.Serializable]
    public class ScenePoolLevelBehaviourSettings
    {
        public ScenePoolData ScenePoolData;
    }
}
