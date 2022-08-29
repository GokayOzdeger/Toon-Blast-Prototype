using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    // Dependencies

    public LevelConfig Config { get; private set; }
    public LevelSettings Settings { get; private set; }
    public LevelReferences References { get; private set; }

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
        LevelSaveData.Data.ClearSavedLevelState();
    }

    public void LevelCompleted()
    {
        GameManagerSaveData.Data.ProgressLevel();
    }

    private void SaveLevelState()
    {
        LevelSaveData.Data.SaveLevelState(this);
    }
    
    private void LevelEnded()
    {
        // clear leftovers from old scene
        if (ScoreController.IsNewHighScore) GameManager.Instance.ChangeGameState(References.HighScoreGameState);
        else GameManager.Instance.ChangeGameState(References.LevelSelectGameState);
    }
}
