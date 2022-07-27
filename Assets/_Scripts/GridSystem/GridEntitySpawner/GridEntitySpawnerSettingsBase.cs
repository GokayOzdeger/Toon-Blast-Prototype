using System;
using UnityEngine;

[Serializable]
public class GridEntitySpawnerSettings
{
    [BHeader("Grid Start Layout")]
    public GridStartLayout gridStartLayout = new GridStartLayout(9, 9);

    [BHeader("Grid Entity Spawner Settings")]
    [SerializeField] private BasicGridEntityTypeDefinition[] entityTypes;
    [SerializeField] private int spawnHeight;

    public BasicGridEntityTypeDefinition[] EntityTypes => entityTypes;
    public int SpawnHeight => spawnHeight;
}