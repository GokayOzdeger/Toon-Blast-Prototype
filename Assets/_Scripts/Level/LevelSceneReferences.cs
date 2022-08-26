using UnityEngine;


[System.Serializable]
public class LevelReferences
{
    [SerializeField] private TileManagerReferences tileManagerReferences;

    public TileManagerReferences TileManagerReferences => tileManagerReferences;
}

