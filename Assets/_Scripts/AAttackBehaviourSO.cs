using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAttackBehaviourSO : ABehaviourSO, IAttackBehaviour
{
    public abstract List<AHealthBehaviourSO> CurrentTargets { get; }
    public abstract void TickAttack(float deltaTime);
}
