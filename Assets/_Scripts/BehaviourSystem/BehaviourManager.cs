using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BehaviourManager : AutoSingleton<BehaviourManager>
{
    private List<BehaviourController> BehaviourControllers { get; set; } = new List<BehaviourController>();
    private List<AHealthBehaviourSO> HealthBehaviours { get; set; } = new List<AHealthBehaviourSO>();
    private List<AAttackBehaviourSO> AttackBehaviours { get; set; } = new List<AAttackBehaviourSO>();
    private List<AMovementBehaviourSO> MovementBehaviours { get; set; } = new List<AMovementBehaviourSO>();
    private List<ARotationBehaviourSO> RotationBehaviours { get; set; } = new List<ARotationBehaviourSO>();
    
    public void RegisterBehaviourController(BehaviourController controller)
    {
        BehaviourControllers.Add(controller);
        if (controller.HealthBehaviour != null) HealthBehaviours.Add(controller.HealthBehaviour);
        if (controller.AttackBehaviour != null) AttackBehaviours.Add(controller.AttackBehaviour);
        if (controller.MovementBehaviour != null) MovementBehaviours.Add(controller.MovementBehaviour);
        if (controller.RotationBehaviour != null) RotationBehaviours.Add(controller.RotationBehaviour);
    }

    public void UnregisterBehaviourController(BehaviourController controller)
    {
        BehaviourControllers.Remove(controller);
        if (controller.HealthBehaviour != null) HealthBehaviours.Remove(controller.HealthBehaviour);
        if (controller.AttackBehaviour != null) AttackBehaviours.Remove(controller.AttackBehaviour);
        if (controller.MovementBehaviour != null) MovementBehaviours.Remove(controller.MovementBehaviour);
        if (controller.RotationBehaviour != null) RotationBehaviours.Remove(controller.RotationBehaviour);
    }

    public void RemoveEntitiesAndReset()
    {
        // beahaviour controllers unregisters on going to pool
        while (BehaviourControllers.Count > 0)
        {
            BehaviourControllers[0].PoolObject.GoToPool();
        }
        
        BehaviourControllers.Clear();
        MovementBehaviours.Clear();
        AttackBehaviours.Clear();
        HealthBehaviours.Clear();
        RotationBehaviours.Clear();
    }

    public AHealthBehaviourSO ClosestAliveHealthBehaviourInRange(Vector2 requestedPosition, float radius)
    {
        AHealthBehaviourSO result = null;
        float closestDistance = Mathf.Infinity;
        foreach (var healthBehaviour in HealthBehaviours)
        {
            if (healthBehaviour.IsDead) continue;
            if (!healthBehaviour.IsActive) continue;
            
            float distance = Vector2.Distance(healthBehaviour.Controller.transform.position, requestedPosition);
            
            if (distance > closestDistance) continue;
            if (distance > radius ) continue;
            closestDistance = distance;
            result = healthBehaviour;
        }
        return result;
    }
}
