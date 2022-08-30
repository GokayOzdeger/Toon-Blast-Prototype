using UnityEngine;

public abstract class MonoGameStateListener : MonoBehaviour
{
    [SerializeField] protected GameState state;
    protected bool? StateActive { get; private set; }

    protected virtual void Awake()
    {
        if (state == null) Debug.LogError("StateListener " + gameObject.name+" missing GameState !", gameObject);
        GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
    }

    public virtual void OnGameStateChanged(GameState newState)
    {
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
