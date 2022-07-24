using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class GameManager : MonoBehaviour
{
    [SerializeField] LevelConfig[] levelConfigurations;
    [SerializeField] LevelController.LevelSceneReferences levelSceneReferences;

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

    private void OnDrawGizmos()
    {
        if (CurrentLevel == null) return;
        foreach (IGridEntity entity in CurrentLevel.GridController.EntityGrid)
        {
            if (entity == null) continue;
            Gizmos.color = Color.red;
            Extensions.drawString(entity.EntityType.GridEntityTypeName, entity.EntityTransform.position, Color.black);
        }
    }
}
