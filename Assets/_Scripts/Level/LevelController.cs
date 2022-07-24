using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    public LevelConfig Config { get; private set; }

    public GridController GridController { get; private set; }
    
    public LevelController(LevelConfig config, LevelSceneReferences levelSceneReferences)
    {
        this.Config = config;

        GridController = new GridController(Config.GridControllerSettings, levelSceneReferences.GridControllerSceneReferences);
    }

    [System.Serializable]
    public class LevelSceneReferences
    {
        [Group]
        [SerializeField] private GridController.GridControllerSceneReferences gridControllerSceneReferences;
        public GridController.GridControllerSceneReferences GridControllerSceneReferences => gridControllerSceneReferences;
    }
}
