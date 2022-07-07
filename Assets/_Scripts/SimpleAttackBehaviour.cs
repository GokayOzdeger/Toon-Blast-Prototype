using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/SimpleAttackBehaviour")]
public class SimpleAttackBehaviour : AAttackBehaviourSO
{
    [BHeader("Attack")]
    [SerializeField] private float attackRange = 100;
    [SerializeField] private float attackDamage = 20;
    [SerializeField] private Cooldown delayBetweenAttacks = new Cooldown(2);

    [BHeader("Projectile")]
    [SerializeField] private string projectilePrefab;
    
    public override List<AHealthBehaviourSO> CurrentTargets => new List<AHealthBehaviourSO> { _currentTarget };

    private AHealthBehaviourSO _currentTarget;

    public override void TickAttack(float deltaTime)
    {
        if (!IsActive) return;
        UpdateTargets();
        CheckAttack(deltaTime);
    }

    private void CheckAttack(float deltaTime)
    {
        delayBetweenAttacks.Step(deltaTime);
        
        if (_currentTarget == null) return;
        
        if (delayBetweenAttacks.IsReady)
        {
            delayBetweenAttacks.EnterCooldown();
            Attack(_currentTarget);
        }
    }

    private void UpdateTargets()
    {
        _currentTarget = null;
        List<AHealthBehaviourSO> foundTargets = BehaviourManager.Instance.AliveHealthBehavioursInRange(Controller.transform.position, attackRange, 1);
        
        if (foundTargets.Count == 0) return;
        _currentTarget = foundTargets[0];
    }

    private void Attack(AHealthBehaviourSO target)
    {
        GameObject spawnedProjectile = ObjectPooler.Instance.Spawn(projectilePrefab, Controller.transform.position, Quaternion.identity);
        BehaviourController controller = spawnedProjectile.GetComponent<BehaviourController>();
        CommandableMovementBehaviour projectileMoveBehaviour = (CommandableMovementBehaviour) controller.MovementBehaviour;
        projectileMoveBehaviour.CommandedMoveTarget = target.Controller;
        projectileMoveBehaviour.OnTargetReached.AddListener((target) =>
        {
            target.HealthBehaviour.TakeDamage(attackDamage);
            controller.PoolObject.GoToPool();
        });
        Debug.Log($"{Controller.name} attacked {target.Controller.name}");
    }

    private void OnHitTarget(BehaviourController target)
    {
        
    }

    public override void OnSetup(BehaviourController controller)
    {
        //
    }
}
