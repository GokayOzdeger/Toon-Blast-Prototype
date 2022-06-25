using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ClusterSizeCondition")]
public class GroupSizeCondition : ScriptableObject, ICondition
{
    [Tooltip("Inclusive")]
    [SerializeField] private int minNumberOfGroupSize;
    [Tooltip("Exclusive")]
    [SerializeField] private int maxNumberOfGroupSize;
    public bool IsConditionMet(Block block)
    {
        Debug.Log("True");
        return true;
    }
}
