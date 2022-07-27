using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockMatchCondition 
{
    [SerializeField] private ACondition condition;
    [SerializeField] private Sprite sprite;
    [SerializeField] private BasicGridEntityTypeDefinition[] entitiesToSpawnOnMatch; // used for after match spawns like powerups

    public BasicGridEntityTypeDefinition GetRandomEntityToSpawn()
    {
        if (entitiesToSpawnOnMatch == null || entitiesToSpawnOnMatch.Length == 0) return null;
        return entitiesToSpawnOnMatch[Random.Range(0, entitiesToSpawnOnMatch.Length)];
    }

    public Sprite Sprite => sprite;

    public ACondition Condition => condition;
}
