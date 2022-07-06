using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/PathMovementBehaviour")]
public class PathMovementBehaviour : AMovementBehaviourSO
{
    private static readonly float TargetReachThreshold = .001f;
    
    [SerializeField] private float moveSpeed;
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
    
    public override void Setup(BehaviourController controller)
    {
        ControlledTransform = controller.transform;
        TargetPathNode = Path.Instance.GetRandomStartNode();
    }

    public override void TickMove(float deltaTime)
    {
        if (!IsActive) return;
        if (TargetPathNode == null) return;

        Vector2 moveDirection = (TargetPoint - (Vector2) ControlledTransform.position).normalized;
        ControlledTransform.position = (Vector2) ControlledTransform.position + (moveDirection * MoveSpeed * deltaTime);
        if (Vector2.Distance(ControlledTransform.position, TargetPoint) < TargetReachThreshold / deltaTime) TargetPathNodeReached();
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
}
