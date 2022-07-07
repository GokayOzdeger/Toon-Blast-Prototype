using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARotationBehaviourSO : ABehaviourSO, IRotationBehaviour
{
    public abstract void TickRotation(float deltaTime);
}
