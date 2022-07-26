using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : BasicFallingGridEntity
{
    private readonly float DestroyAnimationDuration = .1f;
    

    public override void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        base.OnGridChange(changeCoordinate, gridChangeEventType, entityType);
        if(gridChangeEventType == GridChangeEventType.EntityDestroyed && entityType is BlockTypeDefinition)
        {
            if ((GridCoordinates - changeCoordinate).magnitude <= 1)
            {
                DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent();
                destroyEvent.StartEvent(_gridController, new List<Balloon>() { this });
            }
        }
    }

    public override void DestoryEntity()
    {
        AnimateDestroy();
    }

    public void AnimateDestroy()
    {
        int randomDirection = UnityEngine.Random.value < .5 ? 1 : -1;
        _lastTween.Kill();
        _lastTween = transform.DOPunchScale(new Vector3(.4f, .4f, .4f), DestroyAnimationDuration).SetEase(Ease.OutCubic);
        _lastTween.onComplete += OnEntityDestroy;
    }

    private void OnEntityDestroy()
    {
        OnEntityDestroyed.Invoke(this);
        poolObject.GoToPool();
    }
}
