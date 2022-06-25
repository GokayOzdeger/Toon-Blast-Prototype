using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Block Type Definition")]
public class BlockTypeDefinition : ScriptableObject
{
    [SerializeField] private string blockTypeName;
    [SerializeField] private Sprite defaultBlockIcon;
    [SerializeField] private List<ConditionIconPair> conditionIconPairs = new List<ConditionIconPair>();

    public string BlockTypeName { get { return blockTypeName; } }
    public Sprite DefaultBlockIcon { get { return defaultBlockIcon; } }
    public List<ConditionIconPair> ConditionIconPairs { get { return conditionIconPairs; } }

    [System.Serializable]
    public class ConditionIconPair
    {
        public GroupSizeCondition Condition;
        public Sprite Icon;
    }
}
