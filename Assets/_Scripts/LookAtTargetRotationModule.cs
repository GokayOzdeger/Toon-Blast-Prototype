using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/LookAtTargetRotationBehaviour")]
public class LookAtTargetRotationModule : ARotationBehaviourSO
{
    private IMovementBehaviour _movementBehaviour;
    private Transform _controlledTransform;
    private Vector2 _lastFramesTarget;
    public override void Setup(BehaviourController controller)
    {
        _movementBehaviour = controller.MovementBehaviour;
        _controlledTransform = controller.transform;
    }

    public override void TickRotation(float deltaTime)
    {
        if (_lastFramesTarget == _movementBehaviour.TargetPoint) return;
        _lastFramesTarget = _movementBehaviour.TargetPoint;
        
        _controlledTransform.right = _movementBehaviour.TargetPoint - (Vector2) _controlledTransform.position;
    }
}
