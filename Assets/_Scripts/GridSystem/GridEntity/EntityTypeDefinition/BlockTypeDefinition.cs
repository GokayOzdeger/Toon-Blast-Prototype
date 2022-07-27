using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Block Type Definition")]
public class BlockTypeDefinition : BasicGridEntityTypeDefinition
{
    [SerializeField] private List<BlockMatchCondition> blockMatchConditions = new List<BlockMatchCondition>();
    public List<BlockMatchCondition> BlockMatchConditions => blockMatchConditions;
}
