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
        Debug.Log("Falling group size: " + fallingBlocksGroup.FallingBlocks.Count + "... falling for: " + fallingBlocksGroup.FallDistance);
        foreach(var block in fallingBlocksGroup.FallingBlocks)
        {
            block.DOComplete();
            block.transform.DOMove(block.transform.position-new Vector3(0,_rowSpacing*fallingBlocksGroup.FallDistance,0),1);
        }
    }
}
