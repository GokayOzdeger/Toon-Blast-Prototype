using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthBehaviour
{
    public float CurrentHealth { get; }
    public float MaxHealth { get; }
    public bool IsDead { get; }
    public void TakeDamage(float damage);
}

public struct DamageResult
{
    public bool KilledTarget;
    public float DamageInflicted;
    
}
