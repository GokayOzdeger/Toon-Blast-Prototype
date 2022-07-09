using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/LookAtMoveTargetRotationBehaviour")]
public class LookAtMoveTargetRotationBehaviour : ARotationBehaviourSO
{
    [SerializeField] float rotationSpeed;
    
    private IMovementBehaviour _movementBehaviour;
    private Transform _controlledTransform;

    public override void OnSetup(BehaviourController controller)
    {
        _movementBehaviour = controller.MovementBehaviour;
        _controlledTransform = controller.transform;
    }

    public override void TickRotation(float deltaTime)
    {
        Vector3 vectorToTarget = _movementBehaviour.TargetPoint - (Vector2) _controlledTransform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        _controlledTransform.rotation = Quaternion.Slerp(_controlledTransform.rotation, q, deltaTime * rotationSpeed);
    }
}
