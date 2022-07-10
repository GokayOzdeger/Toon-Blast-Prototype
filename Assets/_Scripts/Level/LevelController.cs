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

    public void LostLevel()
    {
        BehaviourManager.Instance.RemoveEntitiesAndReset();
        GameManager.Instance.CreateNewLevel();
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

    public bool TryGetBehaviour<T>(out T levelBehaviour) where T : ALevelBehaviourSO
    {
        foreach (ALevelBehaviourSO behaviour in LevelBehaviours)
        {
            if (behaviour is T) 
            {
                levelBehaviour = (T)behaviour;
                return true;
            }
        }
        levelBehaviour = null;
        return false;
    }


    [System.Serializable]
    public class LevelSceneReferences
    {
        [Group] public TowerSpawnController.TowerSpawnControllerSceneReferences TowerSpawnControllerReferences;
        [Group] public ScoreCounter.ScoreCounterSceneReferences ScoreCounterSceneReferences;
    }
}
