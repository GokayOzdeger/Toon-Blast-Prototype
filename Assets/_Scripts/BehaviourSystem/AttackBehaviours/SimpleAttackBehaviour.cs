using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject projectilePrefab;
    
    public override List<AHealthBehaviourSO> CurrentTargets { get; protected set; } = new List<AHealthBehaviourSO>();

    public override void TickAttack(float deltaTime)
    {
        if (!IsActive) return;
        UpdateTargets();
        CheckAttack(deltaTime);
    }

    private void CheckAttack(float deltaTime)
    {
        delayBetweenAttacks.Step(deltaTime);
        
        if (CurrentTargets.Count == 0) return;
        
        if (delayBetweenAttacks.IsReady)
        {
            delayBetweenAttacks.EnterCooldown();
            foreach(AHealthBehaviourSO target in CurrentTargets) SetupProjectile(target);
        }
    }

    

    private void UpdateTargets()
    {
        CurrentTargets.Clear();
        AHealthBehaviourSO foundTarget = BehaviourManager.Instance.ClosestAliveHealthBehaviourInRange(Controller.transform.position, attackRange);
        if (foundTarget) CurrentTargets.Add(foundTarget);
    }

    private void SetupProjectile(AHealthBehaviourSO target)
    {
        GameObject spawnedProjectile = ObjectPooler.Instance.Spawn(projectilePrefab.name, Controller.transform.position, Controller.transform.rotation);
        BehaviourController controller = spawnedProjectile.GetComponent<BehaviourController>();
        
        if (!controller.TryGetBehaviour<CommandableMovementBehaviour>(out var commandableMovementBehaviour))
        {
            Debug.LogError("No CommandableMovementBehaviour found on projectile");
            return;
        }
     
        commandableMovementBehaviour.CommandedMoveTarget = target.Controller;
        commandableMovementBehaviour.OnTargetReached.AddListener((target) =>
        {
            target.HealthBehaviour.TakeDamage(attackDamage);
            controller.PoolObject.GoToPool();
        });

        target.OnDeath.AddListener((target) =>
        {
            controller.PoolObject.GoToPool();
        });
        
        Debug.Log($"{Controller.name} attacked -> {target.Controller.name}");
    }

    private void OnHitTarget(BehaviourController target)
    {
        
    }

    public override void OnSetup(BehaviourController controller)
    {
        //
    }
}
