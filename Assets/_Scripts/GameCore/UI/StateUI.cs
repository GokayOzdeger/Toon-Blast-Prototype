using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateUI : MonoGameStateListener
{
    [SerializeField] private Canvas canvas;

    //protected override void Awake()
    //{
    //    if (GameManager.Instance.GameState == state) OnEnterState();
    //}

    public override void OnEnterState()
    {
        canvas.enabled = true;
    }

    public override void OnExitState()
    {
        canvas.enabled = false;
    }
}
