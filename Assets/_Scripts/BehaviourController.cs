using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    [SerializeField] private AMovementBehaviourSO movementBehaviour;
    [SerializeField] private ARotationBehaviourSO rotationBehaviour;
    [SerializeField] private AHealthBehaviourSO healthBehaviour;

    public IMovementBehaviour MovementBehaviour => movementBehaviour;
    public IRotationBehaviour RotationBehaviour => rotationBehaviour;
    public IHealthBehaviour HealthBehaviour => healthBehaviour;

    private void Start()
    {
        CreateBehaviourClones();
        
        movementBehaviour?.Setup(this);
        rotationBehaviour?.Setup(this);
    }

    private void CreateBehaviourClones()
    {
        if(movementBehaviour) movementBehaviour = Instantiate(movementBehaviour);
        if(rotationBehaviour) rotationBehaviour = Instantiate(rotationBehaviour);
    }

    private void Update()
    {
        movementBehaviour?.TickMove(Time.deltaTime);
        rotationBehaviour?.TickRotation(Time.deltaTime);
    }
}
