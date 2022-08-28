using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    public LevelConfig Config { get; private set; }
    public LevelSettings Settings { get; private set; }
    public LevelReferences References { get; private set; }

    public LevelStates LevelState { get; private set; } = LevelStates.InProgress;

    // Level Components

    public TileController TileController { get; private set; }
    public WordController WordController { get; private set; }
    public ScoreController ScoreController { get; private set; }

    public LevelController(LevelReferences references, LevelSettings settings, LevelConfig config)
    {
        this.References = references;
        this.Settings = settings;
        this.Config = config;
        CreateLevelControllers();
    }

    private void CreateLevelControllers()
    {
        TileController = new TileController(References.TileManagerReferences, Settings.TileManagerSettings, Config.TileManagerConfig);
        WordController = new WordController(References.WordControllerReferences, Settings.WordControllerSettings, Config.WordControllerConfig);
        ScoreController = new ScoreController(References.ScoreControllerReferences, Settings.ScoreControllerSettings);

        ScoreController.SetupScoreController();
        TileController.SetupTileManager(WordController);
        WordController.SetupWordController(TileController, ScoreController);
    }

    public void LevelFailed()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Failed;
        CreateLevelResultFlyingText("Level Failed");

        LevelSaveData.Data.ClearSavedLevelState();
    }

    public void LevelCleared()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Cleared;
        CreateLevelResultFlyingText("Level Cleared");

        
        LevelSaveData.Data.ClearSavedLevelState();
        GameManagerSaveData.Data.ProgressLevel();
    }

    private void SaveLevelState()
    {
        LevelSaveData.Data.SaveLevelState(this);
    }

    private void CreateLevelResultFlyingText(string levelResult)
    {
        Vector2 flyingTextStartPos = UIEffectsManager.Instance.GetReferencePointByName("TopCenterOutside");
        Vector2 flyingTextWaitingPos = UIEffectsManager.Instance.GetReferencePointByName("ScreenCenter");
        Vector2 flyingTextEndPos = UIEffectsManager.Instance.GetReferencePointByName("RightCenterOutside");
        UIEffectsManager.Instance.CreatePassingByFlyingText(levelResult, 120, flyingTextStartPos, flyingTextWaitingPos, flyingTextEndPos, UIEffectsManager.CanvasLayer.OverGridUnderUI, 2.5f, 1, LevelEnded);
    }
    
    private void LevelEnded()
    {
        // clear leftovers from old scene

        GameManager.Instance.CreateCurrentLevel();
    }

    public enum LevelStates
    {
        InProgress,
        Cleared,
        Failed
    }
}
