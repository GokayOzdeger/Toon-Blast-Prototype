using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    private LevelConfig _config;

    private GridController _gridController;
    
    public LevelController(LevelConfig config, LevelSceneReferences levelSceneReferences)
    {
        this._config = config;

        _gridController = new GridController(_config.GridControllerSettings, levelSceneReferences.GridControllerSceneReferences);
    }

    [System.Serializable]
    public class LevelSceneReferences
    {
        [Group]
        [SerializeField] private GridController.GridControllerSceneReferences gridControllerSceneReferences;
        public GridController.GridControllerSceneReferences GridControllerSceneReferences => gridControllerSceneReferences;
    }
}
