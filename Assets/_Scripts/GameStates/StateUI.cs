using UnityEngine;

public class StateUI : MonoGameStateListener
{
    [SerializeField] private Canvas canvas;

    //protected override void Awake()
    //{
    //    if (GameManager.Instance.GameState == state) OnEnterState();
    //}

    protected override void OnEnterState()
    {
        canvas.enabled = true;
    }

    protected override void OnExitState()
    {
        canvas.enabled = false;
    }
}