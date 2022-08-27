using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using SaveSystem;
public class GameManager : AutoSingleton<GameManager>
{
    [SerializeField] LevelConfig[] levelList;
    [Group][SerializeField] LevelSettings levelSettings;
    [Group][SerializeField] LevelReferences levelSceneReferences;
    
    public LevelController CurrentLevel { get; private set; }

    public LevelConfig CurrentLevelConfig
    {
        get
        {
            if(GameManagerSaveData.Data.CurrentLevelIndex == levelList.Length)
            {
                Debug.Log("All Levels Completed Restarting");
                GameManagerSaveData.Data.ResetLevelIndex();
            }
            return levelList[GameManagerSaveData.Data.CurrentLevelIndex];
        }
    }
    private LevelConfig chosenLevelConfig;

    private void Start()
    {
        CreateCurrentLevel();   
    }

    public void CreateCurrentLevel()
    {
        CurrentLevel = new LevelController(levelSceneReferences, levelSettings, CurrentLevelConfig);
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
        SaveHandler.DeleteAll();
    }
    
    private void OnDrawGizmos()
    {
        //
    }


#endif
}
