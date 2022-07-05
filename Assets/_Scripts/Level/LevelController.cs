using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    private LevelConfig _config;
    
    public LevelController(LevelConfig config, LevelSceneReferences levelSceneReferences)
    {
        this._config = config;
    }

    [System.Serializable]
    public class LevelSceneReferences
    {

    }
}
