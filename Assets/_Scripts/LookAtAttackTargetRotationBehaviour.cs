using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/LookAtAttackTargetRotationBehaviour")]
public class LookAtAttackTargetRotationBehaviour : ARotationBehaviourSO
{
    private IAttackBehaviour _attackBehaviour;
    private Transform _controlledTransform;

    public override void OnSetup(BehaviourController controller)
    {
        _attackBehaviour = controller.AttackBehaviour;
        _controlledTransform = controller.transform;
    }

    public override void TickRotation(float deltaTime)
    {
        if (_attackBehaviour.CurrentTargets[0] == null) return;
        _controlledTransform.right = (Vector2)_attackBehaviour.CurrentTargets[0].Controller.transform.position - (Vector2)_controlledTransform.position;
    }
}
