using UnityEngine;


[System.Serializable]
public class LevelReferences
{
    [SerializeField][Group] private TileControllerReferences tileManagerReferences;
    [SerializeField][Group] private WordControllerReferences wordControllerReferences;

    public TileControllerReferences TileManagerReferences => tileManagerReferences;
    public WordControllerReferences WordControllerReferences => wordControllerReferences;
}

