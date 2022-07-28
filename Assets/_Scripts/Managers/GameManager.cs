using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class GameManager : AutoSingleton<GameManager>
{
    [SerializeField] LevelConfig[] levelList;
    [Group][SerializeField] LevelSceneReferences levelSceneReferences;
    
    public LevelController CurrentLevel { get; private set; }

    public LevelConfig CurrentLevelConfig => levelList[GameManagerSaveData.Data.CurrentLevelIndex];

    private LevelConfig chosenLevelConfig;

    private void Start()
    {
        CreateCurrentLevel();   
    }

    public void CreateCurrentLevel()
    {
        CurrentLevel = new LevelController(CurrentLevelConfig, levelSceneReferences);
    }

    

#if UNITY_EDITOR

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void WinLevel()
    {
        CurrentLevel.LevelCleared();
    }

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void LoseLevel()
    {
        CurrentLevel.LevelFailed();
    }

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.DisabledInPlayMode)]
    private void DeleteAllSaves()
    {
        DeleteAllData.Delete();
    }
    
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
