using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Min Group Size Condition")]
public class MinGroupSizeCondition : ACondition
{
    [Tooltip("Inclusive")]
    [SerializeField] private int minGroupSize;

    public override bool IsConditionMet(List<Block> blockGroup)
    {
        return blockGroup.Count >= minGroupSize;
    }
}
