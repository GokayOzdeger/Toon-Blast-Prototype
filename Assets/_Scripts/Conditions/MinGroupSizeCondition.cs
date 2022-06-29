using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Min Group Size Condition")]
public class MinGroupSizeCondition : ScriptableObject, ICondition
{
    [Tooltip("Inclusive")]
    [SerializeField] private int minGroupSize;

    public bool IsConditionMet(List<Block> blockGroup)
    {
        return blockGroup.Count >= minGroupSize;
    }
}
