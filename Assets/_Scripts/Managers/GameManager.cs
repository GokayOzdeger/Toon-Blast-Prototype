using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class GameManager : MonoBehaviour
{
    [SerializeField] LevelConfig[] levelConfigurations;
    [SerializeField] LevelController.LevelSceneReferences levelSceneReferences;
    public LevelConfig ChosenLevelConfig 
    { 
        get 
        {
            if (chosenLevelConfig == null) chosenLevelConfig = levelConfigurations[0];
            return chosenLevelConfig;
        } 
    }

    private LevelConfig chosenLevelConfig;

    private void Start()
    {
        CreateNewLevel();   
    }

    private void CreateNewLevel()
    {
        LevelController controller = new LevelController(ChosenLevelConfig, levelSceneReferences);
    }
}
