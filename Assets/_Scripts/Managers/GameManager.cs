using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class GameManager : AutoSingleton<GameManager>
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

    public LevelController CurrentLevel { get; private set; }

    private LevelConfig chosenLevelConfig;

    private void Start()
    {
        CreateNewLevel();   
    }

    private void Update()
    {
        CurrentLevel.TickLevel(Time.deltaTime);
    }

    private void CreateNewLevel()
    {
        CurrentLevel = new LevelController(ChosenLevelConfig, levelSceneReferences);
    }
}
