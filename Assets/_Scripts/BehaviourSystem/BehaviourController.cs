using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BehaviourController : MonoBehaviour, IPoolable
{
    [SerializeField] private bool _startsInScene = false;
    
    [SerializeField] private AMovementBehaviourSO movementBehaviour;
    [SerializeField] private ARotationBehaviourSO rotationBehaviour;
    [SerializeField] private AHealthBehaviourSO healthBehaviour;
    [SerializeField] private AAttackBehaviourSO attackBehaviour;

    public AMovementBehaviourSO MovementBehaviour => movementBehaviour;
    public ARotationBehaviourSO RotationBehaviour => rotationBehaviour;
    public AHealthBehaviourSO HealthBehaviour => healthBehaviour;
    public AAttackBehaviourSO AttackBehaviour => attackBehaviour;
    public PoolObject PoolObject => _poolObject;

    private List<IBehaviour> allBehaviours = new List<IBehaviour>();
    private PoolObject _poolObject;

    private void Awake()
    {
        _poolObject = GetComponent<PoolObject>();
    }

    private void Start()
    {
        if (_startsInScene)
        {
            CreateBehaviourInstances();
            BehaviourManager.Instance.RegisterBehaviourController(this);
        }
    }

    private void CreateBehaviourInstances()
    {
        allBehaviours.Clear();
        
        if (movementBehaviour) movementBehaviour = Instantiate(movementBehaviour);
        if (rotationBehaviour) rotationBehaviour = Instantiate(rotationBehaviour);
        if (healthBehaviour) healthBehaviour = Instantiate(healthBehaviour);
        if (attackBehaviour) attackBehaviour = Instantiate(attackBehaviour);

        if (movementBehaviour) allBehaviours.Add(movementBehaviour);
        if (rotationBehaviour) allBehaviours.Add(rotationBehaviour);
        if (healthBehaviour) allBehaviours.Add(healthBehaviour);
        if (attackBehaviour) allBehaviours.Add(attackBehaviour);

        movementBehaviour?.Setup(this);
        rotationBehaviour?.Setup(this);
        healthBehaviour?.Setup(this);
        attackBehaviour?.Setup(this);
    }

    private void Update()
    {
        movementBehaviour?.TickMove(Time.deltaTime);
        rotationBehaviour?.TickRotation(Time.deltaTime);
        attackBehaviour?.TickAttack(Time.deltaTime);
    }

    public bool TryGetBehaviour<T>(out T result) where T : IBehaviour
    {
        foreach (IBehaviour behaviour in allBehaviours)
        {
            if (behaviour is T) 
            { 
                result = (T)behaviour;
                return true;
            }
        }
        result = default(T);
        return false;
    }

    public void OnGoToPool()
    {
        BehaviourManager.Instance.UnregisterBehaviourController(this);
    }

    public void OnPoolSpawn()
    {
        CreateBehaviourInstances();
        BehaviourManager.Instance.RegisterBehaviourController(this);
    }
}
