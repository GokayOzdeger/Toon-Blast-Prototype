using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Min Group Size Condition")]
public class MinBlockGroupSizeCondition : ACondition
{
    [Tooltip("Inclusive")]
    [SerializeField] private int minGroupSize;

    public override bool IsConditionMet(object obj)
    {
        Block block = obj as Block;
        if (block == null) return false;   
        return block.GroupSize >= minGroupSize;
    }
}
