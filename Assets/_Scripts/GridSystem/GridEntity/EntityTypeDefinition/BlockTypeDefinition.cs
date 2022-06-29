using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Block Type Definition")]
public class BlockTypeDefinition : BasicGridEntityTypeDefinition
{
    [BHeader("Block Type Settings")]
    [SerializeField] private List<ConditionSpritePair> conditionSpritePairs = new List<ConditionSpritePair>();

    public List<ConditionSpritePair> ConditionSpritePairs { get { return conditionSpritePairs; } }

    public Sprite GetBlockGroupIcon(List<Block> blockGroup)
    {
        foreach(ConditionSpritePair conditionSpritePair in conditionSpritePairs)
        {
            if (!conditionSpritePair.Condition.IsConditionMet(blockGroup)) continue;
            return conditionSpritePair.Icon;
        }
        return defaultSprite;
    }


    [System.Serializable]
    public class ConditionSpritePair
    {
        public ACondition Condition;
        public Sprite Icon;
    }
}
