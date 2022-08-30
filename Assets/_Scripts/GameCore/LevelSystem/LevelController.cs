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

    public LevelController(LevelReferences references, LevelSettings settings, LevelConfig config, CurrentLevelSaveData savedLevelState)
    {
        this.References = references;
        this.Settings = settings;
        this.Config = config;

        TileController = new TileController(References.TileManagerReferences, Settings.TileManagerSettings, Config.TileManagerConfig);
        WordController = new WordController(References.WordControllerReferences, Settings.WordControllerSettings, Config.WordControllerConfig);
        ScoreController = new ScoreController(References.ScoreControllerReferences, Settings.ScoreControllerSettings);

        if (savedLevelState != null && savedLevelState.HasSavedLevel) LoadLevelControllers(savedLevelState);
        else SetupLevelControllers();
    }

    private void SetupLevelControllers()
    {
        ScoreController.SetupScoreController(Config.LevelTitle);
        TileController.SetupTileController(WordController);
        WordController.StartWordController(TileController, ScoreController);
    }

    public void LoadLevelControllers(CurrentLevelSaveData currentLevelSaveData)
    {
        ScoreController.LoadScoreController(Config.LevelTitle, currentLevelSaveData.CurrentTotalScore);
        TileController.LoadTileController(WordController, currentLevelSaveData.TilesLeft);
        WordController.LoadWordController(TileController, ScoreController, currentLevelSaveData.SubmittedWords);
    }

    public void ClearLevelControllers()
    {
        TileController.ClearTileController();
        WordController.ClearWordController();
        ScoreController.ClearScoreController();
    }
}
