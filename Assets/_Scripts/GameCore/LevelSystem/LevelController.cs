using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream:Assets/_Scripts/Level/LevelController.cs
public class LevelController
=======
public class LevelController 
>>>>>>> Stashed changes:Assets/_Scripts/GameCore/LevelSystem/LevelController.cs
{
    public LevelConfig Config { get; private set; }
    public LevelSceneReferences LevelSceneReferences { get; private set; }

    public LevelStates LevelState { get; private set; } = LevelStates.InProgress;

    // Level Components
    public GridController GridController { get; private set; }
    public GridEntitySpawnController GridEntitySpawnController { get; private set; }
    public ShuffleController ShuffleController { get; private set; }
    public GridGoalsController GridGoalController { get; private set; }
    public MovesController MovesController { get; private set; }

    public LevelController(LevelConfig config, LevelSceneReferences levelSceneReferences)
    {
<<<<<<< Updated upstream:Assets/_Scripts/Level/LevelController.cs
=======
        this.References = references;
        this.Settings = settings;
>>>>>>> Stashed changes:Assets/_Scripts/GameCore/LevelSystem/LevelController.cs
        this.Config = config;
        this.LevelSceneReferences = levelSceneReferences;
        CreateLevelControllers();
    }

    private void CreateLevelControllers()
    {
<<<<<<< Updated upstream:Assets/_Scripts/Level/LevelController.cs
        GridController = new GridController(Config.GridControllerSettings, LevelSceneReferences.GridControllerSceneReferences);
        GridController.OnGridInterractable.AddListener(SaveLevelState);

        GridEntitySpawnController = new GridEntitySpawnController(GridController, Config.GridEntitySpawnerSettings, LevelSceneReferences.GridEntitySpawnerSceneReferences);
        ShuffleController = new ShuffleController(GridController, LevelSceneReferences.ShuffleControllerSceneReferences);
        GridGoalController = new GridGoalsController(Config.GridGoalsControllerSettings, LevelSceneReferences.GridGoalsControllerReferences);
        MovesController = new MovesController(GridController, GridEntitySpawnController, Config.MovesControllerSettings, LevelSceneReferences.MovesControllerReferences);
        GridController.StartGrid(ShuffleController, GridEntitySpawnController, GridGoalController);
=======
        Config.AutoSolve();

        TileController = new TileController(References.TileManagerReferences, Settings.TileManagerSettings, Config.TileManagerConfig);
        WordController = new WordController(References.WordControllerReferences, Settings.WordControllerSettings, Config.WordControllerConfig);
        ScoreController = new ScoreController(References.ScoreControllerReferences, Settings.ScoreControllerSettings);

        ScoreController.SetupScoreController();
        TileController.SetupTileManager(WordController);
        WordController.SetupWordController(TileController, ScoreController);
>>>>>>> Stashed changes:Assets/_Scripts/GameCore/LevelSystem/LevelController.cs
    }

    public void LevelFailed()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Failed;
        CreateLevelResultFlyingText("Level Failed");
        GridController.GridDestroyOnLevelFailed();
        LevelSaveData.Data.ClearSavedLevelState();
    }

    public void LevelCleared()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Cleared;
        CreateLevelResultFlyingText("Level Cleared");
        GridController.GridDestroyOnLevelClear();
        
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
        GridGoalController.ClearUIElementsOnLevelEnd();

        GameManager.Instance.CreateCurrentLevel();
    }

    public enum LevelStates
    {
        InProgress,
        Cleared,
        Failed
    }
}
