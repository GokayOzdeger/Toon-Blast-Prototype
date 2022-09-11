using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class GameManager : AutoSingleton<GameManager>
{
    [SerializeField] private GameState startingGameState;

    public UnityEvent<GameState> OnGameStateChanged { get; } = new();


    private void Start()
    {
        ChangeGameState(startingGameState);
    }

    public void ChangeGameState(GameState newState)
    {
        OnGameStateChanged.Invoke(newState);
    }
}