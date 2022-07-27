using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "ScriptableObjects/Condition")]
public interface ICondition
{
    public bool IsConditionMet(object obj);
}
