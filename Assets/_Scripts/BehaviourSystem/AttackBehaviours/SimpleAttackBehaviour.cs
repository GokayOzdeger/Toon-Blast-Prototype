using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using DG.Tweening;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/SimpleAttackBehaviour")]
public class SimpleAttackBehaviour : AAttackBehaviourSO
{
    [BHeader("Attack")]
    [SerializeField] private float attackRange = 100;
    [SerializeField] private float attackDamage = 20;
    [SerializeField] private Cooldown delayBetweenAttacks = new Cooldown(2);

    [BHeader("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 projectileSummonPositionOffset;

    [BHeader("Animation Options")]
    [SerializeField] private float punchScaleDuration = 0.5f;
    [SerializeField] private float punchScaleAmount = 0.1f;

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
            AnimateAttack();
            foreach (AHealthBehaviourSO target in CurrentTargets) SetupProjectile(target);
        }
    }

    private void AnimateAttack()
    {
        Controller.AnimationController.PunchScale(punchScaleAmount, punchScaleDuration);
    }

    private void UpdateTargets()
    {
        CurrentTargets.Clear();
        AHealthBehaviourSO foundTarget = BehaviourManager.Instance.ClosestAliveHealthBehaviourInRange(Controller.transform.position, attackRange);
        if (foundTarget) CurrentTargets.Add(foundTarget);
    }

    private void SetupProjectile(AHealthBehaviourSO target)
    {
        Vector2 offsetPositionAdd = (Controller.transform.right * projectileSummonPositionOffset.x) + (Controller.transform.up * projectileSummonPositionOffset.y);
        Vector2 summonPosition = (Vector2) Controller.transform.position + offsetPositionAdd;
        GameObject spawnedProjectile = ObjectPooler.Instance.Spawn(projectilePrefab.name, summonPosition, Controller.transform.rotation);
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

    public override void OnSetup(BehaviourController controller)
    {
        //
    }
}
