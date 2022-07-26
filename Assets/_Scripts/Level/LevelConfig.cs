using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Group][SerializeField] private GridControllerSettings gridControllerSettings;
    [Group][SerializeField] private GridEntitySpawnerSettings gridEntitySpawnerSettings;
    [Group][SerializeField] private GridGoalsControllerSettings goalsControllerSettings;
    [Group][SerializeField] private MovesControllerSettings movesController;

    public GridControllerSettings GridControllerSettings => gridControllerSettings;
    public GridEntitySpawnerSettings GridEntitySpawnerSettings => gridEntitySpawnerSettings;
    public GridGoalsControllerSettings GridGoalsControllerSettings => goalsControllerSettings;
    public MovesControllerSettings MovesControllerSettings => movesController;
}
