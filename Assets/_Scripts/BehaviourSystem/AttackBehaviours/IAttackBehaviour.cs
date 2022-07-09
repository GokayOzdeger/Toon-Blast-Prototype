using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackBehaviour 
{
    public List<AHealthBehaviourSO> CurrentTargets { get; }
    public void TickAttack(float deltaTime);
}
