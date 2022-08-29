using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateListener
{
    public void OnGameStateChanged(GameState newState);
}

[System.Serializable]
public abstract class MonoGameStateListener : MonoBehaviour
{
    [SerializeField] protected GameState state;
    protected bool? StateActive { get; private set; }

    protected virtual void Awake()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
    }

    public virtual void OnGameStateChanged(GameState newState)
    {
        Debug.Log("New State: " + newState.name);
        if(!StateActive.HasValue)
        {
            if (newState == state) OnEnterState();
            else OnExitState();
        }
        else if (!StateActive.Value)
        {
            if (newState != state) return;
            StateActive = true;
            OnEnterState();
        }
        else
        {
            if (newState == state) return;
            StateActive = false;
            OnExitState();
        }
    }

    public abstract void OnEnterState();

    public abstract void OnExitState();
}
