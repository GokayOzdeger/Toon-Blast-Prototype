using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/CommandableMovementBehaviour")]
public class CommandableMovementBehaviour : AMovementBehaviourSO
{
    private static readonly float TargetReachThreshold = 1f;

    [SerializeField] private float moveSpeed;
    
    public UnityEvent<BehaviourController> OnTargetReached { get; private set; } = new UnityEvent<BehaviourController>();
    public BehaviourController CommandedMoveTarget { get; set; }
    public override Vector2 TargetPoint 
    { 
        get 
        {
            if (CommandedMoveTarget == null) return Controller.transform.position;
            return CommandedMoveTarget.transform.position; 
        } 
    }
    public float MoveSpeed => moveSpeed;

    public override void OnSetup(BehaviourController controller)
    {
        //
    }

    public override void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    public override void TickMove(float deltaTime)
    {
        Debug.Log("TickMove: "+CommandedMoveTarget);
        if (!IsActive) return;
        if (!CommandedMoveTarget) return;

        Vector2 moveDirection = (TargetPoint - (Vector2)Controller.transform.position).normalized;
        Controller.transform.position = (Vector2)Controller.transform.position + (moveDirection * MoveSpeed * deltaTime);
        if (Vector2.Distance(Controller.transform.position, CommandedMoveTarget.transform.position) < TargetReachThreshold) TargetReached();
    }

    private void TargetReached()
    {
        OnTargetReached.Invoke(CommandedMoveTarget);
        OnTargetReached.RemoveAllListeners();
    }

#if UNITY_EDITOR
    private void OnDrawGismoz()
    {
        if (CommandedMoveTarget)
        {
            UnityEditor.Handles.DrawLine(Controller.transform.position, CommandedMoveTarget.transform.position);
        }
    }
#endif
}
