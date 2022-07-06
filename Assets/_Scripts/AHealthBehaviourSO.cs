using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AHealthBehaviourSO : ScriptableObject, IHealthBehaviour, IBehaviour
{
    public bool IsActive { get; set; }

    public abstract float CurrentHealth { get; protected set; }

    public abstract float MaxHealth { get; }

    public abstract bool IsDead { get; protected set; }

    public abstract void Setup(BehaviourController controller);
    
    public abstract void TakeDamage(float damage);
    
    public virtual void StartBehaviour()
    {
        IsActive = true;
    }

    public virtual void StopBehaviour()
    {
        IsActive = false;
    }
}
