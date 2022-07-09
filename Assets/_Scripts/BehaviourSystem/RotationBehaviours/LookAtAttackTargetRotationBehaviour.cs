using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/LookAtAttackTargetRotationBehaviour")]
public class LookAtAttackTargetRotationBehaviour : ARotationBehaviourSO
{
    [SerializeField] float rotationSpeed;

    private IAttackBehaviour _attackBehaviour;
    private Transform _controlledTransform;

    public override void OnSetup(BehaviourController controller)
    {
        _attackBehaviour = controller.AttackBehaviour;
        _controlledTransform = controller.transform;
    }

    public override void TickRotation(float deltaTime)
    {
        if (_attackBehaviour.CurrentTargets.Count == 0 || _attackBehaviour.CurrentTargets[0] == null) return;

        Vector3 vectorToTarget = _attackBehaviour.CurrentTargets[0].Controller.transform.position - _controlledTransform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        _controlledTransform.rotation = Quaternion.Slerp(_controlledTransform.rotation, q, deltaTime * rotationSpeed);
    }
}
