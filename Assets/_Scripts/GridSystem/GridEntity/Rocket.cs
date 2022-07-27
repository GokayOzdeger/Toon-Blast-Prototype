using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Rocket : BasicFallingGridEntity
{
    [SerializeField] RocketDirection direction;
    public void OnClickRocket()
    {
        if (!_gridController.GridInterractable) return;
        DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent();
        destroyEvent.StartEvent(_gridController, new List<Rocket>() { this });
    }

    public override void DestoryEntity()
    {
        StartExplosion();
        base.DestoryEntity();
    }

    private void StartExplosion()
    {
        DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent();
        
        List<IGridEntity> entitiesInDirection = null;
        
        switch (direction)
        {
            case RocketDirection.Horizontal:
                entitiesInDirection = _gridController.GetEntitiesInRow(GridCoordinates.x);
                break;
            case RocketDirection.Vertical:
                entitiesInDirection = _gridController.GetEntitiesInColumn(GridCoordinates.y);
                break;
        }

        destroyEvent.StartEvent(_gridController, entitiesInDirection);
    }

    private enum RocketDirection
    {
        Horizontal,
        Vertical
    }
}
