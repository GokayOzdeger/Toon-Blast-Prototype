using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BehaviourController : MonoBehaviour, IPoolable
{
    [SerializeField] private AMovementBehaviourSO movementBehaviour;
    [SerializeField] private ARotationBehaviourSO rotationBehaviour;
    [SerializeField] private AHealthBehaviourSO healthBehaviour;
    [SerializeField] private AAttackBehaviourSO attackBehaviour;

    public AMovementBehaviourSO MovementBehaviour => movementBehaviour;
    public ARotationBehaviourSO RotationBehaviour => rotationBehaviour;
    public AHealthBehaviourSO HealthBehaviour => healthBehaviour;
    public AAttackBehaviourSO AttackBehaviour => attackBehaviour;
    public PoolObject PoolObject => _poolObject;

    private PoolObject _poolObject;

    private void Awake()
    {
        CreateBehaviourClones();

        _poolObject = GetComponent<PoolObject>();
        movementBehaviour?.Setup(this);
        rotationBehaviour?.Setup(this);
        healthBehaviour?.Setup(this);
        attackBehaviour?.Setup(this);
    }

    private void Start()
    {
        BehaviourManager.Instance.RegisterBehaviourController(this);
    }

    private void CreateBehaviourClones()
    {
        if (movementBehaviour) movementBehaviour = Instantiate(movementBehaviour);
        if (rotationBehaviour) rotationBehaviour = Instantiate(rotationBehaviour);
        if (healthBehaviour) healthBehaviour = Instantiate(healthBehaviour);
        if (attackBehaviour) attackBehaviour = Instantiate(attackBehaviour);
    }

    private void Update()
    {
        movementBehaviour?.TickMove(Time.deltaTime);
        rotationBehaviour?.TickRotation(Time.deltaTime);
        attackBehaviour?.TickAttack(Time.deltaTime);
    }

    public void OnGoToPool()
    {

    }

    public void OnPoolSpawn()
    {

    }
}
