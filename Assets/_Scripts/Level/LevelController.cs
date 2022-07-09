using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    private List<ALevelBehaviourSO> LevelBehaviours { get; set; } = new List<ALevelBehaviourSO>();

    private LevelConfig _config;
    private LevelController.LevelSceneReferences _sceneReferences;

    public LevelController(LevelConfig config, LevelSceneReferences levelSceneReferences)
    {
        this._config = config;
        this._sceneReferences = levelSceneReferences;
    }

    public void SetupLevel()
    {
        CreateLevelBehaviourInstances();
    }

    private void CreateLevelBehaviourInstances()
    {
        foreach (ALevelBehaviourSO behaviour in _config.LevelBehaviours)
        {
            ALevelBehaviourSO behaviourInstance = MonoBehaviour.Instantiate(behaviour);
            behaviourInstance.Setup(_config, _sceneReferences);
            LevelBehaviours.Add(behaviourInstance);
        }
    }

    public void TickLevel(float deltaTime)
    {
        foreach (ALevelBehaviourSO behaviour in LevelBehaviours) behaviour.Tick(deltaTime);
    }

    public T GetBehaviour<T>() where T : ALevelBehaviourSO
    {
        foreach (ALevelBehaviourSO behaviour in LevelBehaviours)
        {
            if (behaviour is T) return (T)behaviour;
        }
        return null;
    }


    [System.Serializable]
    public class LevelSceneReferences
    {
        [Group] public TowerSpawnController.TowerSpawnControllerSceneReferences TowerSpawnControllerReferences;
    }
}
