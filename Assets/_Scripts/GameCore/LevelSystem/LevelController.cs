public class LevelController
{
    public LevelController(LevelReferences references, LevelSettings settings, LevelConfig config,
        CurrentLevelSaveData savedLevelState)
    {
        References = references;
        Settings = settings;
        Config = config;

        TileController = new TileController(References.TileManagerReferences, Settings.TileManagerSettings,
            Config.TileManagerConfig);
        WordController = new WordController(References.WordControllerReferences, Settings.WordControllerSettings);
        ScoreController = new ScoreController(References.ScoreControllerReferences, Settings.ScoreControllerSettings);

        if (savedLevelState is { HasSavedLevel: true }) LoadLevelControllers(savedLevelState);
        else SetupLevelControllers();
    }
    // Dependencies

    public LevelConfig Config { get; }
    public LevelSettings Settings { get; }
    public LevelReferences References { get; }

    // Level Components

    public TileController TileController { get; }
    public WordController WordController { get; }
    public ScoreController ScoreController { get; }

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