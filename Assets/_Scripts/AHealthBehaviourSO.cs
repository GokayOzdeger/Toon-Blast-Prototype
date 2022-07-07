using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AHealthBehaviourSO : ABehaviourSO, IHealthBehaviour
{
    public abstract float CurrentHealth { get; protected set; }

    public abstract float MaxHealth { get; }

    public abstract bool IsDead { get; protected set; }
    
    public abstract void TakeDamage(float damage);
}
