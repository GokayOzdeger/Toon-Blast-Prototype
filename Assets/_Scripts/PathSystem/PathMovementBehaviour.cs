using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/PathMovementBehaviour")]
public class PathMovementBehaviour : AMovementBehaviourSO
{
    private static readonly float TargetReachThreshold = 1f;
    
    [SerializeField] private float moveSpeed;

    public float DistanceTravelled { get; private set; } = 0;
    public Transform ControlledTransform { get; private set; }
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    public PathNode TargetPathNode 
    { 
        get 
        { 
            return _targetNode; 
        }
        
        private set
        {
            _targetNode = value;
            if (_targetNode != null) _targetPoint = _targetNode.GetRandomPoint();
        } 
    }
    public override Vector2 TargetPoint { get => _targetPoint; }

    private PathNode _targetNode;
    private Vector2 _targetPoint;

    public override void TickMove(float deltaTime)
    {
        if (!IsActive) return;
        if (TargetPathNode == null) return;

        Vector2 moveDirection = (TargetPoint - (Vector2) ControlledTransform.position).normalized;
        Vector2 distanceToMove = moveDirection * MoveSpeed * deltaTime;
        ControlledTransform.position = (Vector2) ControlledTransform.position + distanceToMove;
        DistanceTravelled += distanceToMove.magnitude;
        if (Vector2.Distance(ControlledTransform.position, TargetPoint) < TargetReachThreshold) TargetPathNodeReached();
    }

    public override void SetMoveSpeed(float moveSpeed)
    { 
        MoveSpeed = moveSpeed;
    }
    
    public void SetNode(PathNode node)
    {
        TargetPathNode = node;
    }

    private void TargetPathNodeReached()
    {
        TargetPathNode = TargetPathNode.GetRandomNextNode();
    }

    public override void OnSetup(BehaviourController controller)
    {
        ControlledTransform = controller.transform;
    }
}
