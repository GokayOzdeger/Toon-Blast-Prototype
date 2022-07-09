using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AHealthBehaviourSO : ABehaviourSO, IHealthBehaviour
{
    public UnityEvent<AHealthBehaviourSO> OnDeath { get; protected set; } = new UnityEvent<AHealthBehaviourSO>();
    public abstract float CurrentHealth { get; protected set; }

    public abstract float MaxHealth { get; }

    public abstract bool IsDead { get; protected set; }
    
    public abstract void TakeDamage(float damage);
}
