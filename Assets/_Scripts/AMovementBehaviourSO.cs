using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMovementBehaviourSO : ABehaviourSO, IMovementBehaviour
{
    public abstract Vector2 TargetPoint { get; }

    public abstract void TickMove(float deltaTime);

    public abstract void SetMoveSpeed(float moveSpeed);
}
