using UnityEngine;

[System.Serializable]
public class LevelSettings 
{
    [SerializeField] private TileManagerSettings tileManagerSettings;

    public TileManagerSettings TileManagerSettings => tileManagerSettings;
} 
