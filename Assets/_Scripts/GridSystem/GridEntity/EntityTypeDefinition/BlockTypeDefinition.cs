using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Block Type Definition")]
public class BlockTypeDefinition : BasicGridEntityTypeDefinition
{
    [BHeader("Block Type Settings")]
    [SerializeField] private bool explodesOnClick = true;

    public bool MatchesOnClick => explodesOnClick;

}
