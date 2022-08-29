using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoGameStateListener
{
    [SerializeField] private LevelConfig[] levelList;
    [Group][SerializeField] private LevelSettings levelSettings;
    [Group][SerializeField] private LevelReferences levelSceneReferences;

    public LevelController CurrentLevelController { get; private set; }
    public LevelConfig CurrentLevelConfig
    {
        get
        {
            if (GameManagerSaveData.Data.CurrentLevelIndex == levelList.Length)
            {
                Debug.Log("All Levels Completed Restarting");
                GameManagerSaveData.Data.ResetLevelIndex();
            }
            return levelList[GameManagerSaveData.Data.CurrentLevelIndex];
        }
    }
    private LevelConfig chosenLevelConfig;

    public override void OnEnterState()
    {
        CreateCurrentLevel();
    }

    public override void OnExitState()
    {
        //
    }

    public void CreateCurrentLevel()
    {
        CurrentLevelController = new LevelController(levelSceneReferences, levelSettings, CurrentLevelConfig);
    }

    #region EDITOR
#if UNITY_EDITOR

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void WinLevel()
    {
        CurrentLevelController.LevelCompleted();
    }

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void LoseLevel()
    {
        CurrentLevelController.LevelFailed();
    }

#endif
    #endregion
}
