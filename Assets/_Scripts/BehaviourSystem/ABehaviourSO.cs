using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ABehaviourSO : ScriptableObject, IBehaviour
{
    public virtual BehaviourController Controller { get; protected set; }

    public virtual bool IsActive { get; protected set; } = true;

    public virtual void Setup(BehaviourController controller)
    {
        Controller = controller;
        OnSetup(controller);
    }

    public abstract void OnSetup(BehaviourController controller);

    public virtual void StartBehaviour()
    {
        IsActive = true;
    }

    public virtual void StopBehaviour()
    {
        IsActive = false;
    }
}
