using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARotationBehaviourSO : ScriptableObject, IRotationBehaviour, IBehaviour
{
    public virtual bool IsActive { get; set; } = true;

    public abstract void Setup(BehaviourController controller);
    
    public virtual void StartBehaviour()
    {
        IsActive = true;
    }

    public virtual void StopBehaviour()
    {
        IsActive = false;
    }

    public abstract void TickRotation(float deltaTime);
}
