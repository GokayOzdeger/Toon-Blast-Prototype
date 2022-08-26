using UnityEngine;

[System.Serializable]
public class LevelSettings 
{
    [SerializeField][Group] private TileControllerSettings tileManagerSettings;
    [SerializeField][Group] private WordControllerSettings wordControllerSettings;

    public TileControllerSettings TileManagerSettings => tileManagerSettings;
    public WordControllerSettings WordControllerSettings => wordControllerSettings;
} 
