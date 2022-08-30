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
        CurrentLevelController = null;
    }
    public void LevelCompleted()
    {
        GameState nextState;
        if (CurrentLevelController.ScoreController.IsNewHighScore) nextState = levelSceneReferences.HighScoreGameState;
        else nextState = levelSceneReferences.LevelSelectGameState;

        LevelSaveData.SaveLevelData(CurrentLevelController.Config.LevelTitle, CurrentLevelController.ScoreController.CurrentTotalScore);
        CurrentLevelSaveData.Data.ClearSavedLevelStateData();
        EndLevelState(nextState);
    }

    private void EndLevelState(GameState nextState)
    {
        CurrentLevelController.ClearLevelControllers();
        GameManager.Instance.ChangeGameState(nextState);
    }

    public void SaveLevelState()
    {
        CurrentLevelSaveData.Data.SaveLevelState(CurrentLevelController);
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
    private void CompleteLevelNormal()
    {
        LevelCompleted();
    }

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void CompleteLevelWithHighScore()
    {
        CurrentLevelController.ScoreController.SetCurrentScoreToHighScore();
        LevelCompleted();
    }

#endif
    #endregion
}
