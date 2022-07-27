using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Block Type Definition")]
public class BlockTypeDefinition : BasicGridEntityTypeDefinition
{
    [SerializeField] private List<SpriteConditionPair> spriteConditionPairs = new List<SpriteConditionPair>();
    public List<SpriteConditionPair> SpriteConditionPairs => spriteConditionPairs;
}
