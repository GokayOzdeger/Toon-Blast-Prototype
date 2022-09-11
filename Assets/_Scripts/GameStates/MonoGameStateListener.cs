using UnityEngine;

public abstract class MonoGameStateListener : MonoBehaviour
{
    [SerializeField] protected GameState state;
    private bool? StateActive { get; set; }

    protected virtual void Awake()
    {
        if (state == null) Debug.LogError("StateListener " + gameObject.name + " missing GameState !", gameObject);
        GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
    }

    protected virtual void OnGameStateChanged(GameState newState)
    {
        if (!StateActive.HasValue)
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

    protected abstract void OnEnterState();

    protected abstract void OnExitState();
}