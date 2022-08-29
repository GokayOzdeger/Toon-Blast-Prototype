using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using SaveSystem;
using UnityEngine.Events;

public class GameManager : AutoSingleton<GameManager>
{
    [SerializeField] private GameState startingGameState;

    public GameState GameState { get; private set; }
    public UnityEvent<GameState> OnGameStateChanged { get; private set; } = new UnityEvent<GameState>();


    private void Start()
    {
        ChangeGameState(startingGameState); 
    }

    public void ChangeGameState(GameState newState)
    {
        GameState = newState;
        OnGameStateChanged.Invoke(newState);
    }


#if UNITY_EDITOR

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.DisabledInPlayMode)]
    private void DeleteAllSaves()
    {
        SaveHandler.DeleteAll();
    }

#endif
}
