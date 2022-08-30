using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonGameStateListener<LevelManager>
{
    [SerializeField] private LevelConfig[] levelList;
    [Group][SerializeField] private LevelSettings levelSettings;
    [Group][SerializeField] private LevelReferences levelSceneReferences;

    public LevelController CurrentLevelController { get; private set; }
    public LevelConfig[] LevelList => levelList;
    public LevelConfig ChosenLevelConfig { get; private set; }


    public override void OnEnterState()
    {
        CurrentLevelController = new LevelController(levelSceneReferences, levelSettings, ChosenLevelConfig);
    }

    public override void OnExitState()
    {
        //
    }

    public void SaveLevelState()
    {
        LevelSaveData.Data.SaveLevelState(CurrentLevelController);
    }

    public void CreateLevel(LevelConfig config)
    {
        ChosenLevelConfig = config;
        GameManager.Instance.ChangeGameState(state);
    }

    #region EDITOR
#if UNITY_EDITOR

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void FindWord()
    {
        bool wordFound = CurrentLevelController.WordController.CheckHasPossibleWord();
        Debug.Log("Word Exists: " + wordFound);
    }

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
