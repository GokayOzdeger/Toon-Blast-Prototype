using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementBehaviour
{
    public Vector2 TargetPoint { get; }
    public void TickMove(float deltaTime);
    public void SetMoveSpeed(float moveSpeed);
}
