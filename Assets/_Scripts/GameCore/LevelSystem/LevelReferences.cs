using UnityEngine;


[System.Serializable]
public class LevelReferences
{
    [SerializeField] private GameState highScoreGameState;
    [SerializeField] private GameState levelSelectGameState;
    [SerializeField][Group] private TileControllerReferences tileManagerReferences;
    [SerializeField][Group] private WordControllerReferences wordControllerReferences;
    [SerializeField][Group] private ScoreControllerReferences scoreControllerReferences;

    public GameState HighScoreGameState => highScoreGameState;
    public GameState LevelSelectGameState => levelSelectGameState;
    public TileControllerReferences TileManagerReferences => tileManagerReferences;
    public WordControllerReferences WordControllerReferences => wordControllerReferences;
    public ScoreControllerReferences ScoreControllerReferences => scoreControllerReferences;
}

