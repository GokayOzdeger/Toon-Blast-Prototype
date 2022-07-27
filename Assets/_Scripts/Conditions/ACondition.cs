using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACondition : ScriptableObject, ICondition
{
    public abstract bool IsConditionMet(object obj);
}
