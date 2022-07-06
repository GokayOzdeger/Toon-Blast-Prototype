using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMovementBehaviourSO : ScriptableObject, IMovementBehaviour, IBehaviour
{
    public bool IsActive { get; set; } = true;
    public abstract Vector2 TargetPoint { get; }

    public abstract void TickMove(float deltaTime);

    public abstract void SetMoveSpeed(float moveSpeed);

    public abstract void Setup(BehaviourController controller);

    public virtual void StartBehaviour()
    {
        IsActive = true;
    }

    public virtual void StopBehaviour()
    {
        IsActive = false;
    }
}
