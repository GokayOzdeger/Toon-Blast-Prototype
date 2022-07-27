using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    public LevelConfig Config { get; private set; }
    public LevelSceneReferences LevelSceneReferences { get; private set; }

    public LevelStates LevelState { get; private set; } = LevelStates.InProgress;

    // Level Managers
    public GridController GridController { get; private set; }
    public GridEntitySpawner GridEntitySpawner { get; private set; }
    public ShuffleController ShuffleController { get; private set; }
    public GridGoalsController GridGoalsController { get; private set; }
    public MovesController MovesController { get; private set; }

    public LevelController(LevelConfig config, LevelSceneReferences levelSceneReferences)
    {
        this.Config = config;
        this.LevelSceneReferences = levelSceneReferences;
        CreateLevelControllers();
    }

    private void CreateLevelControllers()
    {
        GridController = new GridController(Config.GridControllerSettings, LevelSceneReferences.GridControllerSceneReferences);
        GridEntitySpawner = new GridEntitySpawner(GridController, Config.GridEntitySpawnerSettings, LevelSceneReferences.GridEntitySpawnerSceneReferences);
        ShuffleController = new ShuffleController(GridController, LevelSceneReferences.ShuffleControllerSceneReferences);
        GridGoalsController = new GridGoalsController(Config.GridGoalsControllerSettings, LevelSceneReferences.GridGoalsControllerReferences);
        MovesController = new MovesController(GridController, GridEntitySpawner, Config.MovesControllerSettings, LevelSceneReferences.MovesControllerReferences);
        GridController.StartGrid(ShuffleController, GridEntitySpawner, GridGoalsController);
    }

    public void LevelFailed()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Failed;
        CreateLevelResultFlyingText("Level Failed");
        GridController.GridDestroyOnLevelFailed();
    }

    public void LevelCleared()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Cleared;
        CreateLevelResultFlyingText("Level Cleared");
        GridController.GridDestroyOnLevelClear();
    }

    private void CreateLevelResultFlyingText(string levelResult)
    {
        Vector2 flyingTextStartPos = UIEffectsManager.Instance.GetReferencePointByName("TopCenterOutside");
        Vector2 flyingTextWaitingPos = UIEffectsManager.Instance.GetReferencePointByName("ScreenCenter");
        Vector2 flyingTextEndPos = UIEffectsManager.Instance.GetReferencePointByName("RightCenterOutside");
        UIEffectsManager.Instance.CreatePassingByFlyingText(levelResult, 120, flyingTextStartPos, flyingTextWaitingPos, flyingTextEndPos, UIEffectsManager.CanvasLayer.OverGridUnderUI, 2.5f, 1);
    }

    private void ResetLevel()
    {
        
    }

    public enum LevelStates
    {
        InProgress,
        Cleared,
        Failed
    }
}
