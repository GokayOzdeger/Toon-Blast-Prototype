﻿using UnityEngine;


[System.Serializable]
public class LevelSceneReferences
{
    [Group][SerializeField] private GridControllerSceneReferences gridControllerSceneReferences;
    [Group][SerializeField] private GridEntitySpawnerSceneReferences gridEntitySpawnerSceneReferences;
    [Group][SerializeField] private ShuffleControllerSceneReferences shuffleControllerSceneReferences;
    [Group][SerializeField] private GridGoalsControllerReferences goalsControllerReferences;
    [Group][SerializeField] private MovesControllerSceneReferences movesControllerReferences;
    public GridControllerSceneReferences GridControllerSceneReferences => gridControllerSceneReferences;
    public GridEntitySpawnerSceneReferences GridEntitySpawnerSceneReferences => gridEntitySpawnerSceneReferences;
    public ShuffleControllerSceneReferences ShuffleControllerSceneReferences => shuffleControllerSceneReferences;
    public GridGoalsControllerReferences GridGoalsControllerReferences => goalsControllerReferences;
    public MovesControllerSceneReferences MovesControllerReferences => movesControllerReferences;
}

