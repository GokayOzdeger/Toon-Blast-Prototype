using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FallingBlocksController
{
    private float _rowSpacing;

    public FallingBlocksController(float rowSpacing)
    {
        this._rowSpacing = rowSpacing;
    }

    public void CreateFallingBlockGroup(List<FallingBlocksGroup> fallingBlocksGroups)
    {
        foreach (var fallingBlocksGroup in fallingBlocksGroups) CreateFallingBlockGroup(fallingBlocksGroup);
    }

    public void CreateFallingBlockGroup(FallingBlocksGroup fallingBlocksGroup)
    {
        foreach(var block in fallingBlocksGroup.FallingBlocks)
        {
            block.CompleteLastTween();
            Vector3 targetPos = block.transform.position - new Vector3(0, _rowSpacing * fallingBlocksGroup.FallDistance, 0);
            Tweener moveTween = block.transform.DOMoveY(targetPos.y, .75f / Mathf.Pow(fallingBlocksGroup.FallDistance,1/3f));
            block.CacheTween(moveTween);
        }
    }
}
