using System.Linq;
using EasyButtons;
using UnityEngine;

public class LevelManager : SingletonGameStateListener<LevelManager>
{
    [SerializeField] private LevelConfig[] levelList;
    [Group] [SerializeField] private LevelSettings levelSettings;
    [Group] [SerializeField] private LevelReferences levelSceneReferences;

    private LevelController CurrentLevelController { get; set; }
    public LevelConfig[] LevelList => levelList;
    private LevelConfig ChosenLevelConfig { get; set; }

    private void Start()
    {
        if (CurrentLevelSaveData.Data.HasSavedLevel) LoadLevel();
    }

    protected override void OnEnterState()
    {
        CurrentLevelSaveData levelSaveData = CurrentLevelSaveData.Data;
        CurrentLevelController =
            new LevelController(levelSceneReferences, levelSettings, ChosenLevelConfig, levelSaveData);
    }

    protected override void OnExitState()
    {
        if (CurrentLevelController == null) return;
        CurrentLevelController.ClearLevelControllers();
        CurrentLevelSaveData.Data.ClearSavedLevelStateData();
        CurrentLevelController = null;
    }

    public void LevelCompleted()
    {
        GameState nextState;
        if (CurrentLevelController.ScoreController.IsNewHighScore) nextState = levelSceneReferences.HighScoreGameState;
        else nextState = levelSceneReferences.LevelSelectGameState;

        LevelSaveData.SaveLevelData(CurrentLevelController.Config.LevelTitle,
            CurrentLevelController.ScoreController.CurrentTotalScore);
        EndLevelState(nextState);
    }

    private void EndLevelState(GameState nextState)
    {
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

    private void LoadLevel()
    {
        ChosenLevelConfig = LevelList.First(config => config.LevelTitle == CurrentLevelSaveData.Data.LevelTitle);
        GameManager.Instance.ChangeGameState(state);
    }

    #region EDITOR

#if UNITY_EDITOR
    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void FindWord()
    {
        bool wordFound = CurrentLevelController.WordController.CheckHasPossibleWord();
        Debug.Log("Word Exists: " + wordFound);
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void CompleteLevelNormal()
    {
        LevelCompleted();
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void CompleteLevelWithHighScore()
    {
        CurrentLevelController.ScoreController.SetCurrentScoreToHighScore();
        LevelCompleted();
    }

#endif

    #endregion
}