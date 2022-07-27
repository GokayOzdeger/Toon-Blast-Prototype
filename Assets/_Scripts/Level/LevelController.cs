using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    public LevelConfig Config { get; private set; }
    public LevelSceneReferences LevelSceneReferences { get; private set; }

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
}
