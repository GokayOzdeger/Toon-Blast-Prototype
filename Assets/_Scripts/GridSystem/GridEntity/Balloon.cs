using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : FallingGridEntity
{
    public override void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        base.OnGridChange(changeCoordinate, gridChangeEventType, entityType);
        if(gridChangeEventType == GridChangeEventType.EntityMatched && entityType is BlockTypeDefinition)
        {
            if ((GridCoordinates - changeCoordinate).magnitude <= 1)
            {
                DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent(EntityDestroyTypes.DestroyedByNearbyMatch);
                destroyEvent.StartEvent(_gridController, new List<Balloon>() { this });
            }
        }
    }

    public override void DestoryEntity(EntityDestroyTypes destroyType)
    {
        AnimateDestroy();
    }

    public void AnimateDestroy()
    {
        CompleteLastTween();
        _lastTween = TweenHelper.PunchScale(transform, OnEntityDestroy);
    }

    private void OnEntityDestroy()
    {
        OnEntityDestroyed.Invoke(this);
        poolObject.GoToPool();
    }
}
