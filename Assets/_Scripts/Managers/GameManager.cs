using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class GameManager : AutoSingleton<GameManager>
{
    [SerializeField] LevelConfig[] levelConfigurations;
    [SerializeField] LevelSceneReferences levelSceneReferences;

    public LevelController CurrentLevel { get; private set; }
    
    public LevelConfig ChosenLevelConfig 
    { 
        get 
        {
            if (chosenLevelConfig == null) chosenLevelConfig = levelConfigurations[0];
            return chosenLevelConfig;
        } 
    }

    private LevelConfig chosenLevelConfig;

    private void Start()
    {
        CreateNewLevel();   
    }

    private void CreateNewLevel()
    {
        CurrentLevel = new LevelController(ChosenLevelConfig, levelSceneReferences);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawGridEntityNames();
    }

    private void DrawGridEntityNames()
    {
        if (CurrentLevel == null) return;
        foreach (IGridEntity entity in CurrentLevel.GridController.EntityGrid)
        {
            if (entity == null || entity.EntityType == null) continue;
            Gizmos.color = Color.red;
            Extensions.drawString(entity.EntityType.GridEntityTypeName, entity.EntityTransform.position, Color.black);
        }
    }
#endif
}
